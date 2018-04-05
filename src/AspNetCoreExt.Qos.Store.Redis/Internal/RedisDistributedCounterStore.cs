using AspNetCoreExt.Qos.Abstractions.Stores;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Store.Redis.Internal
{
    public class RedisDistributedCounterStore : IQosDistributedCounterStore, IDisposable
    {
        // ARGV[1] = increment
        // ARGV[2] = ttl
        private static readonly string AddScript = @"
            local exists = 1
            if tonumber(ARGV[2]) > 0 then
                exists = redis.call('EXISTS', KEYS[1])
            end
            local counter = redis.call('INCRBY', KEYS[1], ARGV[1]) -- Using INCRBY to leave the TTL untouched
            if exists ~= 1 then
                redis.call('PEXPIRE', KEYS[1], ARGV[2])
            end
            return counter
            ";

        // ARGV[1] = increment
        // ARGV[2] = max count
        // ARGV[3] = ttl
        private static readonly string TryAddScript = @"
                local counter = redis.call('GET', KEYS[1])
                if not counter then
                    redis.call('SET', KEYS[1], ARGV[1])
                    if tonumber(ARGV[3]) > 0 then
                        redis.call('PEXPIRE', KEYS[1], ARGV[3])
                    end
                    return ARGV[1]
                else
                    if counter + tonumber(ARGV[1]) <= tonumber(ARGV[2]) then
                        return redis.call('INCRBY', KEYS[1], ARGV[1]) -- Using INCRBY to leave the TTL untouched
                    else
                        return -counter -- By convention, a negative value means that the Try failed
                    end
                end
            ";

        private readonly ConnectionMultiplexer _connection;

        private readonly string _instanceName;

        public RedisDistributedCounterStore(IOptions<QosRedisStoreOptions> options)
        {
            var opt = options.Value;

            _connection = ConnectionMultiplexer.Connect(opt.Configuration);
            Database = _connection.GetDatabase();
            _instanceName = !string.IsNullOrEmpty(opt.InstanceName) ? opt.InstanceName + "_" : "";
        }

        public void Dispose()
        {
            _connection?.Close();
        }

        public IDatabase Database { get; }

        public async Task<long> GetAsync(string key)
        {
            var result = await Database.StringGetAsync(GetFullKey(key));
            if (result.TryParse(out long value))
            {
                return value;
            }
            return 0;
        }

        public async Task<long> AddAsync(string key, long increment, TimeSpan? period)
        {
            var ttl = period != null ? (long)period.Value.TotalMilliseconds : 0;

            var result = (RedisValue[])(await Database.ScriptEvaluateAsync(
                AddScript,
                new RedisKey[] { GetFullKey(key) },
                new RedisValue[] { increment, ttl }));

            if (result[0].TryParse(out long value))
            {
                return value;
            }
            return 0;
        }

        public async Task<CounterStoreAddResult> TryAddAsync(string key, long increment, long maxValue, TimeSpan? period)
        {
            var ttl = period != null ? (long)period.Value.TotalMilliseconds : 0;

            var result = (RedisValue[])(await Database.ScriptEvaluateAsync(
                TryAddScript,
                new RedisKey[] { GetFullKey(key) },
                new RedisValue[] { increment, maxValue, ttl }));

            if (result[0].TryParse(out long value))
            {
                return new CounterStoreAddResult()
                {
                    Success = value >= 0,
                    NewCounter = Math.Abs(value)
                };
            }

            return new CounterStoreAddResult()
            {
                Success = false
            };
        }

        public Task RemoveAsync(string key)
        {
            return Database.KeyDeleteAsync(GetFullKey(key));
        }

        private string GetFullKey(string key) => _instanceName + key;
    }
}

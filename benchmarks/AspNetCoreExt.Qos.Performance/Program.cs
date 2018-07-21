using BenchmarkDotNet.Running;

namespace AspNetCoreExt.Qos.Performance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<DefaultMemoryQosCounterStoreBenchmark>();
        }
    }
}

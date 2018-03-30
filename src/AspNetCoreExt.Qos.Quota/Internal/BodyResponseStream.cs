using Microsoft.AspNetCore.Http.Features;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Quota.Internal
{
    public class BodyResponseStream : Stream, IHttpSendFileFeature
    {
        private Stream _innerStream;

        public BodyResponseStream(Stream innerStream, IHttpSendFileFeature innerSendFileFeature)
        {
            _innerStream = innerStream;
            InnerSendFileFeature = innerSendFileFeature;
        }

        protected override void Dispose(bool disposing)
        {
            if (_innerStream != null)
            {
                _innerStream.Dispose();
                _innerStream = null;
            }
        }

        public IHttpSendFileFeature InnerSendFileFeature { get; }

        public long WrittenLength { get; private set; }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;

        public override long Position
        {
            get
            {
                return _innerStream.Position;
            }
            set
            {
                _innerStream.Position = value;
            }
        }

        public override void Flush() => _innerStream.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken) => _innerStream.FlushAsync();

        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

        public override void SetLength(long value) => _innerStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            WrittenLength += count;
            _innerStream.Write(buffer, offset, count);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, Object state)
        {
            WrittenLength += count;
            return _innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void EndWrite(IAsyncResult asyncResult) => _innerStream.EndWrite(asyncResult);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            WrittenLength += count;
            return _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public Task SendFileAsync(string path, long offset, long? count, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var fileLength = new FileInfo(path).Length;
            if (offset >= 0 && offset < fileLength)
            {
                if (count.HasValue)
                {
                    if (count.Value > 0 && count.Value < fileLength - offset)
                    {
                        WrittenLength += count.Value;
                    }
                    else
                    {
                        WrittenLength += fileLength - offset;
                    }
                }
                else
                {
                    WrittenLength += fileLength - offset;
                }
            }

            return InnerSendFileFeature.SendFileAsync(path, offset, count, cancellation);
        }
    }
}

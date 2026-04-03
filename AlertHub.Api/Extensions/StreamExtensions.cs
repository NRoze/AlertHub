namespace AlertHub.Api.Extensions;

internal static class StreamExtensions
{
    extension(Stream stream)
    {
        public bool IsEmpty(int emptyLength)
        {
            return (stream is null || stream.Length <= emptyLength);
        }
    }
}

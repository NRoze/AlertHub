using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AlertHub.Api.Extensions
{
    static public class StreamExtensions
    {
        extension(Stream stream)
        {
            public bool IsEmpty(int emptyLength)
            {
                return (stream is null || stream.Length <= emptyLength);
            }
        }
    }
}

﻿using System;
using System.IO;

namespace ProtoBufExample
{
    public  class MessageHeaderContract
    {
        public static int FixedSize = 8;
        public readonly long HeaderBytes;

        public MessageHeaderContract(long headerBytes)
        {
            HeaderBytes = headerBytes;
        }

        public static MessageHeaderContract ReadHeader(byte[] buffer)
        {
            var headerBytes = BitConverter.ToInt64(buffer, 0);
            return new MessageHeaderContract(headerBytes);
        }

        public void WriteHeader(Stream stream)
        {
            stream.Write(BitConverter.GetBytes(HeaderBytes), 0, 8);
        }
    }
}
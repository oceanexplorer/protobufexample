using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ProtoBufExample
{
    public static class TapeStreamSerializer
    {
        private static readonly byte[] _readableHeaderStart = Encoding.UTF8.GetBytes("/* header ");
        private static readonly byte[] _readableHeaderEnd = Encoding.UTF8.GetBytes(" */\r\n");
        private static readonly byte[] _readableFooterStart = Encoding.UTF8.GetBytes("\r\n/* footer ");
        private static readonly byte[] _readableFooterEnd = Encoding.UTF8.GetBytes(" */\r\n");

        public static void WriteRecord(Stream stream, byte[] data, long versionToWrite)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            using (var managed = new SHA1Managed())
            {
                writer.Write(_readableHeaderStart);
                WriteReadableInt64(writer, data.Length);
                writer.Write(_readableHeaderEnd);

                writer.Write(data);
                writer.Write(_readableFooterStart);
                WriteReadableInt64(writer, data.Length);
                WriteReadableInt64(writer, versionToWrite);
                WriteReadableHash(writer, managed.ComputeHash(data));
                writer.Write(_readableFooterEnd);

                ms.Seek(0, SeekOrigin.Begin);
                ms.CopyTo(stream);
            }
        }

        public static TapeRecord ReadRecord(Stream file)
        {
            ReadAndVerifySignature(file, _readableHeaderStart, "Start");
            var dataLength = ReadReadableInt64(file);
            ReadAndVerifySignature(file, _readableHeaderEnd, "Header-End");

            var data = new byte[dataLength];
            file.Read(data, 0, (int)dataLength);

            ReadAndVerifySignature(file, _readableFooterStart, "Footer-Start");

            ReadReadableInt64(file);

            var recVersion = ReadReadableInt64(file);
            var hash = ReadReadableHash(file);

            using (var managed = new SHA1Managed())
            {
                var computed = managed.ComputeHash(data);

                if (!computed.SequenceEqual(hash))
                    throw new InvalidOperationException("Hash Corrupted");
            }

            ReadAndVerifySignature(file, _readableFooterEnd, "End");

            return new TapeRecord(recVersion, data);
        }

        private static void WriteReadableHash(BinaryWriter writer, byte[] hash)
        {
            var buffer = Encoding.UTF8.GetBytes(Convert.ToBase64String(hash));
            writer.Write(buffer);
        }

        private static void WriteReadableInt64(BinaryWriter writer, long value)
        {
            var buffer = Encoding.UTF8.GetBytes(value.ToString("x16"));
            writer.Write(buffer);
        }

        private static long ReadReadableInt64(Stream stream)
        {
            var buffer = new byte[16];
            stream.Read(buffer, 0, 16);

            var s = Encoding.UTF8.GetString(buffer);

            return Int64.Parse(s, NumberStyles.HexNumber);
        }

        private static IEnumerable<byte> ReadReadableHash(Stream stream)
        {
            var buffer = new byte[28];
            stream.Read(buffer, 0, buffer.Length);

            var hash = Convert.FromBase64String(Encoding.UTF8.GetString(buffer));

            return hash;
        }

        public static void ReadAndVerifySignature(Stream source, byte[] signature, string name)
        {
            for (var i = 0; i < signature.Length; i++)
            {
                var readByte = source.ReadByte();

                if (readByte == -1)
                {
                    throw new InvalidOperationException($"Expected byte[{i}] of signature '{name}', but found EOL");
                }

                if (readByte != signature[i])
                {
                    throw new InvalidOperationException($"Signature Failed: {name}");
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.IO;

namespace ProtoBufExample
{
    public class FileTapeStream
    {
        private readonly FileInfo _file;

        public FileTapeStream(string name)
        {
            _file = new FileInfo(name);
        }

        private FileStream OpenForWrite()
        {
            return _file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        private FileStream OpenForRead()
        {
            return _file.OpenRead();
        }

        public IEnumerable<TapeRecord> ReadRecords()
        {
            var records = new List<TapeRecord>();

            using (var file = OpenForRead())
            {
                while (file.Position < file.Length)
                {
                    var record = TapeStreamSerializer.ReadRecord(file);
                    records.Add(record);
                }

            }

            return records;
        }

        public void Append(byte[] buffer)
        {
            using (var file = OpenForWrite())
            {
                var versionToWrite = 1;
                TapeStreamSerializer.WriteRecord(file, buffer, versionToWrite);
            }
        }
    }
}

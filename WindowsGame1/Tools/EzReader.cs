using System.IO;
using System.Text;

namespace CloudberryKingdom
{
    public class EzReader
    {
        public string FileName;

        FileStream stream;
        public BinaryReader reader;

        public EzReader(string FileName)
        {
            this.FileName = FileName;

            stream = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
            reader = new BinaryReader(stream, Encoding.UTF8);
        }

        public void Dispose()
        {
            reader.Close();
            stream.Close();
        }
    }
}
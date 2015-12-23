using System.IO;
using System.Text;

namespace CloudberryKingdom
{
    public class CoreReader
    {
        public string FileName;

        FileStream stream;
        public BinaryReader reader;

        public CoreReader(string FileName)
        {
            this.FileName = FileName;

            Tools.UseInvariantCulture();
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
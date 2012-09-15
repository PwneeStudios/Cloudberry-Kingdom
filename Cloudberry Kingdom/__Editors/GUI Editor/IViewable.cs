using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom
{
    public class InstancePlusName
    {
        public ViewReadWrite instance; public string name;
        public InstancePlusName(ViewReadWrite i, string n)
        {
            instance = i; name = n;
        }
    }

    public interface IReadWrite
    {
        void Write(StreamWriter writer);
        void Read(StreamReader reader);

        void WriteCode(string prefix, StreamWriter writer);
    }

    public class ViewReadWrite : IReadWrite
    {
        public virtual string GetConstructorString()
        {
            return null;
        }

        public virtual string[] GetViewables() { return new string[] { }; }

        public virtual void Write(StreamWriter writer)
        {
            Tools.WriteFields(this, writer, GetViewables());
        }

        public virtual void Read(StreamReader reader)
        {
            Tools.ReadFields(this, reader);
        }

        public virtual void WriteCode(string prefix, StreamWriter writer)
        {
            Tools.WriteFieldsToCode(this, prefix, writer, GetViewables());
        }

        public virtual void ProcessMouseInput(Vector2 shift, bool ShiftDown)
        {
        }

        public virtual string CopyToClipboard(string suffix)
        {
            return "";
        }
    }

    public interface IViewableList
    {
        void GetChildren(List<InstancePlusName> ViewableChildren);
    }
}
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public class InstancePlusName
    {
        public IViewable instance; public string name;
        public InstancePlusName(IViewable i, string n)
        {
            instance = i; name = n;
        }
    }

    public interface IViewable
    {
        string[] GetViewables();
    }

    public interface IViewableList
    {
        void GetChildren(List<InstancePlusName> ViewableChildren);
    }
}
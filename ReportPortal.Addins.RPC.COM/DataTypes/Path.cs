using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM.DataTypes
{
    [ComVisible(false)]
    public class Path : IEnumerable<Location>
    {
        private readonly List<Location> _testNames;

        public Path(string testFullName)
        {
            var names = testFullName.Split(Constants.PathSeparator);
            _testNames = new List<Location>(names.Length);
            for (int i = 0; i < names.Length; i++)
            {
                _testNames.Add(new Location(names[i], i));
            }
        }

        public int Length => _testNames.Count;

        public IEnumerator<Location> GetEnumerator()
        {
            return _testNames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    [ComVisible(false)]
    public struct Location
    {
        public Location(string name, int index)
        {
            Name = name;
            Index = index;
        }

        public string Name { get; private set; }
        public int Index { get; private set; }
    }
}
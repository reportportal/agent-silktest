using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportPortal.Addins.RPC.COM
{
    class LengthComparer : IComparer<String>
    {
        public int Compare(string x, string y)
        {
            int lengthComparison = y.Length.CompareTo(x.Length);
            if (lengthComparison == 0)
            {
                return y.CompareTo(x);
            }
            else
            {
                return lengthComparison;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    internal class LengthComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == null)
            {
                return y == null? 0 : 1;
            }
            if (y == null)
            {
                return -1;
            }
            int lengthComparison = y.Length.CompareTo(x.Length);
            if (lengthComparison == 0)
            {
                lengthComparison = string.Compare(y, x, StringComparison.Ordinal);
            }
            return lengthComparison;
        }
    }
}

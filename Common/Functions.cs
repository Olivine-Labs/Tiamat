using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    //Contains utility functions
    public class Functions
    {
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return diff.TotalSeconds;
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;

namespace spf3
{
    abstract class Report
    {
        public Report()
        {
            Spf = new Specification();
        }

        public Report(Specification t)
        {
            Spf = t;
        }

        public abstract void Save();

        public Specification Spf
        {
            get; set;
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return "";
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        static Dictionary<string, int> Range;
        public const int R = 153;
        public const int G = 153;
        public const int B = 153;
    }
}

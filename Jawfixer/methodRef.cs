using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jawfixer
{
    public class methodRef
    {
        public string methodName { get; set; }

        public string alias { get; set; }

        public int line { get; set; }

        public methodRef(string input, int i)
        {
            string[] aInput = input.Split('=');

            alias = aInput[0];
            methodName = aInput[1];

            line = i;
        }
    }
}

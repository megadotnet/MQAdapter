using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messag.Utility
{
    public class TreeEntity<T>
    {
        public int id { get; set; }
        public string text { get; set; }
        public string state { get; set; }
        public bool @checked { get; set; }
        public  IEnumerable<TreeEntity<T>> children { get; set; }

        public Attributes attributes { get; set; }

        public T Entity { get; set; }

        public int Depth { get; set; }
    }

    public class Attributes
    {
        public string typecode { get; set; }
        public Nullable<int> parentid { get; set; }
        public string url { get; set; }
    }
}

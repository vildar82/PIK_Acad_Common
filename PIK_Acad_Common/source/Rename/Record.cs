using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_Acad_Common.Rename
{
    public class Record
    {
        public string Name { get; set; }
        public string Rename { get; set; }
        public string Error { get; set; }

        public Record (string name)
        {
            Name = name;            
        }
    }
}

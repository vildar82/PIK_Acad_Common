using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_Acad_Common.Utils.BlockBeside
{
    public class BlockModel
    {
        public BlockModel(string blName)
        {
            Name = blName;
        }

        public string Name { get; private set; }
        public bool IsSelected { get; set; }
    }
}

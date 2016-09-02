using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_Acad_Common.ExportBlock.Targets
{
    public interface IExportTarget
    {
        void Export (DataTable exportData);
    }
}

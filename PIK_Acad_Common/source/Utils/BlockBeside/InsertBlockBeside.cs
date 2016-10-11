using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Autodesk.AutoCAD.ApplicationServices;

namespace PIK_Acad_Common.Utils.BlockBeside
{
    /// <summary>
    /// Вставка блоков в ряд
    /// </summary>
    public static class InsertBlockBeside
    {
        public static InsertBlockBesideWindow win;

        public static void Insert (Document doc)
        {
            win = new InsertBlockBesideWindow();            

            Application.ShowModalWindow(win);            
        }
    }
}

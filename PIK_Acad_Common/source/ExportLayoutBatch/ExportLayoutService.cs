using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_Acad_Common.ExportLayoutsBatch
{
    public class ExportLayoutService
    {
        /// <summary>
        /// Экспорт листов из файлов в выбранной папке в один файл в Модель
        /// </summary>
        public void Export ()
        {
            // Листы
            using (var sheetServ = new SheetService())
            {
                var fileSheets = sheetServ.Select();

                // Сборка в общий файл
                var collect = new CollectService();
                collect.Collect(fileSheets);
            }
        }
    }
}

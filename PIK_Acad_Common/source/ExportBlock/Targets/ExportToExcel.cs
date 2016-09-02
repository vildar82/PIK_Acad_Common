using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcadLib;
using OfficeOpenXml;

namespace PIK_Acad_Common.ExportBlock.Targets
{
    class ExportToExcel : IExportTarget
    {
        public void Export (DataTable data)
        {            
            string file = GetFile();
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            using (var xls = new ExcelPackage())
            { 
                var ws = xls.Workbook.Worksheets.Add("Блоки");

                int r = 1;
                int c = 1;                                
                foreach (DataColumn col in data.Columns)
                {   
                    ws.Cells[r, c].Value = col.ColumnName;
                    c++;
                }

                r++;                                
                foreach (DataRow row in data.Rows)
                {
                    c = 1;                    
                    foreach (DataColumn col in data.Columns)
                    {                        
                        ws.Cells[r, c].Value = row[col.ColumnName];
                        c++;
                    }
                    r++;
                }
                xls.SaveAs(new FileInfo (file));
            }
        }

        private string GetFile ()
        {            
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = true;
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Файл экспорта блоков (*.xlsx)|*.xlsx";
            dlg.RestoreDirectory = true;            
            dlg.FileName = "ExportBlocks";
            dlg.Title = "Сохранение файла экспорта";
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                throw new CancelByUserException();
            }
            return dlg.FileName;
        }
    }
}

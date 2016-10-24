using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PIK_Acad_Common.ExportLayoutsBatch
{
    /// <summary>
    /// Листы - экспорт
    /// </summary>
    public class SheetService : IDisposable
    {
        private string tempFolder;        

        public List<FileSheets> Select ()
        {
            // Выбор папки с файлами dwg для экспорта
            var filesDwg = GetFiles();
            // Определение листов для экспорта
            var filesSheets = GetFilesSheets(filesDwg);
            return filesSheets;
        }

        private List<FileSheets> GetFilesSheets (List<FileInfo> filesDwg)
        {
            var fileSheets = new List<FileSheets>();

            // Папка в темпе
            tempFolder = AcadLib.IO.Path.GetTemporaryDirectory();

            foreach (var item in filesDwg)
            {
                var sheet = new FileSheets(item);
                sheet.Export(tempFolder);
                fileSheets.Add(sheet);
            }

            return fileSheets;
        }

        /// <summary>
        /// Выбор папки с файлами dwg для экспорта листов
        /// </summary>
        /// <returns>Список файлов</returns>
        private List<FileInfo> GetFiles ()
        {
            // Выбор папки
            var dlg = new AcadLib.UI.FileFolderDialog();
            dlg.IsFolderDialog = true;
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                throw new AcadLib.CancelByUserException();
            }

            List<FileInfo> files;
            string firstFileNameWoExt = Path.GetFileNameWithoutExtension(dlg.Dialog.FileNames.First());
            if (!firstFileNameWoExt.Equals("п", StringComparison.OrdinalIgnoreCase))
            {
                var filesS = dlg.Dialog.FileNames;
                files = new List<FileInfo>();   
                foreach (var item in filesS)
                {
                    FileInfo fi = new FileInfo(item);
                    if (fi.Extension.Equals ("dwg", StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(fi);
                    }
                }
            }
            else
            {
                DirectoryInfo dir = new DirectoryInfo(dlg.SelectedPath);
                files = dir.GetFiles("*.dwg").ToList();
            }
            return files;
        }

        public void Dispose ()
        {
            if (tempFolder != null)
            {
                try
                {
                    Directory.Delete(tempFolder, true);
                }
                catch { }                             
            }
        }
    }
}

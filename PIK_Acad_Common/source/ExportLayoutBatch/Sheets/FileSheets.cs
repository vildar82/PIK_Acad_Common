using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.XRef;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using AcadLib;

namespace PIK_Acad_Common.ExportLayoutsBatch
{
    public class FileSheets
    {
        private FileInfo file;        
        private List<string> exportLayoutFiles;
        private List<ObjectId> idsXref;        

        public FileSheets (FileInfo file)
        {
            this.file = file;
        }

        /// <summary>
        /// Вставка ссылки в текущий чертеж
        /// </summary>        
        public void Insert (ref Point3d ptInsert, Database db)
        {
            idsXref = new List<ObjectId>();
            foreach (var item in exportLayoutFiles)
            {
                ObjectId idXrefBtr;
                ObjectId idXrefBlRef;
                var blNameXref = GetName(item);
                if (db.XrefAttachAndInsert(item, ptInsert, out idXrefBtr,out idXrefBlRef, blNameXref))
                {
                    idsXref.Add(idXrefBtr);
                    // смещение точки
                    OffsetPoint(ref ptInsert, idXrefBlRef);
                }
            }
        }

        public void Bind (Database db)
        {   
            ObjectIdCollection xrefCollection = new ObjectIdCollection(idsXref.ToArray());                        
            if (xrefCollection.Count != 0)
                db.BindXrefs(xrefCollection, true);
        }

        private void OffsetPoint (ref Point3d ptInsert, ObjectId idXrefBlRef)
        {
            using (var t = idXrefBlRef.Database.TransactionManager.StartOpenCloseTransaction())
            {
                var xref = t.GetObject(idXrefBlRef, OpenMode.ForRead, false, true) as BlockReference;
                var ext = xref.GeometricExtents;
                ptInsert = new Point3d(ptInsert.X + (ext.MaxPoint.X - ext.MinPoint.X) + Options.OffsetX, ptInsert.Y, 0);
            }
        }

        /// <summary>
        /// Экспорт листов данного файла в отдельную папку в указанной папке
        /// </summary>
        /// <param name="parentFolder"></param>
        public void Export (string parentFolder)
        {
            if (string.IsNullOrEmpty(parentFolder)) return;
            // Открыть файл
            var docFile = Application.DocumentManager.Open(file.FullName, true);
            Application.DocumentManager.MdiActiveDocument = docFile;
            ExportLayoutTT.ExportLayouts(parentFolder, out exportLayoutFiles);
            docFile.CloseAndDiscard();
        }

        private string GetName (string file)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            name = name.GetValidDbSymbolName();
            return name;
        }
    }
}

using AcadTest.GeneratePreview;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadTest
{
    public class Commands
    {
        [CommandMethod (nameof(Test))]
        public void Test ()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var db = doc.Database;

            var blocks = new BlocksModel(db);
            var win = new BlocksView(blocks);
            try
            {
                if (Application.ShowModalWindow(win) == true)
                {
                    var selBlocks = blocks.Selected;
                    using (var t = db.TransactionManager.StartTransaction())
                    {
                        var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
                        foreach (var item in selBlocks)
                        {
                            var blRef = new BlockReference(Point3d.Origin, item.Id);
                            cs.AppendEntity(blRef);
                            t.AddNewlyCreatedDBObject(blRef, true);
                        }
                        t.Commit();
                    }
                }
            }
            catch
            {

            }
        }
    }
}

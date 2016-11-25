using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using AcadLib;
using Autodesk.AutoCAD.EditorInput;
using System.Collections;

namespace PIK_Acad_Common.Utils.BlockBeside
{
    public class BlockBesideModel
    {
        private List<string> blocks;
        private string filter;

        public BlockBesideModel ()
        {
            // Считывание блоков чертежа
            blocks = LoadBlocks();
            blocks.Sort();
            Load();
        }        

        public List<string> Blocks { get; set; }
        public string Filter {
            get { return filter; }
            set {
                filter = value;
                UpdateFilter();
            }
        }
        
        public BesideOrientation Orient { get; set; }        

        private void UpdateFilter ()
        {
            List<string> res = new List<string>();
            if (string.IsNullOrEmpty(Filter))
            {
                res = blocks;
            }
            else
            {
                try
                {
                    res = blocks.Where(b => Regex.IsMatch(b, Filter, RegexOptions.IgnoreCase)).ToList();
                }
                catch
                {
                    res = blocks;
                }
            }

            Blocks = res;
        }

        private List<string> LoadBlocks ()
        {
            var res = new HashSet<string>();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            using (var t = db.TransactionManager.StartTransaction())
            {                
                var selImplRes = ed.SelectImplied();
                if (selImplRes.Status == PromptStatus.OK && selImplRes.Value.Count>0)
                {
                    foreach (var item in selImplRes.Value.GetObjectIds())
                    {
                        var blRef = item.GetObject(OpenMode.ForRead) as BlockReference;
                        if (blRef == null) continue;
                        var blName = blRef.GetEffectiveName();
                        res.Add(blName);
                    }
                }
                else
                {
                    var bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                    foreach (var item in bt)
                    {
                        var btr = item.GetObject(OpenMode.ForRead) as BlockTableRecord;
                        if (btr == null ||
                            btr.IsLayout ||
                            btr.Name.StartsWith("*")) continue;
                        res.Add(btr.Name);
                    }
                }    
                t.Commit();
            }
            return res.ToList();
        }

        public void Insert (List<string> insertBlocks)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;
            List<ObjectId> ids = new List<ObjectId>();

            Point3d pt = Point3d.Origin;
            Extents3d extOld = new Extents3d(pt, pt);
            double length=0;

            using (doc.LockDocument())
            using (var t = db.TransactionManager.StartTransaction())
            {
                var bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                var cs = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;                
                
                foreach (var block in insertBlocks)
                {
                    if (bt.Has(block))
                    {                        
                        var blRef = AcadLib.Blocks.BlockInsert.InsertBlockRef(block, pt, cs, t);                                                
                        if (blRef.Bounds == null)
                        {                            
                        }
                        else
                        {
                            var extNew = blRef.Bounds.Value;
                            if (extOld.Diagonal() > 0)
                            {
                                Matrix3d mat;
                                if (Orient == BesideOrientation.Столбик)
                                {
                                    var h = extNew.MaxPoint.Y - extNew.MinPoint.Y;
                                    var l = extNew.MaxPoint.X - extNew.MinPoint.X;
                                    if (l > length) length = l;
                                    var vec = extOld.MinPoint - new Point3d(extNew.MinPoint.X, extNew.MaxPoint.Y + h * 0.1, 0);
                                    mat = Matrix3d.Displacement(vec);
                                }
                                else
                                {
                                    var h = extNew.MaxPoint.Y - extNew.MinPoint.Y;
                                    var l = extNew.MaxPoint.X - extNew.MinPoint.X;
                                    if (h > length) length = h;
                                    var vec = new Point3d(extOld.MaxPoint.X + l * 0.1, extOld.MinPoint.Y,0) - extNew.MinPoint;
                                    mat = Matrix3d.Displacement(vec);
                                }
                                blRef.TransformBy(mat);
                                extNew.TransformBy(mat);
                            }
                            extOld = extNew;
                        }
                        ids.Add(blRef.Id);
                    }
                }

                using (var UI = ed.StartUserInteraction(InsertBlockBeside.win))
                {
                    AcadLib.Jigs.DragSel.Drag(ed, ids.ToArray(), Point3d.Origin);
                }
                t.Commit();
            }            
            Save();
            Logger.Log.Info($"Insert {insertBlocks?.Count}, Filter={Filter}");
        }

        private void Load ()
        {
            var nod = new DictNOD("InsertBlocksBeside", true);
            Orient = (BesideOrientation)nod.Load("Orient", (int)BesideOrientation.Столбик);
            Filter = nod.Load("Filter", "");
        }

        private void Save ()
        {
            var nod = new DictNOD("InsertBlocksBeside", true);
            nod.Save((int)Orient, "Orient");
            nod.Save(Filter, "Filter");            
        }
    }
}

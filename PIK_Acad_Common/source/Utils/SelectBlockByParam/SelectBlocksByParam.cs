using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using AcadLib;
using PIK_Acad_Common.Utils.SelectBlockByParam.UI;

namespace PIK_Acad_Common.Utils.SelectBlockByParam
{
    /// <summary>
    /// Выбор одинаковых блоков по атрибутам
    /// </summary>
    public static class SelectBlocksByParam
    {
        private const string MenuName = "Выброр по параметрам";
        private static RXClass RxClassBlockRef = RXObject.GetClass(typeof(BlockReference));
        public static SelectBlocksByParamOptions Options = SelectBlocksByParamOptions.Load();
        public static BlockBase blBase;
        public static void AttachContextMenu ()
        {
            var cme = new ContextMenuExtension();
            var menu = new MenuItem(MenuName);
            menu.Click += (o, e) => SelectBlockByParameters();
            cme.MenuItems.Add(menu);
            //cme.MenuItems.Add(new MenuItem(""));            
            cme.Popup += Cme_Popup;
            Application.AddObjectContextMenuExtension(RxClassBlockRef, cme);
        }       

        private static void Cme_Popup (object sender, EventArgs e)
        {
            var contextMenu = sender as ContextMenuExtension;
            if (contextMenu != null)
            {                
                var menu = contextMenu.MenuItems[0];
                // Проверить выбран ли один блок у которого есть аттрибуты                
                if (IsCorrectImpliedSel())
                {
                    menu.Enabled = true;
                    menu.Visible = true;
                }
                else
                {
                    menu.Enabled = false;
                    menu.Visible = false;
                }                
            }
        }

        public static bool IsCorrectImpliedSel()
        {
            bool res = false;
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return res;
            var db = doc.Database;
            var ed = doc.Editor;            
            var selImpl = ed.SelectImplied();
            if (selImpl.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK && selImpl.Value.Count == 1)
            {
                using (var t = db.TransactionManager.StartTransaction())
                {
                    var blRef = selImpl.Value[0].ObjectId.GetObject(OpenMode.ForRead) as BlockReference;
                    if (blRef != null)
                    {
                        blBase = new BlockBase(blRef, blRef.GetEffectiveName());
                        res = true;
                    }
                    t.Commit();
                }
            }
            return res;
        }

        /// <summary>
        /// Выбор блоков по атрибутам
        /// </summary>
        public static void SelectBlockByParameters ()
        {
            if (blBase == null)
            {
                return;
            }

            CommandStart.Start(doc =>
            {
                var selBlVM = new SelectBlockViewModel(blBase);
                var selBlView = new SelectBlockView(selBlVM);
                if (Application.ShowModalWindow(selBlView) == true)
                {
                    var db = doc.Database;
                    var ed = doc.Editor;
                    // Выбор на чертеже блоков с такимиже атрибутами
                    var idsFiltered = Filter(selBlVM.SelectedProperties, db);
                    if (idsFiltered != null && idsFiltered.Any())
                    {
                        ed.SetImpliedSelection(idsFiltered.ToArray());
                        Logger.Log.Info($"SelectBlockByAttr: {blBase.BlName} - {idsFiltered.Count}, {string.Join("; ", selBlVM.SelectedProperties.Select(s => $"{s.Name} = {s.Value}"))}");
                    }
                }
            });
        }

        private static List<ObjectId> Filter (List<Property> selectedProperties, Database db)
        {
            var idsFiltered = new List<ObjectId>();   
            using (var t = db.TransactionManager.StartTransaction())
            {
                var cs = blBase.IdBtrOwner.GetObject(OpenMode.ForRead) as BlockTableRecord;
                var blocksBase = new List<BlockBase>();
                foreach (var entId in cs)
                {
                    if (entId.IsValidEx())
                    {
                        var blRef = entId.GetObject(OpenMode.ForRead) as BlockReference;
                        if (blRef == null) continue;
                        var blName = blRef.GetEffectiveName();
                        if (blName == blBase.BlName)
                        {
                            blocksBase.Add(new BlockBase(blRef, blName));
                        }
                    }
                }
                if (blocksBase.Any())
                {
                    // группировка по параметрам
                    foreach (var item in blocksBase)
                    {
                        bool isOk = true;
                        foreach (var selProp in selectedProperties)
                        {
                            var propBase = item.Properties.Find(p => p.Equals(selProp));
                            if (propBase == null)
                            {
                                isOk = false;
                                break;
                            }
                        }
                        if (isOk)
                            idsFiltered.Add(item.IdBlRef);
                    }
                }
                t.Commit();
            }
            return idsFiltered;
        }
    }
}

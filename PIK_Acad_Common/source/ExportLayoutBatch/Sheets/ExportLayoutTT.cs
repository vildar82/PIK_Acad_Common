using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib;
using AcadLib.Errors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using AcExportLayout = Autodesk.AutoCAD.ExportLayout;

namespace PIK_Acad_Common.ExportLayoutsBatch
{
    public static class ExportLayoutTT
    {
        public static void ExportLayouts (string folder, out List<string> exportLayouts)
        {
            exportLayouts = new List<string>();
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var editor = doc.Editor;
            if ((short)Application.GetSystemVariable("DWGTITLED") == 0)
            {
                editor.WriteMessage("\nCommand cannot be used on an unnamed drawing");
                return;
            }
            string path = Path.Combine(folder, Path.GetFileNameWithoutExtension(doc.Name));

            Dictionary<string, ObjectId> layouts = null;

            using (doc.LockDocument())
            {
                using (var t = doc.TransactionManager.StartTransaction())
                {
                    // Get the localized name of the model tab:
                    var btr = (BlockTableRecord)SymbolUtilityServices.GetBlockModelSpaceId(db).GetObject(OpenMode.ForRead);
                    var layout = (Layout)btr.LayoutId.GetObject(OpenMode.ForRead);
                    string model = layout.LayoutName;
                    // Open the Layout dictionary:
                    var layoutDictionary = (IDictionary)db.LayoutDictionaryId.GetObject(OpenMode.ForRead);
                    // Get the names and ids of all paper space layouts 
                    // into a Dictionary<string,ObjectId>:
                    layouts = layoutDictionary.Cast<DictionaryEntry>()
                       .Where(e => ((string)e.Key) != model)
                       .ToDictionary(
                          e => (string)e.Key,
                          e => (ObjectId)e.Value);

                    t.Commit();
                }

                /// Get the export layout 'engine':
                var engine = AcExportLayout.Engine.Instance();

                using (new ManagedSystemVariable("CTAB"))
                {
                    var entry = layouts.First();
                    //foreach (var entry in layouts)
                    {
                        try
                        {
                            string filename = $"{path}_{entry.Key}.dwg";
                            editor.WriteMessage($"\nЭкспорт листа {entry.Key} => {filename}");
                            Application.SetSystemVariable("CTAB", entry.Key);
                            using (Database database = engine.ExportLayout(entry.Value))
                            {
                                if (engine.EngineStatus == AcExportLayout.ErrorStatus.Succeeded)
                                {
                                    database.SaveAs(filename, DwgVersion.Newest);
                                    exportLayouts.Add(filename);
                                }
                                else
                                {
                                    Inspector.AddError($"\nОшибка экспорта листа {entry.Key} из файла {Path.GetFileName(doc.Name)}: {engine.EngineStatus}",
                                        System.Drawing.SystemIcons.Error);
                                    editor.WriteMessage($"\nОшибка экспорта листа: {engine.EngineStatus}");
                                    //break;
                                }
                            }
                        }
                        catch
                        {
                            Inspector.AddError($"Ошибка экспорта листа {entry.Key} из файла {Path.GetFileName(doc.Name)}");
                        }
                    }
                }
            }
        }
    }
}

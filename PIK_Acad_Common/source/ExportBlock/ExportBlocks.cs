using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using AcadLib.Blocks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using PIK_Acad_Common.ExportBlock.Targets;

namespace PIK_Acad_Common.ExportBlock
{
    internal class ExportBlocks
    {
        Document doc;
        Database db;
        Editor ed;

        string prefixSpecialty;

        public ExportBlocks (Document doc)
        {
            this.doc = doc;
            ed = doc.Editor;
            db = doc.Database;
        }

        public void Export (IExportTarget target)
        {
            // Выбор блоков
            var blocks = SelectBlocks();

            // Группировка блоков
            var groupsBlocks = blocks.GroupBy(g => g).OrderBy(o=>o.Key.BlName);

            // Данные для экспорта
            var exportData = GetExportData(groupsBlocks);

            // Експорт данных
            target.Export(exportData, "ExportBlocks_" + Path.GetFileNameWithoutExtension(doc.Name));
        }
        
        private List<IBlock> SelectBlocks ()
        {
            var blRefs = ed.SelectBlRefs("\nВыбор блоков для экспорта");

            // префикс имени блока данной специальности
            prefixSpecialty = AutoCAD_PIK_Manager.Settings.PikSettings.UserGroup + "_";

            List<IBlock> blocks = new List<IBlock>();
            // Создание описаний блоков
            using (var t = db.TransactionManager.StartTransaction())
            {
                foreach (var idEnt in blRefs)
                {
                    var blRef = idEnt.GetObject(OpenMode.ForRead) as BlockReference;
                    if (blRef == null) continue;

                    string blName = blRef.GetEffectiveName();
                    if (blName.StartsWith(prefixSpecialty))
                    {
                        var block = new BlockBase(blRef, blName);                        
                        blocks.Add(block);
                    }                    
                }
                t.Commit();
            }
            return blocks;
        }

        private System.Data.DataTable GetExportData (IEnumerable<IGrouping<IBlock, IBlock>> groupsBlocks)
        {
            System.Data.DataTable data = new System.Data.DataTable($"Блоки {prefixSpecialty} из файла {doc.Name}, {DateTime.Now}");            
            var uniqProperties = groupsBlocks.SelectMany(s => s.Key.Properties).GroupBy(g => g.Name).Select(r => r.Key).
                OrderBy(o=>o).ToList();

            // Общие свойства - имя блока, кол
            data.Columns.Add("№", typeof(int));
            data.Columns.Add("Имя блока", typeof(string));
            data.Columns.Add("Кол", typeof(int));
            
            foreach (var cname in uniqProperties)
            {
                data.Columns.Add(cname);
            }

            int count = 1;
            foreach (var blocks in groupsBlocks)
            {
                var row = data.NewRow();
                row[0] = count++;
                row[1] = blocks.Key.BlName;
                row[2] = blocks.Count();
                foreach (var item in blocks.Key.Properties)
                {
                    row[item.Name] = item.Value;
                }
                data.Rows.Add(row);
            }
            return data;
        }
    }
}
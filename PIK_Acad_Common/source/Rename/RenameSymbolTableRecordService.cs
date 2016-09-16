using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PIK_Acad_Common.Rename.UI;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace PIK_Acad_Common.Rename
{
    public enum SymbolTableEnum
    {
        Blocks,
        Layers
    }

    public class RenameSymbolTableRecordService
    {
        Database db;
        
        public void Rename (Database db)
        {
            this.db = db;            
            
            RenameView view = new RenameView(this);
            RenameWindow window = new RenameWindow(view);
            var resDlg = Application.ShowModalWindow(window);            
        }       

        public List<Record> GetAllRecords (SymbolTableEnum table)
        {            
            List<Record> recs = new List<Record>();

            var stId = GetSymbolTable(table);
            if (stId.IsNull) return recs;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var st = stId.GetObject(OpenMode.ForRead) as SymbolTable;
                foreach (var item in st)
                {
                    var stRec = item.GetObject(OpenMode.ForRead) as SymbolTableRecord;
                    if (!stRec.IsDependent)
                    {
                        Record rec = new Record(stRec.Name);
                        recs.Add(rec);
                    }
                }
                t.Commit();
            }
            return recs;
        }

        public List<Record> GetRenameRecords (List<Record> records, string search, string replace)
        {
            List<Record> renamed = new List<Record>();
            if (string.IsNullOrEmpty(replace)) return renamed;
            foreach (var item in records)
            {
                var replaced = Regex.Replace(item.Name, search, replace);
                if (replaced != item.Name)
                {
                    item.Rename = replaced;
                    renamed.Add(item);
                }
            }
            return renamed;
        }

        public static bool IsValidName (string name)
        {
            var res = name.IsValidDbSymbolName();
            return res;
        }

        public void RenameRecords (List<Record> renamedRecords, SymbolTableEnum table)
        {
            if (renamedRecords.Count == 0) return;

            using (var t = db.TransactionManager.StartTransaction())
            {
                var stId = GetSymbolTable(table);
                var st = stId.GetObject(OpenMode.ForRead) as SymbolTable;
                foreach (var item in renamedRecords)
                {
                    item.Error = string.Empty;
                    string tableName = st.GetRXClass().Name;
                    if (st.Has(item.Name))
                    {
                        if (st.Has(item.Rename))
                        {
                            item.Error = $"Переименованное имя уже есть в таблице";
                        }
                        else
                        {
                            if (IsValidName(item.Rename))
                            {
                                var rec = st[item.Name].GetObject(OpenMode.ForWrite) as SymbolTableRecord;
                                rec.Name = item.Rename;
                            }
                            else
                            {
                                item.Error = $"Недопустимое имя";
                            }
                        }
                    }
                    else
                    {
                        item.Error = $"Записи с таким именем нет в таблице";
                    }
                }
                t.Commit();
            }
        }

        public ObjectId GetSymbolTable (SymbolTableEnum table)
        {
            switch (table)
            {
                case SymbolTableEnum.Blocks:
                    return db.BlockTableId;
                case SymbolTableEnum.Layers:
                    return db.LayerTableId;
                default:
                    throw new Exception($"Символьная таблица не определена - {table}");
            }            
        }
    }
}

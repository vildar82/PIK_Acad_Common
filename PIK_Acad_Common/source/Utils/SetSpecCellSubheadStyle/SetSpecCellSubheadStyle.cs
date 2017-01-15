using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using AcadLib;
using Autodesk.AutoCAD.Runtime;

namespace PIK_Acad_Common.Utils
{
    public class SetSpecCellSubheadStyle
    {
        Document doc;
        Editor ed;
        Database db;
        RXClass rxTableClass = RXObject.GetClass(typeof(Table));
        static Dictionary<ObjectId, string> dictTextFonts = new Dictionary<ObjectId, string>();

        public SetSpecCellSubheadStyle(Document doc)
        {
            this.doc = doc;
            db = doc.Database;
            ed = doc.Editor;
        }

        public void SetCellSubheadStyle()
        {
            using (var t = db.TransactionManager.StartTransaction())
            {
                // Выбор или получение текущей ячейки
                Table table;
                var cellRange = SelectCell(out table);

                table.UpgradeOpen();                
                if (cellRange.IsSingleCell)
                {
                    var cell = cellRange.First();
                    SetCellSubheadStyle(table.Cells[cell.Row, cell.Column]);
                }
                else
                {
                    foreach (var item in cellRange)
                    {
                        var cell = table.Cells[item.Row, item.Column];
                        SetCellSubheadStyle(cell);
                    }
                }
                t.Commit();
            }
        }

        private void SetCellSubheadStyle (Cell cell)
        {
            cell.State = CellStates.None;
            cell.Alignment = CellAlignment.MiddleCenter;
            string font = getFontName(cell.TextStyleId, cell);
            cell.TextString =  $"{{\\fIsocpeur|b1;\\L{cell.TextString}}}";            
        }

        private string getFontName (ObjectId? cellTextStyleId, Cell cell)
        {
            string fontName;            
            if (cellTextStyleId == null)
            {
                fontName = "ISOCPEUR";
            }
            else
            {
                if (!dictTextFonts.TryGetValue(cellTextStyleId.Value, out fontName))
                {
                    var textStyle = cellTextStyleId.Value.GetObject(OpenMode.ForRead) as TextStyleTableRecord;
                    fontName = textStyle.Font.TypeFace;
                    dictTextFonts.Add(cellTextStyleId.Value, fontName);
                }
            }             
            return fontName;
        }

        private CellRange SelectCell (out Table table)
        {   
            var ptCell = ed.GetPointWCS("\nВыбор ячейки:");
            var idTable = GetTableInPoint(ptCell);

            var cell = getTableCell(idTable, ptCell, out table);
            if (cell == null)
            {
                throw new System.Exception("Не определена ячейка таблицы.");
            }
            return cell;
        }        

        private CellRange getTableCell (ObjectId item, Point3d pt, out Table table)
        {
            table = item.GetObject(OpenMode.ForRead) as Table;
            if (table.HasSubSelection)
            {
                return table.SubSelection;
            }
            else
            {
                if (pt != Point3d.Origin)
                {
                    var hit = table.HitTest(pt, Vector3d.ZAxis);
                    if (hit.Type == TableHitTestType.Cell)
                    {
                        CellRange range = CellRange.Create(table, hit.Row, hit.Column, hit.Row, hit.Column);
                        return range;                        
                    }
                }
            }
            return null;
        }

        private ObjectId GetTableInPoint (Point3d ptCell)
        {
            Point3d p = ptCell;
            Point3d p1 = new Point3d(p.X - 500, p.Y - 500, 0.0);
            Point3d p2 = new Point3d(p.X + 500, p.Y + 500, 0.0);

            var selRes = ed.SelectCrossingWindow(p1, p2);
            if (selRes.Status == PromptStatus.OK)
            {
                foreach (ObjectId item in selRes.Value.GetObjectIds())
                {
                    if (item.ObjectClass == rxTableClass)
                    {
                        return item;
                    }
                }
            }              
            throw new System.Exception($"Не определена таблица.");
        }
    }
}

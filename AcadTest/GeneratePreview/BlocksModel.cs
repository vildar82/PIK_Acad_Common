using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.ObjectModel;
using MicroMvvm;
using AcadLib;
using AcadLib.Blocks;
using PIK_Acad_Common.Utils.BlockBeside;

namespace AcadTest.GeneratePreview
{
    public class BlocksModel : ViewModelBase
    {
        private Database db;        

        public BlocksModel(Database db)
        {
            this.db = db;
            Blocks = GetBlocks(db);
            Ok = new RelayCommand(OnOkExecute, CanOkExecute);
        }        

        public ObservableCollection<Block> Blocks { get; set; }
        public List<Block> Selected { get; set; }
        public RelayCommand Ok { get; set; }

        private ObservableCollection<Block> GetBlocks(Database db)
        {
            var blocks = new ObservableCollection<Block>();
            using (var t = db.TransactionManager.StartTransaction())
            {
                var bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                foreach (var item in bt)
                {
                    var btr = item.GetObject(OpenMode.ForRead) as BlockTableRecord;
                    if (btr.IsUserBlock())
                    {
                        var block = new Block(btr);
                        blocks.Add(block);
                    }
                }
                t.Commit();
            }
            return blocks;            
        }

        private bool CanOkExecute()
        {
            return Selected.Any();
        }

        private void OnOkExecute()
        {
            // Ok
            Selected = Blocks.Where(s => s.IsSelected).ToList();
        }
    }
}

using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AcadTest.GeneratePreview
{
    public class GenerateBlockPreview
    {        
        Database db;
        CancellationTokenSource token;
        public GenerateBlockPreview(Database db)
        {            
            this.db = db;            
        }

        public void Cancel()
        {
            token?.Cancel();
        }

        public void Generate(List<Block> blocks)
        {
            token = new CancellationTokenSource();
            Task.Run(() => NewMethod(blocks, token.Token), token.Token);            
        }

        private void NewMethod(List<Block> blNames, CancellationToken token)
        {            
            using (var t = db.TransactionManager.StartTransaction())
            {
                var bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                foreach (var item in blNames)
                {
                    if (!token.IsCancellationRequested)
                    {
                        if (bt.Has(item.Name))
                        {
                            var btr = bt[item.Name].GetObject(OpenMode.ForRead) as BlockTableRecord;
                            var preview = AcadLib.Blocks.Visual.BlockPreviewHelper.GetPreview(btr);
                            item.Preview = preview;
                        }
                    }
                }
                t.Commit();
            }            
        }        
    }
}

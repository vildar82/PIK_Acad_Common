using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIK_Acad_Common.Utils.BlockBeside
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

        public async Task<Dictionary<string, System.Drawing.Image>> Generate(List<string> blNames)
        {
            token = new CancellationTokenSource();
            Task<Dictionary<string, System.Drawing.Image>> task = Task.Run(() => NewMethod(blNames, token.Token), token.Token);
            return await task;
        }

        private Dictionary<string, System.Drawing.Image> NewMethod(List<string> blNames, CancellationToken token)
        {
            var previews = new Dictionary<string, System.Drawing.Image>();
            using (var t = db.TransactionManager.StartTransaction())
            {
                var bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                foreach (var item in blNames)
                {
                    if (token.IsCancellationRequested)
                    {
                        if (bt.Has(item))
                        {
                            var btr = bt[item].GetObject(OpenMode.ForRead) as BlockTableRecord;
                            var preview = AcadLib.Blocks.Visual.BlockPreviewHelper.GetPreviewImage(btr);
                            previews.Add(item, preview);
                        }
                    }
                }
                t.Commit();
            }
            return previews;
        }

        private async Task TryTask()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(1));
            Task<int> task = Task.Run(() => slowFunc(1, 2, source.Token), source.Token);

            // (A canceled task will raise an exception when awaited).
            await task;
        }

        private int slowFunc(int a, int b, CancellationToken cancellationToken)
        {
            string someString = string.Empty;
            for (int i = 0; i < 200000; i++)
            {
                someString += "a";
                if (i % 1000 == 0)
                    cancellationToken.ThrowIfCancellationRequested();
            }

            return a + b;
        }
    }
}

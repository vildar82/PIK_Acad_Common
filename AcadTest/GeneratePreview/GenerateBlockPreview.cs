using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AcadTest.GeneratePreview
{
    public static class GenerateBlockPreview
    {   
        public static Task<Dictionary<string, ImageSource>> GeneratePreviewsAsync(List<string> blNames, Database db, 
            CancellationToken cancelationToken)
        {
            return Task.Run(() =>
            {
                var res = new Dictionary<string, ImageSource>();
                using (var bt = db.BlockTableId.Open(OpenMode.ForRead) as BlockTable)
                {
                    foreach (var item in blNames)
                    {
                        cancelationToken.ThrowIfCancellationRequested();

                        if (bt.Has(item))
                        {
                            using (var btr = bt[item].Open(OpenMode.ForRead) as BlockTableRecord)
                            {
                                var preview = AcadLib.Blocks.Visual.BlockPreviewHelper.GetPreview(btr);
                                preview.Freeze();
                                res.Add(item, preview);
                            }
                        }
                    }
                }
                return res;
            });
        }     
    }
}

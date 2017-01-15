using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.PlottingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIK_Acad_Common.Utils
{
    public static class ApplyPlotSettingsCurLayoutToAllLayout
    {
        public static void Apply()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            var db = doc.Database;
            using (var t = db.TransactionManager.StartTransaction())
            {
                var cs = db.CurrentSpaceId.GetObject(OpenMode.ForRead) as BlockTableRecord;
                if (!cs.IsLayout)
                    return;
                var curLayout = cs.LayoutId.GetObject( OpenMode.ForRead) as Layout;

                using (var curLPS = new PlotSettings(curLayout.ModelType))
                {
                    curLPS.CopyFrom(curLayout);

                    var dicLayouts = db.LayoutDictionaryId.GetObject(OpenMode.ForRead) as DBDictionary;

                    //var lm = LayoutManager.Current;
                    foreach (var item in dicLayouts)
                    {
                        if (item.Key != "Model" && item.Key != curLayout.LayoutName)
                        {
                            //lm.CurrentLayout = item.Key;
                            var layout = item.Value.GetObject(OpenMode.ForWrite) as Layout;
                            using (var lPS = new PlotSettings(layout.ModelType))
                            {
                                lPS.CopyFrom(layout);
                                SetPlotSettings(lPS, curLPS);
                                layout.CopyFrom(lPS);
                            }
                        }
                    }
                }
                t.Commit();
            }
        }

        private static void SetPlotSettings(PlotSettings destPlotSettings, PlotSettings sourcePlotSettings)
        {   
            destPlotSettings.DrawViewportsFirst = sourcePlotSettings.DrawViewportsFirst;
            destPlotSettings.PlotHidden = sourcePlotSettings.PlotHidden;
            destPlotSettings.PlotPlotStyles = sourcePlotSettings.PlotPlotStyles;
            destPlotSettings.PlotSettingsName = sourcePlotSettings.PlotSettingsName;
            destPlotSettings.PlotTransparency = sourcePlotSettings.PlotTransparency;
            destPlotSettings.PlotViewportBorders = sourcePlotSettings.PlotViewportBorders;
            destPlotSettings.PrintLineweights = sourcePlotSettings.PrintLineweights;
            destPlotSettings.ScaleLineweights = sourcePlotSettings.ScaleLineweights;
            destPlotSettings.ShadePlot = sourcePlotSettings.ShadePlot;
            destPlotSettings.ShadePlotCustomDpi = sourcePlotSettings.ShadePlotCustomDpi;
            destPlotSettings.ShadePlotResLevel = sourcePlotSettings.ShadePlotResLevel;
            destPlotSettings.ShowPlotStyles = sourcePlotSettings.ShowPlotStyles;
            using (var psValidator = PlotSettingsValidator.Current)
            {
                // psValidator.SetPlotType(destLayout, sourceLayout.PlotType); // ??? Область печати - просто не меняется, возможно придется задавать границы печати.
                // psValidator.SetCustomPrintScale(sourceLayout, sourceLayout.CustomPrintScale); // ??? масштаб просто не меняется
                if (destPlotSettings.PlotCentered != sourcePlotSettings.PlotCentered)
                    psValidator.SetPlotCentered(destPlotSettings, sourcePlotSettings.PlotCentered);
                if (!destPlotSettings.PlotOrigin.IsEqualTo(sourcePlotSettings.PlotOrigin))
                    psValidator.SetPlotOrigin(destPlotSettings, sourcePlotSettings.PlotOrigin);
                psValidator.RefreshLists(destPlotSettings);
            }                        
        }
    }
}

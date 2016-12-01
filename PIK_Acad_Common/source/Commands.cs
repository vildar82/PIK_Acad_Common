using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using AcadLib;
using System.Reflection;
using PIK_Acad_Common.ExportBlock;
using PIK_Acad_Common.ExportBlock.Targets;
using PIK_Acad_Common.Utils;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;

[assembly: CommandClass(typeof(PIK_Acad_Common.Commands))]
[assembly: ExtensionApplication(typeof(PIK_Acad_Common.Commands))]

namespace PIK_Acad_Common
{
    public class Commands : IExtensionApplication
    {        
        public const string Group = AutoCAD_PIK_Manager.Commands.Group;

        public void Initialize ()
        {
            Utils.SelectBlockByParam.SelectBlocksByParam.AttachContextMenu();

            LoadService.LoadMicroMvvm();

            //if (System.Windows.Application.Current == null)
            //{
            //    new System.Windows.Application { ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown };
            //}
            //System.Windows.Application.Current.Resources.MergedDictionaries.Add(System.Windows.Application.LoadComponent(
            //new Uri("PIK_Acad_Common;component/source/Dictionary1.xaml", UriKind.Relative)) as System.Windows.ResourceDictionary);
        }

        [CommandMethod(Group, nameof(PIK_Common_About), CommandFlags.Modal)]
        public void PIK_Common_About ()
        {
            CommandStart.Start(doc =>
            {
                doc.Editor.WriteMessage(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            });
        }

        [CommandMethod(Group, nameof(PIK_ExportBlocksBySpecialty), CommandFlags.Modal)]
        public void PIK_ExportBlocksBySpecialty ()
        {
            CommandStart.Start(doc =>
            {
                ExportBlocks exportBlocks = new ExportBlocks(doc);
                exportBlocks.Export(new ExportToExcel());
            });
        }

        [CommandMethod(Group, nameof(PIK_RenameSymbolTableRecords), CommandFlags.Modal)]
        public void PIK_RenameSymbolTableRecords ()
        {
            CommandStart.Start(doc =>
            {
                var rename = new Rename.RenameSymbolTableRecordService();
                rename.Rename(doc.Database);
            });
        }        

        /// <summary>
        /// Вставка блоков текущего чертежа в ряд (по заданному фиотру) в текущее пространство
        /// </summary>
        [CommandMethod(Group, nameof(PIK_InsertBlocksBeside), CommandFlags.Modal | CommandFlags.UsePickSet)]
        public void PIK_InsertBlocksBeside ()
        {
            CommandStart.Start(doc =>
            {
                Utils.BlockBeside.InsertBlockBeside.Insert(doc);
            });
        }

        [CommandMethod(Group, nameof(PIK_ExportLayoutsBatch), CommandFlags.Session)]
        public void PIK_ExportLayoutsBatch ()
        {
            CommandStart.Start(doc =>
            {
                var expLayots = new ExportLayoutsBatch.ExportLayoutService();
                expLayots.Export();
            });
        }

        [CommandMethod(Group, nameof(PIK_SelectBlockByParam), CommandFlags.Transparent | CommandFlags.UsePickSet | CommandFlags.Redraw)]
        public void PIK_SelectBlockByParam()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            if (Utils.SelectBlockByParam.SelectBlocksByParam.IsCorrectImpliedSel())
            {
                Utils.SelectBlockByParam.SelectBlocksByParam.SelectBlockByParameters();
            }
            else
            {
                var selOpt = new PromptEntityOptions("\nВыбор блока:");
                selOpt.SetRejectMessage("\nНужно выбрать блок.");
                selOpt.AddAllowedClass(typeof(BlockReference), true);
                selOpt.AllowNone = false;
                selOpt.AllowObjectOnLockedLayer = true;                
                var selRes = doc.Editor.GetEntity(selOpt);
                if (selRes.Status == PromptStatus.OK)
                {
                    using (var t = doc.TransactionManager.StartTransaction())
                    {
                        var blRef = selRes.ObjectId.GetObject(OpenMode.ForRead) as BlockReference;
                        if (blRef != null)
                        {
                            Utils.SelectBlockByParam.SelectBlocksByParam.blBase = new AcadLib.Blocks.BlockBase(blRef, blRef.GetEffectiveName());
                        }
                    }
                    Utils.SelectBlockByParam.SelectBlocksByParam.SelectBlockByParameters();
                }                
            }
        }

        public void Terminate ()
        {
            Utils.SelectBlockByParam.SelectBlocksByParam.Options.Save();
        }
    }
}

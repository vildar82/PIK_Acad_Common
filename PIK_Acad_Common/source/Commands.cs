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
using System.Windows;
using PIK_Acad_Common.Utils;

[assembly: CommandClass(typeof(PIK_Acad_Common.Commands))]
[assembly: ExtensionApplication(typeof(PIK_Acad_Common.Commands))]

namespace PIK_Acad_Common
{
    public class Commands : IExtensionApplication
    {        
        public const string Group = AutoCAD_PIK_Manager.Commands.Group;

        public void Initialize ()
        {
            Utils.SelectBlockByAttr.SelectBlocksByParam.AttachContextMenu();

            LoadService.LoadMicroMvvm();

            if (System.Windows.Application.Current == null)
            {
                new System.Windows.Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            }
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(System.Windows.Application.LoadComponent(
            new Uri("PIK_Acad_Common;component/source/Dictionary1.xaml", UriKind.Relative)) as ResourceDictionary);
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

        public void Terminate ()
        {            
        }
    }
}

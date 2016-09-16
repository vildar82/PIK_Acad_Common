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

[assembly: CommandClass(typeof(PIK_Acad_Common.Commands))]
[assembly: ExtensionApplication(typeof(PIK_Acad_Common.Commands))]

namespace PIK_Acad_Common
{
    public class Commands : IExtensionApplication
    {        
        public const string Group = AutoCAD_PIK_Manager.Commands.Group;

        public void Initialize ()
        {
            LoadService.LoadMicroMvvm();
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

        public void Terminate ()
        {            
        }
    }
}

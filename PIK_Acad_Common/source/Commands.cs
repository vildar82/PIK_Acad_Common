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

namespace PIK_Acad_Common
{
    public class Commands
    {        
        public const string Group = AutoCAD_PIK_Manager.Commands.Group;

        [CommandMethod(Group, "PIK_Common_About", CommandFlags.Modal)]
        public void About()
        {
            CommandStart.Start(doc =>
            {
                doc.Editor.WriteMessage(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            });
        }

        [CommandMethod(Group, "PIK_ExportBlocksBySpecialty", CommandFlags.Modal)]
        public void ExportBlocksBySpecialty ()
        {
            CommandStart.Start(doc =>
            {
                ExportBlocks exportBlocks = new ExportBlocks(doc);
                exportBlocks.Export(new ExportToExcel());
            });
        }
    }
}

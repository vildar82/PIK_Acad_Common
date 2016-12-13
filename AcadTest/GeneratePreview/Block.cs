using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadTest.GeneratePreview
{
    public class Block
    {
        public Block(BlockTableRecord btr)
        {
            Id = btr.Id;
            Name = btr.Name;            
        }

        public ObjectId Id { get; internal set; }
        public string Name { get; set; }
        public Image Preview { get; set; }
        public bool IsSelected { get; set; }
    }
}

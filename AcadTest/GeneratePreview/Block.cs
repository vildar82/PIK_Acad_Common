using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using MicroMvvm;

namespace AcadTest.GeneratePreview
{
    public class Block : ModelBase
    {
        public Block(BlockTableRecord btr)
        {
            Id = btr.Id;
            Name = btr.Name;            
        }

        public ObjectId Id { get; internal set; }
        public string Name { get; set; }
        public System.Windows.Media.ImageSource Preview {
            get { return preview; }
            set { preview = value; RaisePropertyChanged(); }
        }
        System.Windows.Media.ImageSource preview;
        public bool IsSelected { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using MicroMvvm;

namespace PIK_Acad_Common.Utils.SelectBlockByAttr.UI
{
    public class SelectBlockViewModel : ObservableObject
    {
        public SelectBlockViewModel () { }
        public SelectBlockViewModel (BlockBase blBase)
        {
            BlBase = blBase;
            Ok = new RelayCommand(OnOkExecute);
            Properties = blBase.Properties.Select(s => new PropertyViewModel(s)).ToList();
        }        

        public BlockBase BlBase { get; set; }
        public List<PropertyViewModel> Properties { get; set; }
        public List<Property> SelectedProperties { get; set; }

        public RelayCommand Ok { get; set; }

        private void OnOkExecute ()
        {
            SelectedProperties = Properties.Where(p => p.IsChecked).Select(s => s.Property).ToList();
        }
    }
}

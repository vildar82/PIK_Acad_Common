using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadLib.Blocks;
using MicroMvvm;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

namespace PIK_Acad_Common.Utils.SelectBlockByParam.UI
{
    public class SelectBlockViewModel : ObservableObject
    {
        private Action<List<Property>> selectAction;
        public SelectBlockViewModel () { }
        public SelectBlockViewModel (BlockBase blBase, Action<List<Property>> selectAction)
        {
            this.selectAction = selectAction;
            BlBase = blBase;
            Ok = new RelayCommand(OnOkExecute);            

            Properties = blBase.Properties.Where(p=>!p.IsReadOnly).Select(s => new PropertyViewModel(s)).ToList();
            var selBlParams = SelectBlocksByParam.Options.Find(blBase.BlName);
            if (selBlParams != null)
            {
                foreach (var item in selBlParams)
                {
                    var prop = Properties.Find(p => p.Property.Name.Equals(item, StringComparison.OrdinalIgnoreCase));
                    if (prop != null)
                    {
                        prop.IsChecked = true;
                    }
                }
            }
        }        

        public BlockBase BlBase { get; set; }
        public List<PropertyViewModel> Properties { get; set; }
        public List<Property> SelectedProperties { get; set; }
        public RelayCommand Ok { get; set; }        

        private void OnOkExecute ()
        {
            SelectedProperties = Properties.Where(p => p.IsChecked).Select(s => s.Property).ToList();
            SelectBlocksByParam.Options.AddBlockSelParams(BlBase.BlName, SelectedProperties.Select(s => s.Name).ToList());
            selectAction(SelectedProperties);
        }        
    }
}

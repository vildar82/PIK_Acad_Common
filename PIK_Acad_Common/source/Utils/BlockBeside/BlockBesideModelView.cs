using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MicroMvvm;

namespace PIK_Acad_Common.Utils.BlockBeside
{
    public class BlockBesideModelView : ObservableObject
    {
        private BlockBesideModel model;
        public Action CloseAction { get; set; }

        public BlockBesideModelView()
        {
            IsVisible = true;
            Blocks = new ObservableCollection<string>();

            model = new BlockBesideModel();
            Filter = model.Filter;

            Insert = new RelayCommand(() => InsertExecute(), () => Blocks.Count > 0);
            Delete = new RelayCommand<IList>((s) => DeleteExecute(s), (s) =>!string.IsNullOrEmpty(SelectedBlock));            
        }        

        public ICommand Insert { get; set; }
        public ICommand Delete { get; set; }

        private string selectedBlock;
        public string SelectedBlock {
            get { return selectedBlock; }
            set { selectedBlock = value;
                RaisePropertyChanged();
            }
        }

        public string Filter {
            get { return model.Filter; }
            set { model.Filter = value;
                Update();
                RaisePropertyChanged();
            }
        }

        BesideOrientation orient;
        public BesideOrientation Orient {            
            get { return model.Orient; }
            set { orient = value;
                model.Orient = orient;
                RaisePropertyChanged();
            }
        }        

        public ObservableCollection<string> Blocks { get; set; }

        private void Update ()
        {
            Blocks.Clear();
            foreach (var item in model.Blocks)
            {
                Blocks.Add(item);
            }
        }

        private bool isVisible = false;
        public bool IsVisible {
            get { return isVisible; }
            set {
                isVisible = value;
                RaisePropertyChanged();
            }
        }

        private void InsertExecute ()
        {
            IsVisible = false;

            model.Insert(Blocks.ToList());

            CloseAction();
            //IsVisible = true;
        }
        private void DeleteExecute (IList s)
        {
            if (s == null || s.Count == 0) return;

            var removeBlocks = s.Cast<string>().ToList();
            foreach (var item in removeBlocks)
            {
                Blocks.Remove(item);
            }         
            //if (!string.IsNullOrEmpty(SelectedBlock))
            //{
            //    Blocks.Remove(SelectedBlock);
            //}
        }
    }
}

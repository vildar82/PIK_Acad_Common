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

        public BlockBesideModelView()
        {            
            Blocks = new ObservableCollection<BlockModel>();

            model = new BlockBesideModel();
            Filter = model.Filter;

            Insert = new RelayCommand(InsertExecute, () => Blocks.Count > 0);
            Delete = new RelayCommand(DeleteExecute, ()=> Blocks.Any(b=>b.IsSelected));            
        }        

        public RelayCommand Insert { get; set; }
        public RelayCommand Delete { get; set; }

        private string selectedBlock;
        public string SelectedBlock {
            get { return selectedBlock; }
            set { selectedBlock = value;
                RaisePropertyChanged();
                SelectedCount = Blocks.Sum(s => s.IsSelected ? 1 : 0);
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

        public ObservableCollection<BlockModel> Blocks { get; set; }

        public int SelectedCount { get { return selectedCount; } set { selectedCount = value; RaisePropertyChanged(); } }
        int selectedCount;

        private void Update ()
        {
            Blocks.Clear();
            foreach (var item in model.Blocks)
            {
                Blocks.Add(new BlockModel (item));
            }
        }       

        private void InsertExecute ()
        {
            // Блоки для вставки
            var blocksToInsert = Blocks.Where(b => b.IsSelected).ToList();
            if (blocksToInsert.Count<=1)
            {
                blocksToInsert = Blocks.ToList();
            }  
            model.Insert(blocksToInsert.Select(s=>s.Name).ToList());            
        }

        private void DeleteExecute ()
        {
            var blocksSelectes = Blocks.Where(b => b.IsSelected).ToList();                        
            foreach (var item in blocksSelectes)
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

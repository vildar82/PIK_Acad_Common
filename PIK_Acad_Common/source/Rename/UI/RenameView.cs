using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MicroMvvm;

namespace PIK_Acad_Common.Rename.UI
{
    public class RenameView : ObservableObject
    {
        private RenameSymbolTableRecordService service;
        private List<Record> records;       
        private SymbolTableEnum table;
        private string search;
        private string replace;
        private string filter;
        private RecordView selRec;
        private ObservableCollection<RecordView> recordsView;
        private ObservableCollection<RecordView> renameRecordsView;

        public RenameView (RenameSymbolTableRecordService service)
        {
            this.service = service;
            Table = SymbolTableEnum.Blocks;
            Search = "^КР_";
            Replace = "ОиФ_";
        }

        public ObservableCollection<RecordView> Records {
            get { return recordsView; }
            set {
                recordsView = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<RecordView> RenameRecords {
            get { return renameRecordsView; }
            set {
                renameRecordsView = value;
                RaisePropertyChanged();
            }
        }

        public SymbolTableEnum Table {
            get { return table; }
            set {
                table = value;
                FillAllRecords();
                RaisePropertyChanged();
            }
        }

        public string Filter {
            get { return filter; }
            set {
                filter = value;
                FilterRecords();
                RaisePropertyChanged();
            }
        }

        
        public string Search {
            get { return search; }
            set {
                search = value;
                FillRenameRecords();
                RaisePropertyChanged();
            }
        }

        public string Replace {
            get { return replace; }
            set {
                replace = value;
                FillRenameRecords();
                RaisePropertyChanged();
            }
        }

        public RecordView SelectedRecord {
            get { return selRec; }
            set { selRec = value;
                RaisePropertyChanged();
            }
        }

        private void FillRenameRecords ()
        {
            var recsRen = service.GetRenameRecords(recordsView.Select(s=>s.record).ToList(), search, replace);
            RenameRecords = FillRecords(recsRen);            
        }

        private ObservableCollection<RecordView> FillRecords (List<Record> recs)
        {
            var resRenRecs = new ObservableCollection<RecordView>();
            foreach (var item in recs)
            {
                resRenRecs.Add(new RecordView(item));
            }
            return resRenRecs;
        }

        private void FillAllRecords ()
        {
            records = service.GetAllRecords(table);
            Records = FillRecords(records);
        }

        private void FilterRecords ()
        {
            var filterRecs = records.Where(r => Regex.IsMatch(r.Name, filter)).ToList();
            Records = FillRecords(filterRecs);
        }

        public ICommand Rename {
            get {
                return new RelayCommand(() =>
                {
                    service.RenameRecords(renameRecordsView.Select(s=>s.record).ToList(), table);
                    // Обновление переименованных списков
                    FillRenameRecords();
                    Table = table;
                },
                () => renameRecordsView?.Count > 0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MicroMvvm;

namespace PIK_Acad_Common.Rename.UI
{
    public class RecordView : ObservableObject
    {
        private static Brush BadColor = new SolidColorBrush(Colors.Red);
        public Record record;

        public RecordView (Record item)
        {
            record = item;
        }

        public string Name {
            get { return record.Name; }
        }

        public string Rename {
            get { return record.Rename; }
        }

        public Brush Backgraund {
            get { return string.IsNullOrEmpty( record.Error) ? null : BadColor; }            
        }

        public string Description {
            get { return record.Error; }            
        }

        public override string ToString ()
        {
            return Name.ToString();
        }
    }
}

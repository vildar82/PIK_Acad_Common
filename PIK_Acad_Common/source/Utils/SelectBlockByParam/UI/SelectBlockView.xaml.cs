using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PIK_Acad_Common.Utils.SelectBlockByParam.UI
{
    /// <summary>
    /// Логика взаимодействия для SelectBlockWindow.xaml
    /// </summary>
    public partial class SelectBlockView : Window
    {
        public SelectBlockView() : this(null)
        {
            
        }

        public SelectBlockView (SelectBlockViewModel selBlVM)
        {
            InitializeComponent();
            Title = $"Выбор блоков {selBlVM?.BlBase?.BlName} по параметрам";
            DataContext = selBlVM;
        }              

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                bOK.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                bOK.Command.Execute(bOK.CommandParameter);
                e.Handled = true;
                this.Close();
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();                
            }
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            var ed = doc.Editor;
            Hide();
            ed.SetImpliedSelection(new ObjectId[0]);
            var selRes = ed.GetSelection();            

            if (selRes.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK && selRes.Value.Count > 0)
            {
                ed.SetImpliedSelection(selRes.Value.GetObjectIds());
                bOK.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                bOK.Command.Execute(bOK.CommandParameter);
            }
            else
            {
                Show();
            }                
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

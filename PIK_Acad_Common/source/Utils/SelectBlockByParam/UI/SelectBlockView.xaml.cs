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
            DataContext = selBlVM;
        }       

        private void Ok_Click (object sender, RoutedEventArgs e)
        {
            DialogResult = true;            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                bOK.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                bOK.Command.Execute(bOK.CommandParameter);
                e.Handled = true;
            }
        } 
    }
}

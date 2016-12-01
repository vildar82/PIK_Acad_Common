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

namespace PIK_Acad_Common.Utils.BlockBeside
{
    /// <summary>
    /// Логика взаимодействия для InsertBlockBesideWindow.xaml
    /// </summary>
    public partial class InsertBlockBesideWindow : Window
    {
        public InsertBlockBesideWindow ()
        {
            InitializeComponent();            

            var vm = new BlockBesideModelView();
            DataContext = vm;            
            this.PreviewKeyDown += HandleEsc;            
        }

        private void HandleEsc (object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem lbi = sender as ListBoxItem;
                lbi.IsSelected = !lbi.IsSelected;
                lbi.Focus();
                lbBlocks.SelectedItems.Add(lbi);
            }
        }

        private void InsertClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                bInsert.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                bInsert.Command.Execute(bInsert.CommandParameter);
                e.Handled = true;
            }
        }
    }
}

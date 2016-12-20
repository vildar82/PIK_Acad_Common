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

        /// <summary>
        /// Выделение элементов
        /// </summary>        
        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {            
            ListBoxItem lbi = sender as ListBoxItem;
            // Если зажата левая кнопка, то выделение            
            if (e.LeftButton == MouseButtonState.Pressed && !lbi.IsSelected)
            {
                lbi.IsSelected = true;
                lbi.Focus();
                //lbBlocks.SelectedItems.Add(lbi);
            }
            // Если зажата правая кнопка - снятие выделения
            else if (e.RightButton == MouseButtonState.Pressed && lbi.IsSelected)
            {
                lbi.IsSelected = false;
                lbi.Focus();
                //lbBlocks.SelectedItems.Remove(lbi);
            }
        }        

        private void InsertClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// По нажатию клавишь "Enter" или "Esc" - выполнение вставки блоков (нажатие кнопки Вставка)
        /// </summary>        
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

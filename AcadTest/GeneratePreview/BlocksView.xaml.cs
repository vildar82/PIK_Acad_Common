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

namespace AcadTest.GeneratePreview
{
    /// <summary>
    /// Логика взаимодействия для BlocksView.xaml
    /// </summary>
    public partial class BlocksView : Window
    {
        public BlocksView(BlocksModel model)
        {
            InitializeComponent();
            DataContext = model;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

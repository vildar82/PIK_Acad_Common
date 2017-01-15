using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private CancellationTokenSource cancelationToken;        
        private BlocksModel model;

        public BlocksView(BlocksModel model)
        {
            InitializeComponent();
            DataContext = model;
            this.model = model;
            this.Loaded += BlocksView_Loaded;
            this.Closed += BlocksView_Closed;
        }

        private void BlocksView_Closed(object sender, EventArgs e)
        {
            cancelationToken?.Cancel(true);
        }

        private void BlocksView_Loaded(object sender, RoutedEventArgs e)
        {
            GeneratePreviews(model.Blocks.ToList());
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {            
            DialogResult = true;
            cancelationToken?.Cancel(true);
            
        }

        private async void GeneratePreviews(List<Block> blocks)
        {
            var blNames = blocks.Select(s => s.Name).ToList();
            cancelationToken = new CancellationTokenSource();
            try
            {
                var previews = await GenerateBlockPreview.GeneratePreviewsAsync(blNames, model.db, cancelationToken.Token);
                foreach (var item in blocks)
                {
                    var image = previews[item.Name];
                    //image.Freeze();
                    item.Preview = image;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

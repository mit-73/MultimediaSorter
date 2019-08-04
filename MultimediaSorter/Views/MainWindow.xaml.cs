using MultimediaSorter.ViewModels;

namespace MultimediaSorter.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = new MainViewModel();
        }
    }
}

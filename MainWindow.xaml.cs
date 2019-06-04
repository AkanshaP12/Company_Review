using Company_Review.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Company_Review
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private UserControl _DisplayUserControl;

        public UserControl DisplayUserControl
        {
            get
            {
                return _DisplayUserControl;
            }
            set
            {
                _DisplayUserControl = value;
                OnPropertyChanged("DisplayUserControl");
            }
        }

        public MainWindow()
        {
            //language = Properties.Settings.Default.language;
            InitializeComponent();
            this.DataContext = this;
        }

        private void OnPropertyChanged(string v)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(v));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void W_Main_Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayUserControl = new ViewReviewsUC(this);
        }
    }
}

using Company_Review.classes;
using Company_Review.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Company_Review
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,  INotifyPropertyChanged
    {

        public string language;
        private UserControl _DisplayUserControl;
        public List<string> cultures = new List<string> { "en english", "de detush" };

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

            language = Properties.Settings.Default.language;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            InitializeComponent();
            this.DataContext = this;
            loadCultures();

        }

        private void loadCultures()
        {
            List<ComboBoxItem> cultureItems = new List<ComboBoxItem>();
            foreach (string culture in cultures)
            {
                ComboBoxItem cb = new ComboBoxItem();
                cb.Content = culture;
                cultureItems.Add(cb);

            }
            Cbx_lang.ItemsSource = cultureItems;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string v)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(v));
            }
        }

        

        private void W_Main_Window_Loaded(object sender, RoutedEventArgs e)
        {
             DisplayUserControl = new ViewReviewsUC(this);
        }

        private void Cbx_lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string txt = ((ComboBoxItem)(sender as ComboBox).SelectedItem).Content.ToString();
            language = txt.Substring(0, 2);
            Properties.Settings.Default.language = language;
            Properties.Settings.Default.Save();
        }
    }
}

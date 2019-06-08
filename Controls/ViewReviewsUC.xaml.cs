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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Company_Review.classes;
using Company_Review.core;
using Company_Review.core.Converter;
using System.Threading;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel;

namespace Company_Review.Controls
{


    /// <summary>
    /// Interaction logic for ViewReviewsUC.xaml
    /// </summary>
    public partial class ViewReviewsUC : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<CompanyReview> companies;
        public ObservableCollection<CompanyReview> companiesForComboBox;
        public MainWindow mainWindow;
        public string language;
        public List<string> cultures = new List<string> { "en english", "de detush" };
        public List<JobDepartment> jobDepartments { get; set; } = new List<JobDepartment>();

        public List<JobLocation> jobLocations { get; set; } = new List<JobLocation>();
        private bool handleCompanySelection = true;

        private string comboBoxSearchText_;

        public string comboBoxSearchText
        {
            get { return comboBoxSearchText_; }
            set
            {
                if (comboBoxSearchText_ == value) return;
                comboBoxSearchText_ = value;
                OnPropertyChanged("comboBoxSearchText");
            }
        }

        public ViewReviewsUC(MainWindow mainWindow)
        {

            language = Properties.Settings.Default.language;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);


            InitializeComponent();
            //GenerateCompanies();
            loadFromFile();
            //Lbx_companies.ItemsSource = companies;
            Itc_reviews.ItemsSource = companies;
            this.DataContext = this;
            this.mainWindow = mainWindow;
            loadCultures();
            companiesForComboBox = new ObservableCollection<CompanyReview>(from n in companies where n.companyOverview.departmentName == "All departments" select n);
            cb_companyName.ItemsSource = companiesForComboBox;
            Grd_Company_Selected.Visibility = Visibility.Collapsed;
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

        private void loadFromFile()
        {
            Companies companiesFromFile = XMLSerializerWrapper.ReadXml<Companies>("data\\companies.xml");
            List<CompanyOverview> companyOverviewsFromFile = companiesFromFile.companyDetails;
            companies = new ObservableCollection<CompanyReview>();
            Reviews reviews = XMLSerializerWrapper.ReadXml<Reviews>("data\\reviews.xml");
            foreach (CompanyOverview companyOverview in companyOverviewsFromFile)
            {
                CompanyReview companyReview = new CompanyReview();
                companyReview.companyOverview = companyOverview;
                companyReview.reviews = (from n in reviews.reviews where n.companyId == companyOverview.id select n).ToList();
                companyOverview.noOfReviews = companyReview.reviews.Count;
                if (companyOverview.noOfReviews > 0)
                {
                    companyOverview.avgRating = (from n in reviews.reviews where n.companyId == companyOverview.id select float.Parse(n.rating)).Sum() / companyOverview.noOfReviews;
                }

                if (companyOverview.noOfReviews > 0)
                {
                    companyOverview.wouldRecommended = (from n in reviews.reviews where n.companyId == companyOverview.id & n.wouldYouRecommend == "Yes" select n.wouldYouRecommend).Count() * 100 / companyOverview.noOfReviews;
                }
                companies.Add(companyReview);

                List<IGrouping<string, Review>> reviewsByDepartment = (from n in companyReview.reviews group n by n.jobDepartment into d select d).ToList();
                foreach(IGrouping<string, Review> groupedReviews in reviewsByDepartment)
                {
                    CompanyReview companyReviewByDepartment = new CompanyReview();
                    CompanyOverview companyOverviewByDepartment = new CompanyOverview();
                    companyOverviewByDepartment.id = companyOverview.id;
                    companyOverviewByDepartment.name = companyOverview.name;
                    companyOverviewByDepartment.logoPath = companyOverview.logoPath;
                    companyOverviewByDepartment.departmentName = groupedReviews.Key;

                    companyReviewByDepartment.reviews = groupedReviews.ToList();

                    companyOverviewByDepartment.noOfReviews = companyReviewByDepartment.reviews.Count;
                    if (companyOverviewByDepartment.noOfReviews > 0)
                    {
                        companyOverviewByDepartment.avgRating = (from n in companyReviewByDepartment.reviews select float.Parse(n.rating)).Sum() / companyOverviewByDepartment.noOfReviews;
                    }

                    if (companyOverviewByDepartment.noOfReviews > 0)
                    {
                        companyOverviewByDepartment.wouldRecommended = (from n in companyReviewByDepartment.reviews where n.wouldYouRecommend == "Yes" select n.wouldYouRecommend).Count() * 100 / companyOverviewByDepartment.noOfReviews;
                    }
                    companyReviewByDepartment.companyOverview = companyOverviewByDepartment;
                    companies.Add(companyReviewByDepartment);
                }

            }

        }

        //private void GenerateCompanies()
        //{
        //    CompanyOverview sapOverView = new CompanyOverview { name = "SAP", noOfReviews = 44, logoPath= "Resources\\Images\\Logos\\sap.png" };
        //    CompanyOverview abbOVerView = new CompanyOverview { name = "ABB", noOfReviews = 30, logoPath = "Resources\\Images\\Logos\\abb.png" };
        //    CompanyOverview basfOverView = new CompanyOverview { name = "BaSF", noOfReviews = 10, logoPath = "Resources\\Images\\Logos\\basf.png" };
        //    companyOverviews = new List<CompanyOverview>();
        //    companyOverviews.Add(sapOverView);
        //    companyOverviews.Add(abbOVerView);
        //    companyOverviews.Add(basfOverView);

        //    companies = new ObservableCollection<CompanyReview>();
        //    companies.Add(new CompanyReview {  companyOverview= sapOverView });
        //    companies.Add(new CompanyReview { companyOverview= abbOVerView });
        //    companies.Add(new CompanyReview { companyOverview= basfOverView });
        //}

        //private void Tbx_filter_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var filter = Tbx_filter.Text;
        //    if (filter == "")
        //        Lbx_companies.ItemsSource = companies;
        //    else
        //    {
        //        var results = from s in companies where s.companyOverview.name.ToLower().Contains(filter.ToLower()) select s;
        //        Lbx_companies.ItemsSource = results;
        //    }
        //}

        //private void Lbx_companies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (Lbx_companies.SelectedItem != null)
        //    {
        //        Grd_All_Companies.Visibility = Visibility.Collapsed;
        //        Grd_Company_Selected.Visibility = Visibility.Visible;
        //        Stp_View_Reviews_Filters.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        Grd_All_Companies.Visibility = Visibility.Visible;
        //        Grd_Company_Selected.Visibility = Visibility.Collapsed;
        //        Stp_View_Reviews_Filters.Visibility = Visibility.Collapsed;

        //    }
        //}

        private void Btn_Back_To_All_Companies_Click(object sender, RoutedEventArgs e)
        {
            //Lbx_companies.SelectedItem = null;
        }

        private void Btn_AddReview_Click(object sender, RoutedEventArgs e)
        {
            this.mainWindow.DisplayUserControl = new AddReviewUC();
        }

        private void Cbx_lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string txt = ((ComboBoxItem)(sender as ComboBox).SelectedItem).Content.ToString();
            language = txt.Substring(0, 2);
            Properties.Settings.Default.language = language;
            Properties.Settings.Default.Save();
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

        private void Cb_companyName_KeyUp(object sender, KeyEventArgs e)
        {
            //cb_companyName.IsDropDownOpen = true;
            if (String.IsNullOrEmpty(comboBoxSearchText))
            {
                cb_companyName.ItemsSource = companiesForComboBox;
                return;
            }

            ObservableCollection<CompanyReview> tempCompanies = new ObservableCollection<CompanyReview>((from n in companiesForComboBox where n.companyOverview.name.ToLower().StartsWith(comboBoxSearchText.ToLower()) select n).ToList());
            if (tempCompanies != null)
            {
                cb_companyName.ItemsSource = tempCompanies;
                comboBoxSearchText = comboBoxSearchText;
            }
        }

        private void Cb_companyName_KeyDown(object sender, KeyEventArgs e)
        {
            cb_companyName.IsDropDownOpen = true;
        }

        private void Cb_companyName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox companyComboBox = sender as ComboBox;
            if(companyComboBox.SelectedItem != null)
            {
                CompanyReview companyReview = (CompanyReview)companyComboBox.SelectedItem;
                if(companyReview.reviews != null)
                {
                    List<string> allCompanyReviewedDepartments = new List<string> { "All departments" };
                    allCompanyReviewedDepartments.AddRange((from n in companyReview.reviews select n.jobDepartment).Distinct().OrderBy(x => x).ToList());
                    if(allCompanyReviewedDepartments.Count > 0)
                    {
                        foreach(string department in allCompanyReviewedDepartments)
                        {
                            JobDepartment jobDepartment = new JobDepartment { departmentName = department, isSelected = false };
                            jobDepartments.Add(jobDepartment);
                        }
                        Cmb_Departments.ItemsSource = jobDepartments;
                    }

                    List<string> allCompanyReviewedLocations = new List<string> { "All locations" };
                    allCompanyReviewedLocations.AddRange((from n in companyReview.reviews select n.jobLocation).Distinct().OrderBy(x => x).ToList());
                    if(allCompanyReviewedLocations.Count >0)
                    {
                        foreach(string location in allCompanyReviewedLocations)
                        {
                            JobLocation jobLocation = new JobLocation { location = location, isSelected = false };
                            jobLocations.Add(jobLocation);
                        }
                    }
                    Cmb_Locations.ItemsSource = jobLocations;
                }
            }
        }

        private void Cb_companyName_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox companyComboBox = sender as ComboBox;
            companyComboBox.SelectedItem = null;
        }

        private void Cbx_department_selection_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedCheckBox = sender as CheckBox;
            handleDepartmentCheckbox(selectedCheckBox);
        }

        private void Cbx_department_selection_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedCheckBox = sender as CheckBox;
            handleDepartmentCheckbox(selectedCheckBox);
        }

        private void handleDepartmentCheckbox(CheckBox selectedCheckBox)
        {
            JobDepartment selectedJobDepartment = (JobDepartment)selectedCheckBox.DataContext;
            if (selectedJobDepartment.departmentName == "All departments")
            {
                foreach (JobDepartment eachDepartment in jobDepartments)
                {
                    if (eachDepartment.departmentName == "All departments")
                    {
                        continue;
                    }
                    eachDepartment.isSelected = (bool)selectedCheckBox.IsChecked;
                }
            }
        }

        private void Cbx_location_selection_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedCheckBox = sender as CheckBox;
            handleLocationCheckbox(selectedCheckBox);
        }

        private void Cbx_location_selection_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedCheckBox = sender as CheckBox;
            handleLocationCheckbox(selectedCheckBox);
        }

        private void handleLocationCheckbox(CheckBox selectedCheckBox)
        {
            JobLocation selectedJobLocation = (JobLocation)selectedCheckBox.DataContext;
            if (selectedJobLocation.location == "All locations")
            {
                foreach (JobLocation eachlocation in jobLocations)
                {
                    if (eachlocation.location == "All locations")
                    {
                        continue;
                    }
                    eachlocation.isSelected = (bool)selectedCheckBox.IsChecked;
                }
            }
        }
    }

}
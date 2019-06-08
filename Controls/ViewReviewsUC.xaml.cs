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
        public ObservableCollection<CompanyReview> companiesInTransit;
        public ObservableCollection<CompanyReview> companiesForComboBox;
        private List<CompanyOverview> companyOverviewsFromFile;
        public MainWindow mainWindow;
        public string language;
        public List<string> cultures = new List<string> { "en english", "de detush" };
        public List<JobDepartment> jobDepartments { get; set; } = new List<JobDepartment>();

        public List<JobLocation> jobLocations { get; set; } = new List<JobLocation>();
        private bool handleDepartmentSelection = true;

        private string comboBoxSearchText_;

        private Reviews reviews;

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
            companiesInTransit = companies;
            //Lbx_companies.ItemsSource = companies;
            Itc_reviews.ItemsSource = companiesInTransit;
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
            companyOverviewsFromFile = companiesFromFile.companyDetails;
            companies = new ObservableCollection<CompanyReview>();
            reviews = XMLSerializerWrapper.ReadXml<Reviews>("data\\reviews.xml");
            foreach (CompanyOverview companyOverview in companyOverviewsFromFile)
            {
                collectCompanyReviews(companyOverview, reviews.reviews, companies);
            }
        }

        private void collectCompanyReviews(CompanyOverview companyOverview, List<Review> reviews,ObservableCollection<CompanyReview> targetCollection)
        {
            CompanyReview companyReview = new CompanyReview();
            companyReview.companyOverview = companyOverview;
            companyReview.reviews = (from n in reviews where n.companyId == companyOverview.id select n).ToList();
            companyOverview.calculateReviewStatistics(companyReview.reviews);
            targetCollection.Add(companyReview);

            List<IGrouping<string, Review>> reviewsByDepartment = (from n in companyReview.reviews group n by n.jobDepartment into d select d).ToList();
            foreach (IGrouping<string, Review> groupedReviews in reviewsByDepartment)
            {
                CompanyReview companyReviewByDepartment = new CompanyReview();
                CompanyOverview companyOverviewByDepartment = new CompanyOverview();
                companyOverviewByDepartment.id = companyOverview.id;
                companyOverviewByDepartment.name = companyOverview.name;
                companyOverviewByDepartment.logoPath = companyOverview.logoPath;
                companyOverviewByDepartment.departmentName = groupedReviews.Key;

                companyReviewByDepartment.reviews = groupedReviews.ToList();

                companyOverviewByDepartment.calculateReviewStatistics(companyReviewByDepartment.reviews);
                companyReviewByDepartment.companyOverview = companyOverviewByDepartment;
                targetCollection.Add(companyReviewByDepartment);
            }
        }

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
                    List<string> allCompanyReviewedDepartments = new List<string> { "Select all" };
                    allCompanyReviewedDepartments.AddRange((from n in companyReview.reviews select n.jobDepartment).Distinct().OrderBy(x => x).ToList());
                    if(allCompanyReviewedDepartments.Count > 0)
                    {
                        foreach(string department in allCompanyReviewedDepartments)
                        {
                            JobDepartment jobDepartment = new JobDepartment { departmentName = department, isSelected = true };
                            jobDepartments.Add(jobDepartment);
                        }
                        Cmb_Departments.ItemsSource = jobDepartments;
                    }

                    List<string> allCompanyReviewedLocations = new List<string> { "Select all" };
                    allCompanyReviewedLocations.AddRange((from n in companyReview.reviews select n.jobLocation).Distinct().OrderBy(x => x).ToList());
                    if(allCompanyReviewedLocations.Count >0)
                    {
                        foreach(string location in allCompanyReviewedLocations)
                        {
                            JobLocation jobLocation = new JobLocation { location = location, isSelected = true };
                            jobLocations.Add(jobLocation);
                        }
                    }
                    Cmb_Locations.ItemsSource = jobLocations;

                    //load selected company items
                    companiesInTransit = new ObservableCollection<CompanyReview>(from n in companiesInTransit where n.companyOverview.id == companyReview.companyOverview.id select n);
                    Itc_reviews.ItemsSource = companiesInTransit;
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
            if(handleDepartmentSelection)
            {
                handleDepartmentSelection = false;
                handleDepartmentCheckbox(selectedCheckBox);
                handleDepartmentSelection = true;
            }
        }

        private void Cbx_department_selection_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedCheckBox = sender as CheckBox;
            if (handleDepartmentSelection)
            {
                handleDepartmentSelection = false;
                handleDepartmentCheckbox(selectedCheckBox);
                handleDepartmentSelection = true;
            }
        }

        private void handleDepartmentCheckbox(CheckBox selectedCheckBox)
        {
            JobDepartment selectedJobDepartment = (JobDepartment)selectedCheckBox.DataContext;
            bool isSelectAllCheckbox = false;
            if (selectedJobDepartment.departmentName == "Select all")
            {
                isSelectAllCheckbox = true;
            }

            
            foreach (JobDepartment eachDepartment in jobDepartments)
            {
                if (eachDepartment.departmentName == "Select all" && (eachDepartment.isSelected == true) && (isSelectAllCheckbox == false))
                {
                    eachDepartment.isSelected = !eachDepartment.isSelected;
                    continue;
                }

                if(isSelectAllCheckbox)
                {
                    eachDepartment.isSelected = (bool)selectedCheckBox.IsChecked;
                }
            }

            bool isAllDepartmentSelected = (from n in jobDepartments where n.isSelected == false select n).Count() == 1;
            if(isAllDepartmentSelected)
            {
                JobDepartment selectAllDepartment = (from n in jobDepartments where n.departmentName == "Select all" select n).ToList()[0];
                selectAllDepartment.isSelected = true;
            }

            List<string> filterDepartments = new List<string>();
            bool isAnyDepartmentSelected = false;
            foreach (JobDepartment department in jobDepartments)
            {
                if(department.isSelected == true)
                {
                    isAnyDepartmentSelected = true;
                    if (department.departmentName == "Select all")
                    {
                        filterDepartments.Add("All departments");
                    }
                    else
                    {
                        filterDepartments.Add(department.departmentName);
                    }
                }
            }

            if(isAnyDepartmentSelected)
            {
                //load selected departments
                Itc_reviews.ItemsSource = new ObservableCollection<CompanyReview>(from n in companiesInTransit where filterDepartments.Contains(n.companyOverview.departmentName) select n);
            }
            else
            {
                //load selected departments
                Itc_reviews.ItemsSource = new ObservableCollection<CompanyReview>(from n in companiesInTransit where n.companyOverview.departmentName == "All departments" select n);
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

            bool isSelectAllCheckbox = false;
            if (selectedJobLocation.location == "Select all")
            {
                isSelectAllCheckbox = true;
            }


            foreach (JobLocation eachlocation in jobLocations)
            {
                if (eachlocation.location == "Select all" && (eachlocation.isSelected == true) && (isSelectAllCheckbox == false))
                {
                    eachlocation.isSelected = !eachlocation.isSelected;
                    continue;
                }

                if (isSelectAllCheckbox)
                {
                    eachlocation.isSelected = (bool)selectedCheckBox.IsChecked;
                }
            }

            List<string> filterLocations = new List<string>();
            bool isAnyLocationSelected = false;
            foreach (JobLocation location in jobLocations)
            {
                if (location.isSelected == true)
                {
                    isAnyLocationSelected = true;
                    if (location.location == "Select all")
                    {
                        filterLocations.Add("All locations");
                    }
                    else
                    {
                        filterLocations.Add(location.location);
                    }
                }
            }

            bool isAllLocationSelected = (from n in jobLocations where n.isSelected == false select n).Count() == 1;
            if (isAllLocationSelected)
            {
                JobLocation selectAllLocation = (from n in jobLocations where n.location == "Select all" select n).ToList()[0];
                selectAllLocation.isSelected = true;
            }

            List<Review> reviewsByLocations;
            if (isAnyLocationSelected)
            {
                reviewsByLocations = (from n in reviews.reviews where filterLocations.Contains(n.jobLocation) select n).ToList();

            }
            else
            {
                reviewsByLocations = (from n in reviews.reviews select n).ToList();
            }

            ObservableCollection<CompanyReview> companyReviewsByLocation = new ObservableCollection<CompanyReview>();
            foreach(CompanyReview companyReview in companiesInTransit)
            {
                if(companyReview.companyOverview.departmentName == "All departments")
                {
                    collectCompanyReviews(companyReview.companyOverview, reviewsByLocations, companyReviewsByLocation);
                }
            }
            companiesInTransit = companyReviewsByLocation;
            Itc_reviews.ItemsSource = companiesInTransit;
        }
    }

}
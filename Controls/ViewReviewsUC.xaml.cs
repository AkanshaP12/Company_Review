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
using Company_Review.classes.core;

namespace Company_Review.Controls
{


    /// <summary>
    /// Interaction logic for ViewReviewsUC.xaml
    /// </summary>
    public partial class ViewReviewsUC : UserControl, INotifyPropertyChanged
    {
        public CompanyReviewFilter companyReviewFilter;
        public ObservableCollection<CompanyReview> companiesForComboBox;
        public MainWindow mainWindow;
        public List<JobDepartment> jobDepartments { get; set; } = new List<JobDepartment>();

        public List<JobLocation> jobLocations { get; set; } = new List<JobLocation>();

        private bool handleDepartmentSelection = true;
        private bool handleLocationSelection = true;

        private string comboBoxSearchText_;

        private Reviews reviews;
        private Companies companiesFromFile;

        private bool isNoDataVisible_ { get; set; }

        public bool isNoDataVisible
        {
            get { return isNoDataVisible_; }
            set
            {
                isNoDataVisible_ = value;
                OnPropertyChanged("isNoDataVisible");
            }
        }

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

            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void UC_View_Reviews_Loaded(object sender, RoutedEventArgs e)
        {
            //GenerateCompanies();
            loadFromFile();

            //Lbx_companies.ItemsSource = companies;
            ObservableCollection<CompanyReview> filteredCompanies = companyReviewFilter.filterByCriteria();
            Itc_reviews.ItemsSource = filteredCompanies;
            if(filteredCompanies.Count > 0)
            {
                isNoDataVisible = false;
            }
            else
            {
                isNoDataVisible = true;
            }
            this.DataContext = this;
            
            companiesForComboBox = new ObservableCollection<CompanyReview>(from n in filteredCompanies where n.companyOverview.departmentName == "All departments" select n);
            cb_companyName.ItemsSource = companiesForComboBox;

            populateDepartmentsAndLocations(null);
            Cmb_Empl_Status.SelectedValue = "All status";
            Itc_FilterTags.ItemsSource = companyReviewFilter.getUIFilterTags();
        }

        private void loadFromFile()
        {
            companiesFromFile = XMLSerializerWrapper.ReadXml<Companies>("data\\companies.xml");

            int highestCompanyId = (from n in companiesFromFile.companyDetails select n).OrderByDescending(x => x.id).ToList()[0].id;
            Properties.Settings.Default.next_company_id = highestCompanyId + 1;
            Properties.Settings.Default.Save();

            reviews = XMLSerializerWrapper.ReadXml<Reviews>("data\\reviews.xml");

            int highestReviewId = (from n in reviews.reviews select n).OrderByDescending(x => x.id).ToList()[0].id;
            Properties.Settings.Default.next_review_id = highestReviewId + 1;
            Properties.Settings.Default.Save();

            companyReviewFilter = new CompanyReviewFilter(companiesFromFile, reviews.reviews);
        }

        private void Btn_Back_To_All_Companies_Click(object sender, RoutedEventArgs e)
        {
            //Lbx_companies.SelectedItem = null;
        }

        private void Btn_AddReview_Click(object sender, RoutedEventArgs e)
        {
            this.mainWindow.DisplayUserControl = new AddReviewUC(mainWindow);
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
                applyCompanyFilter();
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
            applyCompanyFilter();
        }

        private void applyCompanyFilter()
        {
            if (cb_companyName.SelectedItem != null && !String.IsNullOrEmpty(comboBoxSearchText))
            {
                CompanyReview companyReview = (CompanyReview)cb_companyName.SelectedItem;
                if (companyReview.reviews != null)
                {
                    populateDepartmentsAndLocations(companyReview);

                    //load selected company items
                    companyReviewFilter.setCompanyId(companyReview.companyOverview.id);
                    ObservableCollection<CompanyReview> tempReviews = companyReviewFilter.filterByCriteria();
                    Itc_reviews.ItemsSource = tempReviews;
                    if (tempReviews.Count > 0)
                    {
                        isNoDataVisible = false;
                    }
                    else
                    {
                        isNoDataVisible = true;
                    }
                    Itc_FilterTags.ItemsSource = companyReviewFilter.getUIFilterTags();
                }
            }
            else if(cb_companyName.SelectedItem != null)
            {
                companyReviewFilter.clearAllFilters();
                ObservableCollection<CompanyReview> filteredCompanies = companyReviewFilter.filterByCriteria();
                Itc_reviews.ItemsSource = filteredCompanies;
                if(filteredCompanies.Count > 0)
                {
                    isNoDataVisible = false;
                }
                else
                {
                    isNoDataVisible = true;
                }
                cb_companyName.ItemsSource = companiesForComboBox;

                populateDepartmentsAndLocations(null);
                Cmb_Empl_Status.SelectedValue = "All status";
                Itc_FilterTags.ItemsSource = companyReviewFilter.getUIFilterTags();
            }
        }

        private void populateDepartmentsAndLocations(CompanyReview companyReview)
        {
            List<string> allCompanyReviewedDepartments = new List<string> { "Select all" };
            List<string> allCompanyReviewedLocations = new List<string> { "Select all" };
            List<Review> reviewsForDL;
            if (companyReview != null)
            {
                reviewsForDL = companyReview.reviews;
            }
            else
            {
                reviewsForDL = reviews.reviews;
            }

            allCompanyReviewedDepartments.AddRange((from n in reviewsForDL select n.jobDepartment).Distinct().OrderBy(x => x).ToList());
            jobDepartments  = new List<JobDepartment>();
            if (allCompanyReviewedDepartments.Count > 0)
            {
                foreach (string department in allCompanyReviewedDepartments)
                {
                    JobDepartment jobDepartment = new JobDepartment { departmentName = department, isSelected = true };
                    jobDepartments.Add(jobDepartment);
                }
            }
            Cmb_Departments.ItemsSource = jobDepartments;


            allCompanyReviewedLocations.AddRange((from n in reviewsForDL select n.jobLocation).Distinct().OrderBy(x => x).ToList());
            jobLocations = new List<JobLocation>();
            if (allCompanyReviewedLocations.Count > 0)
            {
                foreach (string location in allCompanyReviewedLocations)
                {
                    JobLocation jobLocation = new JobLocation { location = location, isSelected = true };
                    jobLocations.Add(jobLocation);
                }
            }
            Cmb_Locations.ItemsSource = jobLocations;
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
                JobDepartment selectedJobDepartment = (JobDepartment)selectedCheckBox.DataContext;
                companyReviewFilter.removeDepartment(selectedJobDepartment.departmentName);
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

            foreach (JobDepartment department in jobDepartments)
            {
                if(department.isSelected == true)
                {
                    if (department.departmentName == "Select all")
                    {
                        companyReviewFilter.addDepartment("All departments");
                        break;
                    }
                    else
                    {
                        companyReviewFilter.addDepartment(department.departmentName);
                    }
                }
            }
            ObservableCollection<CompanyReview> tempReviews = companyReviewFilter.filterByCriteria();
            Itc_reviews.ItemsSource = tempReviews;
            if (tempReviews.Count > 0)
            {
                isNoDataVisible = false;
            }
            else
            {
                isNoDataVisible = true;
            }
            Itc_FilterTags.ItemsSource = companyReviewFilter.getUIFilterTags();
        }

        private void Cbx_location_selection_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedCheckBox = sender as CheckBox;
            if(handleLocationSelection)
            {
                handleLocationSelection = false;
                handleLocationCheckbox(selectedCheckBox);
                handleLocationSelection = true;
            }
        }

        private void Cbx_location_selection_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedCheckBox = sender as CheckBox;
            if (handleLocationSelection)
            {
                JobLocation selectedJobLocation = (JobLocation)selectedCheckBox.DataContext;
                companyReviewFilter.removeLocation(selectedJobLocation.location);
                handleLocationSelection = false;
                handleLocationCheckbox(selectedCheckBox);
                handleLocationSelection = true;
            }
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

            bool isAllLocationSelected = (from n in jobLocations where n.isSelected == false select n).Count() == 1;
            if (isAllLocationSelected)
            {
                JobLocation selectAllLocation = (from n in jobLocations where n.location == "Select all" select n).ToList()[0];
                selectAllLocation.isSelected = true;
            }

            foreach (JobLocation location in jobLocations)
            {
                if (location.isSelected == true)
                {
                    if (location.location == "Select all")
                    {
                        companyReviewFilter.addLocation("All locations");
                        break;
                    }
                    else
                    {
                        companyReviewFilter.addLocation(location.location);
                    }
                }
            }

            ObservableCollection<CompanyReview> tempReviews = companyReviewFilter.filterByCriteria();
            Itc_reviews.ItemsSource = tempReviews;
            if (tempReviews.Count > 0)
            {
                isNoDataVisible = false;
            }
            else
            {
                isNoDataVisible = true;
            }
            Itc_FilterTags.ItemsSource = companyReviewFilter.getUIFilterTags();
        }

        private void Cmb_Empl_Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox empStatusCB = (ComboBox)sender;
            companyReviewFilter.addEmpStatusFilter((string)empStatusCB.SelectedValue);
            ObservableCollection<CompanyReview> tempReviews = companyReviewFilter.filterByCriteria();
            Itc_reviews.ItemsSource = tempReviews;
            if (tempReviews.Count > 0)
            {
                isNoDataVisible = false;
            }
            else
            {
                isNoDataVisible = true;
            }
            Itc_FilterTags.ItemsSource = companyReviewFilter.getUIFilterTags();
        }

        private void Cmb_Sort_By_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox sortRatingCB = (ComboBox)sender;
            if(((string)sortRatingCB.SelectedValue).Equals("Highest rating") || ((string)sortRatingCB.SelectedValue).Equals("Lowest rating"))
            {
                companyReviewFilter.setSortingByTime(null);
                companyReviewFilter.setSorting(((string)sortRatingCB.SelectedValue).Equals("Highest rating") ? true : false);
            }
            else if(((string)sortRatingCB.SelectedValue).Equals("Newest first") || ((string)sortRatingCB.SelectedValue).Equals("Oldest first"))
            {
                companyReviewFilter.setSorting(null);
                companyReviewFilter.setSortingByTime(((string)sortRatingCB.SelectedValue).Equals("Newest first") ? true : false);
            }
            ObservableCollection<CompanyReview> tempReviews = companyReviewFilter.filterByCriteria();
            Itc_reviews.ItemsSource = tempReviews;
            if (tempReviews.Count > 0)
            {
                isNoDataVisible = false;
            }
            else
            {
                isNoDataVisible = true;
            }
            Itc_FilterTags.ItemsSource = companyReviewFilter.getUIFilterTags();
        }

        
    }

}
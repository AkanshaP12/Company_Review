using Company_Review.classes;
using Company_Review.core;
using Company_Review.core.Converter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Company_Review.Controls
{
    /// <summary>
    /// Interaction logic for AddReviewUC.xaml
    /// </summary>
    public partial class AddReviewUC : UserControl, INotifyPropertyChanged
    {
        public MainWindow mainWindow;
        public Review currentReview { get; set; } 
        public ObservableCollection<CompanyOverview> companiesForComboBox;
        public ObservableCollection<String> departmentForComboBox;
        //private string comboBoxSearchText_;
        //public string comboBoxSearchText
        //{
        //    get { return comboBoxSearchText_; }
        //    set
        //    {
        //        if (comboBoxSearchText_ == value) return;
        //        comboBoxSearchText_ = value;
        //        OnPropertyChanged("comboBoxSearchText");
        //    }
        //}

        public AddReviewUC(MainWindow parentWindow)
        {
            InitializeComponent();
            this.DataContext = this;
            this.mainWindow = parentWindow;
            currentReview = new Review();
        }

        private void Btn_Back_To_All_Companies_Click(object sender, RoutedEventArgs e)
        {
            this.mainWindow.DisplayUserControl = new ViewReviewsUC(this.mainWindow);
        }

        private void Btn_nextReview_Click(object sender, RoutedEventArgs e)
        {
            foreach (TabItem tab in Tbc_Add_ReviewTC.Items)
            {
                if((string)tab.Header == "Review")
                {
                    tab.IsSelected = true;
                }
                else
                {
                    tab.IsSelected = false;
                }
            }
        }

        private void Btn_BackToJobDetails_Click(object sender, RoutedEventArgs e)
        {
            foreach (TabItem tab in Tbc_Add_ReviewTC.Items)
            {
                if ((string)tab.Header == "Job Details")
                {
                    tab.IsSelected = true;
                }
                else
                {
                    tab.IsSelected = false;
                }
            }
        }

        private void Btn_SubmitReview_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = validateInputs();
            if(isValid)
            {
                writeToReviewsXml();
                currentReview = null;
                this.mainWindow.DisplayUserControl = new ViewReviewsUC(this.mainWindow);
            }
        }

        private bool validateInputs()
        {
            currentReview.postedOnDateTime = DateTime.Now;

            if (currentReview == null)
            {
                MessageBox.Show("Company review not provided.");
                return false;
            }

            if(String.IsNullOrEmpty(currentReview.companyName))
            {
                MessageBox.Show("Company name not provided.");
                return false;
            }
            if(string.IsNullOrEmpty(currentReview.jobLocation))
            {
                MessageBox.Show("Location not provided");
                return false;
            }
            if(string.IsNullOrEmpty(currentReview.jobDepartment))
            {
                MessageBox.Show("Department not provided");
                return false;
            }
            if(string.IsNullOrEmpty(currentReview.jobDesignation))
            {
                MessageBox.Show("Designation not provided");
                return false;
            }
            if(string.IsNullOrEmpty(currentReview.employmentStatus))
            {
                MessageBox.Show("Please select Employment Status");
                return false;
            }
            if(string.IsNullOrEmpty(currentReview.skillsForInput))
            {
                MessageBox.Show("Please provide skills for the profile");
                return false;
            }

            if(string.IsNullOrEmpty(currentReview.reviewTitle))
            {
                MessageBox.Show("Review Title not provided");
                return false;
            }

            populatePros();
            populateCons();
            populateSkills();

            if((bool)Rtn_Is_Current.IsChecked)
            {
                currentReview.jobStatus = "Current";
            }
            else
            {
                currentReview.jobStatus = "Former";
            }

            if((bool) Rtn_Recomm_Yes.IsChecked)
            {
                currentReview.wouldYouRecommend = "Yes";
            }
            else
            {
                currentReview.wouldYouRecommend = "No";
            }

            return true;
        }

        private void populatePros()
        {
            string[] stringSeparators = new string[] { "\r\n" };
            String[] prosInput = currentReview.prosAsString.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            Pros pros = new Pros();
            List<Pro> allPros = new List<Pro>();
            if (prosInput != null)
            {
                foreach (String pro in prosInput)
                {
                    Pro newPro = new Pro() { comment = pro };
                    allPros.Add(newPro);
                }
            }
            pros.pros = allPros;
            currentReview.pros = pros;
        }

        private void populateCons()
        {
            string[] stringSeparators = new string[] { "\r\n" };
            String[] consInput = currentReview.consAsString.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            Cons cons = new Cons();
            List<Con> allCons = new List<Con>();
            if (consInput != null)
            {
                foreach (String con in consInput)
                {
                    Con newCon = new Con() { comment = con };
                    allCons.Add(newCon);
                }
            }
            cons.cons = allCons;
            currentReview.cons = cons;
        }

        private void populateSkills()
        {
            SkillRequired skillRequired = new SkillRequired();
            List<Skill> skills = new List<Skill>();
            if (String.IsNullOrEmpty(currentReview.skillsForInput) == false)
            {
                string[] allSkills = currentReview.skillsForInput.Split(',');
                foreach(string skill in allSkills)
                {
                    Skill newSkill = new Skill() { name = skill.Trim() };
                    skills.Add(newSkill);
                }
            }
            skillRequired.skills = skills;

            currentReview.skillRequired = skillRequired;
        }

        private void writeToReviewsXml()
        {
            Companies companiesFromFile = XMLSerializerWrapper.ReadXml<Companies>("data\\companies.xml");
            CompanyOverview selectedCompany = null;

            foreach(CompanyOverview eachCompany in companiesFromFile.companyDetails)
            {
                if(eachCompany.name.ToLower().Equals(currentReview.companyName.ToLower()))
                {
                    selectedCompany = eachCompany;
                }
            }

            if(selectedCompany != null)
            {
                currentReview.companyId = selectedCompany.id;
            }
            else
            {
                int newCompanyId = Properties.Settings.Default.next_company_id;
                currentReview.companyId = newCompanyId;
                CompanyOverview newCompanyOverview = new CompanyOverview() { id = newCompanyId, name= currentReview.companyName};

                companiesFromFile.companyDetails.Add(newCompanyOverview);
                XMLSerializerWrapper.WriteXml<Companies>(companiesFromFile, "data\\companies.xml");

                Properties.Settings.Default.next_company_id = newCompanyId + 1;
                Properties.Settings.Default.Save();
            }

            Reviews reviews = XMLSerializerWrapper.ReadXml<Reviews>("data\\reviews.xml");

            currentReview.id = Properties.Settings.Default.next_company_id;
            Properties.Settings.Default.next_company_id = Properties.Settings.Default.next_company_id + 1;
            Properties.Settings.Default.Save();
            reviews.reviews.Add(currentReview);
            XMLSerializerWrapper.WriteXml<Reviews>(reviews, "data\\reviews.xml");
            MessageBox.Show("Review added successfully");

        }

        private void Rtn_Is_Former_Checked(object sender, RoutedEventArgs e)
        {
            Stk_endDate.Visibility = Visibility.Visible;
            
        }

        private void Rtn_Is_Former_Unchecked(object sender, RoutedEventArgs e)
        {
            Stk_endDate.Visibility = Visibility.Collapsed;

        }

        private void Cb_companyName_KeyUp(object sender, KeyEventArgs e)
        {
            if (String.IsNullOrEmpty(currentReview.companyName))
            {
                cb_companyName.ItemsSource = companiesForComboBox;
                return;
            }

            ObservableCollection<CompanyOverview> tempCompanies = new ObservableCollection<CompanyOverview>((from n in companiesForComboBox where n.name.ToLower().StartsWith(currentReview.companyName.ToLower()) select n).ToList());
            if (tempCompanies != null && tempCompanies.Count > 0)
            {
                cb_companyName.ItemsSource = tempCompanies;
                currentReview.companyName = currentReview.companyName;
            }
            else
            {
                cb_companyName.ItemsSource = tempCompanies;
                cb_companyName.IsDropDownOpen = false;
                currentReview.companyName = currentReview.companyName;
            }
        }

        private void Cb_companyName_KeyDown(object sender, KeyEventArgs e)
        {
            if(cb_companyName.Items.Count > 0)
            {
                cb_companyName.IsDropDownOpen = true;
            }
        }

        private void Cb_companyName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox companyComboBox = sender as ComboBox;
            if (companyComboBox.SelectedItem != null)
            {
                CompanyOverview companyOverview = (CompanyOverview)companyComboBox.SelectedItem;
                if (companyOverview != null)
                {
                    Reviews reviews = XMLSerializerWrapper.ReadXml<Reviews>("data\\reviews.xml");
                    departmentForComboBox = new ObservableCollection<string>((from n in reviews.reviews where n.companyId == companyOverview.id select n.jobDepartment).Distinct().OrderBy(x => x).ToList());
                    Cb_departmentName.ItemsSource = departmentForComboBox;
                }
            }
        }

        private void Cb_companyName_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox companyComboBox = sender as ComboBox;
            companyComboBox.SelectedItem = null;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Companies companiesFromFile = XMLSerializerWrapper.ReadXml<Companies>("data\\companies.xml");
            companiesForComboBox = new ObservableCollection<CompanyOverview>(companiesFromFile.companyDetails);
            cb_companyName.ItemsSource = companiesForComboBox;
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

        private void Cb_departmentName_KeyUp(object sender, KeyEventArgs e)
        {
            if (String.IsNullOrEmpty(currentReview.jobDepartment))
            {
                Cb_departmentName.ItemsSource = departmentForComboBox;
                return;
            }
            if(departmentForComboBox == null)
            {
                Cb_departmentName.IsDropDownOpen = false;
                return;
            }

            ObservableCollection<string> tempDepartments = new ObservableCollection<string>((from n in departmentForComboBox where n.ToLower().StartsWith(currentReview.jobDepartment.ToLower()) select n).ToList());
            if (tempDepartments != null)
            {
                Cb_departmentName.ItemsSource = tempDepartments;
                currentReview.jobDepartment = currentReview.jobDepartment;
            }
        }

        private void Cb_departmentName_KeyDown(object sender, KeyEventArgs e)
        {
            if (departmentForComboBox != null)
            {
                Cb_departmentName.IsDropDownOpen = true;
            }
        }

        private void Cb_departmentName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Cb_departmentName_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox departmentComboBox = sender as ComboBox;
            departmentComboBox.SelectedItem = null;
        }

    }
}

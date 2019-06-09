using Company_Review.classes;
using Company_Review.core;
using Company_Review.core.Converter;
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

namespace Company_Review.Controls
{
    /// <summary>
    /// Interaction logic for AddReviewUC.xaml
    /// </summary>
    public partial class AddReviewUC : UserControl
    {
        public MainWindow mainWindow;
        public Review currentReview { get; set; } 
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
            validateInputs();
            writeToReviewsXml();
        }

        private void validateInputs()
        {
            
            if (currentReview == null)
            {
                MessageBox.Show("Company review not provided.");
                return;
            }

            if(String.IsNullOrEmpty(currentReview.companyName))
            {
                MessageBox.Show("Company name not provided.");
                return;
            }
            if(string.IsNullOrEmpty(currentReview.jobLocation))
            {
                MessageBox.Show("Location not provided");
                return;
            }
            if(string.IsNullOrEmpty(currentReview.jobDepartment))
            {
                MessageBox.Show("Department not provided");
                return;
            }
            if(string.IsNullOrEmpty(currentReview.jobDesignation))
            {
                MessageBox.Show("Designation not provided");
                return;
            }
            if(string.IsNullOrEmpty(currentReview.employmentStatus))
            {
                MessageBox.Show("Please select Employment Status");
                return;
            }
            if(string.IsNullOrEmpty(currentReview.skillsForInput))
            {
                MessageBox.Show("Please provide skills for the profile");
                return;
            }

            if(string.IsNullOrEmpty(currentReview.reviewTitle))
            {
                MessageBox.Show("Review Title not provided");
                return;
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
            reviews.reviews.Add(currentReview);
            XMLSerializerWrapper.WriteXml<Reviews>(reviews, "data\\reviews.xml");
            MessageBox.Show("Review added successfully");
        }
    }
}

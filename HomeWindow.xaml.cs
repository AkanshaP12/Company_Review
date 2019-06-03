using Company_Review.classes;
using Company_Review.core;
using Company_Review.core.Converter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Company_Review
{
    /// <summary>
    /// Interaction logic for HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        ObservableCollection<CompanyReview> companies;
        List<CompanyOverview> companyOverviews;

        public HomeWindow()
        {
            InitializeComponent();
            //GenerateCompanies();
            loadFromFile();
            Lbx_companies.ItemsSource = companies;
            Itc_reviews.ItemsSource = companyOverviews;
        }

        private void loadFromFile()
        {
            Companies companyOverviewsFromFile = XMLSerializerWrapper.ReadXml<Companies>("data\\companies.xml");
            companyOverviews = companyOverviewsFromFile.companyDetails;
            companies = new ObservableCollection<CompanyReview>();
            Reviews reviews = XMLSerializerWrapper.ReadXml<Reviews>("data\\reviews.xml");
            foreach(CompanyOverview companyOverview in companyOverviews)
            {
                CompanyReview companyReview = new CompanyReview();
                companyReview.companyOverview = companyOverview;
                companyReview.reviews = (from n in reviews.reviews where n.companyId == companyOverview.id select n).ToList();
                companyOverview.noOfReviews = companyReview.reviews.Count;
                if(companyOverview.noOfReviews > 0)
                {
                    companyOverview.avgRating = (from n in reviews.reviews where n.companyId == companyOverview.id select float.Parse(n.rating)).Sum() / companyOverview.noOfReviews;
                }

                if(companyOverview.noOfReviews > 0)
                {
                    companyOverview.wouldRecommended = (from n in reviews.reviews where n.companyId == companyOverview.id & n.wouldYouRecommend == "Yes" select n.wouldYouRecommend).Count() * 100 / companyOverview.noOfReviews;
                }
                companies.Add(companyReview);
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

        private void Tbx_filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = Tbx_filter.Text;
            if (filter == "")
                Lbx_companies.ItemsSource = companies;
            else
            {
                var results = from s in companies where s.companyOverview.name.ToLower().Contains(filter.ToLower()) select s;
                Lbx_companies.ItemsSource = results;
            }
        }

        private void Lbx_companies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Lbx_companies.SelectedItem != null)
            {
                Grd_All_Companies.Visibility = Visibility.Collapsed;
                Grd_Company_Selected.Visibility = Visibility.Visible;
                Stp_View_Reviews_Filters.Visibility = Visibility.Visible;
            }
            else
            {
                Grd_All_Companies.Visibility = Visibility.Visible;
                Grd_Company_Selected.Visibility = Visibility.Collapsed;
                Stp_View_Reviews_Filters.Visibility = Visibility.Collapsed;

            }
        }

        private void Btn_Back_To_All_Companies_Click(object sender, RoutedEventArgs e)
        {
            Lbx_companies.SelectedItem = null;
        }
    }
}

﻿using System;
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
    public partial class ViewReviewsUC : UserControl
    {
        public ObservableCollection<CompanyReview> companies;
        public List<CompanyOverview> companyOverviews;
        public MainWindow mainWindow;
        public string language;
        public List<string> cultures = new List<string> { "en english", "de detush" };
       
       
        

        public ViewReviewsUC(MainWindow mainWindow)
        {
           
            language = Properties.Settings.Default.language;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            

            InitializeComponent();
            //GenerateCompanies();
            loadFromFile();
            Lbx_companies.ItemsSource = companies;
            Itc_reviews.ItemsSource = companyOverviews;
            this.DataContext = this;
            this.mainWindow = mainWindow;
            loadCultures();
            
            
        }

        private void loadCultures()
        {
            List<ComboBoxItem> cultureItems = new List<ComboBoxItem>();
          foreach(string culture in cultures)
            {
                ComboBoxItem cb = new ComboBoxItem();
                cb.Content = culture;
                cultureItems.Add(cb);

            }
            Cbx_lang.ItemsSource = cultureItems;
        }

        private void loadFromFile()
        {
            Companies companyOverviewsFromFile = XMLSerializerWrapper.ReadXml<Companies>("data\\companies.xml");
            companyOverviews = companyOverviewsFromFile.companyDetails;
            companies = new ObservableCollection<CompanyReview>();
            Reviews reviews = XMLSerializerWrapper.ReadXml<Reviews>("data\\reviews.xml");
            foreach (CompanyOverview companyOverview in companyOverviews)
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

        private void Btn_AddReview_Click(object sender, RoutedEventArgs e)
        {
            this.mainWindow.DisplayUserControl = new AddReviewUC();
        }

        private void Cbx_lang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string txt =((ComboBoxItem) (sender as ComboBox).SelectedItem).Content.ToString();
            language = txt.Substring(0, 2);
            Properties.Settings.Default.language = language;
            Properties.Settings.Default.Save();
        }
    }
}
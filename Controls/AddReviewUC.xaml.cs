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

namespace Company_Review.Controls
{
    /// <summary>
    /// Interaction logic for AddReviewUC.xaml
    /// </summary>
    public partial class AddReviewUC : UserControl
    {
        public AddReviewUC()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        private void Btn_Back_To_All_Companies_Click(object sender, RoutedEventArgs e)
        {
            MainWindow back = new MainWindow();
        }

        private void Dp_startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = "date";
        }

        private void Dp_endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
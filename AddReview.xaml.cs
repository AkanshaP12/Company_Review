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

namespace Company_Review
{
    /// <summary>
    /// Interaction logic for AddReview.xaml
    /// </summary>
    public partial class AddReview : Page
    {
        public AddReview()
        {
            InitializeComponent();
        }

        private void Btn_view_review_Click(object sender, RoutedEventArgs e)
        {
            ViewReviews vr = new ViewReviews();
            this.NavigationService.Navigate(vr);
        }
    }
}

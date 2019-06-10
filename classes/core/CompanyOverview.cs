using Company_Review.core;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System;

namespace Company_Review.classes
{
    public class CompanyOverview
    {
        public int id { get; set; }
        public string name { get; set; }

        [XmlIgnore]
        public int noOfReviews { get; set; }

        [XmlIgnore]
        public float avgRating { get; set; } = 0.0f;

        [XmlIgnore]
        public float wouldRecommended { get; set; } = 0.0f;

        [XmlIgnore]
        public string departmentName { get; set; } = "All departments";

        public string logoPath { get; set; } = @"..\Resources\Images\Logos\unknown.png";

        public string lastPostedDate { get; set; } = "01/01/1970";

        [XmlIgnore]
        public DateTime lastPostedDateTime
        {
            get
            {
                if (String.IsNullOrEmpty(lastPostedDate))
                {
                    return DateTime.Now;
                }

                string[] items = lastPostedDate.Split('/');
                return new DateTime(Int32.Parse(items[2]), Int32.Parse(items[1]), Int32.Parse(items[0]));
            }
            set
            {
                lastPostedDate = "" + value.Day + "/" + value.Month + "/" + value.Year;
            }
        }

        public void calculateReviewStatistics(List<Review> reviews)
        {
            this.noOfReviews = reviews.Count;
            if (this.noOfReviews > 0)
            {
                this.avgRating = (from n in reviews select float.Parse(n.rating)).Sum() / this.noOfReviews;
            }

            if (this.noOfReviews > 0)
            {
                this.wouldRecommended = (from n in reviews where n.wouldYouRecommend == "Yes" select n.wouldYouRecommend).Count() * 100 / this.noOfReviews;
            }

            if(this.noOfReviews > 0)
            {
                this.lastPostedDate = (from n in reviews select n.postedDate).OrderByDescending(x => x).ToList()[0];
            }
        }

    }
}
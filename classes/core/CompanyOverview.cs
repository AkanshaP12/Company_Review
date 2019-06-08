using System.Xml.Serialization;

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

        public string logoPath { get; set; }
    }
}
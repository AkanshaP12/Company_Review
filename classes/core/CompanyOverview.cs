namespace Company_Review.classes
{
    public class CompanyOverview
    {
        public int id { get; set; }
        public string name { get; set; }

        public int noOfReviews { get; set; }

        public float avgRating { get; set; } = 0.0f;

        public float wouldRecommended { get; set; } = 0.0f;

        public string logoPath { get; set; }
    }
}
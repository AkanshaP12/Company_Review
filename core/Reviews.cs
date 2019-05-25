using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company_Review.core
{
    public class Reviews
    {
        public List<Review> customerReviews;
    }
    public class Review
    {
        public int id;
        public int companyId;
        public float ratings;
        public string jobStatus;
        public string jobLocation;
        public int lastYearEmployer;
        public string employmentStatus;
        public List<Skill> skills;
        public string reviewTitle;
        public List<Pro> Pros;
        public List<Con> Cons;
        public string wouldYouRecommend;


    }

    public class Skill
    {
        public string name;
    }
    public class Pro
    {
        public string comment;
    }
    public class Con
    {
        public string comment;
    }
}

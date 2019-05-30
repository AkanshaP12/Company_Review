using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Company_Review.core
{
    [XmlRoot("userReviews")]
    public class Reviews
    {
        [XmlElement("review")]
        public List<Review> reviews;
    }
    public class Review
    {
        
        public int id;
        [XmlElement("company-id")]
        public int companyId;

        //static CultureInfo ci = CultureInfo.InvariantCulture;
        //[XmlIgnore]
        //float _ratings = 0;
        //[XmlIgnore]
        //public float ratings
        //{
        //    get { return _ratings; }
        //    set { _ratings = value; }
        //}
        //[XmlElement("ratings")]
        //public string floatRatings { get { return ratings.ToString("#0.00", ci); } set { float.TryParse(value, NumberStyles.Float, ci, out _ratings);
        //    } }

        public string rating;

        public string jobStatus;
        public string jobLocation;
        public string jobDepartment;
        public string startDate;
        public string endDate;
        public string employmentStatus;
        [XmlElement("skillsrequired")]
        public SkillRequired skillRequired;
        public string reviewTitle;
        public Pros pros;
        public Cons cons;
        public string wouldYouRecommend;
    }

    public class SkillRequired
    {
        [XmlElement("skill")]
        public List<Skill> skills;

    }

    public class Skill
    {
        public string name;
    }

    public class Pros
    {
        [XmlElement("pro")]
        public List<Pro> pros;
    }

    public class Cons
    {
        [XmlElement("cons")]
        public List<Con> cons;
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

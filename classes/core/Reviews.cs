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
        public List<Review> reviews { get; set; }
    }
    public class Review
    {
        
        public int id { get; set; }
        [XmlElement("company-id")]
        public int companyId { get; set; }

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

        public string rating { get; set; }

        public string jobStatus { get; set; }
        public string jobLocation { get; set; }
        public string jobDepartment { get; set; }
        public string jobDesignation { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string employmentStatus { get; set; }
        [XmlElement("skillsrequired")]
        public SkillRequired skillRequired { get; set; }

        [XmlIgnore]
        public string skillsAsString
        {
            get
            {
                if(skillRequired != null)
                {
                    string outputdata = "Skills specified for this job profile: ";
                    foreach(Skill skill in skillRequired.skills)
                    {
                        outputdata += skill.name + ", ";
                    }
                    return outputdata.Substring(0,outputdata.Length-2);
                }
                else
                {
                    return "Skills not specified";
                }
            }
        }
        public string reviewTitle { get; set; }
        public Pros pros { get; set; }
        public Cons cons { get; set; }
        public string wouldYouRecommend { get; set; }
    }

    public class SkillRequired
    {
        [XmlElement("skill")]
        public List<Skill> skills { get; set; }

    }

    public class Skill
    {
        public string name { get; set; }
    }

    public class Pros
    {
        [XmlElement("pro")]
        public List<Pro> pros { get; set; }
    }

    public class Cons
    {
        [XmlElement("con")]
        public List<Con> cons { get; set; }
    }

    public class Pro
    {
        public string comment { get; set; }
    }
    public class Con
    {
        public string comment { get; set; }
    }
}

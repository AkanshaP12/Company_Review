using Company_Review.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Company_Review.core
{
    [XmlRoot("companyDetails")]
   public class Companies
    {
        [XmlElement("company")]
        public List<CompanyOverview> companyDetails;
    }

}

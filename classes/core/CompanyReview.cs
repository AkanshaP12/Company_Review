using Company_Review.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company_Review.classes
{
    public class CompanyReview
    {
        public CompanyOverview companyOverview { get; set; }

        public List<Review> reviews { get; set; }
    }
}

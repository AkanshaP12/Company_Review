using Company_Review.core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company_Review.classes.core
{
    public class CompanyReviewFilter
    {
        public List<CompanyOverview> companyOverviewsFromFile;

        public List<Review> reviewsFromFile;

        public ObservableCollection<CompanyReview> companiesReview = new ObservableCollection<CompanyReview>();

        public int companyIdToBeFiltered = -1;

        public HashSet<string> departmentsToBeFiltered = new HashSet<string>();

        public HashSet<string> locationsToBeFiltered = new HashSet<string>();

        public string empStatusFilter = "All status";

        public bool? isSortByDescending = null;

        public bool? isSortByNewestFirst = null;

        public CompanyReviewFilter(Companies companies, List<Review> allReviews)
        {
            companyOverviewsFromFile = companies.companyDetails;
            reviewsFromFile = allReviews;
        }

        public void setCompanyId(int companyId)
        {
            companyIdToBeFiltered = companyId;
            departmentsToBeFiltered = new HashSet<string>();
            locationsToBeFiltered = new HashSet<string>();
        }

        public void addDepartment(string departmentName)
        {
            if(departmentName == "All departments")
            {
                departmentsToBeFiltered = new HashSet<string>();
                return;
            }
            departmentsToBeFiltered.Add(departmentName);
        }

        public void removeDepartment(string departmentName)
        {
            if(departmentsToBeFiltered.Contains(departmentName))
            {
                departmentsToBeFiltered.Remove(departmentName);
            }
        }

        public void addLocation(string locationName)
        {
            if (locationName == "All locations")
            {
                locationsToBeFiltered = new HashSet<string>();
                return;
            }
            locationsToBeFiltered.Add(locationName);
        }

        public void removeLocation(string locationName)
        {
            if (locationsToBeFiltered.Contains(locationName))
            {
                locationsToBeFiltered.Remove(locationName);
            }
        }

        public void addEmpStatusFilter(string empStatus)
        {
            empStatusFilter = empStatus;
        }

        public void setSorting(bool? sortByDescending)
        {
            isSortByDescending = sortByDescending;
        }

        public void setSortingByTime(bool? sortByNewest)
        {
            isSortByNewestFirst = sortByNewest;
        }

        public ObservableCollection<CompanyReview> filterByCriteria()
        {
            ObservableCollection<CompanyReview> tempFilteredList = new ObservableCollection<CompanyReview>();

            List<Review> sortedReviews = new List<Review>();
            //STEP 0 : Apply sorting
            if(isSortByDescending != null)
            {
                if((bool)isSortByDescending)
                {
                    sortedReviews = (from n in reviewsFromFile select n).OrderByDescending(x => x.rating).ToList();
                }
                else
                {
                    sortedReviews = (from n in reviewsFromFile select n).OrderBy(x => x.rating).ToList();
                }
            }
            else if(isSortByNewestFirst != null)
            {
                if((bool)isSortByNewestFirst)
                {
                    sortedReviews = (from n in reviewsFromFile select n).OrderByDescending(x => x.postedOnDateTime).ToList();
                }
                else
                {
                    sortedReviews = (from n in reviewsFromFile select n).OrderBy(x => x.postedOnDateTime).ToList();
                }
            }
            else
            {
                sortedReviews = reviewsFromFile;
            }

            List<Review> reviewsToBeFiltered;
            //STEP 1 : Filter reviews by location
            if (locationsToBeFiltered.Count > 0)
            {
                reviewsToBeFiltered = (from n in sortedReviews where locationsToBeFiltered.Contains(n.jobLocation) select n).ToList();
            }
            else
            {
                reviewsToBeFiltered = sortedReviews;
            }

            //STEP 2: Apply emp status filter
            if(!empStatusFilter.Equals("All status"))
            {
                reviewsToBeFiltered = (from n in reviewsToBeFiltered where n.employmentStatus.ToLower().Equals(empStatusFilter.ToLower()) select n).ToList();
            }


            //STEP 3 : Apply company id filter
            bool isAnyCompanySelected = false;
            if(companyIdToBeFiltered > 0)
            {
                isAnyCompanySelected = true;
            }

            foreach(CompanyOverview eachCompanyOverview in companyOverviewsFromFile)
            {
                if(isAnyCompanySelected)
                {
                    if(eachCompanyOverview.id == companyIdToBeFiltered)
                    {
                        collectCompanyReviews(eachCompanyOverview, reviewsToBeFiltered, tempFilteredList);
                    }
                }
                else
                {
                    collectCompanyReviews(eachCompanyOverview, reviewsToBeFiltered, tempFilteredList);
                }
            }

            //STEP 4: Apply department filter

            if(departmentsToBeFiltered.Count > 0)
            {
                tempFilteredList = new ObservableCollection<CompanyReview>(from n in tempFilteredList where departmentsToBeFiltered.Contains(n.companyOverview.departmentName) select n);
            }

            //STEP 5: Apply sorting
            if (isSortByDescending != null)
            {
                if ((bool)isSortByDescending)
                {
                    tempFilteredList = new ObservableCollection<CompanyReview>((from n in tempFilteredList select n).OrderByDescending(x => x.companyOverview.avgRating));
                }
                else
                {
                    tempFilteredList = new ObservableCollection<CompanyReview>((from n in tempFilteredList select n).OrderBy(x => x.companyOverview.avgRating));
                }
            }
            else if (isSortByNewestFirst != null)
            {
                if ((bool)isSortByNewestFirst)
                {
                    tempFilteredList = new ObservableCollection<CompanyReview>((from n in tempFilteredList select n).OrderByDescending(x => x.companyOverview.lastPostedDateTime));
                }
                else
                {
                    tempFilteredList = new ObservableCollection<CompanyReview>((from n in tempFilteredList select n).OrderBy(x => x.companyOverview.lastPostedDateTime));
                }
            }

            companiesReview = tempFilteredList;
            return companiesReview;
        }


        private void collectCompanyReviews(CompanyOverview companyOverview, List<Review> reviews, ObservableCollection<CompanyReview> targetCollection)
        {
            CompanyReview companyReview = new CompanyReview();
            companyReview.companyOverview = companyOverview;
            companyReview.reviews = (from n in reviews where n.companyId == companyOverview.id select n).ToList();
            companyOverview.calculateReviewStatistics(companyReview.reviews);
            if(companyReview.reviews.Count <= 0)
            {
                return;
            }
            targetCollection.Add(companyReview);

            List<IGrouping<string, Review>> reviewsByDepartment = (from n in companyReview.reviews group n by n.jobDepartment into d select d).ToList();
            foreach (IGrouping<string, Review> groupedReviews in reviewsByDepartment)
            {
                if(groupedReviews.ToList().Count <= 0)
                {
                    continue;
                }
                CompanyReview companyReviewByDepartment = new CompanyReview();
                CompanyOverview companyOverviewByDepartment = new CompanyOverview();
                companyOverviewByDepartment.id = companyOverview.id;
                companyOverviewByDepartment.name = companyOverview.name;
                companyOverviewByDepartment.logoPath = companyOverview.logoPath;
                companyOverviewByDepartment.departmentName = groupedReviews.Key;

                companyReviewByDepartment.reviews = groupedReviews.ToList();

                companyOverviewByDepartment.calculateReviewStatistics(companyReviewByDepartment.reviews);
                companyReviewByDepartment.companyOverview = companyOverviewByDepartment;
                targetCollection.Add(companyReviewByDepartment);
            }
        }

        public List<string> getUIFilterTags()
        {
            List<string> filterTags = new List<string>();
            if(companyIdToBeFiltered > 0)
            {
                filterTags.Add((from n in companyOverviewsFromFile where n.id == companyIdToBeFiltered select n.name).ToList()[0]);
            }

            if(empStatusFilter == "All status")
            {
                filterTags.Add("All employment status");
            }
            else
            {
                filterTags.Add(empStatusFilter);
            }
            
            if(departmentsToBeFiltered.Count == 0)
            {
                filterTags.Add("All departments");
            }
            else
            {
                filterTags.AddRange(departmentsToBeFiltered.ToList());
            }

            if(locationsToBeFiltered.Count == 0)
            {
                filterTags.Add("All locations");
            }
            else
            {
                filterTags.AddRange(locationsToBeFiltered.ToList());
            }

            return filterTags;
        }
    }
}

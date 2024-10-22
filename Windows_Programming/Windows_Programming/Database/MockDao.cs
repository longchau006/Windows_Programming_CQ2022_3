using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Model;
using Windows_Programming.Database;
using System.Reflection.Metadata;
using System.Security.Principal;

namespace Windows_Programming.Database
{
    public class MockDao : IDao
    {
        public List<Plan> GetAllPlanInHome()
        {

            var result = new List<Plan>
        {
            new Plan
            {
                Name = "Plan 1",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 10),
                Description = "Description for Plan 1",
                StartLocation = "Location A",
                EndLocation = "Location B",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 1", Description = "Description for Activity 1" },
                    new Activity { Name = "Activity 2", Description = "Description for Activity 2" }
                },
                Type = true, // Traveller
                DeletedDate = new DateTime(2023, 1, 8),
            },
            new Plan
            {
                Name = "Plan 2",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 2, 1),
                EndDate = new DateTime(2023, 2, 10),
                Description = "Description for Plan 2",
                StartLocation = "Location C",
                EndLocation = "Location D",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 3", Description = "Description for Activity 3" },
                    new Activity { Name = "Activity 4", Description = "Description for Activity 4" }
                },
                Type = false, // Non-Traveller
                DeletedDate = new DateTime(2023, 2, 8),
            },
            new Plan
            {
                Name = "Plan 3",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 3, 1),
                EndDate = new DateTime(2023, 3, 10),
                Description = "Description for Plan 3",
                StartLocation = "Location E",
                EndLocation = "Location F",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 5", Description = "Description for Activity 5" },
                    new Activity { Name = "Activity 6", Description = "Description for Activity 6" }
                },
                Type = true, // Traveller
                DeletedDate = new DateTime(2023, 3, 8),
            },
            new Plan
            {
                Name = "Plan 4",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 4, 1),
                EndDate = new DateTime(2023, 4, 10),
                Description = "Description for Plan 4",
                StartLocation = "Location G",
                EndLocation = "Location H",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 7", Description = "Description for Activity 7" },
                    new Activity { Name = "Activity 8", Description = "Description for Activity 8" }
                },
                Type = false, // Non-Traveller
                DeletedDate = new DateTime(2023, 3, 8),
            },
            new Plan
            {
                Name = "Plan 5",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 5, 1),
                EndDate = new DateTime(2023, 5, 10),
                Description = "Description for Plan 5",
                StartLocation = "Location I",
                EndLocation = "Location J",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 9", Description = "Description for Activity 9" },
                    new Activity { Name = "Activity 10", Description = "Description for Activity 10" }
                },
                Type = true, // Traveller
                DeletedDate = new DateTime(2023, 5, 8),
            }
        };
            return result;
        }
        public List<Plan> GetAllPlanInTrashCan()
        {

            var result = new List<Plan>
        {
            new Plan
            {
                Name = "Plan 1",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 1, 10),
                Description = "Description for Plan 1",
                StartLocation = "Location A",
                EndLocation = "Location B",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 1", Description = "Description for Activity 1" },
                    new Activity { Name = "Activity 2", Description = "Description for Activity 2" }
                },
                Type = true, // Traveller
                DeletedDate = new DateTime(2023, 1, 8),
            },
            new Plan
            {
                Name = "Plan 2",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 2, 1),
                EndDate = new DateTime(2023, 2, 10),
                Description = "Description for Plan 2",
                StartLocation = "Location C",
                EndLocation = "Location D",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 3", Description = "Description for Activity 3" },
                    new Activity { Name = "Activity 4", Description = "Description for Activity 4" }
                },
                Type = false, // Non-Traveller
                DeletedDate = new DateTime(2023, 2, 8),
            },
            new Plan
            {
                Name = "Plan 3",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 3, 1),
                EndDate = new DateTime(2023, 3, 10),
                Description = "Description for Plan 3",
                StartLocation = "Location E",
                EndLocation = "Location F",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 5", Description = "Description for Activity 5" },
                    new Activity { Name = "Activity 6", Description = "Description for Activity 6" }
                },
                Type = true, // Traveller
                DeletedDate = new DateTime(2023, 3, 8),
            },
            new Plan
            {
                Name = "Plan 4",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 4, 1),
                EndDate = new DateTime(2023, 4, 10),
                Description = "Description for Plan 4",
                StartLocation = "Location G",
                EndLocation = "Location H",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 7", Description = "Description for Activity 7" },
                    new Activity { Name = "Activity 8", Description = "Description for Activity 8" }
                },
                Type = false, // Non-Traveller
                DeletedDate = new DateTime(2023, 3, 8),
            },
            new Plan
            {
                Name = "Plan 5",
                PlanImage = "/Assets/danang.jpg",
                StartDate = new DateTime(2023, 5, 1),
                EndDate = new DateTime(2023, 5, 10),
                Description = "Description for Plan 5",
                StartLocation = "Location I",
                EndLocation = "Location J",
                Activities = new List<Activity>
                {
                    new Activity { Name = "Activity 9", Description = "Description for Activity 9" },
                    new Activity { Name = "Activity 10", Description = "Description for Activity 10" }
                },
                Type = true, // Traveller
                DeletedDate = new DateTime(2023, 5, 8),
            }
        };
            return result;
        }
        public List<Account> GetAllAccount()
        {
            var result = new List<Account>
            {
                new Account
                {
                    Username = "admin",
                    Email = "abd",
                    Address ="xit",
                    Fullname ="haha"
                },
                new Account
                {
                    Username = "dog",
                    Email = "bird",
                    Address ="fish",
                    Fullname ="mouse"
                }
            };
            return result;
        }

        public List<Blog> GetAllBlog()
        {
            var result = new List<Blog>
            {
                new Blog
                {
                    Title = "Hello",
                    Content = "World",
                    Author = "Me",
                    Date = DateTime.Now
                },
                new Blog
                {
                    Title = "Goodbye",
                    Content = "Cruel World",
                    Author = "You",
                    Date = DateTime.Now
                }
            };
            return result;
        }
        public List<Tour> GetAllTour()
        {
            var result = new List<Tour>
            {
                new Tour
                {
                    Id = 1,
                    Name = "Test",
                    Description = "Test",
                    Image = "ms-appx:///Assets/sample-image.jpg",
                    Rating = 4,
                    Places = new List<string> { "ho chi minh", "Da nang" },
                    Price = 1
                }
            };
            return result;
        }
    }
}

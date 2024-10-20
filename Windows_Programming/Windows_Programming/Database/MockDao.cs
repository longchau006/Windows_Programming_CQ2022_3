using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Model;
using Windows_Programming.Database;

namespace Windows_Programming.Database
{
    public class MockDao : IDao
    {       
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

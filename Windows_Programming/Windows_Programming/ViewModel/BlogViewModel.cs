using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Database;
using Windows_Programming.Model;

namespace Windows_Programming.ViewModel
{
    public class BlogViewModel
    {
        public List<Blog> AllBlog { get; set; }
        public void AddBlog(Blog blog)
        {
            IDao dao = FirebaseServicesDAO.Instance;
            dao.AddBlog(blog);
        }

        public async Task GetAllBlog()
        {
            IDao dao = FirebaseServicesDAO.Instance;
            AllBlog = await dao.GetAllBlogAsync();
        }

        public List<Blog> lastestBlog { get; set; }
        public async Task GetLastestBlog()
        {
            IDao dao = FirebaseServicesDAO.Instance;
            lastestBlog = await dao.GetLastestBlog();
        }

        public async Task<Blog> GetBlogById(string id)
        {
            IDao dao = FirebaseServicesDAO.Instance;
            return await dao.GetBlogById(id);
        }

        public async Task<Account> GetAccount(int id)
        {
            FirebaseServicesDAO firebaseServices = FirebaseServicesDAO.Instance;
            return await firebaseServices.GetAccountByID(id);
        }
    }
}

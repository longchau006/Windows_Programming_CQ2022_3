using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows_Programming.Database;
using Windows_Programming.Model;

namespace Windows_Programming.ViewModel
{
    public class BlogViewModel
    {
        public List<Blog> AllBlog { get; set; }
        public List<Blog> OwnBlogs { get; set; }
        public List<Blog> lastestBlog { get; set; }
        public Blog blog { get; set; }

        public BlogViewModel()
        {
            AllBlog = new List<Blog>();
            OwnBlogs = new List<Blog>();
            lastestBlog = new List<Blog>();
        }

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

        public void GetLastestBlog()
        {
            lastestBlog = AllBlog.OrderByDescending(blog => blog.PublishDate).Take(3).ToList();
        }

        public async Task<Blog> GetBlogById(string id)
        {
            IDao dao = FirebaseServicesDAO.Instance;
            blog = await dao.GetBlogById(id);
            return blog;
        }

        public async Task<Account> GetAccount(int id)
        {
            FirebaseServicesDAO firebaseServices = FirebaseServicesDAO.Instance;
            return await firebaseServices.GetAccountByID(id);
        }


        public async Task GetOwnBlog(int id)
        {
            IDao dao = FirebaseServicesDAO.Instance;
            OwnBlogs = await dao.GetOwnBlog(id);
        }

        public async Task UpdateBlog(Blog blog)
        {
            IDao dao = FirebaseServicesDAO.Instance;
            await dao.UpdateBlog(blog);
        }

        public void DeleteBlog(string id)
        {
            IDao dao = FirebaseServicesDAO.Instance;
            dao.DeleteBlog(id);
        }
    }
}

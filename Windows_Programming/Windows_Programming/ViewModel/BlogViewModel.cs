using System;
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
        public List<Blog> blogInBlogList { get; set; }

        public void ViewBlog()
        {
            IDao dao = new MockDao();
            blogInBlogList = dao.GetAllBlog();
        }
        public List<Blog> lastestBlog { get;set; }
        public void ViewLastestBlog()
        {
            IDao dao = new MockDao();
            lastestBlog = dao.GetLastestBlog();
        }

        public Blog GetBlogById(int id)
        {
            IDao dao = new MockDao();
            return dao.GetBlogById(id);
        }
    }
}

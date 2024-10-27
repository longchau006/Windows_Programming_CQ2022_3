using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Model;

namespace Windows_Programming.Database
{
    public interface IDao
    {
        List<Plan> GetAllPlanInTrashCan();
        List<Account> GetAllAccount();
        List<Blog> GetAllBlog();
        List<Tour> GetAllTour();
        List<Blog> GetLastestBlog();
        Blog GetBlogById(int id);
    }
}

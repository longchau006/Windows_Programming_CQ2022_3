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
        List<Plan> GetAllPlanInHome();
        List<Plan> GetAllPlanInTrashCan();
        List<Blog> GetAllBlog();
        List<Tour> GetAllTour();
        List<Blog> GetLastestBlog();
        Blog GetBlogById(int id);
        Tour GetTourById(int id);
        Task addBlog(Blog blog, string path);
        Task addImageToClientStorage(string path);
        Task UpdateFullName(string fullName, int id);
        Task UpdateAddress(string address, int id);
        Task UpdatePassword(string oldPassword, string newPassword);
        Task DeleteUser(string email, string password, int id);
    }
}

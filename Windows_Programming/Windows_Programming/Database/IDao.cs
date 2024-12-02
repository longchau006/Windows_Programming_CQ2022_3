using System;
using System.Collections.Generic;
using System.IO;
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
        Task<List<Blog>> GetAllBlogAsync();
        Task<List<Tour>> GetAllTour();
        Task AddTour(Tour tour);
        Task<List<Blog>> GetLastestBlog();
        Task<Blog> GetBlogById(string id);
        Task<Tour> GetTourById(string id);
        Task AddBlog(Blog blog);
        Task AddImageToClientStorage(string imagePath, string imageNameOnFirebase);
        Task UpdateFullName(string fullName, int id);
        Task UpdateAddress(string address, int id);
        Task UpdatePassword(string oldPassword, string newPassword);
        Task DeleteUser(string email, string password, int id);
        Task <MemoryStream> DownloadImageFromClientStorage(string imageName);
    }
}

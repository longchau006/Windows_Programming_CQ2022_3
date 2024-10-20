using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Model;

namespace Windows_Programming.Database
{
    public interface IDao
    {
        List<Account> GetAllAccount();
        List<Blog> GetAllBlog();
        List<Tour> GetAllTour();
    }
}

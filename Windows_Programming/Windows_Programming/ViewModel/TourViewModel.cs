using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Database;
using Windows_Programming.Model;

namespace Windows_Programming.ViewModel
{
    public class TourViewModel
    {
        public List<Tour> allTour { get; set; }

        public void ViewAllTour()
        {
            IDao dao = new MockDao();
            allTour = dao.GetAllTour();
        }

        public Tour Tour { get; set; }
        public void GetTourById(int id)
        {
            IDao dao = new MockDao();
            Tour =  dao.GetTourById(id);
        }
    }
}

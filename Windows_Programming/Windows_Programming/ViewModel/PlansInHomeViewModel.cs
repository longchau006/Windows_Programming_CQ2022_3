using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Printers;
using Windows_Programming.Database;
using Windows_Programming.Model;

namespace Windows_Programming.ViewModel
{
    public class PlansInHomeViewModel 
    {
        public List<Plan> PlansInHome { get; set; }
        public void Init()
        {
            IDao dao = new MockDao();
            PlansInHome = dao.GetAllPlanInHome();
        }
    }
}

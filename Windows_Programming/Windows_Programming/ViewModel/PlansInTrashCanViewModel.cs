using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Database;
using Windows_Programming.Model;

namespace Windows_Programming.ViewModel
{
    public class PlansInTrashCanViewModel
    {
        public List<Plan> PlansInTrashCan { get; set; }

        public void Init()
        {
            IDao dao = new MockDao();
            PlansInTrashCan = dao.GetAllPlanInTrashCan();
        }
    }
}

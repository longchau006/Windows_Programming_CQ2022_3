using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Database;
using Windows_Programming.Model;

namespace Windows_Programming.ViewModel
{
    public class AccountViewModel
    {
        public Account User { get; set; }

        public void getInformation()
        {
            IDao dao = new MockDao();
            for (int i = 0; i< dao.GetAllAccount().Count; i++)
            {
                if (dao.GetAllAccount()[i].Username == "admin")
                {
                    User = dao.GetAllAccount()[i];
                    break;
                }
            }
        }
    }
}

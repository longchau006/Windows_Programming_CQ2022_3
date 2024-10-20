using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Programming.Model
{
    class AccountModel
    {
        string username { get; set; }
        byte[] password { get; set; }
        string email { get; set; }
        string fullname { get; set; }
        string address { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

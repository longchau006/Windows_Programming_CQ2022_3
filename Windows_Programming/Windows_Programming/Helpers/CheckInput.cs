using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Model;

namespace Windows_Programming.Helpers
{
    internal class CheckInput
    {
       static bool CheckFormatEmail(String emailInput)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(emailPattern);
            return regex.IsMatch(emailInput);
        }

        static bool CheckFormatPassword(String passwordInput)
        {
            // Password must have at least 8 characters, 1 uppercase, 1 lowercase, 1 number, 1 special character
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            Regex regex = new Regex(passwordPattern);
            return regex.IsMatch(passwordInput);
        }
    }
}

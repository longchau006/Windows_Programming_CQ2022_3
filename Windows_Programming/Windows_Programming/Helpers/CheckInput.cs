using System.Text.RegularExpressions;

namespace Windows_Programming.Helpers
{
    public class CheckInput
    {
       public static bool CheckFormatEmail(string emailInput)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(emailPattern);
            return regex.IsMatch(emailInput);
        }

        public static bool CheckFormatPassword(string passwordInput)
        {
            // Password must have at least 8 characters, 1 uppercase, 1 lowercase, 1 number, 1 special character
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            Regex regex = new Regex(passwordPattern);
            return regex.IsMatch(passwordInput);
        }
    }
}

using System.ComponentModel;

namespace Windows_Programming.Model
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public void PrintAccountInfo()
        {
            System.Diagnostics.Debug.WriteLine("=== Account Information ===");
            System.Diagnostics.Debug.WriteLine($"ID: {this.Id}");
            System.Diagnostics.Debug.WriteLine($"Username: {this.Username}");
            System.Diagnostics.Debug.WriteLine($"Password: {this.Password}");
            System.Diagnostics.Debug.WriteLine($"Email: {this.Email}");
            System.Diagnostics.Debug.WriteLine($"Fullname: {this.Fullname}");
            System.Diagnostics.Debug.WriteLine($"Address: {this.Address}");
            System.Diagnostics.Debug.WriteLine("========================");
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}

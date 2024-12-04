using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Database;
using Windows_Programming.Model;
using Windows_Programming.View;

namespace Windows_Programming.ViewModel
{
    public class AccountViewModel
    {
        public Account User { get; set; }
        
        private FirebaseServicesDAO firebaseServices;

        public async Task getInformationAsync()
        {
            firebaseServices = FirebaseServicesDAO.Instance;
            // Get account from Firestore
            User = await firebaseServices.GetAccountByID(MainWindow.MyAccount.Id);
        }

        public async Task UpdateFullNameAsync(string fullName)
        {
            firebaseServices = FirebaseServicesDAO.Instance;
            // Update Fullname in Firestore
            await firebaseServices.UpdateFullName(fullName, MainWindow.MyAccount.Id);
        }

        public async Task UpdateAddressAsync(string address)
        {
            firebaseServices = FirebaseServicesDAO.Instance;
            // Update Address in Firestore
            await firebaseServices.UpdateAddress(address, MainWindow.MyAccount.Id);
        }

        public async Task UpdatePasswordAsync(string oldPassword, string newPassword)
        {
            firebaseServices = FirebaseServicesDAO.Instance;
            // Update Password in Firestore
            await firebaseServices.UpdatePassword(oldPassword, newPassword);
        }

        public async Task DeleteUserAsync(string email, string password)
        {
            firebaseServices = FirebaseServicesDAO.Instance;
            // Delete Account in Firestore
            await firebaseServices.DeleteUser(email, password, MainWindow.MyAccount.Id);
        }
    }
}

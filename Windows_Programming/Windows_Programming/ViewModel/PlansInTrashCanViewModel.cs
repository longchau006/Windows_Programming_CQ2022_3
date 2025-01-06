using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows_Programming.Database;
using Windows_Programming.Model;
using Windows_Programming.View;

namespace Windows_Programming.ViewModel
{
    public class PlansInTrashCanViewModel : INotifyPropertyChanged
    {
        private FirebaseServicesDAO firebaseServices = FirebaseServicesDAO.Instance;
        private ObservableCollection<Plan> _plansInTrashCan = new ObservableCollection<Plan>();

        public ObservableCollection<Plan> PlansInTrashCan
        {
            get => _plansInTrashCan;
            set
            {
                if (_plansInTrashCan != value)
                {
                    _plansInTrashCan = value;
                    OnPropertyChanged(nameof(PlansInTrashCan));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private DispatcherTimer cleanupTimer;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public async void Init()
        {


            var plans = await firebaseServices.GetAllPlan(MainWindow.MyAccount.Id);

            PlansInTrashCan.Clear();

            

            foreach (var plan in plans)
            {
                if (plan.DeletedDate != null)
                {
                    PlansInTrashCan.Add(plan);
                }
            }
            OnPropertyChanged(nameof(PlansInTrashCan));

            await DeleteDayAfterOneMonth();

            //set time for delete after 30 days for app run over 24 hours continue
            //day1: delete a, day2: delete b -> day31: delete a immediately, day32: delete b immediately so time check is 24hours
            cleanupTimer = new DispatcherTimer();
            cleanupTimer.Interval = TimeSpan.FromHours(24); // Check every 24 hours
            cleanupTimer.Tick += CleanupExpiredPlans;
            cleanupTimer.Start();

        }
        private async void CleanupExpiredPlans(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine($"Vào hàm auto xóa vào lúc {DateTime.Now.ToString("HH:mm")}");

            await DeleteDayAfterOneMonth();
            
        }
        
        private async Task DeleteDayAfterOneMonth()
        {
            DateTime now = DateTime.Now;

            var expiredPlans = PlansInTrashCan
                .Where(plan => plan.DeletedDate.HasValue &&
                               (now - plan.DeletedDate.Value).TotalDays >= 30)
                .ToList();

            foreach (var plan in expiredPlans)
            {
                System.Diagnostics.Debug.WriteLine($"Id ke hoac xoa la {plan.Id}");
                // Remove from Firebase and local list
                try
                {
                    await firebaseServices.DeleteImediatelyPlanInFirestore(MainWindow.MyAccount.Id, plan);
                    await firebaseServices.DeleteImageFromStorage(MainWindow.MyAccount.Id, plan.Id);
                    RemovePlanInTrashCan(plan);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting plan: {ex.Message}");
                }
            }
        }
        public void AddPlanInTrashCan(Plan plan)
        {
            PlansInTrashCan.Add(plan);
            OnPropertyChanged(nameof(PlansInTrashCan)); // Thông báo rằng PlansInHome đã thay đổi
        }

        public void RemovePlanInTrashCan(Plan plan)
        {
            if (PlansInTrashCan.Contains(plan))
            {
                PlansInTrashCan.Remove(plan);
                OnPropertyChanged(nameof(PlansInTrashCan)); // Thông báo rằng PlansInHome đã thay đổi
            }
        }
    }
}

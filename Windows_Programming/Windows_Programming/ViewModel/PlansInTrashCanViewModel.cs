using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows_Programming.Database;
using Windows_Programming.Model;

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
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public async void Init()
        {

            var plans = await firebaseServices.GetAllPlan(26);

            PlansInTrashCan.Clear();
            foreach (var plan in plans)
            {
                if (plan.DeletedDate != null)
                {
                    PlansInTrashCan.Add(plan);
                }
            }
            OnPropertyChanged(nameof(PlansInTrashCan));
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

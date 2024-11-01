using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows_Programming.Database;
using Windows_Programming.Model;

namespace Windows_Programming.ViewModel
{
    public class PlansInHomeViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Plan> _plansInHome = new ObservableCollection<Plan>();
        public ObservableCollection<Plan> PlansInHome
        {
            get => _plansInHome;
            set
            {
                if (_plansInHome != value)
                {
                    _plansInHome = value;
                    OnPropertyChanged(nameof(PlansInHome));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Init()
        {
            IDao dao = new MockDao();
            var plans = dao.GetAllPlanInHome(); // đây là List<Plan>

            // Thay vì gán trực tiếp, ta dùng AddRange để thêm từng phần tử
            PlansInHome.Clear();
            foreach (var plan in plans)
            {
                PlansInHome.Add(plan);
            }
        }

        public void AddPlanInHome(Plan plan)
        {
            PlansInHome.Add(plan);
            OnPropertyChanged(nameof(PlansInHome)); // Thông báo rằng PlansInHome đã thay đổi
        }

        public void RemovePlanInHome(Plan plan)
        {
            if (PlansInHome.Contains(plan))
            {
                PlansInHome.Remove(plan);
                OnPropertyChanged(nameof(PlansInHome)); // Thông báo rằng PlansInHome đã thay đổi
            }
        }
        public void UpdatePlanInHome(Plan existingPlan, Plan updatedPlan)
        { 
            if (PlansInHome.Contains(existingPlan))
            {
                // Cập nhật thông tin của kế hoạch
                existingPlan.Name = updatedPlan.Name;
                existingPlan.PlanImage = updatedPlan.PlanImage;
                existingPlan.StartDate = updatedPlan.StartDate;
                existingPlan.EndDate = updatedPlan.EndDate;
                existingPlan.Description = updatedPlan.Description;
                existingPlan.StartLocation = updatedPlan.StartLocation;
                existingPlan.EndLocation = updatedPlan.EndLocation;
                existingPlan.Type = updatedPlan.Type;
                existingPlan.DeletedDate = updatedPlan.DeletedDate;
                existingPlan.Activities = updatedPlan.Activities;

                // Gọi OnPropertyChanged nếu cần thiết
                OnPropertyChanged(nameof(PlansInHome)); // Thông báo rằng PlansInHome đã thay đổi
            }
        }
    }
}

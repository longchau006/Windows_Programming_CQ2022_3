﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows_Programming.Database;
using Windows_Programming.Model;
using Windows_Programming.View;

namespace Windows_Programming.ViewModel
{
    public class PlansInHomeViewModel : INotifyPropertyChanged
    {
        private FirebaseServicesDAO firebaseServices = FirebaseServicesDAO.Instance;

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

        public async void Init()
        {

            var plans = await firebaseServices.GetAllPlan(MainWindow.MyAccount.Id);
            

            PlansInHome.Clear();
            foreach (var plan in plans)
            {
                if (plan.DeletedDate == null)
                {
                    System.Diagnostics.Debug.WriteLine("Anh nhe");
                    System.Diagnostics.Debug.WriteLine(plan.PlanImage);
                    PlansInHome.Add(plan);
                }
            }
            OnPropertyChanged(nameof(PlansInHome));
        }

        public void AddPlanInHome(Plan plan)
        {
            System.Diagnostics.Debug.WriteLine("Add Plan in HomeViewModel");
            System.Diagnostics.Debug.WriteLine(plan.PlanImage);
            Plan newPlan = new Plan
            {
                Id = plan.Id,
                Name = plan.Name,
                PlanImage = plan.PlanImage,
                StartLocation = plan.StartLocation,
                EndLocation = plan.EndLocation,
                StartDate = plan.StartDate,
                EndDate = plan.EndDate,
                Description = plan.Description,
                Type = plan.Type,
                Activities = plan.Activities,
            };
            PlansInHome.Add(newPlan);
            foreach( var myplan in PlansInHome)
            {
                System.Diagnostics.Debug.WriteLine(plan.Id);
                System.Diagnostics.Debug.WriteLine(plan.PlanImage);
            }
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
                System.Diagnostics.Debug.WriteLine("Anh o update");
                System.Diagnostics.Debug.WriteLine(existingPlan.PlanImage);
                // Gọi OnPropertyChanged nếu cần thiết
                OnPropertyChanged(nameof(PlansInHome)); // Thông báo rằng PlansInHome đã thay đổi
            }
        }

        public void AddActivitiesForPlan(Plan specificPlan, Model.Activity activity)
        {
            if (PlansInHome.Contains(specificPlan))
            {
                specificPlan.Activities.Add(activity);
                OnPropertyChanged(nameof(PlansInHome));
            }
        }
        public void DeleteActivityForPlan(Plan specificPlan, Model.Activity activity)
        {
            if (PlansInHome.Contains(specificPlan))
            {
                if (specificPlan.Activities.Contains(activity))
                {
                    specificPlan.Activities.Remove(activity);
                    OnPropertyChanged(nameof(specificPlan.Activities));
                }
                OnPropertyChanged(nameof(PlansInHome));
            }
        }
        public void UpdateActivityForPlan<T>(Plan specificPlan, T existingActivity, T updatedActivity) where T : Model.Activity
        {
            if (PlansInHome.Contains(specificPlan))
            {
                if (specificPlan.Activities.Contains(existingActivity))
                {
                    // Copy các thuộc tính chung
                    existingActivity.Name = updatedActivity.Name;
                    existingActivity.Venue = updatedActivity.Venue;
                    existingActivity.Address = updatedActivity.Address;
                    existingActivity.StartDate = updatedActivity.StartDate;
                    existingActivity.EndDate = updatedActivity.EndDate;
                    existingActivity.Description = updatedActivity.Description;

                    
                    if (existingActivity is Transport transport && updatedActivity is Transport updatedTransport)
                    {
                        transport.Vehicle = updatedTransport.Vehicle;
                        transport.StartLocation = updatedTransport.StartLocation;
                        transport.EndLocation = updatedTransport.EndLocation;
                    }
                    else if (existingActivity is Lodging lodging && updatedActivity is Lodging updatedLodging)
                    {
                        lodging.RoomInfo = updatedLodging.RoomInfo;
                    }
                    else if (existingActivity is Extend extend && updatedActivity is Extend updatedExtend)
                    {
                        extend.NameMore = updatedExtend.NameMore;
                    }

                    OnPropertyChanged(nameof(specificPlan.Activities));
                }
                OnPropertyChanged(nameof(PlansInHome));
            }
        }

        public void SortActivitiesByStartDate(Plan specificPlan)
        {
            if (PlansInHome.Contains(specificPlan))
            {
                var sortedActivities = specificPlan.Activities.OrderBy(activity => activity.StartDate).ToList();
                specificPlan.Activities.Clear();
                foreach (var activity in sortedActivities)
                {
                    specificPlan.Activities.Add(activity);
                }

                OnPropertyChanged(nameof(PlansInHome)); // Thông báo rằng PlansInHome đã thay đổi
            }
        }
    }
}

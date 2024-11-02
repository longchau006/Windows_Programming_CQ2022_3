using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlanTripPage : Page
    {
        public Plan PlanTripViewModel { get; set; }
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public PlansInTrashCanViewModel MyPlansInTrashCanViewModel => MainWindow.MyPlansTrashCanViewModel;
        public PlanTripPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PlanTripViewModel = e.Parameter as Plan;

            if (PlanTripViewModel != null)
            {
                this.DataContext = PlanTripViewModel;
            }
        }
        private void OnNavigationDeleteTripClick(object sender, RoutedEventArgs e)
        {
            if (PlanTripViewModel != null)
            {
                // Gọi phương thức xóa kế hoạch từ PlansInHomeViewModel
                PlanTripViewModel.DeletedDate = DateTime.Now;
                MyPlansInTrashCanViewModel.AddPlanInTrashCan(PlanTripViewModel);
                MyPlansHomeViewModel.RemovePlanInHome(PlanTripViewModel);

                // Chuyển hướng về trang Home sau khi xóa 
                Frame.Navigate(typeof(HomePage));
            }
        }
        private void OnNavigationEditTripInfoForTripClick(object sender, RoutedEventArgs e)
        {
            if (PlanTripViewModel != null)
            {
                Frame.Navigate(typeof(EditTripPage), PlanTripViewModel);
            }
        }
        private void OnNavigationAddActivitiesForTripClick(object sender, RoutedEventArgs e)
        {
                Frame.Navigate(typeof(AddActivitiesTripPage), PlanTripViewModel);
        }
    }
}

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
using System.Diagnostics;
using Windows_Programming.Database;
using ClosedXML.Excel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlanTripPage : Page
    {
        private FirebaseServicesDAO firebaseServices;
        public Plan PlanTripViewModel { get; set; }
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public PlansInTrashCanViewModel MyPlansInTrashCanViewModel => MainWindow.MyPlansTrashCanViewModel;

        int accountId = MainWindow.MyAccount.Id;
        public PlanTripPage()
        {
            this.InitializeComponent();
            firebaseServices = FirebaseServicesDAO.Instance;
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
        private async void OnNavigationDeleteTripClick(object sender, RoutedEventArgs e)
        {
            if (PlanTripViewModel != null)
            {
                // Gọi phương thức xóa kế hoạch từ PlansInHomeViewModel
                PlanTripViewModel.DeletedDate = DateTime.Now;
                MyPlansInTrashCanViewModel.AddPlanInTrashCan(PlanTripViewModel);
                MyPlansHomeViewModel.RemovePlanInHome(PlanTripViewModel);

                // Ghi đối tượng lên Firestore
                try
                {
                    await firebaseServices.UpdateWhenDeletePlanInFirestore(accountId, PlanTripViewModel.Id, PlanTripViewModel);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");

                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Failed to delete the trip in home to Firestore.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    _ = errorDialog.ShowAsync();
                    return;
                }

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

        private async void OnNavigationDeleteActivityClick(object sender, RoutedEventArgs e)
        {
            var selectedActivity = (sender as MenuFlyoutItem).CommandParameter as Model.Activity;
            if (selectedActivity != null)
            {
                MyPlansHomeViewModel.DeleteActivityForPlan(PlanTripViewModel, selectedActivity);

                // Xóa đối tượng trên Firestore
                try
                {
                    await firebaseServices.DeleteActivityInFirestore(accountId, PlanTripViewModel.Id, selectedActivity.Id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to delete activity to Firestore: {ex.Message}");

                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Failed to delete activity in home to Firestore.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    _ = errorDialog.ShowAsync();
                    return;
                }

            }
            Frame.Navigate(typeof(PlanTripPage), PlanTripViewModel);
        }
        private void OnNavigationUpdateActivityClick(object sender, RoutedEventArgs e)
        {
            var selectedActivity = (sender as MenuFlyoutItem).CommandParameter as Model.Activity;
            if (selectedActivity != null)
            { 
                var parameters = Tuple.Create(PlanTripViewModel, selectedActivity);
                Frame.Navigate(typeof(EditActivityPage), parameters);
            }
        }

        public void OnNavigationViewExcelButtonClick(object sender, RoutedEventArgs e)
        {
           
            if (PlanTripViewModel == null)
            {
                // Hiển thị thông báo nếu không có kế hoạch nào được chọn
                ContentDialog noPlanDialog = new ContentDialog
                {
                    Title = "No Plan Selected",
                    Content = "Please select a plan to view in Excel.",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                _ = noPlanDialog.ShowAsync();
                return;
            }

            // Tạo workbook Excel
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet("Plan " + (PlanTripViewModel.Name.Length <= 20 ? PlanTripViewModel.Name : PlanTripViewModel.Name.Substring(0, 20)));

                // Ghi dữ liệu kế hoạch
                worksheet.Cell(1, 1).Value = "NAME";
                worksheet.Cell(1, 2).Value = PlanTripViewModel.Name;

                worksheet.Cell(2, 1).Value = "LOCATION";
                worksheet.Cell(2, 2).Value = PlanTripViewModel.StartLocation + " - " + PlanTripViewModel.EndLocation;

                worksheet.Cell(3, 1).Value = "TIME";
                worksheet.Cell(3, 2).Value = PlanTripViewModel.StartDate.ToString("dd/MM/yyyy") + " - " + PlanTripViewModel.EndDate.ToString("dd/MM/yyyy");

                worksheet.Cell(4, 1).Value = "DESCRIPTION";
                worksheet.Cell(4, 2).Value = PlanTripViewModel.Description;

                worksheet.Cell(5, 1).Value = "LIST OF ACTIVITIES";
                worksheet.Cell(6, 1).Value = "NAME";
                worksheet.Cell(6, 2).Value = "TIME";
                worksheet.Cell(6, 3).Value = "ACTIVITY";
                worksheet.Cell(6, 4).Value = "DESCRIPTION";
                worksheet.Cell(6, 5).Value = "TYPE";

                int i = 7;
                if (PlanTripViewModel.Activities != null && PlanTripViewModel.Activities.Any())
                {
                    MyPlansHomeViewModel.SortActivitiesByStartDate(PlanTripViewModel);
                    foreach (var activity in PlanTripViewModel.Activities)
                    {
                        worksheet.Cell(i, 1).Value = activity.Name;
                        worksheet.Cell(i, 2).Value = (activity.StartDate.HasValue ? activity.StartDate.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : "N/A") +
                                                     " - " +
                                                     (activity.EndDate.HasValue ? activity.EndDate.Value.ToString("dd/MM/yyyy hh:mm:ss tt") : "N/A");

                        if (activity is Model.Transport transport)
                        {
                            worksheet.Cell(i, 3).Value = "Travel by " +
                                                         transport.Vehicle +
                                                         " from " +
                                                         transport.StartLocation +
                                                         " to " +
                                                         transport.EndLocation;
                        }
                        else if (activity is Model.Lodging lodging)
                        {
                            worksheet.Cell(i, 3).Value = "Stay " +
                                                         lodging.RoomInfo +
                                                         " - " +
                                                         lodging.Address;
                        }
                        else if (activity is Model.Extend extend)
                        {
                            worksheet.Cell(i, 3).Value = extend.NameMore +
                                                         extend.Venue +
                                                         " - " +
                                                         extend.Address;
                        }
                        else if (activity is Model.Activity)
                        {
                            worksheet.Cell(i, 3).Value = "Discover" +
                                                         activity.Venue +
                                                         " - " +
                                                         activity.Address;
                        }
                        worksheet.Cell(i, 4).Value = activity.Description;
                        worksheet.Cell(i, 5).Value = activity.Type;
                        i++;
                    }
                }
                // Lưu workbook vào file tạm
                string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Plan " + (PlanTripViewModel.Name.Length <= 20 ? PlanTripViewModel.Name : PlanTripViewModel.Name.Substring(0, 20)) + ".xlsx");
                workbook.SaveAs(tempFilePath);

                // Mở file Excel
                OpenFile(tempFilePath);
            }
        }
        private void OpenFile(string filePath)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Could not open the file: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                _ = errorDialog.ShowAsync();
            }
        }
    }
}

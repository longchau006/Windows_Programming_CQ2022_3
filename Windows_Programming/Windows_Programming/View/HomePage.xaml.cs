using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input.ForceFeedback;
using Windows.System;
using Windows_Programming.Database;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Google.Cloud.Firestore.V1;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Microsoft.UI.Xaml.Controls.Page
    {
        private FirebaseServicesDAO firebaseServices;
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public PlansInTrashCanViewModel MyPlansInTrashCanViewModel => MainWindow.MyPlansTrashCanViewModel;

        int accountId = MainWindow.MyAccount.Id;

        bool check1 = false, check2 = false, check3 = false, check4 = false, check5 = false;
        public HomePage()
        {
            this.InitializeComponent();
            firebaseServices = FirebaseServicesDAO.Instance;
            OnNoNoSchedulePanel();

        }
        public async void OnNoNoSchedulePanel()
        {
            Schedule_Panel.Visibility = Visibility.Visible;
            NoSchedule_Panel.Visibility = Visibility.Collapsed;
            var numPlan = await firebaseServices.GetNumAllPlanInHome(accountId);
            if (numPlan == 0)
            {
                NoSchedule_Panel.Visibility = Visibility.Visible;
                Schedule_Panel.Visibility = Visibility.Collapsed;
            }
        }


        private void OnNavigationAddButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddTripPage));
        }

        private void OnNavigationButtonNameTripClick(object sender, RoutedEventArgs e)
        {
            var selectedPlan = (sender as Button).DataContext as Plan;

            // Điều hướng đến PlanTripPage và truyền đối tượng Plan
            if (selectedPlan != null)
            {
                Frame.Navigate(typeof(PlanTripPage), selectedPlan);
            }

        }
        private void OnNavigationEditTripInfoForTripClick(object sender, RoutedEventArgs e)
        {
            var selectedPlan = (sender as Button).DataContext as Plan;
            if (selectedPlan != null)
            {
                Frame.Navigate(typeof(EditTripPage), selectedPlan);
            }
        }
        private async void OnNavigationDeleteTripClick(object sender, RoutedEventArgs e)
        {
            var selectedPlan = (sender as MenuFlyoutItem).CommandParameter as Plan;
            if (selectedPlan != null)
            {
                // Gọi phương thức xóa kế hoạch từ PlansInHomeViewModel
                selectedPlan.DeletedDate = DateTime.Now;


                // Ghi đối tượng lên Firestore
                try
                {
                    await firebaseServices.UpdateWhenDeletePlanInFirestore(accountId, selectedPlan.Id, selectedPlan);

                    MyPlansInTrashCanViewModel.AddPlanInTrashCan(selectedPlan);
                    MyPlansHomeViewModel.RemovePlanInHome(selectedPlan);
                }
                catch (Exception ex)
                {
                    selectedPlan.DeletedDate = null;

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
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = Search_TextBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Show all plans when search box is empty
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    plan.IsVisible = true;
                }
                return;
            }

            foreach (var plan in MyPlansHomeViewModel.PlansInHome)
            {
                // Search in name, start location, and end location
                bool matchesSearch = plan.Name.ToLower().Contains(searchText) ||
                                   plan.StartLocation.ToLower().Contains(searchText) ||
                                   plan.EndLocation.ToLower().Contains(searchText);

                plan.IsVisible = matchesSearch;
            }
        }

        private void OnNavigationCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var selectedCheckBox = sender as CheckBox;

            if (selectedCheckBox == Traveller_CheckBox)
            {
                NonTraveller_CheckBox.IsChecked = false;
                check1 = true;
                check2 = false;
            }
            else if (selectedCheckBox == NonTraveller_CheckBox)
            {
                Traveller_CheckBox.IsChecked = false;
                check2 = true;
                check1 = false;
            }
            else if (selectedCheckBox == UpcomingTrips_CheckBox)
            {
                PastTrips_CheckBox.IsChecked = false;
                CurrentTrips_CheckBox.IsChecked = false;
                check3 = true;
                check4 = false;
                check5 = false;
               
            }
            else if (selectedCheckBox == PastTrips_CheckBox)
            {
                UpcomingTrips_CheckBox.IsChecked = false;
                CurrentTrips_CheckBox.IsChecked = false;
                check4 = true;
                check3 = false;
                check5 = false;
            }
            else if (selectedCheckBox == CurrentTrips_CheckBox)
            {
                UpcomingTrips_CheckBox.IsChecked = false;
                PastTrips_CheckBox.IsChecked = false;
                check5 = true;
                check3 = false;
                check4 = false;
            }

            CheckFilter();
            
        }
        private void OnNavigationCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            var uncheckedCheckBox = sender as CheckBox;

            if (uncheckedCheckBox == Traveller_CheckBox)
            {
                check1 = false;
            }
            else if (uncheckedCheckBox == NonTraveller_CheckBox)
            {
                check2 = false;
            }
            else if (uncheckedCheckBox == UpcomingTrips_CheckBox)
            {
                check3 = false;
            }
            else if (uncheckedCheckBox == PastTrips_CheckBox)
            {
                check4 = false;
            }
            else if (uncheckedCheckBox == CurrentTrips_CheckBox)
            {
                check5 = false;
            }

            CheckFilter();  
        }

        public void CheckFilter()
        {
            DateTime nowTime = DateTime.Now;

            if (!check1 && !check2 && !check3 && !check4 && !check5)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    plan.IsVisible = true;
                }
                return;
            }
            else if (check1 && check3)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.Type && plan.StartDate > nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check1 && check4)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.Type && plan.EndDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check1 && check5)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.Type && plan.EndDate > nowTime && plan.StartDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check2 && check3)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (!plan.Type && plan.StartDate > nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check2 && check4)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (!plan.Type && plan.EndDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check2 && check5)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (!plan.Type && plan.EndDate > nowTime && plan.StartDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check1)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    plan.IsVisible = plan.Type;
                }
                return;
            }
            else if (check2)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    plan.IsVisible = !plan.Type;
                }
                return;
            }
            else if (check3)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.StartDate > nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check4)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.EndDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
            else if (check5)
            {
                foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                {
                    if (plan.EndDate > nowTime && plan.StartDate < nowTime)
                    {
                        plan.IsVisible = true;
                    }
                    else
                    {
                        plan.IsVisible = false;
                    }
                }
                return;
            }
        }

        public void OnNavigationViewExcelButtonClick(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin kế hoạch cần hiển thị
            var plan = (sender as Button).DataContext as Plan; // Hàm này trả về kế hoạch đang được chọn
            MyPlansHomeViewModel.SortActivitiesByStartDate(plan);
            if (plan == null)
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
                var worksheet = workbook.AddWorksheet("Plan " + plan.Name);

                // Ghi dữ liệu kế hoạch
                worksheet.Cell(1, 1).Value = "NAME";
                worksheet.Cell(1, 2).Value = plan.Name;

                worksheet.Cell(2, 1).Value = "LOCATION";
                worksheet.Cell(2, 2).Value = plan.StartLocation + " - " + plan.EndLocation;

                worksheet.Cell(3, 1).Value = "TIME";
                worksheet.Cell(3, 2).Value = plan.StartDate.ToString("dd/MM/yyyy") + " - " + plan.EndDate.ToString("dd/MM/yyyy");

                worksheet.Cell(4, 1).Value = "DESCRIPTION";
                worksheet.Cell(4, 2).Value = plan.Description;

                worksheet.Cell(5, 1).Value = "LIST OF ACTIVITIES";
                worksheet.Cell(6, 1).Value = "NAME";
                worksheet.Cell(6, 2).Value = "TIME";
                worksheet.Cell(6, 3).Value = "ACTIVITY";
                worksheet.Cell(6, 4).Value = "DESCRIPTION";
                worksheet.Cell(6, 5).Value = "TYPE";
                int i = 7;

                foreach (var activity in plan.Activities)
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
                    // Lưu workbook vào file tạm
                    string tempFilePath = Path.Combine(Path.GetTempPath(), "Plan " + plan.Name+ ".xlsx");
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

        private async void OnNavigationUploadButtonClick(object sender, RoutedEventArgs e)
        {
            // 1. Chọn file Excel
            // Tạo FileOpenPicker để chọn tệp
            var openPicker = new FileOpenPicker();
            Window tempWindow = new();
            var hWnd = WindowNative.GetWindowHandle(tempWindow);
            InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add(".xlsx");
            openPicker.FileTypeFilter.Add(".xls");

            StorageFile file = await openPicker.PickSingleFileAsync();
            tempWindow.Close();

            /*if (file != null)
            {
                using (var stream = await file.OpenStreamForReadAsync())
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); // Sheet đầu tiên
                    try
                    {
                        // Giả sử thông tin kế hoạch nằm trên các ô cụ thể
                        var planName = worksheet.Cell(1, 2).GetValue<string>();
                        var startDateStr = worksheet.Cell(2, 2).GetValue<string>();
                        var endDateStr = worksheet.Cell(3, 2).GetValue<string>();
                        var description = worksheet.Cell(4, 2).GetValue<string>();
                        var startLocation = worksheet.Cell(5, 2).GetValue<string>();
                        var endLocation = worksheet.Cell(6, 2).GetValue<string>();

                        // Chuyển đổi ngày giờ nếu cần
                        DateTime? startDate = DateTime.TryParse(startDateStr, out var parsedStartDate) ? parsedStartDate : null;
                        DateTime? endDate = DateTime.TryParse(endDateStr, out var parsedEndDate) ? parsedEndDate : null;

                        // 3. Tạo đối tượng Plan
                        var newPlan = new Plan
                        {
                            Name = planName,
                            StartDate = startDate,
                            EndDate = endDate,
                            Description = description,
                            StartLocation = startLocation,
                            EndLocation = endLocation
                        };

                        // 4. Thêm vào danh sách kế hoạch
                        MyPlansHomeViewModel.PlansInHome.Add(newPlan);

                        // Thông báo thành công
                        ContentDialog dialog = new ContentDialog
                        {
                            Title = "Upload Successful",
                            Content = "Plan has been successfully added!",
                            CloseButtonText = "OK"
                        };
                        await dialog.ShowAsync();
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = "Failed to read file: " + ex.Message,
                            CloseButtonText = "OK"
                        };
                        await errorDialog.ShowAsync();
                    }
                }
            }
            else
            {
                // Người dùng không chọn tệp nào
                var dialog = new ContentDialog
                {
                    Title = "No File Selected",
                    Content = "No file was selected. Please choose an image file.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }*/

        }

    }
}

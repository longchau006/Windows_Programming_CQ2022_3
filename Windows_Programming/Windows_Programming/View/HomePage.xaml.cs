using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows_Programming.Database;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;
using WinRT.Interop;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Google.Cloud.Firestore.V1;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using System.Globalization;
using Windows.Devices.Sensors;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = iText.Layout.Document;
using Paragraph = iText.Layout.Element.Paragraph;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Microsoft.UI.Xaml.Controls.Page
    {
        private ContentDialog loadingDialog;
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

        private async Task ExportPlanToPdfAsync(Plan plan, string filePath)
        {
            await Task.Run(() => ExportPlanToPdf(plan, filePath));
        }

        private async void ExportPDF_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            Window w = new();
            var hWnd = WindowNative.GetWindowHandle(w);
            InitializeWithWindow.Initialize(savePicker, hWnd);
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("PDF", new List<string>() { ".pdf" });
            savePicker.SuggestedFileName = "TravelPlan";
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            w.Close();
            if (file != null)
            {
                var selectedPlan = (sender as Button).DataContext as Plan;
                CreateLoadingDialog();
                var loadingTask = loadingDialog.ShowAsync();
                await ExportPlanToPdfAsync(selectedPlan, file.Path);
                loadingDialog.Hide();
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Exported to PDF successfully",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
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
            var selectedCheckBox = sender as Microsoft.UI.Xaml.Controls.CheckBox;

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
            var uncheckedCheckBox = sender as Microsoft.UI.Xaml.Controls.CheckBox;

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
                var worksheet = workbook.AddWorksheet("Plan " + (plan.Name.Length <= 20 ? plan.Name : plan.Name.Substring(0, 20)));

                // Ghi dữ liệu kế hoạch
                worksheet.Cell(1, 1).Value = "NAME";
                worksheet.Cell(1, 2).Value = plan.Name;

                worksheet.Cell(2, 1).Value = "LOCATION";
                worksheet.Cell(2, 2).Value = plan.StartLocation + " - " + plan.EndLocation;

                worksheet.Cell(3, 1).Value = "TIME";
                worksheet.Cell(3, 2).Value = plan.StartDate.ToString("dd/MM/yyyy hh:mm:ss tt") + " - " + plan.EndDate.ToString("dd/MM/yyyy hh:mm:ss tt");

                worksheet.Cell(4, 1).Value = "DESCRIPTION";
                worksheet.Cell(4, 2).Value = plan.Description;

                worksheet.Cell(5, 1).Value = "LIST OF ACTIVITIES";
                worksheet.Cell(6, 1).Value = "NAME";
                worksheet.Cell(6, 2).Value = "TIME";
                worksheet.Cell(6, 3).Value = "ACTIVITY";
                worksheet.Cell(6, 4).Value = "DESCRIPTION";
                int i = 7;
                if (plan.Activities != null && plan.Activities.Any())
                {
                    MyPlansHomeViewModel.SortActivitiesByStartDate(plan);
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
                            worksheet.Cell(i, 3).Value = "Discover " +
                                                         activity.Venue +
                                                         " - " +
                                                         activity.Address;
                        }
                        worksheet.Cell(i, 4).Value = activity.Description;
                        i++;
                    }
                }
                // Lưu workbook vào file tạm
                string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Plan " + (plan.Name.Length <= 20 ? plan.Name : plan.Name.Substring(0, 20)) + ".xlsx");
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
            int newId = MyPlansHomeViewModel.PlansInHome.Any() ? (MyPlansHomeViewModel.PlansInHome.Max(plan => plan.Id)) + 1 : 0;
            if (MyPlansInTrashCanViewModel.PlansInTrashCan.Count > 0 && (MyPlansInTrashCanViewModel.PlansInTrashCan.Max(plan => plan.Id) + 1) > newId)
            {
                newId = MyPlansInTrashCanViewModel.PlansInTrashCan.Max(plan => plan.Id) + 1;
            }

            string selectedImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "danang.jpg");

            var openPicker = new FileOpenPicker();
            Window tempWindow = new();
            var hWnd = WindowNative.GetWindowHandle(tempWindow);
            InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add(".xlsx");
            openPicker.FileTypeFilter.Add(".xls");

            StorageFile file = await openPicker.PickSingleFileAsync();
            tempWindow.Close();

            if (file != null)
            {
                // Kiểm tra định dạng tệp
                string fileExtension = Path.GetExtension(file.Path).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    // Hiển thị thông báo lỗi nếu định dạng không hợp lệ
                    var dialog = new ContentDialog
                    {
                        Title = "Invalid File Format",
                        Content = "Please select an image file with a valid format (XLSX or XLS).",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await dialog.ShowAsync();
                    return;
                }
                using (var stream = await file.OpenStreamForReadAsync())
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1); 
                    try
                    {
                        string planName = worksheet.Cell(1, 2).GetValue<string>();
                        string locationStr = worksheet.Cell(2, 2).GetValue<string>();
                        string timeStr = worksheet.Cell(3, 2).GetValue<string>();
                        string description = worksheet.Cell(4, 2).GetValue<string>();

                        // Danh sách chứa các thông báo lỗi
                        List<string> errorMessages = new List<string>();
                        // Kiểm tra các trường có bị trống hay không
                        if (string.IsNullOrWhiteSpace(planName) ||
                            string.IsNullOrWhiteSpace(locationStr) ||
                            string.IsNullOrWhiteSpace(timeStr)||
                            !locationStr.Contains("-")||
                            !timeStr.Contains("-"))
                                errorMessages.Add("File error form");
                        // Nếu có lỗi, hiển thị thông báo
                        if (errorMessages.Count > 0)
                        {
                            ContentDialog dialog = new ContentDialog
                            {
                                Title = "Incomplete or Invalid Information",
                                Content = string.Join("\n", errorMessages),
                                CloseButtonText = "OK",
                                XamlRoot = this.XamlRoot
                            };
                            _ = dialog.ShowAsync();
                            return;
                        }
                        var locations = locationStr.Split(" - ");
                        string startLocation = locations[0];
                        string endLocation = locations[1];

                        var dates = timeStr.Split(" - ");
                        DateTime? startDate = null;
                        DateTime? endDate = null;

                        string dateTimeFormat = "dd/MM/yyyy hh:mm:ss tt";
                        CultureInfo culture = CultureInfo.InvariantCulture;

                        if (dates.Length == 2)
                        {
                            startDate = DateTime.TryParseExact(dates[0], dateTimeFormat, culture, DateTimeStyles.None, out DateTime parsedStartDate)
                                        ? parsedStartDate : null;

                            endDate = DateTime.TryParseExact(dates[1], dateTimeFormat, culture, DateTimeStyles.None, out DateTime parsedEndDate)
                                      ? parsedEndDate : null;
                        }
                        // Kiểm tra các trường có bị trống hay không
                        if (string.IsNullOrWhiteSpace(startLocation) ||
                            string.IsNullOrWhiteSpace(endLocation) ||
                            startDate == null ||
                            endDate == null ||
                            (startDate != null && endDate != null && startDate >= endDate))
                            errorMessages.Add("File error form");
                        // Nếu có lỗi, hiển thị thông báo
                        if (errorMessages.Count > 0)
                        {
                            ContentDialog dialog = new ContentDialog
                            {
                                Title = "Incomplete or Invalid Information",
                                Content = string.Join("\n", errorMessages),
                                CloseButtonText = "OK",
                                XamlRoot = this.XamlRoot
                            };
                            _ = dialog.ShowAsync();
                            return;
                        }
                        ContentDialog loadingDialog = new ContentDialog
                        {
                            Title = "Create New Plan From Excel...",
                            Content = new ProgressRing { IsActive = true },
                            XamlRoot = this.XamlRoot
                        };
                        loadingDialog.ShowAsync();

                        // Ghi đối tượng plan lên Firestore
                        try
                        {
                            Plan newPlan = new Plan
                            {
                                Id = newId,
                                Name = planName,
                                PlanImage = selectedImagePath,
                                StartLocation = startLocation,
                                EndLocation = endLocation,
                                StartDate = startDate.Value,
                                EndDate = endDate.Value,
                                Description = description,
                                Type = true
                            };
                            // Phải thêm vào ni trc chứ k nó lỗi khi xóa hết rồi thêm lại cái mới nó sẽ dính ảnh của ảnh cũ
                            MyPlansHomeViewModel.AddPlanInHome(newPlan);


                            System.Diagnostics.Debug.WriteLine($"aaaaaaaaaaaaaaa Image Path: {newPlan.PlanImage}");

                            string imageUrl = null;

                            if (!string.IsNullOrEmpty(selectedImagePath))
                            {

                                imageUrl = await firebaseServices.UploadImageToStorage(
                                    selectedImagePath,
                                    accountId,
                                    newId
                                );
                            }
                            System.Diagnostics.Debug.WriteLine($"Image luc add vao va imageUrl {imageUrl}");
                            newPlan.PlanImage = imageUrl;
                            System.Diagnostics.Debug.WriteLine($"bbbbbbbbbbb Image Path: {MyPlansHomeViewModel.PlansInHome[0].PlanImage}");
                            await firebaseServices.CreatePlanInFirestore(accountId, newPlan);

                            foreach (var plan in MyPlansHomeViewModel.PlansInHome)
                            {
                                if (plan.Id == newId)
                                {
                                    if (plan.Activities == null)
                                    {
                                        plan.Activities = new List<Windows_Programming.Model.Activity>();
                                    }
                                    int i = 7;
                                    while (!string.IsNullOrWhiteSpace(worksheet.Cell(i, 1).GetValue<string>()))
                                    {
                                        Windows_Programming.Model.Activity newActivity = null;

                                        string nameA = worksheet.Cell(i, 1).GetValue<string>();
                                        string timeAStr = worksheet.Cell(i, 2).GetValue<string>();
                                        string activityStr = worksheet.Cell(i, 3).GetValue<string>();
                                        string descriptionA = worksheet.Cell(i, 4).GetValue<string>();

                                        // Kiểm tra các trường có bị trống hay không
                                        if (string.IsNullOrWhiteSpace(nameA) ||
                                            string.IsNullOrWhiteSpace(timeAStr) ||
                                            string.IsNullOrWhiteSpace(activityStr) ||
                                            (!activityStr.Contains("Discover") && 
                                                !activityStr.Contains("Travel") && 
                                                !activityStr.Contains("Stay") && 
                                                !activityStr.Contains("-")) ||
                                            !timeStr.Contains("-"))
                                                errorMessages.Add("File error form");
                                        // Nếu có lỗi, hiển thị thông báo
                                        if (errorMessages.Count > 0)
                                        {
                                            loadingDialog.Hide();
                                            ContentDialog dialog = new ContentDialog
                                            {
                                                Title = "Incomplete or Invalid Information",
                                                Content = string.Join("\n", errorMessages),
                                                CloseButtonText = "OK",
                                                XamlRoot = this.XamlRoot
                                            };
                                            _ = dialog.ShowAsync();
                                            return;
                                        }
                                        var dateAs = timeAStr.Split(" - ");
                                        DateTime? startDateA = null;
                                        DateTime? endDateA = null;

                                        if (dates.Length == 2)
                                        {
                                            startDateA = DateTime.TryParseExact(dateAs[0], dateTimeFormat, culture, DateTimeStyles.None, out DateTime parsedAStartDate)
                                                        ? parsedAStartDate : null;

                                            endDateA = DateTime.TryParseExact(dateAs[1], dateTimeFormat, culture, DateTimeStyles.None, out DateTime parsedAEndDate)
                                                      ? parsedAEndDate : null;
                                        }
                                        // Kiểm tra các trường có bị trống hay không
                                        if (startDateA == null ||
                                            endDateA == null ||
                                            plan.StartDate > startDateA ||
                                            startDateA > endDateA || 
                                            endDateA > plan.EndDate)
                                                errorMessages.Add("File error form");
                                        // Nếu có lỗi, hiển thị thông báo
                                        if (errorMessages.Count > 0)
                                        {
                                            loadingDialog.Hide();
                                            ContentDialog dialog = new ContentDialog
                                            {
                                                Title = "Incomplete or Invalid Information",
                                                Content = string.Join("\n", errorMessages),
                                                CloseButtonText = "OK",
                                                XamlRoot = this.XamlRoot
                                            };
                                            _ = dialog.ShowAsync();

                                            return;
                                        }

                                        if (activityStr.Contains("Discover"))
                                        {
                                            int lastDashIndex = activityStr.LastIndexOf('-');
                                            string address = activityStr.Substring(lastDashIndex + 1).Trim();
                                            string venus = activityStr.Substring("Discover".Length, lastDashIndex - "Discover".Length).Trim();

                                            // Kiểm tra các trường có bị trống hay không
                                            if (string.IsNullOrWhiteSpace(address) ||
                                                string.IsNullOrWhiteSpace(venus) )
                                                    errorMessages.Add("File error form");
                                            // Nếu có lỗi, hiển thị thông báo
                                            if (errorMessages.Count > 0)
                                            {
                                                loadingDialog.Hide();
                                                ContentDialog dialog = new ContentDialog
                                                {
                                                    Title = "Incomplete or Invalid Information",
                                                    Content = string.Join("\n", errorMessages),
                                                    CloseButtonText = "OK",
                                                    XamlRoot = this.XamlRoot
                                                };
                                                _ = dialog.ShowAsync();
                                                return;
                                            }

                                            newActivity = new Windows_Programming.Model.Activity
                                            {
                                                Id = plan.Activities.Any() ? (plan.Activities.Max(activity => activity.Id) + 1) : 0,
                                                Type = 1,
                                                Name = nameA,
                                                Venue = venus,
                                                Address = address,
                                                StartDate = startDateA,
                                                EndDate = endDateA,
                                                Description = descriptionA
                                            };
                                        }
                                        else if (activityStr.Contains("Travel"))
                                        {
                                            int byIndex = activityStr.IndexOf("by") + "by".Length;
                                            int fromIndex = activityStr.IndexOf("from");
                                            int toIndex = activityStr.IndexOf("to");

                                            string vehicle = activityStr.Substring(byIndex, fromIndex - byIndex).Trim();
                                            string startLocationA = activityStr.Substring(fromIndex + "from".Length, toIndex - (fromIndex + "from".Length)).Trim();
                                            string endLocationA = activityStr.Substring(toIndex + "to".Length).Trim();

                                            // Kiểm tra các trường có bị trống hay không
                                            if (string.IsNullOrWhiteSpace(vehicle) ||
                                                string.IsNullOrWhiteSpace(startLocationA) ||
                                                string.IsNullOrWhiteSpace(endLocationA))
                                                    errorMessages.Add("File error form");
                                            // Nếu có lỗi, hiển thị thông báo
                                            if (errorMessages.Count > 0)
                                            {
                                                loadingDialog.Hide();
                                                ContentDialog dialog = new ContentDialog
                                                {
                                                    Title = "Incomplete or Invalid Information",
                                                    Content = string.Join("\n", errorMessages),
                                                    CloseButtonText = "OK",
                                                    XamlRoot = this.XamlRoot
                                                };
                                                _ = dialog.ShowAsync();
                                                return;
                                            }

                                            newActivity = new Windows_Programming.Model.Transport
                                            {
                                                Id = plan.Activities.Any() ? (plan.Activities.Max(activity => activity.Id) + 1) : 0,
                                                Type = 2,
                                                Name = nameA,
                                                Vehicle = vehicle,
                                                StartLocation = startLocationA,
                                                EndLocation = endLocationA,
                                                StartDate = startDateA,
                                                EndDate = endDateA,
                                                Description = descriptionA
                                            };
                                        }
                                        else if (activityStr.Contains("Stay"))
                                        {
                                            int lastDashIndex = activityStr.LastIndexOf('-');
                                            string address = activityStr.Substring(lastDashIndex + 1).Trim();
                                            string room = activityStr.Substring("Stay".Length, lastDashIndex - "Stay".Length).Trim();

                                            // Kiểm tra các trường có bị trống hay không
                                            if (string.IsNullOrWhiteSpace(address) ||
                                                string.IsNullOrWhiteSpace(room))
                                                errorMessages.Add("File error form");
                                            // Nếu có lỗi, hiển thị thông báo
                                            if (errorMessages.Count > 0)
                                            {
                                                loadingDialog.Hide();
                                                ContentDialog dialog = new ContentDialog
                                                {
                                                    Title = "Incomplete or Invalid Information",
                                                    Content = string.Join("\n", errorMessages),
                                                    CloseButtonText = "OK",
                                                    XamlRoot = this.XamlRoot
                                                };
                                                _ = dialog.ShowAsync();
                                                return;
                                            }

                                            newActivity = new Windows_Programming.Model.Lodging
                                            {
                                                Id = plan.Activities.Any() ? (plan.Activities.Max(activity => activity.Id) + 1) : 0,
                                                Type = 3,
                                                Name = nameA,
                                                RoomInfo = room,
                                                Address = address,
                                                StartDate = startDateA,
                                                EndDate = endDateA,
                                                Description = descriptionA
                                            };
                                        }
                                        else 
                                        {
                                            int firstSpaceIndex = activityStr.IndexOf(' ');
                                            int lastDashIndex = activityStr.LastIndexOf('-');

                                            string act = activityStr.Substring(0, firstSpaceIndex).Trim();
                                            string address = activityStr.Substring(lastDashIndex + 1).Trim();
                                            string venus = activityStr.Substring(firstSpaceIndex + 1, lastDashIndex - firstSpaceIndex - 1).Trim();

                                            // Kiểm tra các trường có bị trống hay không
                                            if (string.IsNullOrWhiteSpace(act) ||
                                                string.IsNullOrWhiteSpace(address) ||
                                                string.IsNullOrWhiteSpace(venus))

                                                errorMessages.Add("File error form");
                                            // Nếu có lỗi, hiển thị thông báo
                                            if (errorMessages.Count > 0)
                                            {
                                                loadingDialog.Hide();
                                                ContentDialog dialog = new ContentDialog
                                                {
                                                    Title = "Incomplete or Invalid Information",
                                                    Content = string.Join("\n", errorMessages),
                                                    CloseButtonText = "OK",
                                                    XamlRoot = this.XamlRoot
                                                };
                                                _ = dialog.ShowAsync();
                                                return;
                                            }

                                            newActivity = new Windows_Programming.Model.Extend
                                            {
                                                Id = plan.Activities.Any() ? (plan.Activities.Max(activity => activity.Id) + 1) : 0,
                                                Type = 4,
                                                NameMore = act,
                                                Name = nameA,
                                                Venue = venus,
                                                Address = address,
                                                StartDate = startDateA,
                                                EndDate = endDateA,
                                                Description = descriptionA
                                            };
                                        }

                                        try
                                        {
                                            await firebaseServices.CreateActivityInFirestore(accountId, plan.Id, newActivity);

                                            MyPlansHomeViewModel.AddActivitiesForPlan(plan, newActivity);
                                        }
                                        catch (Exception ex)
                                        {
                                            loadingDialog.Hide();
                                            Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");

                                            ContentDialog errorDialog = new ContentDialog
                                            {
                                                Title = "Error",
                                                Content = "Failed to save the activity to Firestore.",
                                                CloseButtonText = "OK",
                                                XamlRoot = this.XamlRoot
                                            };
                                            _ = errorDialog.ShowAsync();
                                            return;
                                        }
                                        i++;
                                    }
                                    break;    
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");
                            loadingDialog.Hide();
                            ContentDialog errorDialog = new ContentDialog
                            {
                                Title = "Error",
                                Content = "Failed to save the trip to Firestore.",
                                CloseButtonText = "OK",
                                XamlRoot = this.XamlRoot
                            };
                            _ = errorDialog.ShowAsync();
                            return;
                        }
                        loadingDialog.Hide();
                        // Hiển thị thông báo thành công
                        ContentDialog successDialog = new ContentDialog
                        {
                            Title = "Trip Added Successfully",
                            Content = "Your trip has been saved.",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        _ = successDialog.ShowAsync();

                        Frame.Navigate(typeof(HomePage));

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
                var dialog = new ContentDialog
                {
                    Title = "No File Selected",
                    Content = "No file was selected. Please choose an image file.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }

        }

        public static void ExportPlanToPdf(Plan plan, string filePath)
        {
            string fontPath = @"C:\Windows\Fonts\times.ttf"; // Đường dẫn tới phông chữ Arial

            if (!File.Exists(fontPath))
            {
                throw new FileNotFoundException("Font không tồn tại. Kiểm tra đường dẫn tới phông chữ Arial hoặc sử dụng một phông chữ khác.");
            }

            PdfFont font = PdfFontFactory.CreateFont(fontPath, iText.IO.Font.PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

            using (PdfWriter writer = new PdfWriter(filePath))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);

                    // Sử dụng phông chữ tùy chỉnh
                    document.SetFont(font);

                    // Tiêu đề kế hoạch
                    document.Add(new Paragraph(plan.Name)
                        .SetFontSize(20)
                        .SetFont(font)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetMarginBottom(20));

                    // image
                    if (plan.PlanImage != null)
                    {
                        try
                        {
                            iText.Layout.Element.Image image = new iText.Layout.Element.Image(iText.IO.Image.ImageDataFactory.Create(plan.PlanImage))
                                                                        .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER)
                                                                        .SetMaxWidth(400);
                            document.Add(image.SetMarginBottom(20));
                        }
                        catch (Exception)
                        {

                        }
                    }

                    // Thông tin tổng quan
                    document.Add(new Paragraph($"Địa điểm đi: {plan.StartLocation}")
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Địa điểm đến: {plan.EndLocation}")
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Ngày bắt đầu: {plan.StartDate:yyyy-MM-dd}")
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Ngày kết thúc: {plan.EndDate:yyyy-MM-dd}")
                        .SetFontSize(12));
                    document.Add(new Paragraph($"Mô tả: {plan.Description}")
                        .SetFontSize(12)
                        .SetMarginBottom(20));

                    // Các hoạt động
                    foreach (var activity in plan.Activities)
                    {
                        string activityType = activity switch
                        {
                            Transport => "Phương tiện di chuyển",
                            Lodging => "Chỗ ở",
                            Model.Extend => "Hoạt động khác",
                            _ => "Địa điểm vui chơi"
                        };

                        document.Add(new Paragraph(activityType)
                            .SetFontSize(16)
                            .SetFont(font)
                            .SetUnderline()
                            .SetMarginTop(10));

                        document.Add(new Paragraph($"- Tên sự kiện: {activity.Name}"));
                        document.Add(new Paragraph($"- Địa điểm: {activity.Venue}"));
                        document.Add(new Paragraph($"- Địa chỉ: {activity.Address}"));
                        document.Add(new Paragraph($"- Bắt đầu: {activity.StartDate:yyyy-MM-dd HH:mm}"));
                        document.Add(new Paragraph($"- Kết thúc: {activity.EndDate:yyyy-MM-dd HH:mm}"));
                        document.Add(new Paragraph($"- Mô tả: {activity.Description}")
                            .SetMarginBottom(10));
                    }

                    document.Close();
                }


            }
        }

        private void CreateLoadingDialog()
        {
            StackPanel dialogContent = new StackPanel
            {
                Spacing = 10,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center
            };

            ProgressRing progressRing = new ProgressRing
            {
                IsActive = true,
                Width = 50,
                Height = 50
            };

            TextBlock messageText = new TextBlock
            {
                Text = "Please wait...",
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center
            };

            dialogContent.Children.Add(progressRing);
            dialogContent.Children.Add(messageText);

            loadingDialog = new ContentDialog
            {
                Content = dialogContent,
                IsPrimaryButtonEnabled = false,
                IsSecondaryButtonEnabled = false,
                XamlRoot = this.XamlRoot  // Set the XamlRoot from the current page
            };
        }

    }
}

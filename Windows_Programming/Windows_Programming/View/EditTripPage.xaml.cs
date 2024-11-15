using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditTripPage : Page
    {
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public Plan PlanTripViewModel { get; set; }

        private string selectedImagePath;
        public EditTripPage()
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
                selectedImagePath = PlanTripViewModel.PlanImage;
            }

        }
        private async void OnNavigationChangePhotoButtonClick(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();
            Window w = new();
            var hWnd = WindowNative.GetWindowHandle(w);
            InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");
            StorageFile file = await openPicker.PickSingleFileAsync();
            w.Close();

            if (file != null)
            {
                selectedImagePath = file.Path;
                System.Diagnostics.Debug.WriteLine($"new: {selectedImagePath}");
                // Sử dụng BitmapImage để tạo nguồn cho Image
                var bitmapImage = new BitmapImage();
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await bitmapImage.SetSourceAsync(stream);
                }
                Trip_Image.Source = bitmapImage;
            }
        }
        private void OnNavigationCancelButtonClick(object sender, RoutedEventArgs e)
        {
            TripName_TextBox.Text = PlanTripViewModel.Name.ToString();
            StartLocation_TextBox.Text = PlanTripViewModel.StartLocation.ToString();
            EndLocation_TextBox.Text = PlanTripViewModel.EndLocation.ToString();
            Start_DatePicker.Date = new DateTimeOffset(PlanTripViewModel.StartDate);
            End_DatePicker.Date = new DateTimeOffset(PlanTripViewModel.EndDate);
            selectedImagePath = PlanTripViewModel.PlanImage;
            if (!string.IsNullOrEmpty(PlanTripViewModel.PlanImage))
            {
                try
                {
                    var image = new BitmapImage(new Uri(PlanTripViewModel.PlanImage, UriKind.Absolute));
                    Trip_Image.Source = image;
                }
                catch (Exception ex)
                {
                    Trip_Image.Source = null;
                }
            }
            System.Diagnostics.Debug.WriteLine($"new: {selectedImagePath}");
        }
        private void OnNavigationSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin từ các trường
            string tripName = TripName_TextBox.Text;
            string startLocation = StartLocation_TextBox.Text;
            string endLocation = EndLocation_TextBox.Text;
            DateTime? startDate = Start_DatePicker.SelectedDate?.DateTime;
            DateTime? endDate = End_DatePicker.SelectedDate?.DateTime;

            // Danh sách chứa các thông báo lỗi
            List<string> errorMessages = new List<string>();

            // Kiểm tra có thay đổi không
            if ((tripName == PlanTripViewModel.Name) &&
                (startLocation == PlanTripViewModel.StartLocation) &&
                (endLocation == PlanTripViewModel.EndLocation) &&
                (startDate.HasValue && startDate.Value == PlanTripViewModel.StartDate) &&
                (endDate.HasValue && endDate.Value == PlanTripViewModel.EndDate) &&
                (selectedImagePath == PlanTripViewModel.PlanImage))
            {
                errorMessages.Add("No edits");
            }

            // Kiểm tra các trường có bị trống hay không
            if (string.IsNullOrWhiteSpace(tripName))
                errorMessages.Add("Trip Name is required.");
            if (string.IsNullOrWhiteSpace(startLocation))
                errorMessages.Add("Start Location is required.");
            if (string.IsNullOrWhiteSpace(endLocation))
                errorMessages.Add("End Location is required.");
            if (startDate == null)
                errorMessages.Add("Start Date is required.");
            if (endDate == null)
                errorMessages.Add("End Date is required.");

            // Kiểm tra nếu Start Date và End Date đều có giá trị, đảm bảo Start Date phải trước End Date
            if (startDate != null && endDate != null && startDate >= endDate)
                errorMessages.Add("Invalid Date: Start Date must be before End Date.");

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

            // Tạo đối tượng Plan từ các thông tin đã nhập
            Plan newPlan = new Plan
            {
                Name = tripName,
                PlanImage = selectedImagePath,
                StartLocation = startLocation,
                EndLocation = endLocation,
                StartDate = startDate.Value,
                EndDate = endDate.Value
            };

            // Sửa plan trong danh sách kế hoạch trong ViewModel
            MyPlansHomeViewModel.UpdatePlanInHome(PlanTripViewModel, newPlan);

            // Hiển thị thông báo thành công hoặc điều hướng đến trang khác
            ContentDialog successDialog = new ContentDialog
            {
                Title = "Trip Edited Successfully",
                Content = "Your trip has been fixed.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            _ = successDialog.ShowAsync();

            // Điều hướng quay lại nếu Frame tồn tại và có thể quay lại
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

    }
}

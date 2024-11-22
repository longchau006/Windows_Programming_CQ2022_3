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
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Diagnostics;
using Windows_Programming.ViewModel;
using Windows_Programming.Model;
using WinRT.Interop;
using Windows_Programming.Database;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddTripPage : Page
    {
        private FirebaseServicesDAO firebaseServices;
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public PlansInTrashCanViewModel MyPlansTrashCanViewModel=> MainWindow.MyPlansTrashCanViewModel;

        private string selectedImagePath = "/Assets/danang.jpg";

        int accountId = MainWindow.MyAccount.Id;
        public AddTripPage()
        {
            this.InitializeComponent();

            firebaseServices = FirebaseServicesDAO.Instance;
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
                // Sử dụng BitmapImage để tạo nguồn cho Image
                var bitmapImage = new BitmapImage();
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await bitmapImage.SetSourceAsync(stream);
                }
                Trip_Image.Source = bitmapImage;
            }
        }


        // Xóa các thông tin nhập khi nhấn Cancel
        private void OnNavigationCancelButtonClick(object sender, RoutedEventArgs e)
        {
            TripName_TextBox.Text = string.Empty;
            StartLocation_TextBox.Text = string.Empty;
            EndLocation_TextBox.Text = string.Empty;
            Start_DatePicker.SelectedDate = null;
            End_DatePicker.SelectedDate = null; 
            var image = new BitmapImage(new Uri("ms-appx:///Assets/danang.jpg"));
            Trip_Image.Source = image;
            selectedImagePath = "/Assets/danang.jpg";
            Description_TextBox.Text = string.Empty;
        }

        // Lấy thông tin khi nhấn Save
        private async void OnNavigationSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin từ các trường
            string tripName = TripName_TextBox.Text;
            string startLocation = StartLocation_TextBox.Text;
            string endLocation = EndLocation_TextBox.Text;
            DateTime? startDate = Start_DatePicker.SelectedDate?.DateTime;
            DateTime? endDate = End_DatePicker.SelectedDate?.DateTime;
            string description = Description_TextBox.Text;
            // Danh sách chứa các thông báo lỗi
            List<string> errorMessages = new List<string>();

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
            int newId = MyPlansHomeViewModel.PlansInHome.Any() ? (MyPlansHomeViewModel.PlansInHome.Max(plan => plan.Id))+1 : 0;
            if (MyPlansTrashCanViewModel.PlansInTrashCan.Count>0 && (MyPlansTrashCanViewModel.PlansInTrashCan.Max(plan => plan.Id)+1)>newId)
            {
                newId = MyPlansTrashCanViewModel.PlansInTrashCan.Max(plan => plan.Id)+1;
            }


            // Tạo đối tượng Plan từ các thông tin đã nhập
            Plan newPlan = new Plan
            {
                Id = newId,
                Name = tripName,
                PlanImage = selectedImagePath,
                StartLocation = startLocation,
                EndLocation = endLocation,
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                Description = description
            };

            // Thêm đối tượng mới vào danh sách kế hoạch trong ViewModel
            MyPlansHomeViewModel.AddPlanInHome(newPlan);

            // Ghi đối tượng lên Firestore
            try
            {
                await firebaseServices.CreatePlanInFirestore(accountId, newPlan); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");

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

            // Hiển thị thông báo thành công
            ContentDialog successDialog = new ContentDialog
            {
                Title = "Trip Added Successfully",
                Content = "Your trip has been saved.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            _ = successDialog.ShowAsync();

            // Điều hướng về trang trước
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }


    }
}

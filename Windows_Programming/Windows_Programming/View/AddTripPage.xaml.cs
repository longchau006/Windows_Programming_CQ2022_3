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

        private string selectedImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "danang.jpg");

        int accountId = MainWindow.MyAccount.Id;
        public AddTripPage()
        {
            this.InitializeComponent();

            firebaseServices = FirebaseServicesDAO.Instance;
        }

        private async void OnNavigationChangePhotoButtonClick(object sender, RoutedEventArgs e)
        {
            // Tạo FileOpenPicker để chọn tệp
            var openPicker = new FileOpenPicker();
            Window tempWindow = new();
            var hWnd = WindowNative.GetWindowHandle(tempWindow);
            InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            tempWindow.Close();

            if (file != null)
            {
                try
                {
                    // Kiểm tra định dạng tệp
                    string fileExtension = Path.GetExtension(file.Path).ToLower();
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                    {
                        // Hiển thị thông báo lỗi nếu định dạng không hợp lệ
                        var dialog = new ContentDialog
                        {
                            Title = "Invalid File Format",
                            Content = "Please select an image file with a valid format (JPG, JPEG, or PNG).",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await dialog.ShowAsync();
                        return;
                    }

                    // Lưu đường dẫn ảnh đã chọn
                    selectedImagePath = file.Path;
                    System.Diagnostics.Debug.WriteLine($"Selected Image Path: {selectedImagePath}");

                    // Hiển thị ảnh trong Image control
                    var bitmapImage = new BitmapImage();
                    using (var stream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        await bitmapImage.SetSourceAsync(stream);
                    }
                    Trip_Image.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    // Hiển thị thông báo lỗi nếu có ngoại lệ xảy ra
                    var dialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = $"An error occurred while loading the image: {ex.Message}",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await dialog.ShowAsync();
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
            }
        }


        // Xóa các thông tin nhập khi nhấn Cancel
        private void OnNavigationCancelButtonClick(object sender, RoutedEventArgs e)
        {
            //TripName_TextBox.Text = string.Empty;
            //StartLocation_TextBox.Text = string.Empty;
            //EndLocation_TextBox.Text = string.Empty;
            //Start_DatePicker.SelectedDate = null;
            //End_DatePicker.SelectedDate = null; 
            //var image = new BitmapImage(new Uri("ms-appx:///Assets/danang.jpg"));
            //Trip_Image.Source = image;

            //selectedImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "danang.jpg");
            //Description_TextBox.Text = string.Empty;

            // Điều hướng về trang trước
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }

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

            int newId = MyPlansHomeViewModel.PlansInHome.Any() ? (MyPlansHomeViewModel.PlansInHome.Max(plan => plan.Id)) + 1 : 0;
            if (MyPlansTrashCanViewModel.PlansInTrashCan.Count>0 && (MyPlansTrashCanViewModel.PlansInTrashCan.Max(plan => plan.Id) + 1) > newId)
            {
                newId = MyPlansTrashCanViewModel.PlansInTrashCan.Max(plan => plan.Id) + 1;
            }


           

            // Ghi đối tượng lên Firestore
            try
            {
                

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

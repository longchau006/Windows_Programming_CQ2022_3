using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;
using WinRT.Interop;
using Windows_Programming.Database;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditTripPage : Page
    {
        private FirebaseServicesDAO firebaseServices;
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public Plan PlanTripViewModel { get; set; }

        private string selectedImagePath;

        int accountId = MainWindow.MyAccount.Id;

        int strType = -1;
        public EditTripPage()
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
                selectedImagePath = PlanTripViewModel.PlanImage;
            }

        }

        public bool NonTravellerCheckBox_IsChecked => !PlanTripViewModel.Type;

        private void OnNavigationCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            var selectedCheckBox = sender as CheckBox;

            // Ensure only one checkbox is selected
            if (selectedCheckBox == Traveller_CheckBox)
            {
                strType = 1;
                NonTraveller_CheckBox.IsChecked = false;
            }
            else if (selectedCheckBox == NonTraveller_CheckBox)
            {
                strType = 0;
                Traveller_CheckBox.IsChecked = false;
            }
        }

        private void OnNavigationCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            var uncheckedCheckBox = sender as CheckBox;

            if (!Traveller_CheckBox.IsChecked.GetValueOrDefault() && !NonTraveller_CheckBox.IsChecked.GetValueOrDefault())
            {
                strType = -2;
            }

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

        private void OnNavigationCancelButtonClick(object sender, RoutedEventArgs e)
        {
            /*TripName_TextBox.Text = PlanTripViewModel.Name.ToString();
            StartLocation_TextBox.Text = PlanTripViewModel.StartLocation.ToString();
            EndLocation_TextBox.Text = PlanTripViewModel.EndLocation.ToString();
            Start_DatePicker.Date = new DateTimeOffset(PlanTripViewModel.StartDate);
            End_DatePicker.Date = new DateTimeOffset(PlanTripViewModel.EndDate);
            Description_TextBox.Text = PlanTripViewModel.Description.ToString();
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
            }*/
            // Điều hướng về trang trước
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            //System.Diagnostics.Debug.WriteLine($"new: {selectedImagePath}");
        }
        private async void OnNavigationSaveButtonClick(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin từ các trường
            bool type = PlanTripViewModel.Type;
            string tripName = TripName_TextBox.Text;
            string startLocation = StartLocation_TextBox.Text;
            string endLocation = EndLocation_TextBox.Text;
            DateTime? startDate = Start_DatePicker.SelectedDate?.DateTime;
            DateTime? endDate = End_DatePicker.SelectedDate?.DateTime;
            string description = Description_TextBox.Text;
            if (strType == 1)
            {
                type = true;
            } else if (strType == 0)
            {
                type = false;
            }    
            // Danh sách chứa các thông báo lỗi
            List<string> errorMessages = new List<string>();

            // Kiểm tra có thay đổi không
            if ((tripName == PlanTripViewModel.Name) &&
                (startLocation == PlanTripViewModel.StartLocation) &&
                (endLocation == PlanTripViewModel.EndLocation) &&
                (startDate.HasValue && startDate.Value == PlanTripViewModel.StartDate) &&
                (endDate.HasValue && endDate.Value == PlanTripViewModel.EndDate) &&
                (description == PlanTripViewModel.Description) &&
                (selectedImagePath == PlanTripViewModel.PlanImage) &&
                (type == PlanTripViewModel.Type))
            {
                errorMessages.Add("No edits");
            }

            // Kiểm tra các trường có bị trống hay không
            if (strType ==-2)
                errorMessages.Add("Traveller, Non-Traveller is required.");
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

            ContentDialog loadingDialog = new ContentDialog
            {
                Title = "Edit Plan...",
                Content = new ProgressRing { IsActive = true },
                XamlRoot = this.XamlRoot
            };
            loadingDialog.ShowAsync();

            // Ghi đối tượng lên Firestore
            try
            {
                
                string imageUrl = PlanTripViewModel.PlanImage;
                if (!string.IsNullOrEmpty(selectedImagePath) && selectedImagePath != PlanTripViewModel.PlanImage)
                {
                    imageUrl = await firebaseServices.UploadImageToStorage(
                        selectedImagePath,
                        accountId,
                        PlanTripViewModel.Id
                    );
                }

                // Tạo đối tượng Plan từ các thông tin đã nhập
                Plan newPlan = new Plan
                {
                    Name = tripName,
                    PlanImage = selectedImagePath,
                    StartLocation = startLocation,
                    EndLocation = endLocation,
                    StartDate = startDate.Value,
                    EndDate = endDate.Value,
                    Description = description,
                    Type = type
                };
                MyPlansHomeViewModel.UpdatePlanInHome(PlanTripViewModel, newPlan);
                //Do cai nay la string nen khi thay doi o home phai gan planImage la selceted trc cho no update o local trc
                //roi sau do ta se gan lai newplan.image sau de update len db
                //tuc la cai update planInHome phai viet trc cai updata plan in firestore
                newPlan.PlanImage = imageUrl;
                await firebaseServices.UpdatePlanInFirestore(accountId, PlanTripViewModel.Id, newPlan);

                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");

                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Failed to update the trip to Firestore.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                _ = errorDialog.ShowAsync();
                loadingDialog.Hide();
                return;
            }
            loadingDialog.Hide();
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

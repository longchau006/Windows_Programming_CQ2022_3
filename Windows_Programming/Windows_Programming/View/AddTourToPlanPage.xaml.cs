﻿using Microsoft.UI.Xaml;
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
using Windows.Storage;
using Windows_Programming.Database;
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
    public sealed partial class AddTourToPlanPage : Page
    {
        private FirebaseServicesDAO firebaseServices;
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public PlansInTrashCanViewModel MyPlansTrashCanViewModel => MainWindow.MyPlansTrashCanViewModel;

        private string selectedImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "danang.jpg");

        int accountId = MainWindow.MyAccount.Id;

        Tour Tour { get; set; }

        public AddTourToPlanPage()
        {
            this.InitializeComponent();

            firebaseServices = FirebaseServicesDAO.Instance;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is Tour tour)
            {
                AddTrip(tour);
                Tour = new Tour();
                Tour = tour; 
            }
        }

        private void AddTrip(Tour tour)
        {
            TripName_TextBox.Text = tour.Name;
            TripName_TextBox.IsReadOnly = true;

            EndLocation_TextBox.Text = tour.Places;
            EndLocation_TextBox.IsReadOnly = true;

            Description_TextBox.Text = tour.Description;
        }


        private void Start_DatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            if (e.NewDate != null)
            {
                End_DatePicker.Date = e.NewDate.AddDays(1);
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
                await dialog.ShowAsync();
                return;
            }

            int newId = MyPlansHomeViewModel.PlansInHome.Any() ? (MyPlansHomeViewModel.PlansInHome.Max(plan => plan.Id)) + 1 : 0;
            if (MyPlansTrashCanViewModel.PlansInTrashCan.Count > 0 && (MyPlansTrashCanViewModel.PlansInTrashCan.Max(plan => plan.Id) + 1) > newId)
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
                Plan currentPlan = new Plan
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
                MyPlansHomeViewModel.AddPlanInHome(currentPlan);

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

                // Hiển thị thông báo thành công
                ContentDialog successDialog = new ContentDialog
                {
                    Title = "Trip Added Successfully",
                    Content = "Your trip has been saved.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();

                // Truyền Tour và StartDate vào AddTourToActivityPage
                /*var parameters = new Dictionary<string, object>
            {
                { "Tour", Tour},
                {"Plan", newPlan},
                { "StartDate", startDate }
            };
                Frame.Navigate(typeof(AddTourToActivityPage), parameters);*/

                if (newPlan.Activities == null)
                {
                    newPlan.Activities = new List<Model.Activity>();
                }
                var startTime = TimeSpan.Parse("07:00:00");
                var startDateTime = new DateTime(
                    startDate.Value.Year,
                    startDate.Value.Month,
                    startDate.Value.Day,
                    startTime.Hours,
                    startTime.Minutes,
                    startTime.Seconds
                );
                var endTime = startTime.Add(TimeSpan.FromHours(2));
                var endDateTime = new DateTime(
                    startDate.Value.Year,
                    startDate.Value.Month,
                    startDate.Value.Day,
                    endTime.Hours,
                    endTime.Minutes,
                    endTime.Seconds
                );
                foreach (var activity in Tour.Activities)
                {
                    
                    var newActivity = new Model.Activity
                    {
                        Id = newPlan.Activities.Any() ? (newPlan.Activities.Max(activity => activity.Id) + 1) : 0,
                        Type = 1,
                        Name = activity.Value,
                        Venue = activity.Key,
                        Address = activity.Key,
                        StartDate = startDate.Value,
                        EndDate = startDate.Value,
                        Description = ""
                    };
                    newPlan.Activities.Add(newActivity);
                    try
                    {
                        await firebaseServices.CreateActivityInFirestore(accountId, newPlan.Id, newActivity);

                        MyPlansHomeViewModel.AddActivitiesForPlan(currentPlan, newActivity);

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");

                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = "Failed to save the activity to Firestore.",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }

                    ContentDialog success1Dialog = new ContentDialog
                    {
                        Title = "Activity Added Successfully",
                        Content = "Your activity has been saved.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await success1Dialog.ShowAsync();

                    startDateTime = endDateTime;
                    endTime = endTime.Add(TimeSpan.FromHours(2));
                }
                foreach (var transport in Tour.Transport)
                {
                    var newTransport = new Model.Transport
                    {
                        Id = newPlan.Activities.Any() ? (newPlan.Activities.Max(activity => activity.Id) + 1) : 0,
                        Type = 2,
                        Name = transport.Key,
                        Vehicle = transport.Value["vehicle"],
                        StartLocation = transport.Value["start_location"],
                        EndLocation = transport.Value["end_location"],
                        StartDate = startDate.Value,
                        EndDate = startDate.Value,
                        Description = ""
                    };
                    newPlan.Activities.Add(newTransport);
                    try
                    {
                        await firebaseServices.CreateActivityInFirestore(accountId, newPlan.Id, newTransport);
                        MyPlansHomeViewModel.AddActivitiesForPlan(currentPlan, newTransport);

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = "Failed to save the transport to Firestore.",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                    ContentDialog success2Dialog = new ContentDialog
                    {
                        Title = "Transport Added Successfully",
                        Content = "Your transport has been saved.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    await success2Dialog.ShowAsync();
                }

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
                await errorDialog.ShowAsync();
            }

            Frame.Navigate(typeof(TourListPage));

        }
    }
}
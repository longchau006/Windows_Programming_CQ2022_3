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
            }
        }
            private async void OnNavigationChangePhotoButtonClick(object sender, RoutedEventArgs e)
        {
            
        }
        private void OnNavigationCancelButtonClick(object sender, RoutedEventArgs e)
        {
            TripName_TextBox.Text = PlanTripViewModel.Name.ToString();
            StartLocation_TextBox.Text = PlanTripViewModel.StartLocation.ToString();
            EndLocation_TextBox.Text = PlanTripViewModel.EndLocation.ToString();
            Start_DatePicker.Date = new DateTimeOffset(PlanTripViewModel.StartDate);
            End_DatePicker.Date = new DateTimeOffset(PlanTripViewModel.EndDate);

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
            //Kiểm tra có thay đổi không
            if (tripName == PlanTripViewModel.Name.ToString() &&
                startLocation == PlanTripViewModel.StartLocation.ToString() &&
                endLocation == PlanTripViewModel.EndLocation.ToString() &&
                startDate.HasValue &&
                startDate.Value == PlanTripViewModel.StartDate &&
                endDate.HasValue &&
                endDate.Value == PlanTripViewModel.EndDate)
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
                    Content = string.Join("\n", errorMessages), // Nối các thông báo lỗi thành chuỗi
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                _ = dialog.ShowAsync(); // Dùng _ để bỏ qua warning về await

                return;
            }

            Plan newPlan = new Plan
            {
                Name = tripName,
                PlanImage = "/Assets/danang.jpg",
                StartLocation = startLocation,
                EndLocation = endLocation,
                StartDate = startDate.Value, // Vì đã kiểm tra null, nên có thể dùng Value
                EndDate = endDate.Value
            };

            // Sửa plan trong danh sách kế hoạch trong ViewModel
            MyPlansHomeViewModel.UpdatePlanInHome(PlanTripViewModel, newPlan);

            // Có thể hiển thị thông báo thành công hoặc điều hướng đến trang khác
            ContentDialog successDialog = new ContentDialog
            {
                Title = "Trip Edited Successfully",
                Content = "Your trip has been fixed.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            _ = successDialog.ShowAsync();

            Frame.GoBack();

        }
    }
}

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows_Programming.Database;
using Windows_Programming.Model;
using Windows_Programming.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddActivitiesTripPage : Page
    {
        private FirebaseServicesDAO firebaseServices;
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public Plan PlanTripViewModel { get; set; }
        int flag = 0;
        int accountId = MainWindow.MyAccount.Id;
        public AddActivitiesTripPage()
        {
            this.InitializeComponent();
            Discover_Panel.Visibility = Visibility.Collapsed;
            Transport_Panel.Visibility = Visibility.Collapsed;
            Lodging_Panel.Visibility = Visibility.Collapsed;
            Extend_Panel.Visibility = Visibility.Collapsed;

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
        private void OnNavigationButtonClick(object sender, RoutedEventArgs e)
        {
            saveButton.Visibility = Visibility.Visible;
            Discover_Button.Style = (Style)Resources["ButtonStyle"];
            Transport_Button.Style = (Style)Resources["ButtonStyle"];
            Lodging_Button.Style = (Style)Resources["ButtonStyle"];
            Extend_Button.Style = (Style)Resources["ButtonStyle"];

            Button clickedButton = sender as Button;
            clickedButton.Style = (Style)Resources["SelectedButtonStyle"];

            if (clickedButton == Discover_Button)
            {
                flag = 1;
                Discover_Panel.Visibility = Visibility.Visible;
                Transport_Panel.Visibility = Visibility.Collapsed;
                Lodging_Panel.Visibility = Visibility.Collapsed;
                Extend_Panel.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == Transport_Button)
            {
                flag = 2;
                Transport_Panel.Visibility = Visibility.Visible;
                Discover_Panel.Visibility = Visibility.Collapsed;
                Lodging_Panel.Visibility = Visibility.Collapsed;
                Extend_Panel.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == Lodging_Button)
            {
                flag = 3;
                Lodging_Panel.Visibility = Visibility.Visible;
                Transport_Panel.Visibility = Visibility.Collapsed;
                Discover_Panel.Visibility = Visibility.Collapsed;
                Extend_Panel.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == Extend_Button)
            {
                flag = 4;
                Extend_Panel.Visibility = Visibility.Visible;
                Discover_Panel.Visibility = Visibility.Collapsed;
                Transport_Panel.Visibility = Visibility.Collapsed;
                Lodging_Panel.Visibility = Visibility.Collapsed;
            }
        }
        private void OnNavigationCancelButtonClick(object sender, RoutedEventArgs e)
        {
            /*if (flag == 1)
            {
                NameDiscover_TextBox.Text = string.Empty;
                VenueDiscover_TextBox.Text = string.Empty;
                AddressDiscover_TextBox.Text = string.Empty;
                StartDiscover_DatePicker.SelectedDate = null;
                EndDiscover_DatePicker.SelectedDate = null;
                StartDiscover_TimePicker.SelectedTime = null;
                EndDiscover_TimePicker.SelectedTime = null;
                DescriptionDiscover_TextBox.Text = string.Empty;
            }
            else if (flag == 2)
            {
                NameTransport_TextBox.Text = string.Empty;
                VehicleTransport_TextBox.Text = string.Empty;
                StartLocationTransport_TextBox.Text = string.Empty;
                EndLocationTransport_TextBox.Text = string.Empty;
                StartTransport_DatePicker.SelectedDate = null;
                EndTransport_DatePicker.SelectedDate = null;
                StartTransport_TimePicker.SelectedTime = null;
                EndTransport_TimePicker.SelectedTime = null;
                DescriptionTransport_TextBox.Text = string.Empty;
            }
            else if (flag == 3)
            {
                NameLodging_TextBox.Text = string.Empty;
                RoomInfoLodging_TextBox.Text = string.Empty;
                AddressLodging_TextBox.Text = string.Empty;
                StartLodging_DatePicker.SelectedDate = null;
                EndLodging_DatePicker.SelectedDate = null;
                StartLodging_TimePicker.SelectedTime = null;
                EndLodging_TimePicker.SelectedTime = null;
                DescriptionLodging_TextBox.Text = string.Empty;
            }
            else if (flag == 4)
            {
                ActivityExtend_TextBox.Text = string.Empty;
                NameExtend_TextBox.Text = string.Empty;
                VenueExtend_TextBox.Text = string.Empty;
                AddressExtend_TextBox.Text = string.Empty;
                StartExtend_DatePicker.SelectedDate = null;
                EndExtend_DatePicker.SelectedDate = null;
                StartExtend_TimePicker.SelectedTime = null;
                EndExtend_TimePicker.SelectedTime = null;
                DescriptionExtend_TextBox.Text = string.Empty;
            }*/
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
        private async void OnNavigationSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (PlanTripViewModel.Activities == null)
            {
                PlanTripViewModel.Activities = new List<Windows_Programming.Model.Activity>();
            }
            Windows_Programming.Model.Activity newActivity = null;
            if (flag == 1)
            {
                string nameDiscover = NameDiscover_TextBox.Text;
                string venueDiscover = VenueDiscover_TextBox.Text;
                string addressDiscover = AddressDiscover_TextBox.Text;
                DateTime? startDate = StartDiscover_DatePicker.SelectedDate?.DateTime;
                DateTime? endDate = EndDiscover_DatePicker.SelectedDate?.DateTime;
                TimeSpan? startTime = StartDiscover_TimePicker.SelectedTime;
                TimeSpan? endTime = EndDiscover_TimePicker.SelectedTime;
                string descriptionDiscover = DescriptionDiscover_TextBox.Text;

                DateTime? startDateTime = null;
                if (startDate.HasValue && startTime.HasValue)
                {
                    startDateTime = new DateTime(
                        startDate.Value.Year,
                        startDate.Value.Month,
                        startDate.Value.Day,
                        startTime.Value.Hours,
                        startTime.Value.Minutes,
                        startTime.Value.Seconds
                    );
                }

                DateTime? endDateTime = null;
                if (endDate.HasValue && endTime.HasValue)
                {
                    endDateTime = new DateTime(
                        endDate.Value.Year,
                        endDate.Value.Month,
                        endDate.Value.Day,
                        endTime.Value.Hours,
                        endTime.Value.Minutes,
                        endTime.Value.Seconds
                    );
                }

                List<string> errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(nameDiscover))
                    errorMessages.Add("Name is required.");
                if (string.IsNullOrWhiteSpace(venueDiscover))
                    errorMessages.Add("Venue is required.");
                if (string.IsNullOrWhiteSpace(addressDiscover))
                    errorMessages.Add("Address is required");
                if (startDate == null)
                    errorMessages.Add("Start Date is required.");
                if (endDate == null)
                    errorMessages.Add("End Date is required.");
                if (startTime == null)
                    errorMessages.Add("Start Time is required.");
                if (endTime == null)
                    errorMessages.Add("End Time is required.");
                if (PlanTripViewModel.StartDate > startDateTime || startDateTime > endDateTime || endDateTime > PlanTripViewModel.EndDate)
                    errorMessages.Add("Time error.");

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


                newActivity = new Windows_Programming.Model.Activity
                {
                    Id = PlanTripViewModel.Activities.Any() ? (PlanTripViewModel.Activities.Max(activity => activity.Id) + 1) : 0,
                    Type = flag,
                    Name = nameDiscover,
                    Venue = venueDiscover,
                    Address = addressDiscover,
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    Description = descriptionDiscover
                };
            }
            else if (flag == 2)
            {
                string nameTransport = NameTransport_TextBox.Text;
                string vehicleTransport = VehicleTransport_TextBox.Text;
                string startLocation = StartLocationTransport_TextBox.Text;
                string endLocation = EndLocationTransport_TextBox.Text;
                DateTime? startDate = StartTransport_DatePicker.SelectedDate?.DateTime;
                DateTime? endDate = EndTransport_DatePicker.SelectedDate?.DateTime;
                TimeSpan? startTime = StartTransport_TimePicker.SelectedTime;
                TimeSpan? endTime = EndTransport_TimePicker.SelectedTime;
                string descriptionTransport = DescriptionTransport_TextBox.Text;

                DateTime? startDateTime = null;
                if (startDate.HasValue && startTime.HasValue)
                {
                    startDateTime = new DateTime(
                        startDate.Value.Year,
                        startDate.Value.Month,
                        startDate.Value.Day,
                        startTime.Value.Hours,
                        startTime.Value.Minutes,
                        startTime.Value.Seconds
                    );
                }

                DateTime? endDateTime = null;
                if (endDate.HasValue && endTime.HasValue)
                {
                    endDateTime = new DateTime(
                        endDate.Value.Year,
                        endDate.Value.Month,
                        endDate.Value.Day,
                        endTime.Value.Hours,
                        endTime.Value.Minutes,
                        endTime.Value.Seconds
                    );
                }

                List<string> errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(nameTransport))
                    errorMessages.Add("Name is required.");
                if (string.IsNullOrWhiteSpace(vehicleTransport))
                    errorMessages.Add("Vehicle is required.");
                if (string.IsNullOrWhiteSpace(startLocation))
                    errorMessages.Add("Start Location is required.");
                if (string.IsNullOrWhiteSpace(endLocation))
                    errorMessages.Add("End Location is required.");
                if (startDate == null)
                    errorMessages.Add("Start Date is required.");
                if (endDate == null)
                    errorMessages.Add("End Date is required.");
                if (startTime == null)
                    errorMessages.Add("Start Time is required.");
                if (endTime == null)
                    errorMessages.Add("End Time is required.");
                if (PlanTripViewModel.StartDate > startDateTime || startDateTime > endDateTime || endDateTime > PlanTripViewModel.EndDate)
                    errorMessages.Add("Time error.");

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

                newActivity = new Transport
                {
                    Id = PlanTripViewModel.Activities.Any() ? (PlanTripViewModel.Activities.Max(activity => activity.Id) + 1) : 0,
                    Type = flag,
                    Name = nameTransport,
                    Vehicle = vehicleTransport,
                    StartLocation = startLocation,
                    EndLocation = endLocation,
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    Description = descriptionTransport,
                };

            }
            else if (flag == 3)
            {
                string nameLodging = NameLodging_TextBox.Text;
                string roomLodging = RoomInfoLodging_TextBox.Text;
                string addressLodging = AddressLodging_TextBox.Text;
                DateTime? startDate = StartLodging_DatePicker.SelectedDate?.DateTime;
                DateTime? endDate = EndLodging_DatePicker.SelectedDate?.DateTime;
                TimeSpan? startTime = StartLodging_TimePicker.SelectedTime;
                TimeSpan? endTime = EndLodging_TimePicker.SelectedTime;
                string descriptionLodging = DescriptionLodging_TextBox.Text;

                DateTime? startDateTime = null;
                if (startDate.HasValue && startTime.HasValue)
                {
                    startDateTime = new DateTime(
                        startDate.Value.Year,
                        startDate.Value.Month,
                        startDate.Value.Day,
                        startTime.Value.Hours,
                        startTime.Value.Minutes,
                        startTime.Value.Seconds
                    );
                }

                DateTime? endDateTime = null;
                if (endDate.HasValue && endTime.HasValue)
                {
                    endDateTime = new DateTime(
                        endDate.Value.Year,
                        endDate.Value.Month,
                        endDate.Value.Day,
                        endTime.Value.Hours,
                        endTime.Value.Minutes,
                        endTime.Value.Seconds
                    );
                }

                List<string> errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(nameLodging))
                    errorMessages.Add("Name is required.");
                if (string.IsNullOrWhiteSpace(roomLodging))
                    errorMessages.Add("Room Info is required.");
                if (string.IsNullOrWhiteSpace(addressLodging))
                    errorMessages.Add("Address is required");
                if (startDate == null)
                    errorMessages.Add("Start Date is required.");
                if (endDate == null)
                    errorMessages.Add("End Date is required.");
                if (startTime == null)
                    errorMessages.Add("Start Time is required.");
                if (endTime == null)
                    errorMessages.Add("End Time is required.");
                if (PlanTripViewModel.StartDate > startDateTime || startDateTime > endDateTime || endDateTime > PlanTripViewModel.EndDate)
                    errorMessages.Add("Time error.");

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

                newActivity = new Lodging
                {
                    Id = PlanTripViewModel.Activities.Any() ? (PlanTripViewModel.Activities.Max(activity => activity.Id) + 1) : 0,
                    Type = flag,
                    Name = nameLodging,
                    RoomInfo = roomLodging,
                    Address = addressLodging,
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    Description = descriptionLodging,
                };
            }
            else if (flag == 4)
            {
                string activityExtend = ActivityExtend_TextBox.Text;
                string nameExtend = NameExtend_TextBox.Text;
                string venueExtend = VenueExtend_TextBox.Text;
                string addressExtend = AddressExtend_TextBox.Text;
                DateTime? startDate = StartExtend_DatePicker.SelectedDate?.DateTime;
                DateTime? endDate = EndExtend_DatePicker.SelectedDate?.DateTime;
                TimeSpan? startTime = StartExtend_TimePicker.SelectedTime;
                TimeSpan? endTime = EndExtend_TimePicker.SelectedTime;
                string descriptionExtend = DescriptionExtend_TextBox.Text;

                DateTime? startDateTime = null;
                if (startDate.HasValue && startTime.HasValue)
                {
                    startDateTime = new DateTime(
                        startDate.Value.Year,
                        startDate.Value.Month,
                        startDate.Value.Day,
                        startTime.Value.Hours,
                        startTime.Value.Minutes,
                        startTime.Value.Seconds
                    );
                }

                DateTime? endDateTime = null;
                if (endDate.HasValue && endTime.HasValue)
                {
                    endDateTime = new DateTime(
                        endDate.Value.Year,
                        endDate.Value.Month,
                        endDate.Value.Day,
                        endTime.Value.Hours,
                        endTime.Value.Minutes,
                        endTime.Value.Seconds
                    );
                }

                List<string> errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(activityExtend))
                    errorMessages.Add("Activity is required.");
                if (string.IsNullOrWhiteSpace(nameExtend))
                    errorMessages.Add("Name is required.");
                if (string.IsNullOrWhiteSpace(venueExtend))
                    errorMessages.Add("Venue is required.");
                if (string.IsNullOrWhiteSpace(addressExtend))
                    errorMessages.Add("Address is required");
                if (startDate == null)
                    errorMessages.Add("Start Date is required.");
                if (endDate == null)
                    errorMessages.Add("End Date is required.");
                if (startTime == null)
                    errorMessages.Add("Start Time is required.");
                if (endTime == null)
                    errorMessages.Add("End Time is required.");
                if (PlanTripViewModel.StartDate > startDateTime || startDateTime > endDateTime || endDateTime > PlanTripViewModel.EndDate)
                    errorMessages.Add("Time error.");

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

                newActivity = new Extend
                {
                    Id = PlanTripViewModel.Activities.Any() ? (PlanTripViewModel.Activities.Max(activity => activity.Id) + 1) : 0,
                    Type = flag,
                    NameMore = activityExtend,
                    Name = nameExtend,
                    Venue = venueExtend,
                    Address = addressExtend,
                    StartDate = startDateTime,
                    EndDate = endDateTime,
                    Description = descriptionExtend
                };
            }

            try
            {
                await firebaseServices.CreateActivityInFirestore(accountId, PlanTripViewModel.Id, newActivity);

                MyPlansHomeViewModel.AddActivitiesForPlan(PlanTripViewModel, newActivity);
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
                _ = errorDialog.ShowAsync();
                return;
            }

            ContentDialog successDialog = new ContentDialog
            {
                Title = "Activity Added Successfully",
                Content = "Your activity has been saved.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            _ = successDialog.ShowAsync();
            MyPlansHomeViewModel.SortActivitiesByStartDate(PlanTripViewModel);
            if (Frame != null && Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

    }
}
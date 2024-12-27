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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
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
                            Extend => "Hoạt động khác",
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

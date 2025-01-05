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
using Windows_Programming.Model;
using Windows_Programming.ViewModel;
using System.Diagnostics;
using Windows_Programming.Database;
using WinRT.Interop;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using System.Threading.Tasks;
using ClosedXML.Excel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlanTripPage : Page
    {
        private ContentDialog loadingDialog;
        private FirebaseServicesDAO firebaseServices;
        public Plan PlanTripViewModel { get; set; }
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        public PlansInTrashCanViewModel MyPlansInTrashCanViewModel => MainWindow.MyPlansTrashCanViewModel;

        int accountId = MainWindow.MyAccount.Id;
        public PlanTripPage()
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
            }
        }
        private async void OnNavigationDeleteTripClick(object sender, RoutedEventArgs e)
        {
            if (PlanTripViewModel != null)
            {
                // Gọi phương thức xóa kế hoạch từ PlansInHomeViewModel
                PlanTripViewModel.DeletedDate = DateTime.Now;
                MyPlansInTrashCanViewModel.AddPlanInTrashCan(PlanTripViewModel);
                MyPlansHomeViewModel.RemovePlanInHome(PlanTripViewModel);

                // Ghi đối tượng lên Firestore
                try
                {
                    await firebaseServices.UpdateWhenDeletePlanInFirestore(accountId, PlanTripViewModel.Id, PlanTripViewModel);
                }
                catch (Exception ex)
                {
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

                // Chuyển hướng về trang Home sau khi xóa 
                Frame.Navigate(typeof(HomePage));
            }
        }
        private void OnNavigationEditTripInfoForTripClick(object sender, RoutedEventArgs e)
        {
            if (PlanTripViewModel != null)
            {
                Frame.Navigate(typeof(EditTripPage), PlanTripViewModel);
            }
        }
        private void OnNavigationAddActivitiesForTripClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddActivitiesTripPage), PlanTripViewModel);
        }

        private async void OnNavigationDeleteActivityClick(object sender, RoutedEventArgs e)
        {
            var selectedActivity = (sender as MenuFlyoutItem).CommandParameter as Model.Activity;
            if (selectedActivity != null)
            {
                MyPlansHomeViewModel.DeleteActivityForPlan(PlanTripViewModel, selectedActivity);

                // Xóa đối tượng trên Firestore
                try
                {
                    await firebaseServices.DeleteActivityInFirestore(accountId, PlanTripViewModel.Id, selectedActivity.Id);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to delete activity to Firestore: {ex.Message}");

                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Failed to delete activity in home to Firestore.",
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    };
                    _ = errorDialog.ShowAsync();
                    return;
                }

            }
            Frame.Navigate(typeof(PlanTripPage), PlanTripViewModel);
        }
        private void OnNavigationUpdateActivityClick(object sender, RoutedEventArgs e)
        {
            var selectedActivity = (sender as MenuFlyoutItem).CommandParameter as Model.Activity;
            if (selectedActivity != null)
            {
                var parameters = Tuple.Create(PlanTripViewModel, selectedActivity);
                Frame.Navigate(typeof(EditActivityPage), parameters);
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
            savePicker.SuggestedFileName = "Plan " + (PlanTripViewModel.Name.Length <= 20 ? PlanTripViewModel.Name : PlanTripViewModel.Name.Substring(0, 20));
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            w.Close();
            if (file != null)
            {
                CreateLoadingDialog();
                var loadingTask = loadingDialog.ShowAsync();
                var selectedPlan = (sender as Button).DataContext as Plan;
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

        public void OnNavigationViewExcelButtonClick(object sender, RoutedEventArgs e)
        {
           
            if (PlanTripViewModel == null)
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
                var worksheet = workbook.AddWorksheet("Plan " + (PlanTripViewModel.Name.Length <= 20 ? PlanTripViewModel.Name : PlanTripViewModel.Name.Substring(0, 20)));

                // Ghi dữ liệu kế hoạch
                worksheet.Cell(1, 1).Value = "NAME";
                worksheet.Cell(1, 2).Value = PlanTripViewModel.Name;

                worksheet.Cell(2, 1).Value = "LOCATION";
                worksheet.Cell(2, 2).Value = PlanTripViewModel.StartLocation + " - " + PlanTripViewModel.EndLocation;

                worksheet.Cell(3, 1).Value = "TIME";
                worksheet.Cell(3, 2).Value = PlanTripViewModel.StartDate.ToString("dd/MM/yyyy") + " - " + PlanTripViewModel.EndDate.ToString("dd/MM/yyyy");

                worksheet.Cell(4, 1).Value = "DESCRIPTION";
                worksheet.Cell(4, 2).Value = PlanTripViewModel.Description;

                worksheet.Cell(5, 1).Value = "LIST OF ACTIVITIES";
                worksheet.Cell(6, 1).Value = "NAME";
                worksheet.Cell(6, 2).Value = "TIME";
                worksheet.Cell(6, 3).Value = "ACTIVITY";
                worksheet.Cell(6, 4).Value = "DESCRIPTION";

                int i = 7;
                if (PlanTripViewModel.Activities != null && PlanTripViewModel.Activities.Any())
                {
                    MyPlansHomeViewModel.SortActivitiesByStartDate(PlanTripViewModel);
                    foreach (var activity in PlanTripViewModel.Activities)
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
                        i++;
                    }
                }
                // Lưu workbook vào file tạm
                string tempFilePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Plan " + (PlanTripViewModel.Name.Length <= 20 ? PlanTripViewModel.Name : PlanTripViewModel.Name.Substring(0, 20)) + ".xlsx");
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
    }
}

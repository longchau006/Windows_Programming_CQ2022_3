using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class TrashCanPage : Page
    {
        private FirebaseServicesDAO firebaseServices;
        public PlansInTrashCanViewModel MyPlansTrashCanViewModel => MainWindow.MyPlansTrashCanViewModel;
        public PlansInHomeViewModel MyPlansHomeViewModel => MainWindow.MyPlansHomeViewModel;
        int accountId=MainWindow.MyAccount.Id;
        public TrashCanPage()
        {
            this.InitializeComponent();
            firebaseServices = FirebaseServicesDAO.Instance;
        }
        private async void OnNavigationRestoreClick(object sender, RoutedEventArgs e)
        {
            var selectedPlan = (sender as Button).DataContext as Plan;

            if (selectedPlan != null)
            {
                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Confirm Restore",
                    Content = "Do you want to restore this plan to your home?",
                    PrimaryButtonText = "Cancel",
                    SecondaryButtonText = "Restore",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

               
                var secondaryButtonStyle = new Style(typeof(Button));
                secondaryButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Microsoft.UI.Colors.Blue)));
                secondaryButtonStyle.Setters.Add(new Setter(Button.ForegroundProperty, new SolidColorBrush(Microsoft.UI.Colors.White)));
                secondaryButtonStyle.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(10, 5, 10, 5)));
                secondaryButtonStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new CornerRadius(5)));
                confirmDialog.SecondaryButtonStyle = secondaryButtonStyle;

                

                var result = await confirmDialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    var temp = selectedPlan.DeletedDate;
                    System.Diagnostics.Debug.WriteLine($"ATYTTTTT:  {selectedPlan.DeletedDate}");
                    selectedPlan.DeletedDate = null;

                    try
                    {
                        System.Diagnostics.Debug.WriteLine($"ATYTTTTT:  {selectedPlan.DeletedDate}");
                        await firebaseServices.UpdateWhenDeletePlanInFirestore(accountId, selectedPlan.Id, selectedPlan);

                        MyPlansHomeViewModel.AddPlanInHome(selectedPlan);
                        MyPlansTrashCanViewModel.RemovePlanInTrashCan(selectedPlan);
                    }
                    catch (Exception ex)
                    {
                        selectedPlan.DeletedDate = temp;
                        Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");

                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = "Failed to restore the trip in home. Try again.",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                }
            }
        }

        private async void OnNavigationDeleteClick(object sender, RoutedEventArgs e)
        {
            var selectedPlan = (sender as Button).DataContext as Plan;

            if (selectedPlan != null)
            {
                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Confirm Delete",
                    Content = "Are you sure you want to permanently delete this plan?",
                    PrimaryButtonText = "Cancel",
                    SecondaryButtonText = "Delete",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };

                // Style the delete button
                confirmDialog.SecondaryButtonStyle = new Style(typeof(Button))
                {
                    Setters =
            {
                new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.Red)),
                new Setter(Button.ForegroundProperty, new SolidColorBrush(Colors.White))
            }
                };

                var result = await confirmDialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    try
                    {
                        await firebaseServices.DeleteImediatelyPlanInFirestore(accountId, selectedPlan);
                        MyPlansTrashCanViewModel.RemovePlanInTrashCan(selectedPlan);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to save to Firestore: {ex.Message}");

                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = "Failed to delete the trip. Try again after",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                }
            }
        }

    }
}

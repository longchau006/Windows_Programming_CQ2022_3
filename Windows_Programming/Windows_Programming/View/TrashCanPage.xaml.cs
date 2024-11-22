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
using System.ComponentModel;
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
        private List<Plan> selectedPlans = new List<Plan>();
        private bool isMultiSelectMode = false;
        public static readonly DependencyProperty ChooseButtonVisibilityProperty =
        DependencyProperty.Register(nameof(ChooseButtonVisibility), typeof(Visibility), typeof(TrashCanPage), new PropertyMetadata(Visibility.Collapsed));

        private bool isUpdatingCheckAll = false;
        public Visibility ChooseButtonVisibility
        {
            get => (Visibility)GetValue(ChooseButtonVisibilityProperty);
            set => SetValue(ChooseButtonVisibilityProperty, value);
        }
        public TrashCanPage()
        {
            this.InitializeComponent();
            firebaseServices = FirebaseServicesDAO.Instance;
            // Subscribe to collection changes
            MyPlansTrashCanViewModel.PlansInTrashCan.CollectionChanged += PlansInTrashCan_CollectionChanged;

            // Initial check
            UpdateChooseButtonVisibility();
        }

        private void PlansInTrashCan_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateChooseButtonVisibility();
        }

        private void UpdateChooseButtonVisibility()
        {
            ChooseButtonVisibility = MyPlansTrashCanViewModel.PlansInTrashCan.Any()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        private async void OnNavigationRestoreClick(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < MyPlansTrashCanViewModel.PlansInTrashCan.Count; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine($"K {MyPlansTrashCanViewModel.PlansInTrashCan[i].Id}");
            //}
            var selectedPlan = (sender as Button).DataContext as Plan;

            //System.Diagnostics.Debug.WriteLine($"kdskdkskskdskd:  {selectedPlan.Id} ");
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





        private void ShowCheckboxes(bool show)
        {
            var items = PlansInTrashCan_ListView.Items;
            foreach (Plan plan in items)
            {
                var container = PlansInTrashCan_ListView.ContainerFromItem(plan) as ListViewItem;
                if (container != null)
                {
                    var checkbox = FindDescendant<CheckBox>(container, "PlanCheckBox");
                    if (checkbox != null)
                    {
                        checkbox.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
                        if (!show)
                        {
                            checkbox.IsChecked = false;
                        }
                    }
                }
            }
        }

        // Helper method to find descendant control by name
        private T FindDescendant<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T && child is FrameworkElement fe && fe.Name == name)
                    return (T)child;

                var result = FindDescendant<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }



        private void OnChooseClick(object sender, RoutedEventArgs e)
        {
            isMultiSelectMode = true;
            Choose_Button.Visibility = Visibility.Collapsed;
            MultiSelectButtons.Visibility = Visibility.Visible;
            MultiSelectContainer.Visibility = Visibility.Visible;
            CheckAllPlan_CheckBox.Visibility = Visibility.Visible;
            ShowCheckboxes(true);
        }

        private void OnPlanChecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var plan = checkbox.DataContext as Plan;
            plan.IsSelected = true;
            //System.Diagnostics.Debug.WriteLine($"------------------------");
            //System.Diagnostics.Debug.WriteLine($"Khi check sau {plan.Id}");
            //for (int i = 0; i < selectedPlans.Count; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Khi check trc {selectedPlans[i].Id} {selectedPlans[i].DeletedDate}");
            //}
            if (plan != null && !selectedPlans.Contains(plan))
            {
                selectedPlans.Add(plan);
            }
            //for (int i = 0; i < selectedPlans.Count; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Khi check sau {selectedPlans[i].Id} {selectedPlans[i].DeletedDate}");
            //}
            UpdateMultiSelectButtonsState();
            UpdateCheckAllState();
        }

        private void OnPlanUnchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var plan = checkbox.DataContext as Plan;
            plan.IsSelected = false;
            //System.Diagnostics.Debug.WriteLine($"------------------------");
            //System.Diagnostics.Debug.WriteLine($"Khi uncheck sau {plan.Id}");
            //for (int i = 0; i < selectedPlans.Count; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Khi Uncheck trc {selectedPlans[i].Id} {selectedPlans[i].DeletedDate}");
            //}
            if (plan != null)
            {
                selectedPlans.Remove(plan);
            }
            //for (int i = 0; i < selectedPlans.Count; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Khi Uncheck sau {selectedPlans[i].Id} {selectedPlans[i].DeletedDate}");
            //}
            UpdateMultiSelectButtonsState();
            UpdateCheckAllState();
        }

        private void UpdateMultiSelectButtonsState()
        {
            bool hasSelection = selectedPlans.Count > 0;
            MultiRestore_Button.IsEnabled = hasSelection;
            MultiDelete_Button.IsEnabled = hasSelection;

            var restoreColor = hasSelection ? Colors.Blue : Colors.Gray;
            var deleteColor = hasSelection ? Colors.Red : Colors.Gray;

            ((TextBlock)MultiRestore_Button.Content).Foreground = new SolidColorBrush(restoreColor);
            ((TextBlock)MultiDelete_Button.Content).Foreground = new SolidColorBrush(deleteColor);
        }
        private async void OnMultiRestoreClick(object sender, RoutedEventArgs e)
        {
            var messageText = selectedPlans.Count == 1
                ? "Do you want to restore this plan to your home?"
                : $"Do you want to restore {selectedPlans.Count} plans to your home?";

            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confirm Restore",
                Content = messageText,
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
                foreach (var plan in selectedPlans.ToList())
                {
                    plan.DeletedDate = null;
                    plan.IsSelected = false;

                    try
                    {
                        await firebaseServices.UpdateWhenDeletePlanInFirestore(accountId, plan.Id, plan);
                        MyPlansHomeViewModel.AddPlanInHome(plan);
                        MyPlansTrashCanViewModel.RemovePlanInTrashCan(plan);
                    }
                    catch (Exception ex)
                    {
                        ContentDialog errorDialog = new ContentDialog
                        {
                            Title = "Error",
                            Content = $"Failed to restore plan {plan.Id}: {ex.Message}",
                            CloseButtonText = "OK",
                            XamlRoot = this.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                    }
                }

                ExitMultiSelectMode();
            }
        }

        private async void OnMultiDeleteClick(object sender, RoutedEventArgs e)
        {
            var messageText = selectedPlans.Count == 1
                ? "Are you sure you want to permanently delete this plan?"
                : $"Are you sure you want to permanently delete {selectedPlans.Count} plans?";

            ContentDialog confirmDialog = new ContentDialog
            {
                Title = "Confirm Delete",
                Content = messageText,
                PrimaryButtonText = "Cancel",
                SecondaryButtonText = "Delete",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

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
                foreach (var plan in selectedPlans.ToList())
                {
                    try
                    {
                        await firebaseServices.DeleteImediatelyPlanInFirestore(accountId, plan);
                        MyPlansTrashCanViewModel.RemovePlanInTrashCan(plan);
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
                ExitMultiSelectMode();
            }
        }

        private void OnCancelMultiSelectClick(object sender, RoutedEventArgs e)
        {
            foreach (var plan in selectedPlans.ToList())
            {
                plan.IsSelected = false;
            }
            ExitMultiSelectMode();
        }


        private void ExitMultiSelectMode()
        {
            isMultiSelectMode = false;
            CheckAllPlan_CheckBox.IsChecked = false;
            selectedPlans.Clear();
            Choose_Button.Visibility = Visibility.Visible;
            MultiSelectButtons.Visibility = Visibility.Collapsed;
            MultiSelectContainer.Visibility = Visibility.Collapsed;
            CheckAllPlan_CheckBox.Visibility = Visibility.Collapsed;
            ShowCheckboxes(false);
            UpdateChooseButtonVisibility();
        }

        private void OnButtonAllPlansCheck(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Vao Check all");
            if (!isUpdatingCheckAll)
            {
                System.Diagnostics.Debug.WriteLine("Vao Check all 1");
                var items = PlansInTrashCan_ListView.Items;
                foreach (Plan plan in items)
                {
                    var container = PlansInTrashCan_ListView.ContainerFromItem(plan) as ListViewItem;
                    if (container != null)
                    {
                        var checkbox = FindDescendant<CheckBox>(container, "PlanCheckBox");
                        if (checkbox != null)
                        {
                            checkbox.IsChecked = true;
                            plan.IsSelected = true;
                            if (!selectedPlans.Contains(plan))
                            {
                                selectedPlans.Add(plan);
                            }
                        }
                    }
                }
                UpdateMultiSelectButtonsState();
            }
           
            
        }

        private void OnButtonAllPlansUnCheck(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Vao Uncheck all");
            if (!isUpdatingCheckAll)
            {
                var items = PlansInTrashCan_ListView.Items;
                foreach (Plan plan in items)
                {
                    var container = PlansInTrashCan_ListView.ContainerFromItem(plan) as ListViewItem;
                    if (container != null)
                    {
                        var checkbox = FindDescendant<CheckBox>(container, "PlanCheckBox");
                        if (checkbox != null)
                        {
                            checkbox.IsChecked = false;
                            plan.IsSelected = false;
                            selectedPlans.Remove(plan);
                        }
                    }
                }
                UpdateMultiSelectButtonsState();
            }
            
   
        }
        private void UpdateCheckAllState()
        {
            var items = PlansInTrashCan_ListView.Items;
            bool allChecked = true;

            foreach (Plan plan in items)
            {
                var container = PlansInTrashCan_ListView.ContainerFromItem(plan) as ListViewItem;
                if (container != null)
                {
                    var checkbox = FindDescendant<CheckBox>(container, "PlanCheckBox");
                    if (checkbox != null && checkbox.IsChecked == false)
                    {
                        allChecked = false;
                        break; 
                    }
                }
            }

            isUpdatingCheckAll = true;
            CheckAllPlan_CheckBox.IsChecked = allChecked;
            isUpdatingCheckAll = false;
        }





    }
}

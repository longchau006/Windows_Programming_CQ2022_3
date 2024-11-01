using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows_Programming.Model;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddActivitiesTripPage : Page
    {
        public Plan PlanTripViewModel { get; set;}
        int flag = 0;
        public AddActivitiesTripPage()
        {
            this.InitializeComponent();
            Discover_Panel.Visibility = Visibility.Collapsed;
            Vehicle_Panel.Visibility = Visibility.Collapsed;
            Lodging_Panel.Visibility = Visibility.Collapsed;
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
            Discover_Button.Style = (Style)Resources["ButtonStyle"];
            Vehicle_Button.Style = (Style)Resources["ButtonStyle"];
            Lodging_Button.Style = (Style)Resources["ButtonStyle"];

            Button clickedButton = sender as Button;
            clickedButton.Style = (Style)Resources["SelectedButtonStyle"];

            if (clickedButton == Discover_Button)
            {
                flag = 1;  
                Discover_Panel.Visibility = Visibility.Visible;
                Vehicle_Panel.Visibility = Visibility.Collapsed;
                Lodging_Panel.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == Vehicle_Button)
            {
                flag = 2;
                Vehicle_Panel.Visibility = Visibility.Visible;
                Discover_Panel.Visibility = Visibility.Collapsed;
                Lodging_Panel.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton == Lodging_Button)
            {
                flag = 3;
                Lodging_Panel.Visibility = Visibility.Visible;
                Vehicle_Panel.Visibility = Visibility.Collapsed;
                Discover_Panel.Visibility = Visibility.Collapsed;
            }
         }
        private void OnNavigationCancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (flag == 1)
            {
                NameDiscover_TextBox.Text = string.Empty;
                DescriptionDiscover_TextBox.Text = string.Empty;


            }
            else if (flag == 2)
            {
                NameVehicle_TextBox.Text = string.Empty;
                DescriptionVehicle_TextBox.Text = string.Empty;
            }    
            else if(flag == 3) 
            { 
                NameLodging_TextBox.Text = string.Empty;
                DescriptionLodging_TextBox.Text = string.Empty;
            }
        }
        private void OnNavigationSaveButtonClick(object sender, RoutedEventArgs e)
        { 
            if (flag == 1)
            {
                string nameDiscover = NameDiscover_TextBox.Text;
                string descriptionDiscover = DescriptionDiscover_TextBox.Text;

                List<string> errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(nameDiscover))
                    errorMessages.Add("Discover Name is required.");
                if (string.IsNullOrWhiteSpace(descriptionDiscover))
                    errorMessages.Add("Discover Description is required.");

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

            }
            else if (flag == 2)
            {
                string nameVehicle = NameVehicle_TextBox.Text;
                string descriptionVehicle = DescriptionVehicle_TextBox.Text;

                List<string> errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(nameVehicle))
                    errorMessages.Add("Vehicle Name is required.");
                if (string.IsNullOrWhiteSpace(descriptionVehicle))
                    errorMessages.Add("Vehicle Description is required.");

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
            }
            else if (flag == 3)
            {
                string nameLodging = NameLodging_TextBox.Text;
                string descriptionLodging = DescriptionLodging_TextBox.Text;

                List<string> errorMessages = new List<string>();

                if (string.IsNullOrWhiteSpace(nameLodging))
                    errorMessages.Add("Lodging Name is required.");
                if (string.IsNullOrWhiteSpace(descriptionLodging))
                    errorMessages.Add("Lodging Description is required.");

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
            }
        }

    }
}

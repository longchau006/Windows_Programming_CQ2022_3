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
using Microsoft.UI;
using Windows.UI;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class KeHoachPage : Page
    {
        public KeHoachPage()
        {
            this.InitializeComponent();
            Trips.Visibility = Visibility.Visible;
            SchedulePanel.Visibility = Visibility.Collapsed;
            NoSchedulePanel.Visibility = Visibility.Visible;
            Trip.Visibility = Visibility.Collapsed;


        }

        private void OnNavigationButtonClick(object sender, RoutedEventArgs e)
        {
            // Đặt lại tất cả các nút về trạng thái không được chọn (màu mặc định)
            Upcoming_Trips.Style = (Style)Resources["ButtonStyle"];
            Past_Trips.Style = (Style)Resources["ButtonStyle"];

            // Đổi màu của nút hiện tại thành màu đã chọn
            Button clickedButton = sender as Button;
            clickedButton.Style = (Style)Resources["SelectedButtonStyle"];

            // Kiểm tra nút nào đã được nhấn và thay đổi nội dung TextBlock tương ứng
            if (clickedButton == Upcoming_Trips)
            {
                NoSchedule_TextBlock.Text = "No Upcoming Trips";
            }
            else if (clickedButton == Past_Trips)
            {
                NoSchedule_TextBlock.Text = "No Past Trips";
            }
        }

        private void OnNavigationFilterButtonClick(object sender, RoutedEventArgs e)
        {
            // Đặt lại tất cả các nút về trạng thái không được chọn (màu mặc định)
            Traveler.Style = (Style)Resources["FilterButtonStyle"];
            Non_Traveler.Style = (Style)Resources["FilterButtonStyle"];
            All.Style = (Style)Resources["FilterButtonStyle"];

            // Đổi màu của nút hiện tại thành màu đã chọn
            Button clickedButton = sender as Button;
            clickedButton.Style = (Style)Resources["SelectedFilterButtonStyle"];

            /*while (clickedButton != null)
            {
                if (clickedButton == Traveler)
                {
                }
                else if (clickedButton == Non_Traveler)
                {
                }
                else
                {
                }
            }*/
        }

        private void OnNavigationAddButtonClick(object sender, RoutedEventArgs e)
        {
            Trips.Visibility = Visibility.Visible;
            NoSchedulePanel.Visibility = Visibility.Collapsed;
            SchedulePanel.Visibility = Visibility.Visible;
            Trip.Visibility = Visibility.Collapsed;
        }

        private void OnNavigationButtonNameTripClick(object sender, RoutedEventArgs e)
        {
            Name_Alone_TextBlock.Text = Name_Button.Content.ToString();
            Location_Alone_TextBlock.Text = Location_TextBlock.Text;
            Date_Alone_TextBlock.Text = Date_TextBlock.Text;

            Trip_Alone_Image.Source = Trip_Image.Source;

            Trips.Visibility = Visibility.Collapsed;
            Trip.Visibility = Visibility.Visible;
        }

        private void OnNavigationBackToTripClick(object sender, RoutedEventArgs e)
        {
            Trip.Visibility = Visibility.Collapsed;
            Trips.Visibility = Visibility.Visible;
        }

        private void OnNavigationStartButtonClick(object sender, RoutedEventArgs e)
        {
            int number = 1;
            Count_Trip_TextBlock.Text = number.ToString();
        }
        private void OnNavigationIncreaseButtonClick(object sender, RoutedEventArgs e)
        {
            int number = int.Parse(Count_Trip_TextBlock.Text);
            if (number > 1)
            {
                number -= 1;
            }
            Count_Trip_TextBlock.Text = number.ToString();
        }
        private void OnNavigationReduceButtonClick(object sender, RoutedEventArgs e)
        {
            int number = int.Parse(Count_Trip_TextBlock.Text);
            if (number <100)
            {
                number += 1;
            }
            Count_Trip_TextBlock.Text = number.ToString();
        }
        private void OnNavigationEndButtonClick(object sender, RoutedEventArgs e)
        {
            int number = 100;
            Count_Trip_TextBlock.Text = number.ToString();
        }
    }
}

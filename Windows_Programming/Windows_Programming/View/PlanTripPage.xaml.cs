﻿using System;
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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlanTripPage : Page
    {
        public Plan PlanTripViewModel { get; set; }
        public PlanTripPage()
        {
            this.InitializeComponent();
        }

        private void HomeNagigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = args.SelectedItem as NavigationViewItem;
            if (selectedItem != null)
            {
                if (selectedItem == AccountMenuItem)
                {
                    AccountMenuFlyout.ShowAt(AccountMenuItem);
                    Home_Nagigation.SelectedItem = null;
                }
                else
                {
                    string selectedTag = selectedItem.Tag.ToString();
                    Type pageType = null;

                    switch (selectedTag)
                    {
                        case "HomePage":
                            pageType = typeof(HomePage);
                            break;
                        case "TrashCanPage":
                            pageType = typeof(TrashCanPage);
                            break;
                        case "TourPage":
                            pageType = typeof(TourPage);
                            break;
                        case "BlogPage":
                            pageType = typeof(BlogPage);
                            break;
                    }
                    if (pageType != null)
                    {
                        // Điều hướng sang trang mới
                        Frame.Navigate(pageType);
                    }
                }
            }

        }
        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            // Kiểm tra nếu có thể quay lại trang trước đó
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
        private void OnNavigationProfileClick(object sender, RoutedEventArgs e)
        {
            // Điều hướng đến trang AccountPage
            Frame.Navigate(typeof(AccountPage));
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Nh?n ??i t??ng Plan t? HomePage
            PlanTripViewModel = e.Parameter as Plan;

            // Ki?m tra n?u ??i t??ng ???c truy?n h?p l?
            if (PlanTripViewModel != null)
            {
                // Binding ??i t??ng Plan vào giao di?n
                this.DataContext = PlanTripViewModel;
            }
        }
        private void OnNavigationBackToTripClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HomePage));
        }
    }
}

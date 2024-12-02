using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows_Programming.ViewModel;

namespace Windows_Programming.View
{
    public sealed partial class TourPage : Page
    {
        public TourPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string tourId)
            {
                LoadTour(tourId);
            }
        }

        private async void LoadTour(string tourId)
        {
            var viewModel = new TourViewModel();
            await viewModel.GetTourById(tourId);
            TourDetail.DataContext = viewModel;
        }

        private void AddToPlanClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

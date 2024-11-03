using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows_Programming.ViewModel;

namespace Windows_Programming.View
{
    public sealed partial class TourPage : Page
    {
        private TourViewModel viewModel = new TourViewModel();

        public TourPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is int tourId)
            {
                viewModel.GetTourById(tourId);
                TourDetail.DataContext = viewModel; // Set DataContext to the existing viewModel
            }
        }

        private void AddToPlanClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

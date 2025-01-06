using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            System.Diagnostics.Debug.WriteLine("4");
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine("5");
            LoginFrame.Navigate(typeof(LoginPage), this);
            System.Diagnostics.Debug.WriteLine("6");
        }
        //Ham nay la de su dung ben trong loginpage cho preprocess
        public bool IsLoginWindowClosed { get; set; } = false;
    }
}

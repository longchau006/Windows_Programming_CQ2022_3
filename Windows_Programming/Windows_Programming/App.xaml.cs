using Microsoft.UI.Xaml;
using Windows_Programming.View;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    //public partial class App : Application
    //{
    //    /// <summary>
    //    /// Initializes the singleton application object.  This is the first line of authored code
    //    /// executed, and as such is the logical equivalent of main() or WinMain().
    //    /// </summary>
    //    public App()
    //    {
    //        this.InitializeComponent();
    //    }

    //    /// <summary>
    //    /// Invoked when the application is launched.
    //    /// </summary>
    //    /// <param name="args">Details about the launch request and process.</param>
    //    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    //    {
    //        m_window = new MainWindow();
    //        m_window.Activate();
    //    }

    //    private Window m_window;
    //}

    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("1");
            LoginWindow = new LoginWindow();
            System.Diagnostics.Debug.WriteLine("2");
            //Sau khi close ben preprocess ben login thi mainwindow mac du da bi dong nhung khong null
            //nen k ktra theo kieu do dc, phai ktra theo bien IsMainWindowClosed
            if (!LoginWindow.IsLoginWindowClosed)
            {
                LoginWindow.Activate();
            }

            System.Diagnostics.Debug.WriteLine("3");
        }

        public LoginWindow LoginWindow { get; private set; }

    }
}


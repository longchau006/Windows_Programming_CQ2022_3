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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows_Programming.ViewModel;
using Windows_Programming.Model;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Security.Cryptography;
using Windows.Storage;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Text;
using Windows_Programming.Database;
using System.Threading.Tasks;
using Windows_Programming.Helpers;
using System.Collections.ObjectModel;
using Google.Protobuf;
using Windows.UI.Core;
using static Google.Rpc.Context.AttributeContext.Types;
using Windows_Programming.Configs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Windows_Programming.View
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //ChatBot
        private readonly GeminiService _geminiService;
        private static PlansInHomeViewModel _myPlansHomeViewModel;
        private static PlansInTrashCanViewModel _myPlansInTrashCanViewModel;
        private static Account myAccount=null;


        private bool isChatVisible = false;
        // Change the Messages property to ObservableCollection

        public MainWindow() 
        {
            this.InitializeComponent();
            // Initialize messages in constructor
            //Khoi tao user after go tu dang nhap hoac dang ki
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            _myMessagesChatViewModel = new MessagesChatViewModel();
            _myMessagesChatViewModel.Init();
            _geminiService = new GeminiService(EnvVariables.API_GEMINI_KEY);
            myAccount = new Account
            {
                Id = -1,
                Email = "Xamxaml",
                Username = "Xamxaml",
                Address = "Xamxaml",
                Fullname = "Xamxaml",
            };

            // Kiểm tra "Id" có tồn tại trong localSettings không
            if (localSettings.Values.ContainsKey("Id"))
            {
                // Chuyển giá trị của "Id" thành int
                myAccount.Id = int.TryParse(localSettings.Values["Id"]?.ToString(), out int id) ? id : 0;
            }

            // Kiểm tra và lấy các giá trị khác
            if (localSettings.Values.ContainsKey("Username"))
                myAccount.Username = localSettings.Values["Username"]?.ToString();


            if (localSettings.Values.ContainsKey("Email"))
                myAccount.Email = localSettings.Values["Email"]?.ToString();

            if (localSettings.Values.ContainsKey("Fullname"))
                myAccount.Fullname = localSettings.Values["Fullname"]?.ToString();

            if (localSettings.Values.ContainsKey("Address"))
                myAccount.Address = localSettings.Values["Address"]?.ToString();
            myAccount.PrintAccountInfo();

            _myPlansHomeViewModel = new PlansInHomeViewModel();
            _myPlansHomeViewModel.Init();

            _myPlansInTrashCanViewModel = new PlansInTrashCanViewModel();
            _myPlansInTrashCanViewModel.Init();




            //contentNavigation.Navigate(typeof(HomePage));
            Home_Nagigation.SelectedItem = Home_Nagigation.MenuItems[0];

    

        }

        private void HomeNagigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = args.SelectedItem as NavigationViewItem;
            if (selectedItem != null)
            {
                if (selectedItem == SignOut)
                {
                    SignOutUser();
                    Home_Nagigation.SelectedItem = null;
                }
                else
                {
                    string selectedTag = selectedItem.Tag.ToString();
                    Type pageType = null;
                    Window w = null;

                    switch (selectedTag)
                    {
                        case "HomePage":
                            pageType = typeof(HomePage);
                            break;
                        case "TrashCanPage":
                            pageType = typeof(TrashCanPage);
                            break;
                        case "TourPage":
                            pageType = typeof(TourListPage);
                            break;
                        case "BlogPage":
                            pageType = typeof(BlogListPage);
                            break;
                        case "AccountPage":
                            pageType = typeof(AccountPage);
                            w = this;
                            break;
                    }
                    if (pageType != null)
                    {
                        // Điều hướng sang trang mới
                        contentNavigation.Navigate(pageType, w);
                    }
                }
            }

        }
        private void SignOutUser()
        {
            // Clear local settings
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("UserToken");
            localSettings.Values.Remove("Id");
            localSettings.Values.Remove("Username");
            localSettings.Values.Remove("Email");
            localSettings.Values.Remove("Fullname");
            localSettings.Values.Remove("Address");

            // clear all data
            myAccount = null;
            _myPlansHomeViewModel = null;
            _myPlansInTrashCanViewModel = null;

            // Navigate to LoginPage
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Activate();
            
            this.Close();
        }
        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            // Kiểm tra nếu có thể quay lại trang trước đó
            if (contentNavigation.CanGoBack)
            {
                contentNavigation.GoBack();
            }
        }

        public static PlansInHomeViewModel MyPlansHomeViewModel
        {
            get { return _myPlansHomeViewModel; }
        }

        public static PlansInTrashCanViewModel MyPlansTrashCanViewModel
        {
            get { return _myPlansInTrashCanViewModel; }
        }
        public static Account MyAccount
        {
            get
            {
                return myAccount;
            }
        }

        //ChatBot:

        private bool _isChatVisible = false;

        //Hide/Display Chatbox
        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            _isChatVisible = !_isChatVisible;
            ChatPanel.Visibility = _isChatVisible ? Visibility.Visible : Visibility.Collapsed;
        }
        private static MessagesChatViewModel _myMessagesChatViewModel;
        public MessagesChatViewModel MyMessageChatViewModel => _myMessagesChatViewModel;

        //Send Message
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
            {
                String sendMessageText = MessageTextBox.Text;
                Message newMessage = new Message { Content = MessageTextBox.Text, IsAI = false, TimeMessage = DateTime.Now };
                MessageTextBox.Text = string.Empty;
                MyMessageChatViewModel.AddNewMessageInHome(newMessage);

                //Message newAIMessage = new Message { Content = "Loading", IsAI = true };
                //MyMessageChatViewModel.AddNewMessageInHome(newAIMessage);
                // Wait for UI to update
                await Task.Delay(50);

                // Scroll to bottom
                ChatScrollViewer.UpdateLayout();
                ChatScrollViewer.ChangeView(null, double.MaxValue, null);


                //Wait for chatbot
                try
                {
                    SendButton.IsEnabled = false;
                    ResultTextBlock.Visibility = Visibility.Visible;
                    ResultTextBlock.Text = "Processing...";

                    string prompt = sendMessageText;
                    string response = await _geminiService.ProcessPrompt(prompt);

                    Message newAIMessage = new Message { Content = $"{response}", IsAI = true, TimeMessage = DateTime.Now };
                    MyMessageChatViewModel.AddNewMessageInHome(newAIMessage);
                    ResultTextBlock.Visibility = Visibility.Collapsed;
                    ResultTextBlock.Text = $"";
                }
                catch (Exception ex)
                {
                    ResultTextBlock.Visibility = Visibility.Collapsed;
                    ResultTextBlock.Text = $"";
                    Message newAIMessage = new Message { Content = "Have error", IsAI = true, TimeMessage = DateTime.Now };
                    MyMessageChatViewModel.AddNewMessageInHome(newAIMessage);
                }
                finally
                {
                    SendButton.IsEnabled = true;
                }


                // Wait for UI to update
                await Task.Delay(50);

                // Scroll to bottom
                ChatScrollViewer.UpdateLayout();
                ChatScrollViewer.ChangeView(null, double.MaxValue, null);
            }
        }

        //Delete Message
        private async void DeleteHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            MyMessageChatViewModel.DeleteAllHistoryChat();
            SendButton.IsEnabled = true;
            ResultTextBlock.Visibility= Visibility.Collapsed;
        }
        private void MessageTextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"Key pressed: {e.Key}");

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                //System.Diagnostics.Debug.WriteLine("Enter detected");
                bool isShiftPressed = (Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift) & Windows.UI.Core.CoreVirtualKeyStates.Down) != 0;

                if (!isShiftPressed)
                {
                    //System.Diagnostics.Debug.WriteLine("No shift - sending message");
                    e.Handled = true;
                    SendButton_Click(sender, e);
                    MessageTextBox.Text = string.Empty;
                }
                else
                {
                   // System.Diagnostics.Debug.WriteLine("Shift pressed - new line");
                }
            }
        }

        //Menu flyout for available function
        private void MenuFlyoutItem_ChangeUserInfor(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text = "Change my information with the following details:\n\n-Fullname: \n-Address: ";
            //Set locate con tro chuot
            MessageTextBox.Focus(FocusState.Programmatic);
            MessageTextBox.Select(MessageTextBox.Text.IndexOf("Fullname:") + "Fullname:".Length + 1, 0);
        }
        private void MenuFlyoutItem_AddBlog(object sender, RoutedEventArgs e)
        {
            MessageTextBox.Text = "Create a travel blog for me with the following details:\n\n-Title: \n-Content: Automatically generate a 100-word travel article.\n-Image URL: ";
            //Set locate con tro chuot
            MessageTextBox.Focus(FocusState.Programmatic);
            MessageTextBox.Select(MessageTextBox.Text.IndexOf("Title:") + "Title:".Length + 1, 0);
        }

    }
}

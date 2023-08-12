using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sound_Controller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            var FormattableTitleBar = CoreApplication.GetCurrentView().TitleBar;
            FormattableTitleBar.ExtendViewIntoTitleBar = true;

            var AppTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            AppTitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            Window.Current.SetTitleBar(CustomTitleBar);
        }

        private async void MainNavigation_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            MediaDeviceService deviceService = new MediaDeviceService();

            List<Microsoft.UI.Xaml.Controls.NavigationViewItem> PlaybackDevices = await deviceService.GetAllPlaybackDevicesAsync();

            List<Microsoft.UI.Xaml.Controls.NavigationViewItem> RecordingDevices = await deviceService.GetAllRecordingDevicesAsync();

            if (sender.SelectedItem == HomeMenu)
            {
                AppContentFrame.Navigate(typeof(HomePage));
            }
            else if (sender.SelectedItem == sender.MenuItems[2])
            {
                PlaybackDeviceHelper PlayDevHelper = new PlaybackDeviceHelper();
                PlayDevHelper.DeviceName = PlaybackDevices[0].Content.ToString();
                AppContentFrame.Navigate(typeof(PlaybackDevicesPage), PlayDevHelper);
            }
            else if (sender.SelectedItem == sender.MenuItems[3])
            {
                PlaybackDeviceHelper PlayDevHelper = new PlaybackDeviceHelper();
                PlayDevHelper.DeviceName = PlaybackDevices[1].Content.ToString();
                AppContentFrame.Navigate(typeof(PlaybackDevicesPage), PlayDevHelper);
            }
            else if (sender.SelectedItem == sender.MenuItems[5])
            {
                RecordingDeviceHelper RecDevHelper = new RecordingDeviceHelper();
                RecDevHelper.DeviceName = RecordingDevices[0].Content.ToString();
                AppContentFrame.Navigate(typeof(RecordingDevicesPage), RecDevHelper);
            }
            else if (sender.SelectedItem == sender.MenuItems[6])
            {
                RecordingDeviceHelper RecDevHelper = new RecordingDeviceHelper();
                RecDevHelper.DeviceName = RecordingDevices[1].Content.ToString();
                AppContentFrame.Navigate(typeof(RecordingDevicesPage), RecDevHelper);
            }
            else if (sender.SelectedItem == sender.SettingsItem)
            {
                AppContentFrame.Navigate(typeof(SettingsPage));
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainNavigation.SelectedItem = HomeMenu;

            if (MainNavigation.SelectedItem == HomeMenu)
            {
                AppContentFrame.Navigate(typeof(HomePage));
            }

            MediaDeviceService deviceService = new MediaDeviceService();

            List<Microsoft.UI.Xaml.Controls.NavigationViewItem> PlaybackDevices = await deviceService.GetAllPlaybackDevicesAsync();

            MainNavigation.MenuItems.Add(new Microsoft.UI.Xaml.Controls.NavigationViewItemHeader()
            {
                Content = "Playback Devices"
            });

            foreach (var Device in PlaybackDevices)
            {
                MainNavigation.MenuItems.Add(Device);
            }

            List<Microsoft.UI.Xaml.Controls.NavigationViewItem> RecordingDevices = await deviceService.GetAllRecordingDevicesAsync();

            MainNavigation.MenuItems.Add(new Microsoft.UI.Xaml.Controls.NavigationViewItemHeader()
            {
                Content = "Recording Devices"
            });

            foreach (var Device in RecordingDevices)
            {
                MainNavigation.MenuItems.Add(Device);
            }
        }
    }
}

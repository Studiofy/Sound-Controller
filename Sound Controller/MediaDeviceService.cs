using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;
using Windows.UI.Xaml.Controls;

namespace Sound_Controller
{
    public class MediaDeviceService
    {
        public async Task<List<Microsoft.UI.Xaml.Controls.NavigationViewItem>> GetAllPlaybackDevicesAsync()
        {
            DeviceInformationCollection DeviceCollection = await DeviceInformation.FindAllAsync(MediaDevice.GetAudioRenderSelector());

            List<Microsoft.UI.Xaml.Controls.NavigationViewItem> MenuItemList = new List<Microsoft.UI.Xaml.Controls.NavigationViewItem>();

            if (DeviceCollection != null)
            {
                foreach (var Device in DeviceCollection)
                {
                    Microsoft.UI.Xaml.Controls.NavigationViewItem item = new Microsoft.UI.Xaml.Controls.NavigationViewItem()
                    {
                        Icon = new SymbolIcon() 
                        { 
                            Symbol = Symbol.Volume 
                        },
                        Content = Device.Name.ToString(),
                        Height = 45
                    };
                    MenuItemList.Add(item);
                }
            }
            return MenuItemList;
        }
        
        public async Task<List<Microsoft.UI.Xaml.Controls.NavigationViewItem>> GetAllRecordingDevicesAsync()
        {
            DeviceInformationCollection DeviceCollection = await DeviceInformation.FindAllAsync(MediaDevice.GetAudioCaptureSelector());

            List<Microsoft.UI.Xaml.Controls.NavigationViewItem> MenuItemList = new List<Microsoft.UI.Xaml.Controls.NavigationViewItem>();

            if (DeviceCollection != null)
            {
                foreach (var Device in DeviceCollection)
                {
                    Microsoft.UI.Xaml.Controls.NavigationViewItem item = new Microsoft.UI.Xaml.Controls.NavigationViewItem()
                    {
                        Icon = new SymbolIcon()
                        {
                            Symbol = Symbol.Microphone
                        },
                        Content = Device.Name.ToString(),
                        Height = 45
                    };
                    MenuItemList.Add(item);
                }
            }
            return MenuItemList;
        }

        private async Task<string> GetPlaybackDeviceID()
        {
            return null;
        }

        private async Task<double> GetPlaybackDeviceVolume(Task<string> PlaybackID)
        {
            return 0;
        }
    }
}

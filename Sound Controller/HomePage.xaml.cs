using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.Media.Devices;
using Windows.Media.Render;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Sound_Controller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        static async Task<string> GetDefaultPlaybackDeviceIdAsync()
        {
            string playbackDeviceId = string.Empty;

            // Get the default audio render (playback) device
            string DefaultAudioRenderSelector = MediaDevice.GetAudioRenderSelector();
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DefaultAudioRenderSelector);

            if (devices.Count > 0)
            {
                playbackDeviceId = devices[0].Id;
            }

            return playbackDeviceId;
        }

        private async Task<double> GetPlaybackDeviceVolumeAsync(Task<string> TaskName)
        {
            string PlaybackDeviceID = await TaskName;

            DeviceInformation PlaybackDevice = await DeviceInformation.CreateFromIdAsync(PlaybackDeviceID);
            AudioGraphSettings GraphSettings = new AudioGraphSettings(AudioRenderCategory.Media);

            var AGraph = await AudioGraph.CreateAsync(GraphSettings);
            var OutputNode = await AGraph.Graph.CreateDeviceOutputNodeAsync();
            double Volume = OutputNode.DeviceOutputNode.OutgoingGain;

            return Volume;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await GetPlayBackDevicesAsync();
            await GetRecordingDevicesAsync();
        }

        private async Task GetPlayBackDevicesAsync()
        {
            DeviceInformationCollection Devices = await DeviceInformation.FindAllAsync(MediaDevice.GetAudioRenderSelector());

            double _volume = await GetPlaybackDeviceVolumeAsync(GetDefaultPlaybackDeviceIdAsync());
            Debug.WriteLine(_volume);

            try
            {
                foreach (var Device in Devices)
                {
                    string DeviceName = Device.Name;

                    int ParenthesisIndex = DeviceName.IndexOf('(');

                    string CleanDeviceName = DeviceName.Substring(0, ParenthesisIndex).Trim() ?? null;

                    Expander PlaybackDeviceExpander = new Expander()
                    {
                        Margin = new Thickness(0, 5, 0, 5),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch
                    };

                    Grid ExpanderHeaderGrid = new Grid()
                    {
                        Height = 55,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    SymbolIcon VolumeIcon = new SymbolIcon()
                    {
                        Symbol = Symbol.Volume,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    TextBlock HeaderText = new TextBlock()
                    {
                        Text = CleanDeviceName,
                        Margin = new Thickness(30, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        MinWidth = 100,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    };

                    Slider VolumeSlider = new Slider()
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 200
                    };

                    VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;

                    TextBlock SliderValue = new TextBlock()
                    {
                        Margin = new Thickness(0, 0, 210, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = "Volume: " + _volume.ToString() + "%"
                    };

                    ExpanderHeaderGrid.Children.Add(VolumeIcon);
                    ExpanderHeaderGrid.Children.Add(HeaderText);
                    ExpanderHeaderGrid.Children.Add(SliderValue);
                    ExpanderHeaderGrid.Children.Add(VolumeSlider);

                    PlaybackDeviceExpander.Header = ExpanderHeaderGrid;

                    PlaybackDevicesItemList.Children.Add(PlaybackDeviceExpander);
                }

                Expander PlaybackDeviceOption = new Expander()
                {
                    Margin = new Thickness(0, 5, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                };

                Grid PlaybackDeviceOptionHeader = new Grid()
                {
                    Height = 55,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                SymbolIcon AddDeviceIcon = new SymbolIcon()
                {
                    Symbol = Symbol.Add,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                TextBlock AddDeviceText = new TextBlock()
                {
                    Text = "Add Another Playback Device",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(30, 0, 0, 0)
                };

                PlaybackDeviceOptionHeader.Children.Add(AddDeviceIcon);
                PlaybackDeviceOptionHeader.Children.Add(AddDeviceText);

                PlaybackDeviceOption.Header = PlaybackDeviceOptionHeader;

                StackPanel PlaybackDeviceOptionContent = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                TextBlock OptionItemText = new TextBlock()
                {
                    Text = "To Add Devices, Go To Windows Settings",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(30, 10, 0, 0)
                };

                Button OptionItemButton = new Button()
                {
                    Content = "Open Windows Settings",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, -25, 45, 0)
                };

                OptionItemButton.Click += OptionItemButton_Click;

                PlaybackDeviceOptionContent.Children.Add(OptionItemText);
                PlaybackDeviceOptionContent.Children.Add(OptionItemButton);

                PlaybackDeviceOption.Content = PlaybackDeviceOptionContent;

                PlaybackDevicesItemList.Children.Add(PlaybackDeviceOption);
            }
            catch (Exception ex)
            {
                ItemList.Children.Add(new TextBlock()
                {
                    Text = ex.Message,
                    Margin = new Thickness(10)
                });
            }
        }

        private async Task GetRecordingDevicesAsync()
        {
            DeviceInformationCollection Devices = await DeviceInformation.FindAllAsync(MediaDevice.GetAudioCaptureSelector());

            try
            {
                foreach (var Device in Devices)
                {
                    string DeviceName = Device.Name;

                    int ParenthesisIndex = DeviceName.IndexOf('(');

                    string CleanDeviceName = DeviceName.Substring(0, ParenthesisIndex).Trim() ?? null;

                    Expander RecordingDeviceExpander = new Expander()
                    {
                        Margin = new Thickness(0, 5, 0, 5),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch
                    };

                    Grid ExpanderHeaderGrid = new Grid()
                    {
                        Height = 55,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };

                    SymbolIcon MicrophoneIcon = new SymbolIcon()
                    {
                        Symbol = Symbol.Microphone,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    TextBlock HeaderText = new TextBlock()
                    {
                        Text = CleanDeviceName,
                        Margin = new Thickness(30, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Slider VolumeSlider = new Slider()
                    {
                        Margin = new Thickness(0, 0, 0, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 200
                    };

                    VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;

                    TextBlock SliderValue = new TextBlock()
                    {
                        Margin = new Thickness(0, 0, 210, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = "Volume: " + GetVolumeValue().ToString() + "%"
                    };

                    ExpanderHeaderGrid.Children.Add(MicrophoneIcon);
                    ExpanderHeaderGrid.Children.Add(HeaderText);
                    ExpanderHeaderGrid.Children.Add(SliderValue);
                    ExpanderHeaderGrid.Children.Add(VolumeSlider);

                    RecordingDeviceExpander.Header = ExpanderHeaderGrid;

                    RecordingDevicesItemList.Children.Add(RecordingDeviceExpander);
                }

                Expander RecordingDeviceOption = new Expander()
                {
                    Margin = new Thickness(0, 5, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch
                };

                Grid RecordingDeviceOptionHeader = new Grid()
                {
                    Height = 55,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                SymbolIcon AddDeviceIcon = new SymbolIcon()
                {
                    Symbol = Symbol.Add,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                TextBlock AddDeviceText = new TextBlock()
                {
                    Text = "Add Another Recording Device",
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(30, 0, 0, 0)
                };

                RecordingDeviceOptionHeader.Children.Add(AddDeviceIcon);
                RecordingDeviceOptionHeader.Children.Add(AddDeviceText);

                RecordingDeviceOption.Header = RecordingDeviceOptionHeader;

                Grid RecordingDeviceOptionContent = new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center
                };

                TextBlock OptionItemText = new TextBlock()
                {
                    Text = "To Add Devices, Go To Windows Settings",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(30, 0, 0, 0)
                };

                Button OptionItemButton = new Button()
                {
                    Content = "Open Windows Settings",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 45, 0)
                };

                OptionItemButton.Click += OptionItemButton_Click;

                RecordingDeviceOptionContent.Children.Add(OptionItemText);
                RecordingDeviceOptionContent.Children.Add(OptionItemButton);

                RecordingDeviceOption.Content = RecordingDeviceOptionContent;

                RecordingDevicesItemList.Children.Add(RecordingDeviceOption);
            }
            catch (Exception ex)
            {
                ItemList.Children.Add(new TextBlock()
                {
                    Text = ex.Message,
                    Margin = new Thickness(10)
                });
            }
        }

        private async void OptionItemButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog OpenSettingsPrompt = new ContentDialog()
            {
                Title = "Sound Controller Wants To Open Settings",
                Content = "Do you want to open Windows Settings?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Close
            };

            OpenSettingsPrompt.PrimaryButtonClick += OpenSettingsPrompt_PrimaryButtonClick;

            await OpenSettingsPrompt.ShowAsync();
        }

        private async void OpenSettingsPrompt_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Uri SoundSettingsUri = new Uri("ms-settings:sound");
            bool Success = await Launcher.LaunchUriAsync(SoundSettingsUri);

            if (!Success)
            {
                ContentDialog ErrorDialog = new ContentDialog()
                {
                    Title = "Error Opening Sound Settings",
                    Content = "Please Restart the App and Try Again or Open Windows Settings Directly using Your PC."
                };

                await ErrorDialog.ShowAsync();
            }
        }

        private double Volume;

        private double GetVolumeValue()
        {
            return Volume;
        }

        private void SetVolumeValue(double value)
        {
            Volume = value;
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                SetVolumeValue(e.NewValue);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
    }
}

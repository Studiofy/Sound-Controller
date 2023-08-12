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

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
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

        private async Task GetPlayBackDevicesAsync()
        {
            DeviceInformationCollection Devices = await DeviceInformation.FindAllAsync(MediaDevice.GetAudioRenderSelector());

            double _volume = await GetPlaybackDeviceVolumeAsync(GetDefaultPlaybackDeviceIdAsync());
            Debug.WriteLine(_volume);

            try
            {
                foreach (var Device in Devices)
                {
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
                        Text = Device.Name,
                        Margin = new Thickness(30, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Slider VolumeSlider = new Slider()
                    {
                        Margin = new Thickness(0, 0, 35, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 200
                    };
                    VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;

                    TextBlock SliderValue = new TextBlock()
                    {
                        Margin = new Thickness(0, 0, 245, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = _volume.ToString() + "%"
                    };

                    ToggleButton ToggleMuteButton = new ToggleButton()
                    {
                        Content = new SymbolIcon()
                        {
                            Symbol = Symbol.Mute
                        },
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 45,
                        Margin = new Thickness(0, 0, -15, 0)
                    };

                    ExpanderHeaderGrid.Children.Add(VolumeIcon);
                    ExpanderHeaderGrid.Children.Add(HeaderText);
                    ExpanderHeaderGrid.Children.Add(SliderValue);
                    ExpanderHeaderGrid.Children.Add(VolumeSlider);
                    ExpanderHeaderGrid.Children.Add(ToggleMuteButton);

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

                Grid PlaybackDeviceOptionContent = new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                StackPanel OptionItemList = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                TextBlock OptionItemText = new TextBlock()
                {
                    Text = "To Add Devices, Go To Windows Settings",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(30, 0, 0, 0),
                };

                Button OptionItemButton = new Button()
                {
                    Content = "Open Windows Settings",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };

                OptionItemList.Children.Add(OptionItemText);
                OptionItemList.Children.Add(new TextBlock() { Width = 1 });
                OptionItemList.Children.Add(OptionItemButton);

                PlaybackDeviceOptionContent.Children.Add(OptionItemList);

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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await GetPlayBackDevicesAsync();
            await GetRecordingDevicesAsync();
        }

        private async Task GetRecordingDevicesAsync()
        {
            DeviceInformationCollection Devices = await DeviceInformation.FindAllAsync(MediaDevice.GetAudioCaptureSelector());

            try
            {
                foreach (var Device in Devices)
                {
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
                        Text = Device.Name,
                        Margin = new Thickness(30, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Slider VolumeSlider = new Slider()
                    {
                        Margin = new Thickness(0, 0, 35, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 200
                    };
                    VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;

                    TextBlock SliderValue = new TextBlock()
                    {
                        Margin = new Thickness(0, 0, 245, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Text = VolumeSlider.Value.ToString() + "%"
                    };

                    ToggleButton ToggleMuteButton = new ToggleButton()
                    {
                        Content = new SymbolIcon()
                        {
                            Symbol = Symbol.Mute
                        },
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 45,
                        Margin = new Thickness(0, 0, -15, 0)
                    };

                    ExpanderHeaderGrid.Children.Add(MicrophoneIcon);
                    ExpanderHeaderGrid.Children.Add(HeaderText);
                    ExpanderHeaderGrid.Children.Add(SliderValue);
                    ExpanderHeaderGrid.Children.Add(VolumeSlider);
                    ExpanderHeaderGrid.Children.Add(ToggleMuteButton);

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
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                StackPanel OptionItemList = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                TextBlock OptionItemText = new TextBlock()
                {
                    Text = "To Add Devices, Go To Windows Settings",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(30, 0, 0, 0),
                };

                Button OptionItemButton = new Button()
                {
                    Content = "Open Windows Settings",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };

                OptionItemList.Children.Add(OptionItemText);
                OptionItemList.Children.Add(new TextBlock() { Width = 1 });
                OptionItemList.Children.Add(OptionItemButton);

                RecordingDeviceOptionContent.Children.Add(OptionItemList);

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
    }
}

using System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SN3218.Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private WinHill.Devices.SN3218 device = new WinHill.Devices.SN3218();

        public MainPage()
        {
            this.InitializeComponent();

            device.Initialize();
            device.Enable();
            AllOff();
            SetBrightness(127);
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked.Value)
                AllOn();
            else
                AllOff();
        }

        private void AllOn()
        {
            device.EnableLeds(0x3ffff); // enable all leds
        }

        private void AllOff()
        {
            device.EnableLeds(0x00); // disable all leds
        }

        private void SetBrightness(int value)
        {
            device.Output(value, value, value, value, value, value, value, value, value, value, value, value, value, value, value, value, value, value);
        }

        private void Slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            SetBrightness((int)e.NewValue);
        }
    }
}

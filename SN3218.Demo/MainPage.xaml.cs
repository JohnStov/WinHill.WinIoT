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
        }
    }
}

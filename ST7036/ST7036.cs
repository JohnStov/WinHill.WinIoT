using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace WinHill.Devices
{
    // ReSharper disable once InconsistentNaming
    public class ST7036
    {
        private const string SpiControllerName = "SPI0";
        private const int SpiChipSelect = 0;

        private SpiDevice device;

        private enum Command
        {
            Clear          = 0x01,
            Home           = 0x02,
            Scroll         = 0x10,
            Double         = 0x10,
            Bias           = 0x14,
            SetDisplayMode = 0x08
        }

        public bool Initialize()
        {
            device = Task.Run(async () => await InitializeDevice()).Result;
            return device != null;
        }

        private async Task<SpiDevice> InitializeDevice()
        {
            // initialize I2C communications
            try
            {
                var deviceSelector = SpiDevice.GetDeviceSelector(SpiControllerName);
                var spiDeviceControllers = await DeviceInformation.FindAllAsync(deviceSelector);
                var spiSettings = new SpiConnectionSettings(SpiChipSelect);
                return await SpiDevice.FromIdAsync(spiDeviceControllers[0].Id, spiSettings);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception: {0}", e.Message);
                return null;
            }
        }
    }
}

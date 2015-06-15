using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace WinHill.Devices
{
    public class SN3218
    {
        private const int I2C_ADDRESS = 0x54;
        private const string I2C_CONTROLLER_NAME = "I2C1";

        private I2cDevice device;

        public void Initialize()
        {
            device = Task.Run(async () => { return await InitializeDevice(); }).Result;
        }

        private async Task<I2cDevice> InitializeDevice()
        {
            // initialize I2C communications
            try
            {
                var i2cSettings = new I2cConnectionSettings(I2C_ADDRESS);
                i2cSettings.BusSpeed = I2cBusSpeed.FastMode;
                string deviceSelector = I2cDevice.GetDeviceSelector(I2C_CONTROLLER_NAME);
                var i2cDeviceControllers = await DeviceInformation.FindAllAsync(deviceSelector);
                return await I2cDevice.FromIdAsync(i2cDeviceControllers[0].Id, i2cSettings);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception: {0}", e.Message);
                return null;
            }
        }
    }
}

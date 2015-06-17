using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Windows.Devices.Gpio;

namespace WinHill.Devices
{
    // ReSharper disable once InconsistentNaming
    public class ST7036
    {
        private const string SpiControllerName = "SPI0";
        private const int SpiChipSelect = 0;
        private GpioPin registerSelectPin;
        private int instructionSetTemplate = 0x38;
        private int doubleHeight = 0;


        private const int BLINK_ON   = 0x01;
        private const int CURSOR_ON = 0x02;
        private const int DISPLAY_ON = 0x04;

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

        public void Write(string text)
        {
            registerSelectPin.Write(GpioPinValue.High);

            foreach (var c in text)
            {
                device.Write(new byte[] { (byte)c });
                Task.Delay(1);
            }
        }
                
        public bool Initialize(int registerSelect = 25)
        {
            var gpio = GpioController.GetDefault();
            registerSelectPin = gpio.OpenPin(registerSelect, GpioSharingMode.Exclusive);
            registerSelectPin.SetDriveMode(GpioPinDriveMode.Output);
            registerSelectPin.Write(GpioPinValue.High);

            device = Task.Run(async () => await InitializeDevice()).Result;

            if (device == null)
                return false;

            UpdateDisplayMode();
            // set entry mode (no shift, cursor direction)        
            WriteCommand(0x04 | 0x02);
            SetBias(1);
            SetContrast(40);
            Clear();

            return true;
        }

        private void Clear()
        {
            WriteCommand((int)Command.Clear);
            Home();
        }

        private void Home()
        {
            SetCursorPosition(0, 0);
        }

        private void SetCursorPosition(int column, int row)
        {
            // TODO: calculate row/column offset
            WriteCommand(0x80);
        }

        private void SetContrast(int contrast)
        {
            if (contrast < 0 || contrast > 40)
                throw new ArgumentOutOfRangeException("contrast");

            // For 3.3v operation the booster must be on, which is
            // on the same command as the (2-bit) high-nibble of contrastWriteCommand((0x54 | ((contrast >> 4) & 0x03)), 1);
            WriteCommand(0x6b, 1);

            //Set low-nibble of the contrast
            WriteCommand((0x70 | (contrast & 0x0F)), 1);
        }

        private void SetBias(int bias)
        {
            WriteCommand((int)Command.Bias | (bias << 4) | 1, 1);
        }

        private void UpdateDisplayMode()
        {
            var mask = (int) Command.SetDisplayMode | DISPLAY_ON | CURSOR_ON | BLINK_ON;
            WriteCommand(mask);
        }

        private void WriteCommand(int value, int instructionSet = 0)
        {
            registerSelectPin.Write(GpioPinValue.Low);
            WriteInstructionSet(instructionSet);
            device.Write(new[] { (byte)value });
            Task.Delay(1);
        }

        private void WriteInstructionSet(int instructionSet)
        {
            registerSelectPin.Write(GpioPinValue.Low);
            byte cmd = (byte)(instructionSetTemplate | instructionSet | doubleHeight << 2);
            device.Write(new byte[] { cmd });
            Task.Delay(1);
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

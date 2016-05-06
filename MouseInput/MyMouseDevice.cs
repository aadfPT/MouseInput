using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using HidLibrary;
using ODIF;

namespace MouseInput
{
    public class MyMouseDevice : InputDevice
    {
        internal HidDevice Device;
        internal MouseInputDevice DeviceWrapper;


        private Thread InputPoolingThread { get; }

        private bool StopThread { get; set; }

        public MyMouseDevice(HidDevice deviceInstance)
        {
            this.Device = deviceInstance;
            Device.OpenDevice(DeviceMode.NonOverlapped, DeviceMode.NonOverlapped, ShareMode.ShareWrite | ShareMode.ShareRead);
            this.DeviceWrapper = new MouseInputDevice();
            byte[] serialNumber;
            byte[] product;
            Device.ReadSerialNumber(out serialNumber);
            Device.ReadProduct(out product);
            base.DeviceName = Regex.Replace(Encoding.UTF8.GetString(product), "[^-a-zA-Z0-9 ]", String.Empty)
                              + " (" + Regex.Replace(Encoding.UTF8.GetString(serialNumber), "[^-a-zA-Z0-9 ]", String.Empty) + ")";

            AddChannels();
            InputPoolingThread = new Thread(InputListenerThread);
            InputPoolingThread.Start();
        }

        private void AddChannels()
        {
            AddInputChannels();

        }


        private void AddInputChannels()
        {
            InputChannels.Add(DeviceWrapper.HorizontalAxis);
            InputChannels.Add(DeviceWrapper.VerticalAxis);
            InputChannels.Add(DeviceWrapper.XPosition);
            InputChannels.Add(DeviceWrapper.YPosition);

            InputChannels.Add(DeviceWrapper.XDelta);
            InputChannels.Add(DeviceWrapper.YDelta);

            InputChannels.Add(DeviceWrapper.LeftButton);
            InputChannels.Add(DeviceWrapper.RightButton);
            InputChannels.Add(DeviceWrapper.MiddleMouse);
            InputChannels.Add(DeviceWrapper.WheelUp);
            InputChannels.Add(DeviceWrapper.WheelDown);
            InputChannels.Add(DeviceWrapper.X1);
            InputChannels.Add(DeviceWrapper.X2);
        }


        protected override void Dispose(bool disposing)
        {
            StopThread = true;
            InputPoolingThread?.Abort();
            Device.Dispose();
            base.Dispose(disposing);
        }

        public void InputListenerThread()
        {
            while (!this.StopThread && !Global.IsShuttingDown)
            {
                try
                {
                    var currentState = Device.Read().Data;
                    //if (currentState.Length < 21) continue;
                    DeviceWrapper.LeftButton.Value = false;

                }
                catch (Exception)
                {
                    //error in reading from controller is ignored
                    continue;
                }

            }
        }

    }
}
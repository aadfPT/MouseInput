using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODIF;
using ODIF.Extensions;
using System.Management;
using System.Reflection;
using HidLibrary;

namespace MouseInput
{
    [PluginInfo(
        PluginName = "Mouse Input",
        PluginDescription = "Adds support for the Mouse Input",
        PluginVersion = "1.0.0.0",
        PluginID = 999,
        PluginAuthorName = "André Ferreira",
        PluginAuthorEmail = "aadf.pt [at] gmail [dot] com",
        PluginAuthorURL = "https://github.com/aadfPT",
        PluginIconPath = @"pack://application:,,,/Mouse Input;component/Resources/MIButton.png"
        )]
    public class MouseInputPlugin : InputDevicePlugin
    {
        public MouseInputPlugin()
        {
            Global.HardwareChangeDetected += CheckForControllersEvent;
            CheckForControllers();
        }

        private void CheckForControllersEvent(object sender, EventArrivedEventArgs e)
        {
            CheckForControllers();
        }

        public void CheckForControllers()
        {
            lock (base.Devices)
            {
                var compatibleDevices = HidDevices.EnumerateAllMice().ToList();
                foreach (var deviceInstance in compatibleDevices)
                {
                    if (Devices.Any(d => ((MyMouseDevice)d).Device.DevicePath == deviceInstance.DevicePath))
                    {
                        continue;
                    }
                    Devices.Add(new MyMouseDevice(deviceInstance));
                }
                foreach (var inputDevice in Devices)
                {
                    var deviceReference = (MyMouseDevice)inputDevice;
                    if (compatibleDevices.All(d => d.DevicePath != deviceReference.Device.DevicePath))
                    {
                        Devices.Remove(deviceReference);
                    }
                }
            }
        }
    }
}
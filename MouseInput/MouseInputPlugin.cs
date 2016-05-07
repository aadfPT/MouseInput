using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ODIF;
using ODIF.Extensions;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using RawInput_dll;

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
            const bool captureOnlyInForeground = false;
            using (var curProcess = Process.GetCurrentProcess())
            //using (var curModule = curProcess.MainModule)
            {
                var moduleHandle = curProcess.MainWindowHandle; // Win32.GetModuleHandle(curModule.ModuleName);
                Rawinput = new RawInput(moduleHandle, captureOnlyInForeground);
            }
            Global.HardwareChangeDetected += CheckForControllersEvent;
            CheckForControllers();
        }

        public RawInput Rawinput { get; set; }

        private void CheckForControllers()
        {
            var devices =
                Rawinput.MiceDevices.Where(entry => Devices.All(d => ((MyMouseDevice)d).Device.Key != entry.Key));

            foreach (var device in devices)
            {
                Devices.Add(new MyMouseDevice(Rawinput, device));
            }
        }

        private void CheckForControllersEvent(object sender, EventArrivedEventArgs e)
        {
            CheckForControllers();
        }
    }
}
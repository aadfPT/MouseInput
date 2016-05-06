using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ODIF;
using ODIF.Extensions;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

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
            //Global.HardwareChangeDetected += CheckForControllersEvent;
            //CheckForControllers();
            Devices.Add(new MyMouseDevice());
        }
    }
}
using ODIF;

namespace MouseInput
{
    internal class MouseInputDevice
    {
        public InputChannelTypes.JoyAxis VerticalAxis { get; set; }
        public InputChannelTypes.JoyAxis HorizontalAxis { get; set; }
        public InputChannelTypes.JoyAxis XPosition { get; set; }
        public InputChannelTypes.JoyAxis YPosition { get; set; }
        public InputChannelTypes.JoyAxis XDelta { get; set; }
        public InputChannelTypes.JoyAxis YDelta { get; set; }

        public InputChannelTypes.Button LeftButton { get; set; }
        public InputChannelTypes.Button RightButton { get; set; }

        public InputChannelTypes.Button MiddleMouse { get; set; }
        public InputChannelTypes.Button WheelDown { get; set; }
        public InputChannelTypes.Button WheelUp { get; set; }
        public InputChannelTypes.Button X1 { get; set; }
        public InputChannelTypes.Button X2 { get; set; }
        public OutputChannelTypes.Toggle SupressDeviceEvents { get; set; }

        public MouseInputDevice()
        {
            HorizontalAxis = new InputChannelTypes.JoyAxis("Horizontal Axis", "");//, Properties.Resources._360_Left_Stick.ToImageSource());
            VerticalAxis = new InputChannelTypes.JoyAxis("Vertical Axis", "");//, Properties.Resources._360_Left_Stick.ToImageSource());
            //XPosition = new InputChannelTypes.JoyAxis("Position X", "");//, Properties.Resources._360_Right_Stick.ToImageSource());
            //YPosition = new InputChannelTypes.JoyAxis("Position Y", "");//, Properties.Resources._360_Right_Stick.ToImageSource());
            XDelta = new InputChannelTypes.JoyAxis("Delta X", "");//, Properties.Resources._360_Right_Stick.ToImageSource());
            YDelta = new InputChannelTypes.JoyAxis("Delta Y", "");//, Properties.Resources._360_Right_Stick.ToImageSource());

            LeftButton = new InputChannelTypes.Button("Left button", "");//, Properties.Resources._360_Left_Stick.ToImageSource());
            RightButton = new InputChannelTypes.Button("Right button", "");//, Properties.Resources._360_Right_Stick.ToImageSource());
            MiddleMouse = new InputChannelTypes.Button("Middle button", "");//, Properties.Resources._360_Right_Stick.ToImageSource());
            WheelUp = new InputChannelTypes.Button("Wheel up", "");//, Properties.Resources._360_Right_Stick.ToImageSource());
            WheelDown = new InputChannelTypes.Button("Wheel down", "");//, Properties.Resources._360_Right_Stick.ToImageSource());
            X1 = new InputChannelTypes.Button("X1 button", "");//, Properties.Resources._360_Right_Stick.ToImageSource());
            X2 = new InputChannelTypes.Button("X2 button", "");//, Properties.Resources._360_Right_Stick.ToImageSource());
            SupressDeviceEvents = new OutputChannelTypes.Toggle("Supress device events", "Use this to make windows ignore the device");
        }
    }
}
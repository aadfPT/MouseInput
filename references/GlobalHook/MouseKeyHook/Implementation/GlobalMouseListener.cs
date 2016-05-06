// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System.Windows.Forms;
using Gma.System.MouseKeyHook.WinApi;

namespace Gma.System.MouseKeyHook.Implementation
{
    internal class GlobalMouseListener : MouseListener
    {
        private readonly int m_SystemDoubleClickTime;
        private MouseButtons m_PreviousClicked;
        private Point m_PreviousClickedPosition;
        private int m_PreviousClickedTime;

        public GlobalMouseListener()
            : base(HookHelper.HookGlobalMouse)
        {
            m_SystemDoubleClickTime = MouseNativeMethods.GetDoubleClickTime();
            var HID_USAGE_PAGE_GENERIC = (short) 0x01;
            var HID_USAGE_GENERIC_MOUSE = (short) 0x02;

    //        RAWINPUTDEVICE[] Rid = new
    //        {
    //            new RAWINPUTDEVICE()
    //            {
    //                usUsagePage = HID_USAGE_PAGE_GENERIC,
    //usUsage = HID_USAGE_GENERIC_MOUSE,
    //dwFlags = RIDEV_INPUTSINK,
    //hwndTarget = hWnd
    //            }
    //        };
    //        RegisterRawInputDevices(Rid, 1, sizeof(Rid[0]);
    //case WM_INPUT:
    //        {
    //            UINT dwSize = 40;
    //            static BYTE lpb[40];

    //            GetRawInputData((HRAWINPUT)lParam, RID_INPUT,
    //                            lpb, &dwSize, sizeof(RAWINPUTHEADER));

    //            RAWINPUT* raw = (RAWINPUT*)lpb;

    //            if (raw->header.dwType == RIM_TYPEMOUSE)
    //            {
    //                int xPosRelative = raw->data.mouse.lLastX;
    //                int yPosRelative = raw->data.mouse.lLastY;
    //            }
    //            break;
            }
        }

        protected override void ProcessDown(ref MouseEventExtArgs e)
        {
            if (IsDoubleClick(e))
            {
                e = e.ToDoubleClickEventArgs();
            }
            base.ProcessDown(ref e);
        }

        protected override void ProcessUp(ref MouseEventExtArgs e)
        {
            base.ProcessUp(ref e);
            if (e.Clicks == 2)
            {
                StopDoubleClickWaiting();
            }

            if (e.Clicks == 1)
            {
                StartDoubleClickWaiting(e);
            }
        }

        private void StartDoubleClickWaiting(MouseEventExtArgs e)
        {
            m_PreviousClicked = e.Button;
            m_PreviousClickedTime = e.Timestamp;
            m_PreviousClickedPosition = e.Point;
        }

        private void StopDoubleClickWaiting()
        {
            m_PreviousClicked = MouseButtons.None;
            m_PreviousClickedTime = 0;
            m_PreviousClickedPosition = new Point(0, 0);
        }

        private bool IsDoubleClick(MouseEventExtArgs e)
        {
            return
                e.Button == m_PreviousClicked &&
                e.Point == m_PreviousClickedPosition && // Click-move-click exception, see Patch 11222
                e.Timestamp - m_PreviousClickedTime <= m_SystemDoubleClickTime;
        }

        protected override MouseEventExtArgs GetEventArgs(CallbackData data)
        {
            return MouseEventExtArgs.FromRawDataGlobal(data);
        }
    }
}
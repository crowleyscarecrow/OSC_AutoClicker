using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace OSC_AutoClicker
{
    public class NativeMethod
    {
        [DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void SetCursorPos(Int32 X, Int32 Y);

        [DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 cButtons, Int32 dwExtraInfo);

        [DllImport("USER32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean GetCursorPos(ref POINT pt);

        
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);



        private static readonly Int32 MOUSEEVENTF_LEFTDOWN = 0x2;
        private static readonly Int32 MOUSEEVENTF_LEFTUP = 0x4;

        public static void SetCursor(Int32 X, Int32 Y)
        {
            SetCursorPos(X, Y);
        }

        public static void MouseClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static POINT GetMousePos()
        {
            var point = new POINT();
            GetCursorPos(ref point);
            return new POINT(point.X, point.Y);
        }
        public struct POINT
        {
            public POINT(Int32 _X, Int32 _Y)
            {
                X = _X;
                Y = _Y;
            }
            public Int32 X;
            public Int32 Y;
        }
    }
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OSC_AutoClicker());
        }
    }
}

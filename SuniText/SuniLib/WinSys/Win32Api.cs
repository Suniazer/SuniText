using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SuniText.SuniLib.WinSys
{
    public static class Win32Api
    {

        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_PAINT = 0x000F;
        public const int WM_HOTKEY = 0x0312;

        //判断在此函数被调用时,某个键是处于UP/DOWN状态,及前次调用后,是否按过此键.
        //如果返回值的最高位被置位,那么为DOWN状态
        //如果最低位被置位,那么在前一次调用此函数后,此键被按过
        //否则表示该键没被按过.
        [DllImport("USER32.dll")]
        public static extern short GetAsyncKeyState(int nVirtKey);
        //获取鼠标位置
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPos(ref System.Drawing.Point lpPoint);
        //获取定点颜色
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hDC, int XPos, int YPos);
        //为一个设备创建设备上下文环境
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(string driverName, string deviceName, string output, IntPtr lpinitData);
        //删除指定的设备上下文环境
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr DC);
        [DllImport("gdi32.dll")]
        public static extern bool TextOutA(IntPtr hdc, int x, int y, StringBuilder lpString, int c);
        //注册热键
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        //取消注册热键
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        //释放对象
        [DllImport("gdi32")]

        public static extern int DeleteObject(IntPtr o);
    }
}

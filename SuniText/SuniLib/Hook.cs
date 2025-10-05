using Microsoft.VisualBasic;
using SuniText.SuniLib.WinSys;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows;
using SuniText.SuniClass;

namespace SuniText.SuniLib
{
    public class Hook
    {

        ClipboardNotification ClipNotifi = new ClipboardNotification();
 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookDelegate lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        private delegate int HookDelegate(int nCode, int wParam, IntPtr lParam);
        private readonly HookDelegate KeyboardHookDelegate;
        private readonly int hHook;
        private readonly string Item = string.Empty;
        private bool Copy, Alt = false;
        private readonly Editor Owner;
        private readonly SuniFrm.Mode Mode;
        
        [StructLayout(LayoutKind.Sequential)] //声明键盘钩子的封送结构类型
        public struct KeyboardHookStruct
        {
            public int vkCode, scanCode, flags, time, dwExtraInfo;
            //表示一个在1到254间的虚似键盘码 
            //表示硬件扫描码 
        }

        public Hook(Editor owner)
        {
            Owner = owner; Mode = Owner.Mode;
            //SetHook
            ProcessModule cModule = Process.GetCurrentProcess().MainModule;
            IntPtr mh = GetModuleHandle(cModule.ModuleName);
            KeyboardHookDelegate = new HookDelegate(KeyboardHookProc);
            hHook = SetWindowsHookEx(Win32Api.WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);
            ClipNotifi.ClipboardUpdate += () =>
            {
                if (Copy)
                {
                    Owner.MainText += $"\n{(Mode.GetClip ? AddItemFromClipBoard() : Owner.KeyText)}";
                    Owner.SendMail();
                }
            };
        }
        public void UnHook()
        {
            _ = UnhookWindowsHookEx(hHook);
        }

        /// <summary>Win32中，WPARAM被定义为UINT，而LPARAM被定义为LONG</summary>
        private int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (Mode.LockMode) { return CallNextHookEx(hHook, nCode, wParam, lParam); }
            if (nCode >= 0)
            {
                KeyboardHookStruct KeyDataHooked = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                Key key = KeyInterop.KeyFromVirtualKey(KeyDataHooked.vkCode);
                if (wParam == Win32Api.WM_KEYUP || wParam == Win32Api.WM_SYSKEYUP)//OnKeyUpEvent != null &&
                {
                    if ((key == Key.LeftCtrl || key == Key.RightCtrl) ) 
                    {
                        if (Copy)
                        {
                            if (!Mode.GetClip)
                            {
                                Owner.MainText += $"\n{Owner.KeyText}";
                            }
                        }
                    }
                    Alt = Copy = false;
                }
                if(key == Key.RWin || key == Key.LWin)
                {
                    if (Alt)
                    {
                        Owner.Visibility = Owner.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                        Owner.Sticky_.Visibility = Visibility.Visible;
                    }
                }
                if (wParam == Win32Api.WM_KEYDOWN || wParam == Win32Api.WM_SYSKEYDOWN)//OnKeyDownEvent != null &&
                {
                    if (key == Key.C || key == Key.X) { Copy = true; }
                    if (key == Key.LeftAlt || key == Key.RightAlt) { Alt = true; }
                }
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        private string AddItemFromClipBoard()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipText = Clipboard.GetText();
                    if (Mode.EditClip)
                    {
                        if (Mode.DelCrLf)
                        {
                            clipText = string.Join(
                                Mode.DelAll ? string.Empty : Text.Separator.CrLf,
                                clipText.Split(new string[] { Text.Separator.CrLf }, 
                                StringSplitOptions.RemoveEmptyEntries));
                        }
                        if (Mode.DelWhiteSpace)
                        {
                            clipText = Regex.Replace(clipText, @"\s+", Mode.DelAll ? string.Empty : " ", RegexOptions.Compiled);
                        }
                        if (Mode.ENUS)
                        {
                            clipText = Mode.ToUpperCase ? clipText.ToUpper() : Mode.ToLowerCase ? clipText.ToLower()
                                : Mode.ToProperCase ? Strings.StrConv(clipText, VbStrConv.ProperCase) : @clipText;
                        }
                        if (Mode.ZHCN)
                        {
                            if (Mode.ToTraditional) { clipText = Strings.StrConv(clipText, VbStrConv.TraditionalChinese); }
                            else if (Mode.ToSimplified) { clipText = Strings.StrConv(clipText, VbStrConv.SimplifiedChinese); }
                            else if (Mode.ToPinyin) { clipText = Text.Processor.ChnToPh.ConvertCh(clipText); }
                        }
                        if (Mode.ByRegex)
                        {
                            if (Mode.RegexGet)
                            {
                                Regex mRegex = new Regex(Mode.Pattern);
                                MatchCollection mc = mRegex.Matches(clipText);
                                clipText = string.Empty;
                                if(mc.Count == 0) { return string.Empty; }
                                foreach (Match m in mc)
                                {
                                    clipText += m.Value + Text.Separator.CrLf;
                                }
                            }
                            else if (Mode.RegexDel)
                            {
                                clipText = Regex.Replace(clipText, @Mode.Pattern, string.Empty, RegexOptions.Compiled);
                            }
                        }
                        if (Mode.ReSetClipBoard) { Clipboard.SetText(clipText); }
                    }
                    return Text.Separator.CrLf + clipText.Trim();
                }
                else if (Clipboard.ContainsImage())
                {
                    try
                    {
                        Image.Converter.SaveBmpImgByMD5(Image.Converter.BmpSrc2BmpImg(Clipboard.GetImage()));
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    System.Collections.Specialized.StringCollection ClipFileList = Clipboard.GetFileDropList();
                    string[] files = new string[ClipFileList.Count];
                    ClipFileList.CopyTo(files, 0);
                    return Text.Separator.CrLf + string.Join(Text.Separator.CrLf, files);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return string.Empty;
        }
    }
}
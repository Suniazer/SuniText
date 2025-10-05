using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using static System.Environment;

namespace SuniText.SuniClass
{
    public static class Pairs
    {
        public static string AppName { get; } = "SuniText";
        public static Dictionary<string, string> Get(object obj)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var type = obj.GetType();
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                dict.Add(prop.Name, prop.GetValue(obj) as string);
            }
            return dict;
        }
        public static class Pattern
        {
            public class String
            {
                public static string WhiteSpace { get; } = @"\s+";
                public static string IPV4 { get; } = @"(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}";
                public static string IPV6 { get; } = @"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))";
                public static string ASCII { get; } = @"[-~]";
                public static string MAC { get; } = @"[a-fA-F0-9]{2}(:[a-fA-F0-9]{2}){5}";
                public static string URL { get; } = @"(http[s]{0,1}\:\/\/){0,1}([\w\:\%\-\?\=\#]{1,}[.\/]{0,}){2,}";
                public static string Torrent { get; } = @"(magnet\:\?xt\=urn\:btih\:){0,1}([a-zA-Z0-9]{40}|[a-zA-Z0-9]{32}){1}([\w\&\=\%\.\-]+){0,1}";
                public static string Email { get; } = @"[\w.]{1,}@([0-9a-zA-Z]{1,}[.]{0,1}){2}";
                public static string 提取 { get; } = "(?<=)(.*?)(?=)";
            }
            public static string 提取(string start, string end)
            {
                return $"(?<={start})(.*?)(?={end})";
            }
        }
        public static class Theme
        {
            public static byte Dark { get; } = 0;
            public static byte Light { get; } = 1;
            public static byte System { get; } = 2;
        }
        public static class VirtualKey
        {
            //Modifiers:
            public const uint MOD_NONE = 0x0000; //(none)
            public const uint MOD_ALT = 0x0001; //ALT
            public const uint MOD_CONTROL = 0x0002; //CTRL
            public const uint MOD_CONTROL_ALT = 0x0003; //ctrl+alt
            public const uint MOD_SHIFT = 0x0004; //SHIFT
            public const uint MOD_SHIFT_ALT = 0x0005; //alt+shift
            public const uint MOD_CONTROL_SHIFT = 0x0006; //ctrl+shift
            public const uint MOD_CONTROL_SHIFT_ALT = 0x0007; //ctrl+shift+alt
            public const uint MOD_WIN = 0x0008; //WINDOWS
                                                //VK
            public const uint VK_LBUTTON = 0x1;//鼠标左键
            public const uint VK_RBUTTON = 0x2;//鼠标右键
            public const uint VK_CANCEL = 0x3;//Ctrl-Break键
            public const uint VK_MBUTTON = 0x4;//鼠标中键
            public const uint VK_BACK = 0x8;//Backspace键
            public const uint VK_TAB = 0x9;//Tab键
            public const uint VK_CLEAR = 0x0C;//Clear键
            public const uint VK_RETURN = 0x0D;//Enter键
            public const uint VK_SHIFT = 0x10;//Shift键
            public const uint VK_CONTROL = 0x11;//Ctrl键
            public const uint VK_MENU = 0x12;//Alt键
            public const uint VK_PAUSE = 0x13;//Pause键
            public const uint VK_CAPITAL = 0x14;//Caps Lock键
            public const uint VK_ESCAPE = 0x1B;//Esc键
            public const uint VK_SPACE = 0x20;//Space键
            public const uint VK_PRIOR = 0x21;//Page Up键
            public const uint VK_NEXT = 0x22;//Page Down键
            public const uint VK_END = 0x23;//End键
            public const uint VK_HOME = 0x24;//Home键
            public const uint VK_LEFT = 0x25;//←键
            public const uint VK_UP = 0x26;//↑键
            public const uint VK_RIGHT = 0x27;//→键
            public const uint VK_DOWN = 0x28;//↓键
            public const uint VK_SELECT = 0x29;//Select键
            public const uint VK_PRINT = 0x2A;//Print键
            public const uint VK_EXECUTE = 0x2B;//Execute键
            public const uint VK_SNAPSHOT = 0x2C;//Print Screen键
            public const uint VK_INSERT = 0x2D;//Ins键
            public const uint VK_DELETE = 0x2E;//Del键
            public const uint VK_HELP = 0x2F;//Help键
            public const uint VK_0 = 0x30;//0键
            public const uint VK_1 = 0x31;//1键
            public const uint VK_2 = 0x32;//2键
            public const uint VK_3 = 0x33;//3键
            public const uint VK_4 = 0x34;//4键
            public const uint VK_5 = 0x35;//5键
            public const uint VK_6 = 0x36;//6键
            public const uint VK_7 = 0x37;//7键
            public const uint VK_8 = 0x38;//8键
            public const uint VK_9 = 0x39;//9键
            public const uint VK_A = 0x41;//A键
            public const uint VK_B = 0x42;//B键
            public const uint VK_C = 0x43;//C键
            public const uint VK_D = 0x44;//D键
            public const uint VK_E = 0x45;//E键
            public const uint VK_F = 0x46;//F键
            public const uint VK_G = 0x47;//G键
            public const uint VK_H = 0x48;//H键
            public const uint VK_I = 0x49;//I键
            public const uint VK_J = 0x4A;//J键
            public const uint VK_K = 0x4B;//K键
            public const uint VK_L = 0x4C;//L键
            public const uint VK_M = 0x4D;//M键
            public const uint VK_N = 0x4E;//N键
            public const uint VK_O = 0x4F;//O键
            public const uint VK_P = 0x50;//P键
            public const uint VK_Q = 0x51;//Q键
            public const uint VK_R = 0x52;//R键
            public const uint VK_S = 0x53;//S键
            public const uint VK_T = 0x54;//T键
            public const uint VK_U = 0x55;//U键
            public const uint VK_V = 0x56;//V键
            public const uint VK_W = 0x57;//W键
            public const uint VK_X = 0x58;//X键
            public const uint VK_Y = 0x59;//Y键
            public const uint VK_Z = 0x5A;//Z键
            public const uint VK_LWIN = 0x5B;//左Windows键
            public const uint VK_RWIN = 0x5C;//右Windows键
            public const uint VK_APPS = 0x5D;//应用程序键
            public const uint VK_SLEEP = 0x5F;//休眠键
            public const uint VK_NUMPAD0 = 0x60;//小数字键盘0键
            public const uint VK_NUMPAD1 = 0x61;//小数字键盘1键
            public const uint VK_NUMPAD2 = 0x62;//小数字键盘2键
            public const uint VK_NUMPAD3 = 0x63;//小数字键盘3键
            public const uint VK_NUMPAD4 = 0x64;//小数字键盘4键
            public const uint VK_NUMPAD5 = 0x65;//小数字键盘5键
            public const uint VK_NUMPAD6 = 0x66;//小数字键盘6键
            public const uint VK_NUMPAD7 = 0x67;//小数字键盘7键
            public const uint VK_NUMPAD8 = 0x68;//小数字键盘8键
            public const uint VK_NUMPAD9 = 0x69;//小数字键盘9键
            public const uint VK_MULTIPLY = 0x6A;//乘号键
            public const uint VK_ADD = 0x6B;//加号键
            public const uint VK_SEPARATOR = 0x6C;//分割键
            public const uint VK_SUBSTRACT = 0x6D;//减号键
            public const uint VK_DECIMAL = 0x6E;//小数点键
            public const uint VK_DIVIDE = 0x6F;//除号键
            public const uint VK_F1 = 0x70;//F1键
            public const uint VK_F2 = 0x71;//F2键
            public const uint VK_F3 = 0x72;//F3键
            public const uint VK_F4 = 0x73;//F4键
            public const uint VK_F5 = 0x74;//F5键
            public const uint VK_F6 = 0x75;//F6键
            public const uint VK_F7 = 0x76;//F7键
            public const uint VK_F8 = 0x77;//F8键
            public const uint VK_F9 = 0x78;//F9键
            public const uint VK_F10 = 0x79;//F10键
            public const uint VK_F11 = 0x7A;//F11键
            public const uint VK_F12 = 0x7B;//F12键
            public const uint VK_F13 = 0x7C;//F13键
            public const uint VK_F14 = 0x7D;//F14键
            public const uint VK_F15 = 0x7E;//F15键
            public const uint VK_F16 = 0x7F;//F16键
            public const uint VK_F17 = 0x80;//F17键
            public const uint VK_F18 = 0x81;//F18键
            public const uint VK_F19 = 0x82;//F19键
            public const uint VK_F20 = 0x83;//F20键
            public const uint VK_F21 = 0x84;//F21键
            public const uint VK_F22 = 0x85;//F22键
            public const uint VK_F23 = 0x86;//F23键
            public const uint VK_F24 = 0x87;//F24键
            public const uint VK_NUMLOCK = 0x90;//Num Lock键
            public const uint VK_SCROLL = 0x91;//Scroll Lock键
            public const uint VK_LSHIFT = 0xA0;//左Shift键
            public const uint VK_RSHIFT = 0xA1;//右Shift键
            public const uint VK_LCONTROL = 0xA2;//左Ctrl键
            public const uint VK_RCONTROL = 0xA3;//右Ctrl键
            public const uint VK_LMENU = 0xA4;//左Alt键
            public const uint VK_RMENU = 0xA5;//右Alt键
        }
        public static class FileFormat
        {
            public static string Text { get; } = ".txt";
            public static string Wav { get; } = ".wav";
            public static string Exe { get; } = ".exe";
            public static string Jpeg { get; } = ".jpg";
        }
        public static class DataFolder
        {
            private static int IoCount = 0;
            private static string FileNameDateTime()
            {
                Interlocked.Exchange(ref IoCount, ++IoCount);
                return $"{SuniLib.WinSys.Def.Now(false)}_{$"{DateTime.Now}{IoCount}".GetHashCode()}";
            }
            public static void CreateFolder()
            {
                if (!Directory.Exists(LocalDataFolder)) { MessageBox.Show("需要重启", "提示", MessageBoxButton.OK); }
                SuniLib.WinSys.Path.CreateFolder(folder: LocalDataFolder);
                SuniLib.WinSys.Path.CreateFolder(folder: SuniTextFolder);
                SuniLib.WinSys.Path.CreateFolder(folder: TempFolder);
                SuniLib.WinSys.Path.CreateFolder(folder: HideTrailFolder);
                SuniLib.WinSys.Path.CreateFolder(folder: TextInputFolder);
                SuniLib.WinSys.Path.CreateFolder(folder: AutoSaveFolder);
                SuniLib.WinSys.Path.CreateFolder(folder: RegexFolder);
                SuniLib.WinSys.Path.CreateFolder(folder: TtsRawFolder);
                SuniLib.WinSys.Path.CreateFolder(folder: ImageFolder);
            }
            public static string LocalDataFolder { get; } = GetFolderPath(SpecialFolder.MyDocuments);
            public static string SuniTextFolder { get; } = Path.Combine(LocalDataFolder, AppName);
            public static string TempFolder { get; } = Path.Combine(SuniTextFolder, "Temp");
            public static string TempApp { get; } = Path.Combine(TempFolder, "Temp");
            public static string HideTrailFolder { get; } = Path.Combine(TempFolder, "HideFile_Trail");
            public static string HideTrailFile { get { return Path.Combine(HideTrailFolder, $"{FileNameDateTime()}{FileFormat.Text}"); } }
            public static string TextInputFolder { get; } = Path.Combine(TempFolder, "TextInput_Trail");
            public static string InputTrailFile { get { return Path.Combine(TextInputFolder, $"{FileNameDateTime()}{FileFormat.Text}"); } }
            public static string AutoSaveFolder { get; } = Path.Combine(TempFolder, "AutoSave_Trail");
            public static string AutoSaveFile { get { return Path.Combine(AutoSaveFolder, $"{FileNameDateTime()}{FileFormat.Text}"); } }
            public static string RegexFolder { get; } = Path.Combine(TempFolder, "Regex_Trail");
            public static string RegexFile { get { return Path.Combine(RegexFolder, $"{FileNameDateTime()}{FileFormat.Text}"); } }
            public static string TtsRawFolder { get; } = Path.Combine(TempFolder, "TtsRaw_Trail");
            public static string TtsRawFile { get { return Path.Combine(TtsRawFolder, $"{FileNameDateTime()}{FileFormat.Wav}"); } }
            public static string ImageFolder { get; } = Path.Combine(TempFolder, "Image_Trail");
            public static string ImageFile { get { return Path.Combine(ImageFolder, $"{FileNameDateTime()}{FileFormat.Jpeg}"); } }
        }
    }
}

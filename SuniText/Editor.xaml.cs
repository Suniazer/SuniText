using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Web;
using System.ComponentModel;
using System.Linq;
using SuniText.SuniClass;
using SuniText.SuniLib;
using Pair = SuniText.SuniClass.Pairs;
using Picker = SuniText.SuniLib.Image.Color;
using DataIO = SuniText.SuniClass.DataIO;
using SuniText.SuniFrm;
using System.Threading.Tasks;
using static SuniText.SuniClass.Pairs;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;
using Windows.ApplicationModel;


namespace SuniText
{
    /// <summary>
    /// Editor.xaml 的交互逻辑
    /// </summary>
    public partial class Editor : Window, INotifyPropertyChanged
    {
        private void UserPreferenceChanged_Theme(object sender, UserPreferenceChangedEventArgs e)
        {
            const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string RegistryValueName = "AppsUseLightTheme";
            object result;
            if ((result = Registry.CurrentUser.OpenSubKey(RegistryKeyPath)?.GetValue(RegistryValueName)) is null)
            {
                result = Registry.LocalMachine.OpenSubKey(RegistryKeyPath)?.GetValue(RegistryValueName);
            }
            IsDark = result is int theme && theme == 0;
            RefreshTheme();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            return;
        }
        protected override void OnStateChanged(EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
                case WindowState.Normal:
                    break;
                case WindowState.Minimized:
                    break;
                default:
                    break;
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }
        private void PickColor()
        {
            while (true)
            {
                if (Mode.GetClip) { Thread.Sleep(100); continue; }
                Picker.ColorByPoint(out Color c, out Color nc, out System.Drawing.Point p);
                byte Cyan_ = (byte)(255 - c.R);
                byte Magenta_ = (byte)(255 - c.G);
                byte Yellow_ = (byte)(255 - c.B);
                byte K = (new byte[] { Cyan_, Magenta_, Yellow_ }).Min();
                string rgb = $"rgb({c.R},{c.G},{c.B}) ";
                string hex = $"#{c.R:x2}{c.G:x2}{c.B:x2} ";
                string cmyk = $"cmyk({Cyan_ - K},{Magenta_ - K},{Yellow_ - K},{K})";
                string point = $"p({p.X},{p.Y}) ";
                string info = string.Format("{0}{1}{2}{3}",
                    Mode.GetRGB ? rgb : string.Empty,
                    Mode.GetHEX ? hex : string.Empty,
                    Mode.GetCMYK ? cmyk : string.Empty,
                    Mode.GetPoint ? point : string.Empty).ToUpper();
                Action GetColorInfo = () =>
                {
                    TBox_Key.Text = info;
                    TBox_Key.Foreground = new SolidColorBrush(nc);
                    TBox_Key.Background = new SolidColorBrush(c);
                }; _ = Dispatcher.BeginInvoke(GetColorInfo);
            }
        }
 
        /// <summary>
        ///  向目标路径创建指定文件的快捷方式
        /// </summary>
        /// <param name="directory">目标目录</param>
        /// <param name="shortcutName">快捷方式名字</param>
        /// <param name="targetPath">文件完全路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标地址</param>
        /// <returns>成功或失败</returns>
        private bool CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
        {
            try
            {
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);                         //目录不存在则创建
                //添加引用 Com 中搜索 Windows Script Host Object Model
                string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));          //合成路径
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);    //创建快捷方式对象
                shortcut.TargetPath = targetPath;                                                               //指定目标路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);                                  //设置起始位置
                shortcut.WindowStyle = 1;                                                                       //设置运行方式，默认为常规窗口
                shortcut.Description = description;                                                             //设置备注
                shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;    //设置图标路径
                shortcut.Save();                                                                                //保存快捷方式
                return true;
            }
            catch (Exception ex)
            {
                string temp = ex.Message;
                temp = "";
            }
            return false;
        }
        public Editor()
        {
            InitializeComponent();
            DataContext = this;
            try
            {
                SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(UserPreferenceChanged_Theme);
                UserPreferenceChanged_Theme(null, null);
                Mode = new Mode();
                Hook = new Hook(this);
                Pick = new Thread(() => PickColor());
                Pick.Start();
            }
            catch (Exception ex) {MessageBox.Show(ex.Message); }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Sticky_.Owner = this;
            Sticky_.Show();
            RequestStartup();
            Visibility = Visibility.Hidden;
            DataIO.Init();
        }
        async void RequestStartup()
        {
            //string systemStartPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            //string appAllPath = Process.GetCurrentProcess().MainModule.FileName;
            //CreateShortcut(systemStartPath, "SuniText", appAllPath, "文本编辑器");
            //RegistryKey RKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            //RKey.SetValue(AppDomain.CurrentDomain.FriendlyName.Replace(Pairs.FileFormat.Exe, ""), AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName);
            StartupTask startupTask = await StartupTask.GetAsync("SuniText");
            if (startupTask.State == StartupTaskState.Disabled || startupTask.State == StartupTaskState.DisabledByPolicy || startupTask.State == StartupTaskState.DisabledByUser)
            {
                if (MessageBox.Show($"授予自启动权限", "提示", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) != MessageBoxResult.Yes) { return; }
                var result = await startupTask.RequestEnableAsync();
                MessageBox.Show(result == StartupTaskState.Enabled ? "成功" : "失败", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void TBox_Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Enter)
            {
                if (LastText.Contains(MainText)) { return; }
                SendMail();
                DataIO.Backup(MainText);
                LastText = MainText;
            }
            TBox_Main.ScrollToEnd();
        }
        string LastText = string.Empty;
        private volatile string MainText_ = string.Empty;
        public string MainText
        {
            get => MainText_;
            set
            {
                MainText_ = value;
                RaisePropertyChanged("MainText");
            }
        }
        private volatile string KeyText_ = string.Empty;
        public string KeyText
        {
            get => KeyText_;
            set
            {
                KeyText_ = value;
                RaisePropertyChanged("KeyText");
            }
        }
        private readonly Thread Pick;
        public Mode Mode;
        private volatile Hook Hook;
        private bool SearchMode = true;
        private bool IsSaved = false;
        private bool IsSendToMail = false;
        private volatile bool IsAutoSave = true;
        //private volatile bool IsInputting = false;
        private bool IsDark = false;
        private int index = 0;
        TextFile TFile = new TextFile();
        public Sticky Sticky_ = new Sticky();
        public void RefreshTheme()
        {
            Application.Current.Resources["Brush_Main_Background"] = Application.Current.Resources[IsDark ? "Brush_DarkMain_Background" : "Brush_LightMain_Background"];
            Application.Current.Resources["Brush_TBox_Background"] = Application.Current.Resources[IsDark ? "Brush_DarkTBox_Background" : "Brush_LightTBox_Background"];
            Application.Current.Resources["Brush_TBox_Foreground"] = Application.Current.Resources[IsDark ? "Brush_DarkTBox_Foreground" : "Brush_LightTBox_Foreground"];
            Application.Current.Resources["Brush_Menu_Background"] = Application.Current.Resources[IsDark ? "Brush_DarkMenu_Background" : "Brush_LightMenu_Background"];
            Application.Current.Resources["Brush_Menu_Foreground"] = Application.Current.Resources[IsDark ? "Brush_DarkMenu_Foreground" : "Brush_LightMenu_Foreground"];
            Application.Current.Resources["Brush_MouseOver_Background"] = Application.Current.Resources[IsDark ? "Brush_DarkMouseOver_Background" : "Brush_LightMouseOver_Background"];
            Application.Current.Resources["ArrowOpenBrush"] = Application.Current.Resources[IsDark ? "DarkArrowOpenBrush" : "LightArrowOpenBrush"];
            Application.Current.Resources["ArrowBrush"] = Application.Current.Resources[IsDark ? "DarkArrowBrush" : "LightArrowBrush"];
            Background = (Brush)Application.Current.Resources["Brush_Main_Background"];
            MenuPanel.Background = (Brush)Application.Current.Resources["Brush_Menu_Background"];
            MainMenu.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            Btn_Min.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            Btn_Exit.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            TBox_Key.Background = (Brush)Application.Current.Resources["Brush_TBox_Background"];
            TBox_Key.Foreground = (Brush)Application.Current.Resources["Brush_TBox_Foreground"];
            TBox_Main.Background = (Brush)Application.Current.Resources["Brush_TBox_Background"];
            TBox_Main.Foreground = (Brush)Application.Current.Resources["Brush_TBox_Foreground"];
            TBlk_LenTBox.Foreground = (Brush)Application.Current.Resources["Brush_TBox_Foreground"];
            Lab_Board.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            Bd_Main.BorderBrush = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            Lab_Last.Foreground = (Brush)Application.Current.Resources["Brush_TBox_Foreground"];
            Lab_Next.Foreground = (Brush)Application.Current.Resources["Brush_TBox_Foreground"];
            TBlk_sMode.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            if (IsAutoSave) { TBlk_LenTBox.Foreground = Brushes.Orange; }
            LoadBtnImg(Img_Clear);
            LoadBtnImg(Img_SaveAs);
            LoadBtnImg(Img_Save);
            LoadBtnImg(Img_ViewBackUp);
            LoadBtnImg(Img_Symbol);
            LoadBtnImg(Img_Enter);
            LoadBtnImg(Img_Replace);
            LoadBtnImg(Img_Open);
            LoadBtnImg(Img_New);
            Mode?.RefreshTheme();
            Sticky_.RefreshTheme();
        }
        private void LoadBtnImg(Image img)
        {
            img.Source = SuniLib.Image.Converter.ResBmp2BmpSrc(img.Name.ToLower().Replace("img_", string.Empty) + (IsDark ? "_d" : "_n"));
        }
        private void Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            string name = btn.Name.ToLower().Replace("btn_", string.Empty) + (IsDark ? "_l_d" : "_l_n");
            ((Image)btn.Content).Source = SuniLib.Image.Converter.ResBmp2BmpSrc(name);
        }
        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            string name = btn.Name.ToLower().Replace("btn_", string.Empty) + (IsDark ? "_d" : "_n");
            ((Image)btn.Content).Source = SuniLib.Image.Converter.ResBmp2BmpSrc(name);
        }
        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            SendMail();
            DataIO.Backup(MainText);
            IsSaved = true;
            TBox_Key.Text = string.Empty;
            TBox_Main.Text = string.Empty;
            RefreshTheme();
        }
        private void Btn_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TBox_Main.Text)) { return; }
            string fp = SuniLib.WinSys.Path.GetSavePath();
            if (!File.Exists(fp)) { return; }
            File.WriteAllText(fp, TBox_Main.Text);
            IsSaved = true;
        }
        private void Btn_ViewBackUp_Click(object sender, RoutedEventArgs e)
        {
            DataIO.OpenTempFolder();
        }
        private void Btn_Symbol_Click(object sender, RoutedEventArgs e)
        {
            SuniLib.WinSys.Path.SelectFile(TFile.Path);
        }
        private void Btn_Enter_Click(object sender, RoutedEventArgs e)
        {
            if (!Clipboard.ContainsText() && string.IsNullOrWhiteSpace(Clipboard.GetText())) { return; }
            try
            {
                TBox_Main.AppendText($"\r\n{Clipboard.GetText()}");
                TBox_Main.ScrollToEnd();
            }
            catch {; }
        }
        private void Btn_Replace_Click(object sender, RoutedEventArgs e)
        {
            string txtKey = TBox_Key.Text.Replace("^p", "\r\n").Replace("^|", SuniLib.Text.Separator.Replace.ToString());
            if (txtKey.IndexOf('|') == -1) { return; }
            if (string.IsNullOrEmpty(TBox_Main.Text)) { return; }
            string[] key = txtKey.Split('|');
            TBox_Main.Text = TBox_Main.Text.Replace(@key[0].Replace(SuniLib.Text.Separator.Replace, '|'), @key[1].Replace(SuniLib.Text.Separator.Replace, '|'));
        }
        private void Btn_Open_Click(object sender, RoutedEventArgs e)
        {
            if (IsAutoSave) { goto next; }
            if (!IsSaved)
            {
                bool NotSave = MessageBox.Show("不保存已有的更改?", "警告", MessageBoxButton.OKCancel) != MessageBoxResult.OK;
                if (!NotSave)
                {
                    string sfp = SuniLib.WinSys.Path.GetSavePath();
                    if (string.IsNullOrEmpty(sfp)) { return; }
                    File.WriteAllText(sfp, TBox_Main.Text);
                }
            }
            next: string fp = SuniLib.WinSys.Path.GetFilePath();
            if (string.IsNullOrEmpty(fp)) { return; }
            TBox_Main.Text = File.ReadAllText(fp);
        }
        private void Btn_New_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved)
            {
                bool delete = MessageBox.Show("不保存已有的更改?", "警告", MessageBoxButton.OKCancel) != MessageBoxResult.OK;
                if (!delete)
                {
                    if (!SuniLib.WinSys.Path.FillFilePath(ref TFile)) { return; }
                    File.WriteAllText(TFile.Path, TBox_Main.Text);
                    TBox_Main.Clear();
                    IsSaved = true;
                    return;
                }
            }
            SaveFileDialog saveFDialog = new SaveFileDialog
            {
                Filter = "文本文件|*.txt",
                Title = "新建文本文件"
            };
            if ((bool)saveFDialog.ShowDialog())
            {
                TFile.Path = saveFDialog.FileName;
                Lab_Board.ToolTip = TFile.Path;
                FileInfo info = new FileInfo(TFile.Path);
                string oldBoard = Lab_Board.Text;
                Lab_Board.Text = string.IsNullOrEmpty(TFile.Path) ? $"{info.Name} {oldBoard}" : oldBoard.Replace(TFile.Name, info.Name);
                TFile.Name = info.Name;
                TFile.Parent = info.Directory.FullName;
                IsSaved = false;
            }
        }
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!SuniLib.WinSys.Path.FillFilePath(ref TFile)) { return; }
            File.AppendAllText(TFile.Path, TBox_Main.Text);
        }
        private void SetUp_Theme_Click(object sender, RoutedEventArgs e)
        {
            IsDark = !IsDark;
            RefreshTheme();
        }
        private void SetUp_LockWidth_Click(object sender, RoutedEventArgs e)
        {
            bool resize = ResizeMode == ResizeMode.NoResize;
            ResizeMode = resize ? ResizeMode.CanResize : ResizeMode.NoResize;
            Lab_Board.Text = resize ? Lab_Board.Text.Replace(" 定宽", "") : $"{Lab_Board.Text} 定宽";
        }
        private void SetUp_ResetWidth_Click(object sender, RoutedEventArgs e)
        {
            Width = 335;
            RefreshTheme();
        }
        private void SetUp_TopMost_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
            Lab_Board.Text = Topmost ? $"{Lab_Board.Text} 置顶" : Lab_Board.Text.Replace(" 置顶", "");
        }
        private void SetUp_Mode_Click(object sender, RoutedEventArgs e)
        {
            Mode.Owner = this;
            Mode.ShowDialog();
        }
        private void File_NewFile_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved)
            {
                bool delete = MessageBox.Show("不保存已有的更改?", "警告", MessageBoxButton.OKCancel) != MessageBoxResult.OK;
                if (!delete)
                {
                    if (!SuniLib.WinSys.Path.FillFilePath(ref TFile)) { return; }
                    File.WriteAllText(TFile.Path, TBox_Main.Text);
                    TBox_Main.Clear();
                    IsSaved = true;
                    return;
                }
            }
            SaveFileDialog saveFDialog = new SaveFileDialog
            {
                Filter = "文本文件|*.txt",
                Title = "新建文本文件"
            };
            if ((bool)saveFDialog.ShowDialog())
            {
                TFile.Path = saveFDialog.FileName;
                Lab_Board.ToolTip = TFile.Path;
                FileInfo info = new FileInfo(TFile.Path);
                string oldBoard = Lab_Board.Text;
                Lab_Board.Text = string.IsNullOrEmpty(TFile.Path) ? $"{info.Name} {oldBoard}" : oldBoard.Replace(TFile.Name, info.Name);
                TFile.Name = info.Name;
                TFile.Parent = info.Directory.FullName;
                IsSaved = false;
            }
        }
        private void File_AutoSave_Click(object sender, RoutedEventArgs e)
        {
            IsAutoSave = !IsAutoSave;
            TBlk_LenTBox.Foreground = IsAutoSave ? Brushes.Orange : (Brush)Application.Current.Resources["TBoxFgBrush"];
            Lab_Board.Text = IsAutoSave ? $"{Lab_Board.Text} 自动" : Lab_Board.Text.Replace(" 自动", "");
            RefreshTheme();
        }
        private void File_FileDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SuniLib.WinSys.Path.OpenFolder(TFile.Parent);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void File_History_Click(object sender, RoutedEventArgs e)
        {
            DataIO.OpenTempFolder();
        }
        private void Edit_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.Focus();
            TBox_Main.SelectAll();
        }
        private void Edit_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (TBox_Main.IsSelectionActive)
            {
                try
                {
                    Clipboard.SetText(TBox_Main.SelectedText);
                }
                catch (Exception) {; }
            }
        }
        private void Edit_Cut_Click(object sender, RoutedEventArgs e)
        {
            if (TBox_Main.IsSelectionActive)
            {
                try
                {
                    Clipboard.SetText(TBox_Main.SelectedText);
                    TBox_Main.SelectedText = string.Empty;
                }
                catch (Exception) {; }
            }
        }
        private void Edit_Paste_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.SelectedText = Clipboard.GetText();
        }
        private void Edit_ClearAll_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.Clear();
            TBox_Key.Clear();
            RefreshTheme();
        }
        private void Text_Focus_Start_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.SelectionStart = 0;
        }
        private void Text_Focus_25_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.SelectionStart = (int)(TBox_Main.Text.Length * 0.25);
        }
        private void Text_Focus_Mid_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.SelectionStart = TBox_Main.Text.Length / 2;
        }
        private void Text_Focus_75_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.SelectionStart = (int)(TBox_Main.Text.Length * 0.75);
        }
        private void Text_Focus_End_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.SelectionStart = TBox_Main.Text.Length;
        }
        private void Text_Font_Bigger_Click(object sender, RoutedEventArgs e)
        {
            if (TBox_Main.FontSize > 100) { return; }
            TBox_Main.FontSize += 1;
        }
        private void Text_Font_Resize_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.FontSize = 12;
        }
        private void Text_Font_Smaller_Click(object sender, RoutedEventArgs e)
        {
            if (TBox_Main.FontSize < 8) { return; }
            TBox_Main.FontSize -= 1;
        }
        private void Text_Ex_Aa_Click(object sender, RoutedEventArgs e)
        {
            string text = TBox_Main.SelectedText;
            if (string.IsNullOrEmpty(text)) { return; }
            MessageBoxResult dlgResult = MessageBox.Show("? 小写 : 全部大写 : 首字母大写", "大小写转换引导", MessageBoxButton.YesNoCancel);
            switch (dlgResult)
            {
                case MessageBoxResult.Yes:
                    text = Strings.StrConv(text, VbStrConv.Lowercase);
                    break;
                case MessageBoxResult.No:
                    text = Strings.StrConv(text, VbStrConv.Uppercase);
                    break;
                case MessageBoxResult.Cancel:
                    text = Strings.StrConv(text, VbStrConv.ProperCase);
                    break;
                default:
                    break;
            }
            TBox_Main.SelectedText = text;
        }
        private void Text_Ex_St_Click(object sender, RoutedEventArgs e)
        {
            string text = TBox_Main.SelectedText;
            if (string.IsNullOrEmpty(text)) { return; }
            bool dlgResult = MessageBox.Show("? 繁体 : 简体", "繁简体转换引导", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            TBox_Main.SelectedText = dlgResult ? Strings.StrConv(text, VbStrConv.TraditionalChinese) : Strings.StrConv(text, VbStrConv.SimplifiedChinese);
        }
        private void Text_Ex_Tab_Click(object sender, RoutedEventArgs e)
        {
            string text = TBox_Main.Text;
            bool dlgResult = MessageBox.Show("? 横向 : 纵向", "请选择制表符方向", MessageBoxButton.OKCancel) != MessageBoxResult.OK;
            text = dlgResult ? text.Replace(' ', '\t') : text.Replace(' ', '\v');
            TBox_Main.Text = text;
        }
        private void Text_Ex_Crlf_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.Text = string.Join("\r\n", TBox_Main.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
        }
        private void Text_Ex_Pinyin_Click(object sender, RoutedEventArgs e)
        {
            string text = TBox_Main.SelectedText;
            if (string.IsNullOrEmpty(text)) { return; }
            TBox_Main.SelectedText = SuniLib.Text.Processor.ChnToPh.ConvertCh(text);
        }
        private void Text_EnDe_UTF8_Click(object sender, RoutedEventArgs e)
        {
            string text = TBox_Main.SelectedText;
            if (string.IsNullOrEmpty(text)) { return; }
            bool dlgResult = MessageBox.Show("解码？", "Utf-8译码器引导", MessageBoxButton.OKCancel) != MessageBoxResult.OK;
            TBox_Main.SelectedText = dlgResult ? HttpUtility.UrlEncode(text) : HttpUtility.UrlDecode(text);
        }
        private void Text_EnDe_Unicode_Click(object sender, RoutedEventArgs e)
        {
            string text = TBox_Main.SelectedText;
            if (string.IsNullOrEmpty(text)) { return; }
            bool dlgResult = MessageBox.Show("解码？", "Unicode译码器引导", MessageBoxButton.OKCancel) != MessageBoxResult.OK;
            TBox_Main.SelectedText = dlgResult ? HttpUtility.UrlEncode(text, Encoding.Unicode) : HttpUtility.UrlDecode(text, Encoding.Unicode);
        }
        private void Text_EnDe_Base64_Click(object sender, RoutedEventArgs e)
        {
            string text = TBox_Main.SelectedText;
            if (string.IsNullOrEmpty(text)) { return; }
            bool dlgResult = MessageBox.Show("解码？", "Base64译码器引导", MessageBoxButton.OKCancel) != MessageBoxResult.OK;
            TBox_Main.SelectedText = dlgResult ? Convert.ToBase64String(Encoding.Default.GetBytes(text)) : Encoding.Default.GetString(Convert.FromBase64String(text));
        }
        private void Text_EnDe_MD5_Click(object sender, RoutedEventArgs e)
        {
            string text = TBox_Main.SelectedText;
            if (string.IsNullOrEmpty(text)) { return; }
            TBox_Key.Text = SuniLib.Text.Processor.ToMD5(text);
        }
        private void Text_Write_DateTime_Click(object sender, RoutedEventArgs e)
        {
            TBox_Main.AppendText($"\n{SuniLib.WinSys.Def.Now(true)}");
        }
        private void Text_TTS_Start_Click(object sender, RoutedEventArgs e)
        {
            string selected = TBox_Main.SelectedText;
            if (string.IsNullOrEmpty(selected)) { return; }
            Task.Run(() => DataIO.TtsRead(selected));
        }
        private void Text_TTS_Stop_Click(object sender, RoutedEventArgs e)
        {
            DataIO.TtsStop();
        }
        private void MenuItem_Search_MouseEnter(object sender, MouseEventArgs e)
        {
            Search_Regex_Template.Items.Clear();
            if (TBox_Key.Text.IndexOf('|') != -1)
            {
                string[] keys = TBox_Key.Text.Split('|');
                Pairs.Pattern.提取(keys[0], keys[1]);
            }
            var props = new Pattern.String().GetType().GetProperties().ToList();
            foreach (var prop in props)
            {
                MenuItem menuItem = new MenuItem
                {
                    Header = prop.Name,
                    Tag = prop.GetValue(new Pattern.String()),
                    Template = (ControlTemplate)Application.Current.Resources["SubMenuItem"],
                };
                menuItem.Click += MenuItem_Click;
                _ = Search_Regex_Template.Items.Add(menuItem);
            }
            Search_Regex_Template.Items.Refresh();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TBox_Key.Text = ((MenuItem)sender).Tag.ToString();
        }
        private void Search_KeyWord_Click(object sender, RoutedEventArgs e)
        {
            SearchMode = true;
            TBlk_sMode.Text = "匹配模式: 关键字";
        }
        private void Search_Regex_Click(object sender, RoutedEventArgs e)
        {
            SearchMode = false;
            TBlk_sMode.Text = "匹配模式: 表达式";
        }
        private void Search_Regex_Get_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TBox_Key.Text)) { return; }
            string txtKey = TBox_Key.Text;
            if (string.IsNullOrEmpty(TBox_Main.Text)) { return; }
            string txtMain = TBox_Main.Text;
            Regex mRegex = new Regex(txtKey);
            if (!mRegex.Match(txtMain).Success) { return; }
            MatchCollection mc = mRegex.Matches(txtMain);
            DataIO.WriteRegexTrail(mc);
        }
        private void Search_Online_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TBox_Main.SelectedText)) { return; }
            SuniLib.WinSys.Def.Browse(TBox_Main.SelectedText, "https://cn.bing.com/search?q=");
            SuniLib.WinSys.Def.Browse(TBox_Main.SelectedText, "https://www.baidu.com/s?wd=");
            SuniLib.WinSys.Def.Browse(TBox_Main.SelectedText, "https://www.sogou.com/web?query=");
        }
        private void Tool_GetWifiPsw_Click(object sender, RoutedEventArgs e)
        {
            string code = $"netsh wlan show profile name=\"{TBox_Key.Text}\" key=clear";
            var result = SuniLib.WinSys.Def.Shell(code);
            Console.WriteLine(result);
            foreach (string line in result.Split('\n'))
            {
                if (line.IndexOf("关键") >= 0)
                {
                    TBox_Main.AppendText($"\n{line.Trim()}");
                    TBox_Main.ScrollToEnd();
                    return;
                }
            }
        }
        private void Tool_HideFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBD = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "选择目标目录\n命令\nattrib -/+ s a h r path"
            };
            if (folderBD.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
            string hfp = folderBD.SelectedPath;
            _ = Interaction.Shell($"cmd /c attrib +s +a +h +r \"{hfp}\"");
            DataIO.WriteHideTrail(hfp);
        }
        private void Tool_Shutdown_Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int t = Convert.ToInt32(TBox_Key.Text);
                if (t == 0) { return; }
                _ = Interaction.Shell($"cmd /c shutdown -s -t {t * 60}");
            }
            catch { }
        }
        private void Tool_Shutdown_Cancel_Click(object sender, RoutedEventArgs e)
        {
            _ = Interaction.Shell("cmd /c shutdown -a");
        }
        private void Btn_Min_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            SendMail();
            DataIO.Backup(MainText);
            Visibility = Visibility.Hidden;
            if (!IsSaved)
            {
                if (IsAutoSave) { goto end; }
                bool exit = MessageBox.Show("有未保存的更改\n直接退出??", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK;
                if (exit)
                {
                    Visibility = Visibility.Visible;
                    return;
                }
                end: DataIO.AutoSave(MainText);
            }
            Hook.UnHook();
            Environment.Exit(0);
        }
        private void TBox_Key_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchCount();
        }
        private bool KeyOrMainIsNullOrEmpty()
        {
            return string.IsNullOrEmpty(TBox_Key.Text) | string.IsNullOrEmpty(TBox_Main.Text);
        }
        private void SearchCount()
        {
            if (KeyOrMainIsNullOrEmpty())
            {
                TBlk_sMode.Text = SearchMode ? "匹配模式 : 关键字" : "匹配模式 : 表达式";
                return;
            }
            string txtKey = TBox_Key.Text.Replace("^p", "\r\n");
            int lenKey = txtKey.Length;
            string txtMain = TBox_Main.Text;
            int LenMain = txtMain.Length;
            try
            {
                Regex mRegex = new Regex(txtKey);
                int c = SearchMode ? (LenMain - txtMain.Replace(txtKey, string.Empty).Length) / lenKey : mRegex.Matches(txtMain).Count;
                string mode = SearchMode ? "关键字" : "表达式";
                TBlk_sMode.Text = string.Format("匹配模式 : {0} 已匹配 : {1}项", mode, c);
            }
            catch { }
        }
        private void TBox_Key_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string fp = TBox_Key.Text;
            if (!File.Exists(fp)) { return; }
            TBox_Main.AppendText($"\n{new FileInfo(fp).Name}的MD5值{SuniLib.WinSys.Path.GetMd5(fp)}");
        }
        private void Lab_MouseMove(object sender, MouseEventArgs e)
        {
            ((Button)sender).Foreground = Brushes.Orange;
        }
        private void Lab_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button)sender).Foreground = (Brush)Application.Current.Resources["Brush_TBox_Foreground"];
        }
        private void TBox_Main_PreviewDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            string dataText = null;
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                dataText = e.Data.GetData("UnicodeText", true).ToString();
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fNames = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (fNames.Length == 1)
                {
                    string fn = fNames[0];
                    TBox_Key.Text = fn;
                    TBox_Main.AppendText(SuniLib.WinSys.Path.GetPathInfo(fn));
                    TBox_Main.ScrollToEnd();
                    return;
                }
                dataText = string.Join("\n", fNames);
            }
            if (dataText == TBox_Main.SelectedText) { return; }
            TBox_Main.AppendText("\n" + dataText);
            TBox_Main.ScrollToEnd();
            SendMail();
            DataIO.Backup(MainText);
        }
        private void TBox_Main_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                return;
            }
        }
        private void TBox_Main_MouseMove(object sender, MouseEventArgs e)
        {
            FillTBlkLenTBox();
        }
        private void TBox_Main_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsSaved = false;
            FillTBlkLenTBox();
            SearchCount();
        }
        private void FillTBlkLenTBox()
        {
            int c = TBox_Main.Text.Length;
            TBlk_LenTBox.Text = c.ToString();
            if (c == 0) { TBlk_LenTBox.Text = string.Empty; }
            int cs = TBox_Main.SelectedText.Length;
            if (cs != 0)
            {
                TBlk_LenTBox.Text = string.Format("{0}/{1}", cs, c);
            }
        }
        private void TBox_Main_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(Keyboard.GetKeyStates(Key.LeftCtrl) == KeyStates.Down | Keyboard.GetKeyStates(Key.RightCtrl) == KeyStates.Down)) { return; }
            if (e.Delta > 0)
            {
                if (TBox_Main.FontSize > 100) { return; }
                TBox_Main.FontSize += 1;
            }
            else
            {
                if (TBox_Main.FontSize < 8) { return; }
                TBox_Main.FontSize -= 1;
            }
        }
        private void Lab_Last_Click(object sender, RoutedEventArgs e)
        {
            if (KeyOrMainIsNullOrEmpty()) { return; }
            if (SearchMode) { LastKey(); }
            else { LastMcM(); }
        }
        private void Lab_Next_Click(object sender, RoutedEventArgs e)
        {
            if (KeyOrMainIsNullOrEmpty()) { return; }
            if (SearchMode) { NextKey(); }
            else { NextMcM(); }
        }
        private void NextKey()
        {
            string txtKey = TBox_Key.Text;
            int lenKey = txtKey.Length;
            string txtMain = TBox_Main.Text;
            Re: int newIndex = txtMain.IndexOf(txtKey, index);
            if (newIndex != 0 && newIndex == index)
            {
                index += lenKey;
                goto Re;
            }
            if (newIndex == -1)
            {
                index = 0;
                return;
            }
            TBox_Main.SelectionStart = newIndex;
            _ = TBox_Main.Focus();
            TBox_Main.SelectionLength = lenKey;
            if (newIndex == 0 && index == 0) { newIndex += lenKey; }
            index = newIndex;
        }
        private void LastKey()
        {
            string txtKey = TBox_Key.Text;
            int lenKey = txtKey.Length;
            string txtMain = TBox_Main.Text;
            index = txtMain.LastIndexOf(txtKey, index);
            if (index == -1)
            {
                index = txtMain.Length;
                return;
            }
            TBox_Main.SelectionStart = index;
            _ = TBox_Main.Focus();
            TBox_Main.SelectionLength = lenKey;
        }
        private void NextMcM()
        {
            Regex mRegex = new Regex(TBox_Key.Text);
            string txtMain = TBox_Main.Text;
            string subMainTxt = txtMain.Substring(index, txtMain.Length - index);
            string m = mRegex.Match(subMainTxt).Value;
            if (string.IsNullOrEmpty(m))
            {
                index = 0;
                return;
            }
            int midIndex = subMainTxt.IndexOf(m);
            int lenKey = m.Length;
            TBox_Main.SelectionStart = index + midIndex;
            _ = TBox_Main.Focus();
            TBox_Main.SelectionLength = lenKey;
            index += midIndex + lenKey;
        }
        private void LastMcM()
        {
            Regex mRegex = new Regex(TBox_Key.Text);
            string subMainTxt = TBox_Main.Text.Substring(0, index);
            MatchCollection m = mRegex.Matches(subMainTxt);
            if (m.Count == 0)
            {
                index = TBox_Main.Text.Length;
                return;
            }
            string mTxtR = SuniLib.Text.Processor.Reverse(m[m.Count - 1].Value);
            string subMainTxtR = SuniLib.Text.Processor.Reverse(subMainTxt);
            int midIndex = subMainTxtR.IndexOf(mTxtR);
            int lenKey = mTxtR.Length;
            TBox_Main.SelectionStart = index - midIndex - lenKey;
            _ = TBox_Main.Focus();
            TBox_Main.SelectionLength = lenKey;
            index -= midIndex + lenKey;
        }
        private void Text_Write_DeviceInfo_Click(object sender, RoutedEventArgs e)
        {
            new Task(() =>
            {
                MainText += SuniLib.WinSys.Def.DeviceInfo();
            }).Start();
        }
        private void TBox_Main_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //TBox_Main.ScrollToEnd();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SendMail();
            DataIO.Backup(MainText);
        }
        public void SendMail()
        {
            //if (!IsSendToMail) { return; }
            //if (string.IsNullOrWhiteSpace(MainText)) { return; }
            //new Task(() =>
            //{
            //    string subject = "Text From SuniText";
            //    string body = $"\r\n\r\n{SuniLib.WinSys.Def.Now(true)}\r\n{MainText}";
            //    bool enableSSL = true;
            //    try
            //    {
            //        MailMessage mail = new MailMessage
            //        {
            //            From = new MailAddress(fromEmail)
            //        };
            //        mail.To.Add(toEmail);
            //        mail.Subject = subject;
            //        mail.Body = body;
            //        using (SmtpClient smtp = new SmtpClient(smtpServer))
            //        {
            //            smtp.Credentials = new NetworkCredential(fromEmail, password);
            //            smtp.EnableSsl = enableSSL;
            //            smtp.Send(mail);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}).Start();
        }
        //格式：发件人|密码|收件人|服务器
        private string smtpServer, fromEmail, password, toEmail;
        private void Text_SendToMail_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSendToMail)
            {
                List<string> info = KeyText.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (info.Count < 4) { return; }
                fromEmail = info[0];
                password = info[1];
                toEmail = info[2];
                smtpServer = info[3]; // Replace with your SMTP server
            }
            IsSendToMail = !IsSendToMail;
            if (IsSendToMail)
            {
                ((MenuItem)sender).Header = "发到邮箱*";
            }
            else
            {
                ((MenuItem)sender).Header = "发到邮箱";

            }

        }
    }
}

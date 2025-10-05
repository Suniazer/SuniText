using SuniText.SuniLib;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic;
using System.ComponentModel;
using Path = SuniText.SuniLib.WinSys.Path;
using System.Text.RegularExpressions;

namespace SuniText.SuniFrm
{
    /// <summary>
    /// Mode.xaml 的交互逻辑
    /// </summary>
    public partial class Mode : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        private string RPattern = "(?<=)(.*?)(?=)";
        public string Pattern
        {
            get => RPattern;
            set
            {
                RPattern = value;
                RaisePropertyChanged("Pattern");
            }
        }
        private readonly AssemblyName appInfo = Assembly.GetExecutingAssembly().GetName();
        private string appHash;
        public bool GetClip, EditClip, LockMode;
        public bool GetRGB, GetHEX, GetCMYK, GetPoint;
        public bool DelCrLf, DelWhiteSpace, DelAll, ReSetClipBoard;
        public bool ZHCN, ToTraditional, ToSimplified, ToPinyin;
        public bool ENUS, ToProperCase, ToUpperCase, ToLowerCase;

        private void TBox_Pattern_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Pattern)) { CBox_Regex.IsChecked = false; return; }
            //Pattern = TBox_Pattern.Text;
        }

        public bool ByRegex, RegexGet, RegexDel;

        private void CBox_WhiteSpace_Unchecked(object sender, RoutedEventArgs e)
        {
            CBox_DelAll.IsChecked = false;
        }

        private void CBox_CrLf_Unchecked(object sender, RoutedEventArgs e)
        {
            CBox_DelAll.IsChecked = false;
        }

        private void CBox_DelAll_Checked(object sender, RoutedEventArgs e)
        {
            bool allow = (bool)CBox_CrLf.IsChecked || (bool)CBox_WhiteSpace.IsChecked;
            if (!allow) { CBox_DelAll.IsChecked = false; }
        }

        private void CBox_Regex_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Pattern)) { CBox_Regex.IsChecked = false; return; }
            //Pattern = TBox_Pattern.Text;
        }
        private void CBox_Regex_Checked(object sender, RoutedEventArgs e)
        {
            CBox_Regex.IsChecked = !string.IsNullOrEmpty(Pattern);
        }
        public Mode()
        {
            InitializeComponent();
            FillValue();
            DataContext = this;
        }

        public new void Show()
        {
            appHash = Path.GetAppMD5(appInfo.CodeBase.Replace("file:///", string.Empty));
            TBlk_Header.Text = appHash;
            _ = ShowDialog();
        }
        private void FillValue()
        {
            GetClip = !(bool)RBtn_GetColor.IsChecked;
            LockMode = (bool)CBox_Lock.IsChecked;
            EditClip = (bool)RBtn_HookClip.IsChecked;
            GetRGB = (bool)CBox_RGB.IsChecked;
            GetHEX = (bool)CBox_HEX.IsChecked;
            GetCMYK = (bool)CBox_CMYK.IsChecked;
            GetPoint = (bool)CBox_Point.IsChecked;
            DelCrLf = (bool)CBox_CrLf.IsChecked;
            DelWhiteSpace = (bool)CBox_WhiteSpace.IsChecked;
            DelAll = (bool)CBox_DelAll.IsChecked;
            ZHCN = !(bool)RBtn_ENUS.IsChecked;
            ToTraditional = (bool)RBtn_Traditional.IsChecked;
            ToSimplified = (bool)RBtn_Simplified.IsChecked;
            ToPinyin = (bool)RBtn_Pinyin.IsChecked;
            ENUS = !(bool)RBtn_ENUS.IsChecked;
            ToProperCase = (bool)RBtn_ProperCase.IsChecked;
            ToUpperCase = (bool)RBtn_UpperCase.IsChecked;
            ToLowerCase = (bool)RBtn_LowerCase.IsChecked;
            ByRegex = (bool)CBox_Regex.IsChecked;
            RegexGet = (bool)RBtn_RegexGet.IsChecked;
            RegexDel = (bool)RBtn_RegexDel.IsChecked;
            ReSetClipBoard = (bool)CBox_ReSetClip.IsChecked;
        }
        public void RefreshTheme()
        {
            Background = (Brush)Application.Current.Resources["Brush_Main_Background"];
            Bd_Main.BorderBrush = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            TBlk_Header.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            Dp_Btn.Background = (Brush)Application.Current.Resources["Brush_Main_Background"];
            Dp_Title.Background = (Brush)Application.Current.Resources["Brush_Main_Background"];
            Dp_Mode.Background = (Brush)Application.Current.Resources["Brush_Menu_Background"];
            RBtn_GetColor.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_HookClip.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            CBox_RGB.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            CBox_HEX.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            CBox_CMYK.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            CBox_Point.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            CBox_ReSetClip.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            CBox_CrLf.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            CBox_WhiteSpace.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            CBox_DelAll.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_Traditional.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_Simplified.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_Pinyin.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_ProperCase.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_UpperCase.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_LowerCase.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            CBox_Regex.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_RegexGet.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_RegexDel.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            RBtn_ReadClip.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            Btn_OK.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            Btn_Cancel.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            Btn_Exit.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            TBox_Pattern.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            TBox_Pattern.Background = (Brush)Application.Current.Resources["Brush_Main_Background"];
            CBox_Lock.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
        }
        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            FillValue();
            Visibility = Visibility.Hidden;
        }
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            // Begin dragging the window
            DragMove();
        }
    }
}

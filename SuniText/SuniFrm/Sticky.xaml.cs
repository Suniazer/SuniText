using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Monitor = SuniText.SuniLib.WinSys.Monitor;

namespace SuniText.SuniFrm
{
    /// <summary>
    /// Sticky.xaml 的交互逻辑
    /// </summary>
    public partial class Sticky : Window
    {
        private readonly double screenHeight = SystemParameters.WorkArea.Height;
        private readonly double screenWidth = SystemParameters.WorkArea.Width;
        public Sticky()
        {
            InitializeComponent();
            Top = 2 * Height;
            Left = screenWidth - Width;
            Thread t = new Thread(() => MonitorF());
            t.TrySetApartmentState(ApartmentState.STA);
            t.Start();
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        } 
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetLocation();
        }
        private void ResetLocation()
        {
            if (Top > screenHeight) { Top = screenHeight - Height; }
            if (Top < 2 * Height) { Top = 2 * Height; }
            if (Left > screenWidth - Width - 10)
            {
                Left = screenWidth - Width;
                DockPanel.SetDock(Lab_Side, Dock.Right);
            }
            if (Left < 10)
            {
                Left = 0;
                DockPanel.SetDock(Lab_Side, Dock.Left);
            }
            //var window = Window.GetWindow(this);//获取当前主窗口
            //var intPtr = new WindowInteropHelper(window).Handle;//获取当前窗口的句柄
            //var screen = Screen.FromHandle(intPtr);//获取当前屏幕
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ResetLocation();
        }
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                Owner.Visibility = Visibility.Visible;
            }
            else
            {
                ResetLocation();
                RefreshTheme();
                Visibility = Visibility.Hidden;
            }
     
        }
        private void MonitorF()
        {
            List<System.Diagnostics.PerformanceCounter> TRS = Monitor.GetCounter(Monitor.AutoSelect());
            while (true)
            {
                Action a = () =>
                {
                    List<string> Speed = Monitor.GetSpeed(TRS, out Brush b);
                    TBlk_Total.Text = Speed[0];
                    TBlk_Receive.Text = Speed[1];
                    TBlk_Send.Text = Speed[2];
                    Background = b;
                };
                _ = Dispatcher.BeginInvoke(a);
                Thread.Sleep(1000);
            }
        }
        public void RefreshTheme()
        {
            Background = (Brush)Application.Current.Resources["Brush_Main_Background"];
            Bd_Main.BorderBrush = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            Lab_Side.Background = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            TBlk_Send.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            TBlk_Total.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
            TBlk_Receive.Foreground = (Brush)Application.Current.Resources["Brush_Menu_Foreground"];
        }
    }
}

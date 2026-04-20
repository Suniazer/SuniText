using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Security.AccessControl;
using System.IO;
using System.Reflection;

namespace SuniText
{
    public partial class App : Application
    {
        public EventWaitHandle ProgramStarted { get; set; }
        private readonly string[] dlls = new string[] { "Interop.IWshRuntimeLibrary" };    // 去掉后缀名

        ///<summary>
        /// 该函数设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWindow函数的说明部分</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        /// <summary>
        ///  该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。
        ///  系统给创建前台窗口的线程分配的权限稍高于其他线程。 
        /// </summary>
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>
        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
        /// <summary>
        /// App.xaml 的交互逻辑
        /// </summary>
        public App()
        {
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            string mutexName = "11ce1f5b-f934-4468-937f-84a3ec6cb972";

            ProgramStarted = new EventWaitHandle(false, EventResetMode.AutoReset, mutexName, out var createNew);

            if (!createNew)
            {
                try
                {
                    string name = System.IO.Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
                    var processes = Process.GetProcessesByName(name);
                    if (!processes.Any())
                    {
                    }
                    else
                    {
                        foreach (Process process in processes)
                        {
                            ShowWindowAsync(process.MainWindowHandle, 1);
                            SetForegroundWindow(process.MainWindowHandle);
                            SwitchToThisWindow(process.MainWindowHandle, true);
                        }
                    }
                }
                catch (Exception exception)
                {
                    // Logger.Error(exception, "唤起已启动进程时出错");
                }
                App.Current.Shutdown();
            }
            else
            {
                base.OnStartup(e);
            }
        }
    }
}

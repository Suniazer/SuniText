using SuniText.SuniLib.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuniText.SuniLib.WinSys
{
    public static class Def
    {
        public static string Now(bool mode)
        {
            return DateTime.Now.ToString(mode ? "yyyy年MM月dd日HH时mm分ss秒fff" : "yyyy_MM_dd_HH_mm_ss_fff");
        }
        public static void Browse(string str, string url)
        {
            Process.Start(url + Regex.Replace(str, @"\s+", "%20"));
        }
        public static string Shell(string code)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo("cmd.exe")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                    Arguments = $"/c {code}",
                }
            };
            proc.Start();
            proc.WaitForExit();
            return proc.StandardOutput.ReadToEnd();
        }
        public static string SerialNumber()//主板ID
        {
            try
            {
                ManagementObjectCollection moc = new ManagementClass("Win32_BaseBoard").GetInstances();
                foreach (ManagementObject mo in moc.Cast<ManagementObject>())
                {
                    return mo.Properties["SerialNumber"].Value.ToString();
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return string.Empty;
        }
        public static string DeviceInfo()//主板ID
        {
            string info = string.Empty;
            try
            {
                ManagementObjectCollection moc = new ManagementClass(WindowsAPIType.Win32_BaseBoard.ToString()).GetInstances();
                foreach (ManagementObject mo in moc.Cast<ManagementObject>())
                {
                    foreach (PropertyData prop in mo.Properties)
                    {
                        string name = prop.Name;
                        object value = mo.Properties[prop.Name].Value;
                        if (value == null || value.GetType() == typeof(string[])) { continue; }
                        switch (name)
                        {
                            case "Manufacturer":
                                info += $"\n制造商：{value}{Separator.CrLf}";
                                break;
                            case "Product":
                                info += $"型号：{value}{Separator.CrLf}";
                                break;
                            case "SerialNumber":
                                info += $"序列号：{value}{Separator.CrLf}";
                                break;
                            default:
                                break;
                        }
                    }
                }
                ManagementObjectCollection mocCPU = new ManagementClass(WindowsAPIType.Win32_Processor.ToString()).GetInstances();
                foreach (ManagementObject m in mocCPU)
                {
                    info += $"处理器：{m[WindowsAPIKeys.Name.ToString()]}\n";
                }
                foreach (var item in new ManagementObjectSearcher("Select * from " + WindowsAPIType.Win32_Processor.ToString()).Get())
                {
                    info += $"核心数：{item[WindowsAPIKeys.NumberOfCores.ToString()]}核\n";
                }
                ManagementObjectCollection OperatingSystemC = new ManagementClass(WindowsAPIType.Win32_OperatingSystem.ToString()).GetInstances();
                foreach (ManagementObject m in OperatingSystemC)
                {
                    info += $"操作系统：{m[WindowsAPIKeys.Name.ToString()]}\n";
                }
                ManagementObjectCollection VideoProcessorC = new ManagementClass(WindowsAPIType.Win32_VideoController.ToString()).GetInstances();
                foreach (ManagementObject m in VideoProcessorC)
                {
                    info += $"显卡：{m[WindowsAPIKeys.VideoProcessor.ToString()]}；显存:{Convert.ToInt64(m[WindowsAPIKeys.AdapterRAM.ToString()].ToString()) / 1024 / 1024}MB\n";
                }
            }
            catch (Exception) {; }
            return info;
        }
        public enum WindowsAPIKeys
        {
            /// <summary>
            /// 名称
            /// </summary>
            Name,
            /// <summary>
            /// 显卡芯片
            /// </summary>
            VideoProcessor,
            /// <summary>
            /// 显存大小
            /// </summary>
            AdapterRAM,
            /// <summary>
            /// 分辨率宽
            /// </summary>
            ScreenWidth,
            /// <summary>
            /// 分辨率高
            /// </summary>
            ScreenHeight,
            /// <summary>
            /// 电脑型号
            /// </summary>
            Version,
            /// <summary>
            /// 硬盘容量
            /// </summary>
            Size,
            /// <summary>
            /// 内存容量
            /// </summary>
            Capacity,
            /// <summary>
            /// cpu核心数
            /// </summary>
            NumberOfCores
        }
        public enum WindowsAPIType
        {
            /// <summary>
            /// 内存
            /// </summary>
            Win32_PhysicalMemory,
            /// <summary>
            /// cpu
            /// </summary>
            Win32_Processor,
            /// <summary>
            /// 硬盘
            /// </summary>
            win32_DiskDrive,
            /// <summary>
            /// 电脑型号
            /// </summary>
            Win32_ComputerSystemProduct,
            /// <summary>
            /// 分辨率
            /// </summary>
            Win32_DesktopMonitor,
            /// <summary>
            /// 显卡
            /// </summary>
            Win32_VideoController,
            /// <summary>
            /// 操作系统
            /// </summary>
            Win32_OperatingSystem,
            /// <summary>
            /// 主板
            /// </summary>
            Win32_BaseBoard

        }
        public static void AddFolderSecurityControl(string dirPath)
        {
            //获取文件夹信息
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            //获得该文件夹的所有访问权限
            DirectorySecurity dirSecurity = dir.GetAccessControl(AccessControlSections.All);
            //设定文件ACL继承
            InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            //添加Guest用户组的访问权限规则 完全控制权限
            FileSystemAccessRule guestsFileSystemAccessRule =
                new FileSystemAccessRule("Guests",
                FileSystemRights.ReadAndExecute,
                inherits, PropagationFlags.None,
                AccessControlType.Allow);
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, guestsFileSystemAccessRule, out bool isModified);
            //设置访问权限
            dir.SetAccessControl(dirSecurity);
        }
        public static void DelFolderSecurityControl(string dirPath)
        {
            //获取文件夹信息
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            //获得该文件夹的所有访问权限
            DirectorySecurity dirSecurity = dir.GetAccessControl();
            //设定文件ACL继承
            InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            //删除用户组的访问权限规则 完全控制权限
            dirSecurity.RemoveAccessRule(new FileSystemAccessRule("Guests",
                FileSystemRights.ReadAndExecute,
                inherits, PropagationFlags.None,
                AccessControlType.Allow));
            //设置访问权限
            dir.SetAccessControl(dirSecurity);
        }
    }
}

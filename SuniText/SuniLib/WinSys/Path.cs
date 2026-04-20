using Microsoft.Win32;
using SuniText.SuniClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SuniText.SuniLib.WinSys
{
    public static class Path
    {
        public static void OpenFolder(string filePath)
        {
            _ = Process.Start("explorer.exe", filePath);
        }
        public static void SelectFile(string filePath)
        {
            _ = Process.Start("explorer.exe", " /select," + filePath);
        }
        public static bool CreateFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    _ = Directory.CreateDirectory(folder);
                }
            }
            catch { return false; }
            return true;
        }
        public static bool FillFilePath(ref TextFile info)
        {
            if (File.Exists(info.Path)) { return true; }
            SaveFileDialog saveFDialog = new SaveFileDialog
            {
                Filter = "文本文件|*.txt",
                Title = "设置文本保存路径"
            };
            if ((bool)saveFDialog.ShowDialog())
            {
                info.Path = saveFDialog.FileName;
                info.Name = new FileInfo(info.Path).Name;
                info.Parent = new FileInfo(info.Path).Directory.FullName;
                return true;
            }
            return false;
        }
        public static string GetSavePath()
        {
            SaveFileDialog saveFDialog = new SaveFileDialog
            {
                Filter = "文本文件|*.txt",
                Title = "设置文本保存路径"
            };
            string fp = string.Empty;
            if ((bool)saveFDialog.ShowDialog())
            {
                return saveFDialog.FileName;
            }
            return string.Empty;
        }
        public static string GetFilePath()
        {
            OpenFileDialog saveFDialog = new OpenFileDialog
            {
                Filter = "文本文件|*.txt",
                Title = "打开"
            };
            if ((bool)saveFDialog.ShowDialog())
            {
                return saveFDialog.FileName;
            }
            return string.Empty;
        }
        public static string GetMd5(string fp)
        {
            MD5CryptoServiceProvider md5Cps = new MD5CryptoServiceProvider();
            try
            {
                FileStream fs = new FileStream(fp, FileMode.Open, FileAccess.Read);
                string hash = BitConverter.ToString(md5Cps.ComputeHash(fs)).Replace("-", "");
                fs.Close();
                return hash;
            }
            catch {; }
            return string.Empty;
        }
        public static string GetAppMD5(string startUpPath)
        {
            string tempAppPath = Pairs.DataFolder.TempApp;
            if (File.Exists(tempAppPath)) { File.Delete(tempAppPath); }
            File.Copy(startUpPath, tempAppPath);
            string exeMd5 = GetMd5(tempAppPath);
            File.Delete(tempAppPath);
            return exeMd5;
        }
        public static string GetPathInfo(string path)
        {
            string info = string.Empty;
            if (Directory.Exists(path)) { GetDirInfo(ref info, path); }
            if (File.Exists(path)) { GetFileInfo(ref info, path); }
            return "\n" + info;
        }
        private static void GetFileInfo(ref string fpInfo, string path)
        {
            FileInfo f = new FileInfo(path);
            string ext = f.Extension.Replace(".", string.Empty);
            fpInfo += "名称: " + f.Name + "\n";
            fpInfo += "创建: " + f.CreationTime.ToString() + "\n";
            fpInfo += "访问: " + f.LastAccessTime.ToString() + "\n";
            fpInfo += "修改: " + f.LastWriteTime.ToString() + "\n";
            fpInfo += "大小: " + GetFileLength(f.Length) + "\n";
            fpInfo += "类型: " + Text.Processor.ToExtInfo(ext) + "\n";
            GetAttr(ref fpInfo, path);
        }
        private static string GetFileLength(long l)
        {
            return l < 1024
                ? string.Format("{0}B", l)
                : l >= 1024 * 1024 * 1024
             ? string.Format("{0}GB", ChinaRound((double)l / (1024 * 1024 * 1024)))
             : l >= 1024 * 1024
          ? string.Format("{0}MB", ChinaRound((double)l / (1024 * 1024)))
          : string.Format("{0}KB", ChinaRound((double)l / 1024));
        }
        private static double ChinaRound(double l)
        {
            return Math.Round(l, 2, MidpointRounding.AwayFromZero);
        }
        private static void GetDirInfo(ref string dpInfo, string path)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            dpInfo += "名称: " + d.Name + "\n";
            dpInfo += "创建: " + d.CreationTime.ToString() + "\n";
            dpInfo += "访问: " + d.LastAccessTime.ToString() + "\n";
            dpInfo += "修改: " + d.LastWriteTime.ToString() + "\n";
            GetAttr(ref dpInfo, path);
        }
        private static void GetAttr(ref string info, string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            info += "属性: ";
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory) { info += "目录 "; }
            else { info += "文件 "; }
            if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) { info += "只读 "; }
            if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden) { info += "隐藏 "; }
            if ((attr & FileAttributes.System) == FileAttributes.System) { info += "系统 "; }
            if ((attr & FileAttributes.Archive) == FileAttributes.Archive) { info += "备份 "; }
            if ((attr & FileAttributes.Compressed) == FileAttributes.Compressed) { info += "压缩 "; }
            if ((attr & FileAttributes.Encrypted) == FileAttributes.Encrypted) { info += "加密 "; }
            if ((attr & FileAttributes.Normal) == FileAttributes.Normal) { info += "标准 "; }
            if ((attr & FileAttributes.Offline) == FileAttributes.Offline) { info += "脱机 "; }
            if ((attr & FileAttributes.Temporary) == FileAttributes.Temporary) { info += "临时 "; }
            if ((attr & FileAttributes.SparseFile) == FileAttributes.SparseFile) { info += "稀疏 "; }
            if ((attr & FileAttributes.Device) == FileAttributes.Device) { info += "待用 "; }
            info += "\n";
        }
    }
}

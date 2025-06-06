using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.TextFormatting;
using static System.Environment;
using static System.Net.Mime.MediaTypeNames;
using Path = SuniText.SuniLib.WinSys.Path;

namespace SuniText.SuniClass
{
    public static partial class DataRelay
    {
        private static readonly string LocalDataFolder = GetFolderPath(SpecialFolder.LocalApplicationData);
        private static readonly string SuniTextFolder = LocalDataFolder + "\\SuniText\\";
        private static readonly string TempFolder = SuniTextFolder + "Temp\\";
        private static readonly string HideTrailFile = TempFolder + "HideFile_Trail_" + DateTime.Now.GetHashCode() + ".txt";
        private static int i;
        private static SpeechSynthesizer TxtSpeaker = new SpeechSynthesizer();
        public static void Init()
        {
            Path.CreateFolder(LocalDataFolder);
            Path.CreateFolder(SuniTextFolder);
            Path.CreateFolder(TempFolder);
        }
        public static void AutoSave(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return; }
            File.WriteAllText(SuniTextFolder + $"AutoSave_{$"{DateTime.Now}{i}".GetHashCode()}.txt", text);
        }
        public static void Backup(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return; }
            Interlocked.Exchange(ref i, ++i);
            Task.Run(() => {
                FileStream FsBackup = new FileStream($"{TempFolder}TextInput_Trail_{$"{DateTime.Now}{i}".GetHashCode()}.txt", FileMode.CreateNew);
                StreamWriter SwBackup = new StreamWriter(FsBackup);
                SwBackup.WriteLineAsync(text).Wait();
                SwBackup.FlushAsync().Wait();
                SwBackup.Dispose();
                FsBackup.Dispose();
            });
        }
        public static void OpenIniFolder()
        {
            _ = Process.Start("explorer.exe", SuniTextFolder);
        }
        public static void OpenTempFolder()
        {
            _ = Process.Start("explorer.exe", TempFolder);
        }
        public static void WriteHideTrail(string path)
        {
            File.AppendAllText(HideTrailFile, $"{DateTime.Now:yy年MM月dd日 HH时mm分}->{path}");
        }
        public static void WriteRegexTrail(MatchCollection mc)
        {
            string fp = $"{DataRelay.TempFolder}\\Regex{SuniLib.WinSys.Def.Now(true)}.txt";
            foreach (Match m in mc)
            {
                File.AppendAllText(fp, m.Value);
            }
            try
            {
                _ = Process.Start("explorer.exe", fp);
            }
            catch {; }
        }
        public static bool TtsRead(string selected)
        {
            TxtSpeaker = new SpeechSynthesizer();
            string fp = $"{TempFolder}{DateTime.Now:hh_mm_ss_fff}.wav";
            TxtSpeaker.SetOutputToWaveFile(fp);
            var result = TxtSpeaker.SpeakAsync(selected);
            while (!result.IsCompleted) { Thread.Sleep(100); }
            TxtSpeaker.Dispose();
            _ = Process.Start("explorer.exe", " /select," + fp);
            return true;
        }
        public static void TtsStop()
        {
            TxtSpeaker.Dispose();
            TxtSpeaker = new SpeechSynthesizer();
        }
        public static string TEMPPath()
        {
            return DataRelay.TempFolder + DateTime.Now.GetHashCode() + "TEMP";
        }
        public static string TEMPFolder()
        {
            return DataRelay.TempFolder;
        }
    }
}

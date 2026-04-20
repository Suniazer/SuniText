using System;
using System.IO;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Linq;
using SuniText.SuniLib.WinSys;
using System.Text;
using System.Windows.Forms;


namespace SuniText.SuniClass
{
    public static partial class DataIO
    {
        private static SpeechSynthesizer TxtSpeaker = new SpeechSynthesizer();
        public static void Init()
        {
            Pairs.DataFolder.CreateFolder();
        }
        public static void AutoSave(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return; }
            File.WriteAllText(Pairs.DataFolder.AutoSaveFile, text);
        }
        public static void Backup(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return; }
            //MessageBox.Show(Pairs.DataFolder.InputTrailFile);
            Task.Run(() => {
                File.AppendAllText(Pairs.DataFolder.InputTrailFile, text, Encoding.UTF8);
                //FileStream FsBackup = new FileStream(Pairs.DataFolder.InputTrailFile, FileMode.CreateNew);
                //StreamWriter SwBackup = new StreamWriter(FsBackup);
                //SwBackup.WriteLineAsync().Wait();
                //SwBackup.FlushAsync().Wait();
                //SwBackup.Dispose();
                //FsBackup.Dispose();
            });
        }
        public static void OpenIniFolder()
        {
              SuniLib.WinSys.Path.OpenFolder(Pairs.DataFolder.SuniTextFolder);
        }
        public static void OpenTempFolder()
        {
              SuniLib.WinSys.Path.OpenFolder(Pairs.DataFolder.TempFolder);
        }
        public static void WriteHideTrail(string path)
        {
            File.AppendAllText(Pairs.DataFolder.HideTrailFile, contents: $"\r\n{Def.Now(true)}\r\n{path}");
        }
        public static void WriteRegexTrail(MatchCollection mc)
        {
            string filePath = Pairs.DataFolder.RegexFile;
            List<Match> valueList = new List<Match>();  
            foreach (var m in mc)
            {
                valueList.Add(m as Match);
            }
            File.AppendAllLines(filePath, from match in valueList select match.Value);
            try
            {
                SuniLib.WinSys.Path.OpenFolder(filePath);
            }
            catch {; }
        }
        public static bool TtsRead(string selected)
        {
            TxtSpeaker = new SpeechSynthesizer();
            string filePath = Pairs.DataFolder.TtsRawFile;
            TxtSpeaker.SetOutputToWaveFile(filePath);
            var result = TxtSpeaker.SpeakAsync(selected);
            while (!result.IsCompleted) { Thread.Sleep(100); }
            TxtSpeaker.Dispose();
            SuniLib.WinSys.Path.SelectFile(filePath);
            return true;
        }
        public static void TtsStop()
        {
            TxtSpeaker.Dispose();
            TxtSpeaker = new SpeechSynthesizer();
        }
    }
}

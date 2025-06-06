using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Brushes = System.Windows.Media.Brushes;

namespace SuniText.SuniLib.WinSys
{
    public static class Monitor
    {
        public static string AutoSelect()
        {
            PerformanceCounterCategory pfcc = new PerformanceCounterCategory("Network Interface");
        rf:
            foreach (string name in pfcc.GetInstanceNames())
            {
                int i = 0;
            re: PerformanceCounter counter = pfcc.GetCounters(name)[0];
                _ = counter.NextValue();
                if (counter.NextValue() != 0) { return name; }
                else
                {
                    Thread.Sleep(100);
                    if (++i > 3) { continue; }
                    goto re;
                }
            }
            goto rf;
        }
        public static List<PerformanceCounter> GetCounter(string name)
        {
            List<PerformanceCounter> TRS = new List<PerformanceCounter>();
            PerformanceCounter[] Counters = new PerformanceCounterCategory("Network Interface").GetCounters(name);
            TRS.Add(Counters[0]);
            TRS.Add(Counters[5]);
            TRS.Add(Counters[11]);
            return TRS;
        }
        public static List<string> GetSpeed(List<PerformanceCounter> counters,out System.Windows.Media.Brush brush)
        {
            System.Windows.Media.Brush b = (System.Windows.Media.Brush)System.Windows.Application.Current.Resources["Brush_Main_Background"];
            System.Windows.Media.Brush bl = (System.Windows.Media.Brush)System.Windows.Application.Current.Resources["Brush_LightMain_Background"];
            bool IsDark = b != bl;
            List<string> result = new List<string>() {"↕ ","⇣ ", "⇡ "};
            brush = b;
            for (int i = 2; i >= 0; i--)
            {
                PerformanceCounter counter = counters[i];
                double speed = Math.Round(counter.NextValue() / 1024, 2) / 1024;
                result[i] += speed > 1 ? $"{Math.Round(speed, 2)}MB/s" : $"{speed * 1024}KB/s";
                brush = speed > 10 ? Brushes.OrangeRed : speed > 1 ? Brushes.Orange : speed == 0 ? b : IsDark ? Brushes.Black : Brushes.AliceBlue;
            }
            return result;
        }
    }
}

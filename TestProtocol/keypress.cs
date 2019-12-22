using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace TestProtocol
{
    [Serializable]
    public class keypress
    {
        [XmlAttribute("keys")]
        public string KEY = "";

        [XmlAttribute("after")]
        public string invokeRoutine = "1S";


        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, ShowWindowEnum flags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hwnd);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        private enum ShowWindowEnum
        {
            Hide = 0,
            ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3,
            Maximize = 3, ShowNormalNoActivate = 4, Show = 5,
            Minimize = 6, ShowMinNoActivate = 7, ShowNoActivate = 8,
            Restore = 9, ShowDefault = 10, ForceMinimized = 11
        };
        public keypress() { }
        public void Invoke(int SteamGameProcess) {

            //if routine isnt timebased
            //find what it is
            //get its process
            //exectute routine
            //else 
            //wait
            //execute keys

            string processName = "";
            var file = $@"C:\Program Files (x86)\Steam\steamapps\appmanifest_{SteamGameProcess}.acf";
            if (!File.Exists(file))
            {
                Console.WriteLine("Failed to find application");
                return;
            }
            if ((File.ReadAllText(file) ?? "") == "")
            {
                Console.WriteLine("Failed to find application");
                return;
            }
            //Console.WriteLine(file);
            var match = Regex.Match(File.ReadAllText(file), "(?<=name\"\\t\\t\").*(?=\")");
            if (!match.Success || (match.Value ?? "") == "")
            {
                Console.WriteLine(File.ReadAllText(file));
                Console.WriteLine("Failed to find application");
                return;
            }
            //Console.WriteLine(match.Success);
            processName = match.Value.ToLower().Replace(" ", "").Replace("-", "").Replace("_", "");
            var proc = Process.GetProcesses().Where((p) => p.ProcessName.ToLower().Contains(processName));
            //Console.WriteLine(processName);
            if (proc.Count() == 0)
            {
                Process.Start($"steam://run/{"" + SteamGameProcess}");
            }
            var start = DateTime.Now;
            while (proc.Count() == 0 && (DateTime.Now - start) < new TimeSpan(0, 0, 20))
            {
                Thread.Sleep(200);
                proc = Process.GetProcesses().Where((p) =>
                p.ProcessName.ToLower().Replace(" ", "").Replace("-", "").Replace("_", "")
                .Contains(processName));
            }

            var game = new List<Process>(proc)[0];

            Console.Write("Process is ");
            Console.WriteLine(game.ProcessName);
            if (TimeBasedRoutine()){
                Console.WriteLine("Started time based invoking");
                Thread.Sleep(DecodeTimeBased());
                Console.WriteLine("ended time based invoking");
            }
            else {
                
                if (invokeRoutine.ToLower() == "ONRESPONDING".ToLower())
                {
                    while (!game.Responding) { Thread.Sleep(20); }
                    Console.WriteLine("on responding");
                }
                else if (invokeRoutine.ToLower() == "ONCLOSE")
                {

                }
                else if (invokeRoutine.ToLower() == "ONOPEN")
                {

                }


                //Process.Start("Steam://app/" + SteamGameProcess);
            }
            Console.WriteLine("ending keys");
            if ((KEY ?? "") == "") return;

            KEY = KEY.ToLower().Replace("{enter}", "{ENTER}");

            string NEW = KEY;
            Match replace = Regex.Match(NEW, "%(\\d|A|B|C|D|E|F)(\\d|A|B|C|D|E|F)");
            while (replace.Success)
            {
                char replacement = (char)byte.Parse(replace.Value.Replace("%", ""), System.Globalization.NumberStyles.HexNumber);
                NEW = NEW.Replace(replace.Value, "" + replacement);
                replace = Regex.Match(NEW, "%(\\d|A|B|C|D|E|F)(\\d|A|B|C|D|E|F)");
            }
            KEY = NEW;


            SetWindowPos(game.MainWindowHandle,-1, 0, 0, 0, 0, 3);
            IntPtr window = SetFocus(new HandleRef(null, game.MainWindowHandle));
            Thread.Sleep(100);
            SendKeys.SendWait(KEY);
            Console.WriteLine(KEY);
            //foreach (var item in KEY)
            //{
            //    //key down
            //    PostMessage(GetForegroundWindow(), 0x0100, item, 0);
            //    Thread.Sleep(10);
            //    //key up
            //    PostMessage(GetForegroundWindow(), 0x0101, item, 0);
            //    Console.Write(item);
            //}
            //Console.WriteLine();



            //SendKeys.SendWait(""+ SteamGameProcess);
            //SendKeys.SendWait("%{Tab}{`}Open 74.81.90.101");
        }

        /// <summary>
        /// num M
        /// num S
        /// num MS

        /// </summary>
        /// <returns> true if time based string</returns>
        private bool TimeBasedRoutine() {
            Match m = Regex.Match(invokeRoutine, "^\\d*( |)(s|S|m|M|ms|MS|Ms|mS)$");
            return m.Success && m.Value == invokeRoutine ;
        }
        private enum types { 
            s=1000,
            m=60000,
            ms=1
        }
        private int DecodeTimeBased() {
            string temp = invokeRoutine.Substring(invokeRoutine.Length - 2,2);
            if (temp[0].ToString().ToLower() != "m") {
                temp = ""+temp[1];
            }
            int mult = (int)(types)Enum.Parse(typeof(types), temp);
            Console.WriteLine(mult);
            temp = invokeRoutine.Substring(0, invokeRoutine.Length - (temp.Length));
            Console.WriteLine(int.Parse(temp) * mult);
            return int.Parse(temp) * mult;
        }


    }
}

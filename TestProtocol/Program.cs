using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Diagnostics;

namespace TestProtocol
{







    /// <summary>
    /// possable inputs
    ///  -- Keys in xml form
    ///  -- number seq for steam game 
    ///  
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>




    class Program
    {


        static int Main(string[] args)
        {
            if (args.Length < 0) {
                Console.WriteLine("This process was called incorrectly :(");
                return new Random().Next();
            }
            string NEW = args[0].Substring(args[0].IndexOf("://")+3);
            NEW = NEW.Replace("%13","{ENTER}");
            Match replace = Regex.Match(NEW, "%(\\d|A|B|C|D|E|F)(\\d|A|B|C|D|E|F)");
            while (replace.Success)
            {
                char replacement = (char)byte.Parse(replace.Value.Replace("%", ""),System.Globalization.NumberStyles.HexNumber);
                NEW = NEW.Replace(replace.Value, "" + replacement);
                replace = Regex.Match(NEW, "%(\\d|A|B|C|D|E|F)(\\d|A|B|C|D|E|F)");
            }
            args = NEW.Split(new char[] { '\\' },StringSplitOptions.RemoveEmptyEntries);
            string current = "";
            try
            {

                int CurrentGame = -1;
                foreach (string s2 in args)
                {
                    string s = s2.Trim();
                    current = s;
                    //steam process
                    if (Regex.Match(s, "^\\d*$").Value == s)
                    {
                        CurrentGame = int.Parse(s);
                        Console.WriteLine("gameCode");
                        Process.Start("Steam://app/" + s);
                        //Console.WriteLine("\t" + s);
                    }
                    //xml object for keypress
                    else if (Regex.Match(s.ToLower(), "^<keypress.*/>$").Value == s.ToLower())
                    {
                        Console.WriteLine("XML String");
                        s = s.ToLower();
                        XmlSerializer ser = new XmlSerializer(typeof(keypress));
                        s= "<?xml version=\"1.0\" encoding=\"utf - 8\"?>" + s;
                        s = s.Replace("/>", "></keypress>");
                        keypress press = (keypress)ser.Deserialize(new StringReader(s));
                        press.Invoke(CurrentGame);
                        //Console.WriteLine("\t" + s);
                    }
                    else
                    {
                        Console.WriteLine("other");
                        Console.WriteLine("\t" + s);
                        Console.WriteLine("\t" + s.Length);
                        Console.WriteLine(Regex.Match(s.ToLower(), "^.*$").Value);
                    }

                    //Console.WriteLine("\t" + ProcessInput(s));
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(current);
            }
            //Console.WriteLine("\nPress any key to continue...");
            ////Console.ReadKey();
            //SendKeys.SendWait("%{Tab}{`}Open 74.81.90.101");


            //SendKeys.SendWait("{`}");
            //Console.ReadKey();
            Console.ReadKey();
            return 0;
        }
    }
}

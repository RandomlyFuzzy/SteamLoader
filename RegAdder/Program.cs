using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace RegAdder
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string Location = "";
                foreach (var item in args)
                {
                    Location += "\"" + item + "\" ";
                }
                RegistryKey key;
                key = Registry.ClassesRoot.CreateSubKey("steamLoader");
                key.SetValue("URL Protocol", "");
                key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command").SetValue("", Location);
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}

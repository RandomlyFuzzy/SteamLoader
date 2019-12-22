using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebServer
{
    public enum months { 
        Jan,
        Feb,
        Mar,
        Apr,
        May,
        Jun,
        Jul,
        Aug,
        Sep,
        Oct,
        Nov,
        Dec
    }

    class Program
    {
        static void Main(string[] args)
        {




            TcpListener listen = new TcpListener(81);





            listen.Start();
            TcpClient clie = listen.AcceptTcpClient();

            var ns = clie.GetStream();
            byte[] arr = new byte[4096];
            int len = ns.Read(arr, 0, 4096);
            Console.WriteLine(Encoding.ASCII.GetString(arr,0, len));
            var v = DateTime.Now.DayOfWeek.ToString().Substring(0,3)+", ";
            v += DateTime.Now.Day+" ";
            v += ((months)DateTime.Now.Month-1).ToString().Substring(0,3)+" ";
            v += DateTime.Now.Year.ToString()+" ";
            v += DateTime.Now.ToLongTimeString();
            Console.WriteLine(v);

            string body = $@"
<!DOCTYPE html>
<html>
<body>
hello world
</body>
</html>
";
            string header = $@"
HTTP/1.1 200 ok
Content-Type: text/html; charset=ASCII
Referrer-Policy: no-referrer
Content-Length: {body.Length}
Date: {v}
Cache-Control: no-cache

" +body;


            ns.Write(Encoding.ASCII.GetBytes(header), 0, header.Length);
            Console.WriteLine(header);


            Console.ReadKey();
            ns.Flush();
            ns.Close();
            listen.Stop();

        }
    }
}

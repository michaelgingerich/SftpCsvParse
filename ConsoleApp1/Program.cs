using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            SftpHandler s = new SftpHandler();
            s.LoadConfig();
            s.Connect();
            CsvParser c = new CsvParser();
            c.Parse();
            /*Output*
             *List<string[]> output = c.GetParsedRows(); 
            */
            //Console.ReadLine();
        }
    }
}

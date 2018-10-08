using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class CsvParser
    {
        List<string[]> parsedRows;

        public CsvParser()
        {

        }

        public List<string[]> GetParsedRows()
        {
            return parsedRows;
        }

        public void Parse()
        {
            FileHandler f = new FileHandler();
            f.DeterminePendingFileNames();
            string[] pendingFileNames = f.GetPendingFileNames();
            parsedRows = new List<string[]>();
            string header = null;
            foreach (string file in pendingFileNames)
            {
                
                try
                {
                    using (StreamReader sr = File.OpenText(
                        System.IO.Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory + @"\pending", file)))
                    {
                        string s = String.Empty;
                        while ((s = sr.ReadLine()) != null)
                        {
                            if (header == null)
                            {
                                header = s;
                                parsedRows.Add(s.Split(','));
                            }
                            else if (!s.Equals(header, StringComparison.OrdinalIgnoreCase))
                            {
                                parsedRows.Add(s.Split(','));
                            }
                        }
                        using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                        {
                            sw.WriteLine(DateTime.Now + "\tThe program has parsed " + AppDomain.CurrentDomain.BaseDirectory + @"pending\" + file + " successfully.");
                        }
                    }
                    f.MoveCompleteFile(file);
                }
                catch (Exception e)
                {
                    using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                    {
                        sw.WriteLine(DateTime.Now + "\tThe program has encountered an issue with opening " + AppDomain.CurrentDomain.BaseDirectory + @"pending\" + file + ".");
                    }
                }
            }
        }
    }
}
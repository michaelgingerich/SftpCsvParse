using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class FileHandler
    {
        string[] pendingFileNames;
        string[] completeFileNames;

        public FileHandler()
        {

        }

        public string[] GetPendingFileNames()
        {
            return pendingFileNames;
        }

        public string[] GetCompleteFileNames()
        {
            return completeFileNames;
        }

        /*Sources*
         **https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-copy-delete-and-move-files-and-folders 
        */
        public void DetermineCompleteFileNames()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"\complete";
            try
            {
                string[] files = System.IO.Directory.GetFiles(dir);
                completeFileNames = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    completeFileNames[i] = System.IO.Path.GetFileName(files[i]);
                }
            }
            catch(Exception e)
            {
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                {
                    sw.WriteLine(DateTime.Now + "\tThe program has encountered a problem with access to the complete file cache at " + AppDomain.CurrentDomain.BaseDirectory + @"complete.");
                }
            }
        }

        public void DeterminePendingFileNames()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"\pending";
            try
            {
                string[] files = System.IO.Directory.GetFiles(dir);
                pendingFileNames = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    pendingFileNames[i] = System.IO.Path.GetFileName(files[i]);
                }
            }
            catch(Exception e)
            {
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                {
                    sw.WriteLine(DateTime.Now + "\tThe program has encountered a problem with access to the pending file cache at " + AppDomain.CurrentDomain.BaseDirectory + @"pending.");
                }
            }
        }

        public bool IsComplete(string s)
        {
            bool complete = false;
            if (completeFileNames == null)
            {
                DetermineCompleteFileNames();
            }
            /*Potential performance improvement
             **https://stackoverflow.com/questions/21379255/fastest-way-to-search-in-a-string-collection
            */
            for (int i = 0; i < completeFileNames.Length; i++)
            {
                /*Source*
                 **https://docs.microsoft.com/en-us/dotnet/csharp/how-to/compare-strings
                */
                if (completeFileNames[i].Equals(s, StringComparison.OrdinalIgnoreCase))
                {
                    complete = true;
                }
            }
            return complete;
        }

        public void MoveCompleteFile(string s)
        {
            string pendingDir = AppDomain.CurrentDomain.BaseDirectory + @"\pending";
            string completeDir = AppDomain.CurrentDomain.BaseDirectory + @"\complete";
            /*Source*
             **https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-copy-delete-and-move-files-and-folders
            */
            try
            {
                System.IO.File.Move(
                    System.IO.Path.Combine(pendingDir, s),
                    System.IO.Path.Combine(completeDir, s));
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                {
                    sw.WriteLine(DateTime.Now + "\tThe program has moved " + s + @" successfully from the pending file cache at " + AppDomain.CurrentDomain.BaseDirectory + @"pending to the complete file repository at " + AppDomain.CurrentDomain.BaseDirectory + @"complete.");
                }
            }
            catch (Exception e)
            {
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                {
                    sw.WriteLine(DateTime.Now + "\tThe program has encountered a problem with moving " + s + @" from the pending file cache at " + AppDomain.CurrentDomain.BaseDirectory + @"pending to the complete file repository at " + AppDomain.CurrentDomain.BaseDirectory + @"complete.");
                }
            }
        }
    }
}

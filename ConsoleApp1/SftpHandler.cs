using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;

namespace ConsoleApp1
{
    class SftpHandler
    {
        string host;
        string port;
        string userName;
        string passwordAuthenticationMethod;
        string privateKeyAuthenticationMethod;
        string passPhrase;
        string privateKeyFileLocation;
        string ftpDirectory;

        public SftpHandler()
        {

        }

        public void LoadConfig()
        {
            try
            {
                host = ConfigurationManager.AppSettings.Get("Host");
                port = ConfigurationManager.AppSettings.Get("Port");
                userName = ConfigurationManager.AppSettings.Get("UserName");
                passwordAuthenticationMethod = ConfigurationManager.AppSettings.Get("PasswordAuthenticationMethod");
                privateKeyAuthenticationMethod = ConfigurationManager.AppSettings.Get("PrivateKeyAuthenticationMethod");
                passPhrase = ConfigurationManager.AppSettings.Get("PassPhrase");
                privateKeyFileLocation = ConfigurationManager.AppSettings.Get("PrivateKeyFileLocation");
                ftpDirectory = ConfigurationManager.AppSettings.Get("FTPDirectory");
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                {
                    sw.WriteLine(DateTime.Now + "\tThe program has loaded the authentication configuration successfully.");
                }
            }
            catch (Exception e)
            {
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                {
                    sw.WriteLine(DateTime.Now + "\tThe program has encountered a problem with loading the authentication configuration.");
                }
            }
        }

        public void Connect()
        {


            /*Sources*
             **https://github.com/sshnet/SSH.NET
             **https://lluisfranco.com/2017/11/29/how-to-connect-via-sftp-using-ssh-net/
            */
            /*Authentication Methods*
             **SftpClient can accept ConnectionInfo with 
             **PasswordAuthenticationMethod
             **PrivateKeyAuthenticationMethod
             **PasswordAuthenticationMethod AND PrivateKeyAuthenticationMethod
            */
            ConnectionInfo connectionInfo = null;
            if (passwordAuthenticationMethod.Length > 0)
            {
                connectionInfo = new ConnectionInfo(host, userName, new PasswordAuthenticationMethod(userName, passPhrase));
            }
            else if(privateKeyAuthenticationMethod.Length > 0)
            {
                //var connectionInfo = new ConnectionInfo(host, port, username,new PrivateKeyAuthenticationMethod(username, privateKeyFile));
                var key = File.ReadAllText(privateKeyFileLocation);
                var buf = new MemoryStream(Encoding.UTF8.GetBytes(key));
                var privateKeyFile = new PrivateKeyFile(buf, passPhrase);
                connectionInfo = new ConnectionInfo(host, Convert.ToInt32(port), userName, new PrivateKeyAuthenticationMethod(userName, privateKeyFile));
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                {
                    sw.WriteLine(DateTime.Now + "\tThe program requires an authentication configuration from the App.Config file.  The application is " +
                        "terminating because the App.Config file does not contain a complete authentication configuration.");
                }
                System.Environment.Exit(0);
            }
            using (var client = new SftpClient(connectionInfo))
            {
                client.Connect();
                var files = client.ListDirectory(ftpDirectory);
                //string dest = ".\\";
                string dest = AppDomain.CurrentDomain.BaseDirectory + @"\pending";
                using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true))
                {
                    FileHandler f = new FileHandler();
                    foreach (var file in files)
                    {
                        if (!file.IsDirectory && !f.IsComplete(file.Name))
                        {
                            using (var targetFile = new FileStream(Path.Combine(dest, file.Name), FileMode.Create))
                            {
                                try
                                {
                                    client.DownloadFile(file.FullName, targetFile);
                                    targetFile.Close();
                                    sw.WriteLine(DateTime.Now + "\tThe program has written " + AppDomain.CurrentDomain.BaseDirectory + @"pending\" + file.Name + " successfully.");
                                }
                                catch (Exception e)
                                {
                                    sw.WriteLine(DateTime.Now + "\tThe program has encountered a problem with writing " + AppDomain.CurrentDomain.BaseDirectory + @"pending\" + file.Name + ".");
                                }
                            }
                        }
                    }
                    client.Disconnect();
                }
            }
        }
    }
}

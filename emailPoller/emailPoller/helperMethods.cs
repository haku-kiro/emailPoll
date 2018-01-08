using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AE.Net.Mail;
using System.IO;
using System.Configuration;

namespace emailPoller
{
    class helperMethods
    {
        public void useMessage(MailMessage item)
        {
            //Console.WriteLine(item.Body);
            string pathDir = AppDomain.CurrentDomain.BaseDirectory + @"\inbox";
            if (!Directory.Exists(pathDir))
            {
                Directory.CreateDirectory(pathDir);
            }

            System.DateTime d = System.DateTime.Now;
            System.Globalization.CultureInfo cur = new System.Globalization.CultureInfo("en-US");
            string sdate = d.ToString("yyyyMMddHHmmss", cur); //probably change the format
            string filePath = String.Format("{0}\\{1}{2}", pathDir, sdate, d.Millisecond.ToString("d3"));

            Directory.CreateDirectory(filePath);

            //message to file
            using (StreamWriter sw = new StreamWriter(filePath + "\\EmailContents.txt", true))
            {
                sw.WriteLine("Subject: " + item.Subject);
                sw.WriteLine("\nBody: " + item.Body);
            }

            foreach (var attch in item.Attachments)
            {
                Directory.CreateDirectory(filePath + "\\Attachments");
                attch.Save(filePath + "\\Attachments\\" + attch.Filename);
            }

        }

        public void pollServer(string filter)
        {
            List<MailMessage> inbox = new List<MailMessage>();

            try
            {
                EmailConfiguration email = new EmailConfiguration();
                email.POPServer = ConfigurationManager.AppSettings["host"];
                email.POPUsername = ConfigurationManager.AppSettings["username"];
                email.POPpassword = ConfigurationManager.AppSettings["password"];
                email.IncomingPort = ConfigurationManager.AppSettings["port"];
                email.IsPOPssl = true;


                ImapClient imap = new ImapClient(email.POPServer, email.POPUsername, email.POPpassword, AuthMethods.Login, Convert.ToInt32(email.IncomingPort), (bool)email.IsPOPssl);

                //var messages = imap.SearchMessages(SearchCondition.Unseen(), false, true);

                //List<MailMessage> messages = imap.GetMessages(0, imap.GetMessageCount() - 1, false, false,fa);

                for (int i = imap.GetMessageCount() - 1; i >= 0; i--)
                {
                    var message = imap.GetMessage(i, false, false);
                    if (message.Subject == filter && message.Flags != Flags.Seen) 
                    {
                        // Console.WriteLine(message.Attachments); //figure out how to get attachments to work
                        MailMessage outMessage = new MailMessage(); //should probably just pass the message x_x
                        outMessage.Subject = message.Subject;
                        outMessage.Body = message.Body;

                        if (message.Attachments != null)
                        {
                            outMessage.Attachments = message.Attachments;
                        }

                        imap.AddFlags(Flags.Seen, message);
                        

                        //callback, better to use a delegate here
                        useMessage(outMessage);
                    }
                }
            }
            catch (Exception ex)
            {
               // Console.WriteLine(ex.Message);
                throw;
            }
        }

        //writelogging methods
    }
}

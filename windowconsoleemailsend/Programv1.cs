using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Windows.Forms;

namespace windowconsoleemailsend
{
    class Programv1
    {
        static void Main(string[] args)
        {
            //using thread to send 
            //sendemailusingthread();
           sendemailnow();
        }

        private static void sendemailnow()
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();

                message.From = new MailAddress("jcdesign2000@gmail.com");
                message.To.Add(new MailAddress("jcdesign2000@gmail.com"));
                message.Subject = "Test";
                message.Body = "Content";

                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("jcdesign2000@gmail.com", "moon1son");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException
                  ("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}

using EASendMail;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace windowconsoleemailsend
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i <= 50; i++)
            {
                insert20000records(i);
            }

                sendemailusingthreadwithDB();


        }

        private static void insert20000records(int newid)
        {
            {
                try
                {


                    string connectionString =
                        "Data Source=rosebloom.arvixe.com;Initial Catalog=HowtoDB;User ID=bbgreendragon;Password=yuelong";
                    using (SqlConnection conn =
                        new SqlConnection(connectionString))
                    {
                        conn.Open();

                        using (SqlCommand cmd =
                      new SqlCommand("INSERT INTO [bbgreendragon].[EmailNotification] (Id,Email ,sent) VALUES(" + "@Id, @NewEmail, @sent)", conn))
                        {
                            cmd.Parameters.AddWithValue("@Id", newid);
                            cmd.Parameters.AddWithValue("@NewEmail", "asusal@gmail.com");
                            cmd.Parameters.AddWithValue("@sent", 0);
                          
                            int rows = cmd.ExecuteNonQuery();

                            //rows number of record got inserted
                            Console.Write("{0}", newid.ToString());
                        }
                    }
                }
                catch (SqlException ex)
                {
                    //Log exception
                    //Display Error message
                }
            }
        }

        private static void sendemailusingthreadwithDB()
        {
            string[] emailaddr = new string[200000] ;
            int[] idaddr = new int[200000];
            SqlDataReader rdr = null;

          
            SqlConnection conn = new SqlConnection(
 "Data Source=rosebloom.arvixe.com;Initial Catalog=HowtoDB;User ID=bbgreendragon;Password=yuelong");

            
            SqlCommand cmd = new SqlCommand(
                "select * from bbgreendragon.EmailNotification where sent=0", conn);

            try
            {
                
                conn.Open();

               
                rdr = cmd.ExecuteReader();

  
             
                int linecount = 0;
                while (rdr.Read())
                {  
                    // get the results of each column
                    string contact = (string)rdr["Email"];
                    int Id = (int)rdr["Id"];
                  
                    emailaddr[linecount] = Convert.ToString(contact);
                    idaddr[linecount] = Id;
                  
                    linecount++;
                }
                ////process///
              
                SmtpMail[] smtpMailList = new SmtpMail[linecount];
                SmtpClient[] smtpClientList = new SmtpClient[linecount];
                SmtpClientAsyncResult[] ResultList = new SmtpClientAsyncResult[linecount];
                for (int i = 0; i < linecount; i++)
                {
                    smtpMailList[i] = new SmtpMail("TryIt");
                    smtpClientList[i] = new SmtpClient();
                }

                for (int i = 0; i < linecount; i++)
                {
                    SmtpMail myEmail = smtpMailList[i];
                   
                    myEmail.From = "asusal2000@gmail.com";

                   
                    myEmail.To = emailaddr[i];

                  
                    myEmail.Subject = "Sending 20000 emails QA  " + i.ToString();

                   
                    myEmail.HtmlBody = "<U>Test Mass Mail</U><br>with <b>Sample content goes here</b>." + "URL : <a href='http://www.google.com'>Sample link</a>" + emailaddr[i] + " the index of array is " + i.ToString(); 
                    myEmail.TextBody = "plain body text" + emailaddr[i] + " the index of array is " + i.ToString(); ;

                    Console.WriteLine(System.Text.Encoding.ASCII.GetString(myEmail.EncodedContent));
                   
                    SmtpServer mySmtpServer = new SmtpServer("smtp.gmail.com");

                    // User and password for ESMTP authentication, if your server doesn't require
                    // User authentication, please remove the following codes.
                    mySmtpServer.User = "asusal2000@gmail.com";
                    mySmtpServer.Password = "asusal";

                    mySmtpServer.Port = 465;
                    // If your smtp server requires SSL connection, please add this line
                    mySmtpServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                    SmtpClient mysmtpclient = smtpClientList[i];

                    // Submit email to BeginSendMail method and return
                    // to process another email
                    ResultList[i] = mysmtpclient.BeginSendMail(mySmtpServer, myEmail, null, null);
                    Console.WriteLine(String.Format("Start to send email to {0} ...with index {1}",
                        emailaddr[i], i.ToString()));
                }
                // All emails were sent by BeginSendMail Method
                // now get result by EndSendMail method
                int nSent = 0;
                while (nSent < linecount)
                {
                    for (int i = 0; i < linecount; i++)
                    {
                        // this email has been sent
                        if (ResultList[i] == null)
                            continue;
                        // wait for specified email ...
                        if (!ResultList[i].AsyncWaitHandle.WaitOne(1000, false))
                        {
                            continue;
                        }
                        try
                        {
                            // this email is finished, using EndSendMail to get result
                            smtpClientList[i].EndSendMail(ResultList[i]);
                            Console.WriteLine(String.Format("Send email to {0} successfully,index of array is {1}",
                                emailaddr[i], i.ToString()));

                            // update related msg
                            updateemailstatus(emailaddr[i], idaddr[i]);
                        }
                        catch (Exception ep)
                        {
                            Console.WriteLine(
                               String.Format("Failed to send email to {0} with error {1}: ",
                               emailaddr[i], ep.Message));
                        }
                        // Set this email result to null, then it won't be processed again
                        ResultList[i] = null;
                        nSent++;
                    }
                }
                ///end ofprocess

            }
            finally
            {
                // 3. close the reader
                if (rdr != null)
                {
                    rdr.Close();
                }

                // close the connection
                if (conn != null)
                {
                    conn.Close();
                }
            }
       
        }

        private static void updateemailstatus(string strEmail, int intEmailID)
        {
            try
            {
                string connectionString = "Data Source=rosebloom.arvixe.com;Initial Catalog=HowtoDB;User ID=bbgreendragon;Password=yuelong";

                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd =
                        new SqlCommand("UPDATE bbgreendragon.EmailNotification SET sent=1" +
                                            " WHERE Id=@Id and Email=@NewEmail", conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", intEmailID);
                         cmd.Parameters.AddWithValue("@NewEmail", strEmail);
                        //  cmd.Parameters.AddWithValue("@Address", "Kerala");

                        int rows = cmd.ExecuteNonQuery();

                        //rows number of record got updated
                    }
                }
            }
            catch (SqlException ex)
            {
                
                Console.WriteLine(String.Format("update email status for ({0}) - Failed ,ID of email is {1}. Reason {2}", strEmail, intEmailID,ex.Message));
            }
        }

    
    

       
    }
}

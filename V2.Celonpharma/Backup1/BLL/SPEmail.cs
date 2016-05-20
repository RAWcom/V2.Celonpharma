using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Net.Mail;
using System.Collections.Specialized;
using Microsoft.SharePoint.Utilities;
using System.Net;

namespace SPEmail
{
    public class EmailGenerator
    {
        public static void SendMail(SPWeb web, string from, string to, string subject, string body, bool isBodyHtml, string cc, string bcc)
        {
            StringDictionary headers = new StringDictionary();
            headers.Add("from", from);
            headers.Add("to", to);
            headers.Add("subject", subject);
            if (!String.IsNullOrEmpty(cc)) headers.Add("cc", cc);
            if (!String.IsNullOrEmpty(bcc)) headers.Add("bcc", bcc);
            headers.Add("content-type", "text/html");
            SPUtility.SendEmail(web, headers, body);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Net.Mail;

namespace BLL
{
    public class Tools
    {
        //Email

        const string sender = @"noreply@stafix24.pl";

        public static void SendWarningMessageToGroup(SPWeb web, string groupName, string subject, string bodyHTML)
        {
            SPGroupCollection groupCollection = web.SiteGroups;
            foreach (SPGroup group in groupCollection)
            {
                if (group.Name == groupName)
                {
                    string sender1 = sender;
                    if (!string.IsNullOrEmpty(web.Site.RootWeb.Title)) sender1 = string.Format(@"{1}<{0}>",
                                                                                    sender,
                                                                                    web.Site.RootWeb.Title);
                    

#if DEBUG
                    SPEmail.EmailGenerator.SendMail(web, sender1, web.CurrentUser.Email, subject, bodyHTML, true, string.Empty, string.Empty);
#else                          
                    SPUserCollection userCollection = group.Users;
                    foreach (SPUser user in userCollection)
                    {
                        if (IsValidEmailAddress(user.Email))
                        {
                            SPEmail.EmailGenerator.SendMail(web, sender1, user.Email, subject, bodyHTML, true, string.Empty, string.Empty);
                        }
                    }
                    break;
#endif
                }
            }

        }

        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

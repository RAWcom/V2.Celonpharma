using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Text;

namespace EventReceivers.RozliczeniaER
{
    public class RozliczeniaER : SPItemEventReceiver
    {
        const string sender = "noreply@stafix24.pl";
        const string senderName = "STAFix24 Robot";
        const int targetTimeSpan = 45;

        public override void ItemUpdating(SPItemEventProperties properties)
        {
            SPListItem item = properties.ListItem;

            DateTime modified = DateTime.Parse(item["Created"].ToString());

            if (modified < DateTime.Now.AddDays(-1 * targetTimeSpan))
            {
                if (UserIsMemberOf(properties, "V2 Owners"))
                {
                    //może edytować
                }
                else
                {
                    //przerywa operację ale bez komunikatów
                    properties.Status = SPEventReceiverStatus.CancelNoError;

                    //wyślij wiadomość do właściciela witryny
                    string subject = string.Format(@"Powstrzymana próba edycji rekordu starszego niż {0} dni.", targetTimeSpan.ToString());

                    string editFormUrl = properties.Web.Site.Url + properties.List.DefaultEditFormUrl + "?ID=" + item.ID.ToString();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Powstrzymana próba <b>edycji</b> rekordu <a href='{3}'>ID={0}</a> w rejestrze 'Rozliczenia' przez {1} o godzinie {2}",
                        item.ID.ToString(),
                        properties.Web.CurrentUser.Name,
                        DateTime.Now.ToString(),
                        editFormUrl);
                    string bodyHTML = string.Format("<div>{0}</div>", sb.ToString());

                    BLL.Tools.SendWarningMessageToGroup(item.Web, "V2 Owners", subject, bodyHTML);
                }
            }
            else
            {
                base.ItemUpdated(properties);
            }

        }

        public override void ItemDeleting(SPItemEventProperties properties)
        {
            if (properties.ListItem.ContentType.Name == "Zaliczka")
            {
                if (UserIsMemberOf(properties, "V2 Owners"))
                {
                    //może kasować
                }
                else
                {
                    //przerywa operację ale bez komunikatów
                    properties.Status = SPEventReceiverStatus.CancelNoError;


                    //przerywa operację ale bez komunikatów
                    properties.Status = SPEventReceiverStatus.CancelNoError;

                    SPListItem item = properties.ListItem;

                    //wyślij wiadomość do właściciela witryny
                    string subject = string.Format(@"Powstrzymana próba usunięcia rekordu", targetTimeSpan.ToString());

                    string editFormUrl = properties.Web.Site.Url + properties.List.DefaultEditFormUrl + "?ID=" + item.ID.ToString();
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Powstrzymana próba <b>usunięcia</b> rekordu <a href='{3}'>ID={0}</a> typu 'Zaliczka' w rejestrze 'Rozliczenia' przez {1} o godzinie {2}",
                        item.ID.ToString(),
                        properties.Web.CurrentUser.Name,
                        DateTime.Now.ToString(),
                        editFormUrl);
                    string bodyHTML = string.Format("<div>{0}</div>", sb.ToString());

                    BLL.Tools.SendWarningMessageToGroup(item.Web, "V2 Owners", subject, bodyHTML);

                }
            }
        }

        private bool UserIsMemberOf(SPItemEventProperties properties, string groupName)
        {
            SPUser user = properties.Web.CurrentUser;
            SPGroupCollection groupCollection = user.Groups;

            foreach (string group in groupCollection)
            {
                if (group == groupName) return true;
            }

            return false;
        }

        #region Helpers
        private bool UserCanDelete(SPItemEventProperties properties)
        {
            SPUser user = properties.Web.CurrentUser;
            SPList list = properties.List;
            return list.DoesUserHavePermissions(user, SPBasePermissions.DeleteListItems);
        }
        #endregion

    }
}

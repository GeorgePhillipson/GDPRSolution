using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Editors;
using Web.Security;

namespace Web.Core.Helpers
{
    public class BackOfficeHelper : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Saving += Content_Saving;
             
            EditorModelEventManager.SendingContentModel += (sender, e) =>
            {
                var node = e.Model.Properties.ToList();

                if (e.Model.IsChildOfListView && e.Model.ContentTypeAlias == "example")
                {
                    string firstNameDecrypted   = node.Where(x => x.Alias.ToLower() == "firstname").Select(x => x.Value).First().ToString();

                    var firstName   = e.Model.Properties.FirstOrDefault(x => x.Alias.ToLower() == "firstname");

                    //string decryptFirstName = EncryptionDecryption.DecryptString(firstNameDecrypted,"SupplyPassPhrase");
                    string decryptFirstName = EncryptionDecryption.DecryptWithNoPassPhrase(firstNameDecrypted);

                    if (firstName?.Value != null)
                    {
                        firstName.Value = $"{decryptFirstName}";
                    }
                }
            };
        }

        private void Content_Saving(IContentService sender, SaveEventArgs<IContent> e)
        {
            var node = e.SavedEntities.ToList();

            foreach (var items in node.Where(x => x.ContentType.Alias == "example"))
            {
                string firstName = items.GetValue<string>("firstName");

                //string encryptFirstName = EncryptionDecryption.EncryptString(firstName, "SupplyPassPhrase");
                string encryptFirstName = EncryptionDecryption.EncryptWithNoPassPhrase(firstName);

                items.SetValue("firstName", encryptFirstName);

                sender.SaveAndPublishWithStatus(items, 0, false);
            }
        }
    }
}

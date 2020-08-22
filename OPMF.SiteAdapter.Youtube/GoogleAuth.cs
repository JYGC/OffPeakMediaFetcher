using System;
using System.IO;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace OPMF.SiteAdapter.Youtube
{
    class GoogleAuth
    {
        public static UserCredential GetCredential(string[] scope)
        {
            UserCredential credential;

            using (FileStream stream = new FileStream("gmail-python-quickstart.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                Console.WriteLine("authenticate with google");
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scope,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("credential file saved to: " + credPath);
            }

            return credential;
        }
    }
}

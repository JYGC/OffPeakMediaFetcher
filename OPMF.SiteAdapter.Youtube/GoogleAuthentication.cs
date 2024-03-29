﻿using System;
using System.IO;
using System.Reflection;
using System.Threading;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace OPMF.SiteAdapter.Youtube
{
    public class GoogleAuthentication
    {
        public static UserCredential GetCredential(string[] scope)
        {
            UserCredential credential;

            Uri location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            FileInfo execFileInfo = new FileInfo(location.AbsolutePath);
            using (FileStream stream = new FileStream(Path.Join(execFileInfo.Directory.FullName, Settings.ConfigHelper.Config.GoogleClientSecretPath), FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                Console.WriteLine("authenticate with google");
                string credPath = Settings.ConfigHelper.ReadonlySettings.GetCredentialPath();
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

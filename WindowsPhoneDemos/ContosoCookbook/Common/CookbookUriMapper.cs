// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace ContosoCookbook.Common
{
    class CookbookUriMapper : UriMapperBase
    {
        private static string TargetPageName = "RecipeDetailPage.xaml";
        private static string ProtocolTemplate = "/Protocol?encodedLaunchUri=";
        private static string FileTemplate = "/FileTypeAssociation?fileToken=";
        private static int ProtocolTemplateLength = ProtocolTemplate.Length;

        private string tempUri;

        public override Uri MapUri(Uri uri)
        {
            tempUri = uri.ToString();

            if (tempUri.Contains("/Protocol"))
            {
                tempUri = HttpUtility.UrlDecode(tempUri);

                if (tempUri.Contains("recipe:"))
                {
                    return GetMappedUri(tempUri);
                }

            }

            if (tempUri.Contains("/FileTypeAssociation"))
            {
                tempUri = HttpUtility.UrlDecode(tempUri);

                if (tempUri.Contains("fileToken"))
                {
                    return GetFileMappedUri(tempUri);
                }
            }
            return uri;
        }

        private Uri GetFileMappedUri(string uri)
        {
            string fileToken = "";

            // Extract parameter values from URI.
            if (uri.IndexOf(FileTemplate) > -1)
            {
                int groupIdLen = uri.IndexOf("=", 0);
                fileToken = uri.Substring(groupIdLen + 1);
            }

            string NewURI = String.Format("/{0}?ID={1}&Command=HandleFile", TargetPageName, fileToken);

            return new Uri(NewURI, UriKind.Relative);
        }

        private Uri GetMappedUri(string uri)
        {
            string operation = "";
            string groupUID = "";

            // Extract parameter values from URI.
            if (uri.IndexOf(ProtocolTemplate) > -1)
            {
                int operationLen = uri.IndexOf("?", ProtocolTemplateLength);
                int groupIdLen = uri.IndexOf("=", operationLen + 1);

                operation = uri.Substring(ProtocolTemplateLength, operationLen - ProtocolTemplateLength);
                groupUID = uri.Substring(groupIdLen + 1);
            }

            string NewURI = String.Format("/{0}?ID={1}", TargetPageName, groupUID);

            return new Uri(NewURI, UriKind.Relative);
        }
    }
}

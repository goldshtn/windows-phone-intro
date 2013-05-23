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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using ContosoCookbook.Data;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Notification;
using System.Text;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;
using System.IO.IsolatedStorage;
using Microsoft.Phone;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using ContosoCookbook.Common;
using Windows.Phone.Storage.SharedAccess;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage.Streams;
using Windows.Phone.System.UserProfile;

namespace ContosoCookbook
{
    public partial class RecipeDetailPage : PhoneApplicationPage
    {
        CameraCaptureTask camera;
        private RecipeDataItem item;
        private const string RemoveAlarmUri = "/Assets/Icons/alarmRemove.png";
        private const string AlarmUri = "/Assets/Icons/alarm.png";

        private const string RemoveFavUri = "/Assets/Icons/unlike.png";
        private const string FavUri = "/Assets/Icons/like.png";

        public RecipeDetailPage()
        {
            InitializeComponent();

            camera = new CameraCaptureTask();
            camera.Completed += camera_Completed;
        }

        public ApplicationBarIconButton alarmBtn
        {
            get
            {
                var appBar = (ApplicationBar)ApplicationBar;
                var count = appBar.Buttons.Count;
                for (var i = 0; i < count; i++)
                {
                    ApplicationBarIconButton btn = appBar.Buttons[i] as ApplicationBarIconButton;
                    if (btn.IconUri.OriginalString.Contains("alarm"))
                        return btn;
                }
                return null;
            }
        }

        public ApplicationBarIconButton pinBtn
        {
            get
            {
                var appBar = (ApplicationBar)ApplicationBar;
                var count = appBar.Buttons.Count;
                for (var i = 0; i < count; i++)
                {
                    ApplicationBarIconButton btn = appBar.Buttons[i] as ApplicationBarIconButton;
                    if (btn.IconUri.OriginalString.Contains("like"))
                        return btn;
                }
                return null;
            }
        }

        void SetPinBar()
        {
            var uri = NavigationService.Source.ToString();
            if (Features.Tile.TileExists(uri))
            {
                pinBtn.IconUri = new Uri(RemoveFavUri, UriKind.Relative);
                pinBtn.Text = "Unpin";
            }
            else
            {
                pinBtn.IconUri = new Uri(FavUri, UriKind.Relative);
                pinBtn.Text = "Pin";
            }
        }

        void SetScheduleBar(string name)
        {
            var isScheduled = Features.Notifications.IsScheduled(name);

            if (isScheduled)
            {
                alarmBtn.IconUri = new Uri(RemoveAlarmUri, UriKind.Relative);
                alarmBtn.Text = "Remove Alarm";
            }
            else
            {
                alarmBtn.IconUri = new Uri(AlarmUri, UriKind.Relative);
                alarmBtn.Text = "Set Alarm";
            }
        }

        async void btnPinToStart_Click(object sender, EventArgs e)
        {
            var uri = NavigationService.Source.ToString();
            if (Features.Tile.TileExists(uri))
                Features.Tile.DeleteTile(uri);
            else
            {
                //Setup lockscreen background once setting the recipe as favorite
                if (!LockScreenManager.IsProvidedByCurrentApplication)
                {
                    await LockScreenManager.RequestAccessAsync();
                }

                if (LockScreenManager.IsProvidedByCurrentApplication)
                {
                    Uri imageUri = new Uri("ms-appx:///" + item.ImagePath.LocalPath, UriKind.RelativeOrAbsolute);
                    System.Diagnostics.Debug.WriteLine(imageUri.OriginalString);

                    LockScreen.SetImageUri(imageUri);
                }

                Features.Tile.SetTile(item, uri);
            }

            SetPinBar();
        }

        void btnTakePicture_Click(object sender, EventArgs e)
        {
            try
            {
                camera.Show();
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show("An error occurred.");
            }
        }

        void camera_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Code to display the photo on the page in an image control named myImage.
                System.Windows.Media.Imaging.BitmapImage bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.SetSource(e.ChosenPhoto);

                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!isoStore.DirectoryExists(item.Group.Title))
                        isoStore.CreateDirectory(item.Group.Title);

                    string fileName = string.Format("{0}/{1}.jpg", item.Group.Title, DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss"));

                    using (IsolatedStorageFileStream targetStream = isoStore.CreateFile(fileName))
                    {
                        WriteableBitmap wb = new WriteableBitmap(bmp);
                        wb.SaveJpeg(targetStream, wb.PixelWidth, wb.PixelHeight, 0, 100);
                        targetStream.Close();
                    }

                    if (null == item.UserImages)
                        item.UserImages = new System.Collections.ObjectModel.ObservableCollection<string>();

                    item.UserImages.Add(fileName);
                }
            }
        }

        void btnShareShareTask_Click(object sender, EventArgs e)
        {
            ShareMediaTask shareMediaTask = new ShareMediaTask();

            if (null != item.UserImages && item.UserImages.Count > 0)
                shareMediaTask.FilePath = string.Format("{0}", item.UserImages[0]);
            else
                shareMediaTask.FilePath = string.Format("{0}", item.GetImageUri());

            shareMediaTask.Show();
        }

        void btnStartCooking_Click(object sender, EventArgs e)
        {
            Features.Notifications.SetReminder(item);
            SetScheduleBar(item.UniqueId);
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string UniqueId = "";

            if (NavigationContext.QueryString.ContainsKey("Command"))
            {
                string fileToken = NavigationContext.QueryString["ID"];
                var filename = SharedStorageAccessManager.GetSharedFileName(fileToken);

                var file = await SharedStorageAccessManager.CopySharedFileAsync(Windows.Storage.ApplicationData.Current.LocalFolder,
                                                                                  fileToken + ".rcp", Windows.Storage.NameCollisionOption.ReplaceExisting,
                                                                                  fileToken);

                var content = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                DataReader dr = new DataReader(content);
                await dr.LoadAsync((uint)content.Size);

                //Get XML from file content
                string xml = dr.ReadString((uint)content.Size);

                //Load XML dpocument
                XDocument doc = XDocument.Parse(xml);
                XName attName = XName.Get("ID");
                XAttribute att = doc.Root.Attribute(attName);
                //Get UniqueId from file
                UniqueId = att.Value;
                System.Diagnostics.Debug.WriteLine("ID = " + UniqueId);
            }
            else
                UniqueId = NavigationContext.QueryString["ID"];

            if (!App.Recipes.IsLoaded)
                await App.Recipes.LoadLocalDataAsync();

            NavigateToRecipe(UniqueId);

            base.OnNavigatedTo(e);
        }

        private void NavigateToRecipe(string UniqueId)
        {
            item = App.Recipes.FindRecipe(UniqueId);
            pivot.DataContext = item;
            SetScheduleBar(item.UniqueId);
            SetPinBar();
        }
    }
}
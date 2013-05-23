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
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using ContosoCookbook.Data;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace ContosoCookbook.Common
{
    public class Features
    {
        public class License
        {
            private static LicenseInformation _info = new LicenseInformation();

            private static bool _isTrial = false;
            public static bool IsTrial
            {
                get
                {
                    return _isTrial;
                }
                set
                {
                    _isTrial = value;
                }
            }

            public static void BuyApplication()
            {
                MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();
                _marketPlaceDetailTask.Show();
            }
        }

        public class Notifications
        {
            public static void SetReminder(RecipeDataItem item)
            {
                if (!IsScheduled(item.UniqueId))
                {
                    Microsoft.Phone.Scheduler.Reminder reminder = new Microsoft.Phone.Scheduler.Reminder(item.UniqueId);
                    reminder.Title = item.Title;
                    reminder.Content = "Have you finished cooking?";
                    if (System.Diagnostics.Debugger.IsAttached)
                        reminder.BeginTime = DateTime.Now.AddMinutes(1);
                    else
                        reminder.BeginTime = DateTime.Now.Add(TimeSpan.FromMinutes(Convert.ToDouble(item.PrepTime)));
                    reminder.ExpirationTime = reminder.BeginTime.AddMinutes(5);
                    reminder.RecurrenceType = RecurrenceInterval.None;
                    reminder.NavigationUri = new Uri("/RecipeDetailPage.xaml?ID=" + item.UniqueId + "&GID=" + item.Group.UniqueId, UriKind.Relative);
                    ScheduledActionService.Add(reminder);
                }
                else
                {
                    var schedule = ScheduledActionService.Find(item.UniqueId);
                    ScheduledActionService.Remove(schedule.Name);
                }
            }

            public static bool IsScheduled(string name)
            {
                var schedule = ScheduledActionService.Find(name);
                if (schedule == null)
                {
                    return false;
                }
                else
                {
                    return schedule.IsScheduled;
                }
            }
        }

        public class Tile
        {
            public static bool TileExists(string navDataSource)
            {
                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(o => o.NavigationUri.ToString().Contains(navDataSource));
                return tile == null ? false : true;
            }

            public static void DeleteTile(string navDataSource)
            {
                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(o => o.NavigationUri.ToString().Contains(navDataSource));
                if (tile == null) return;

                tile.Delete();
            }

            public static void SetTile(RecipeDataItem item, string navDataSource)
            {
                FlipTileData tileData = new FlipTileData()
                {
                    //Front square data
                    Title = item.Title,
                    BackgroundImage = new Uri(item.GetImageUri(), UriKind.Relative),
                    SmallBackgroundImage = new Uri(item.GetImageUri(), UriKind.Relative),

                    //Back square data
                    BackTitle = item.Title,
                    BackContent = MakeString(item.Ingredients),
                    BackBackgroundImage = new Uri(item.Group.GetImageUri(), UriKind.Relative),

                    //Wide tile data
                    WideBackgroundImage = new Uri(item.GetImageUri(), UriKind.Relative),
                    WideBackBackgroundImage = new Uri(item.Group.GetImageUri(), UriKind.Relative),
                    WideBackContent = item.Directions
                };

                ShellTile.Create(new Uri(navDataSource, UriKind.Relative), tileData, true);
            }

            private static string MakeString(ObservableCollection<string> ingredients)
            {
                string res = "";

                foreach (var ingredient in ingredients)
                {
                    res += ingredient + "\n";
                }

                return res;
            }

            public static void SetGroupTile(RecipeDataGroup group, string navDataSource)
            {
                List<Uri> list = new List<Uri>();
                foreach (var recipe in group.Items)
                    list.Add(new Uri(recipe.ImagePath.LocalPath, UriKind.Relative));

                CycleTileData tileData = new CycleTileData()
                {
                    Title = group.Title,
                    SmallBackgroundImage = new Uri(group.GetImageUri(), UriKind.RelativeOrAbsolute),
                    CycleImages = list
                };

                ShellTile.Create(new Uri(navDataSource, UriKind.Relative), tileData, true);
            }

            public static void UpdateMainTile(RecipeDataGroup group)
            {
                //Get application's main tile
                var mainTile = ShellTile.ActiveTiles.FirstOrDefault();

                if (null != mainTile)
                {
                    IconicTileData tileData = new IconicTileData()
                    {
                        Count = group.RecipesCount,
                        BackgroundColor = Color.FromArgb(255, 195, 61, 39),
                        Title = "Contoso Cookbook",
                        IconImage = new Uri("/Assets/MediumLogo.png", UriKind.RelativeOrAbsolute),
                        SmallIconImage = new Uri("/Assets/SmallLogo.png", UriKind.RelativeOrAbsolute),
                        WideContent1 = "Recent activity:",
                        WideContent2 = "Browsed " + group.Title + " group",
                        WideContent3 = "which contains " + group.RecipesCount + " recipes"
                    };

                    mainTile.Update(tileData);
                }
            }

        }
    }
}

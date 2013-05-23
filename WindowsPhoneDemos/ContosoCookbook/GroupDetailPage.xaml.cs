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
using ContosoCookbook.Common;
using Microsoft.Phone.Shell;

namespace ContosoCookbook
{
    public partial class GroupDetailPage : PhoneApplicationPage
    {
        private const string RemoveFavUri = "/Assets/Icons/unpin.png";
        private const string FavUri = "/Assets/Icons/pin.png";

        RecipeDataGroup group;
        public GroupDetailPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!App.Recipes.IsLoaded)
                await App.Recipes.LoadLocalDataAsync();

            if (NavigationContext.QueryString.ContainsKey("groupName"))
            {
                string groupName = NavigationContext.QueryString["groupName"];

                group = App.Recipes.FindGroupByName(groupName);
                pivot.DataContext = group;
                SetView();
            }
            else
            {
                string UniqueId = NavigationContext.QueryString["ID"];
                group = App.Recipes.FindGroup(UniqueId);
                pivot.DataContext = group;

                SetView();
            }

            SetPinBar();

            //Update main tile with recently visited group
            Features.Tile.UpdateMainTile(group);

            base.OnNavigatedTo(e);
        }

        void BuyAppClick(object sender, EventArgs e)
        {
            //Features.License.BuyApplication();
            Features.License.IsTrial = false;
            SetView();
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
                    if (btn.IconUri.OriginalString.Contains("pin"))
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

        void SetView()
        {
            if (Features.License.IsTrial && !group.LicensedRequired)
            {
                lstRecipes.Visibility = System.Windows.Visibility.Collapsed;
                btnBuy.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                lstRecipes.Visibility = System.Windows.Visibility.Visible;
                btnBuy.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void lstRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRecipes.SelectedItems.Count > 0 && !group.LicensedRequired)
            {
                // + "&GID=" + (lstRecipes.SelectedItem as RecipeDataItem).Group.UniqueId, UriKind.Relative)
                NavigationService.Navigate(new Uri("/RecipeDetailPage.xaml?ID=" + (lstRecipes.SelectedItem as RecipeDataItem).UniqueId, UriKind.Relative));
            }
            else if (group.LicensedRequired)
            {
                var result = MessageBox.Show("Would you like to buy this product?", "Buy Product", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    group.LicensedRequired = false;
                    lstRecipes_SelectionChanged(sender, e);
                }
            }
        }

        private void btnPinToStart_Click(object sender, EventArgs e)
        {
            var uri = NavigationService.Source.ToString();
            if (Features.Tile.TileExists(uri))
                Features.Tile.DeleteTile(uri);
            else
                Features.Tile.SetGroupTile(group, uri);

            SetPinBar();
        }
    }
}
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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#if !WP7
using Windows.Foundation;
using Windows.Foundation.Collections;
//using System.Net.Http;
//using Windows.Data.Json;
using Windows.ApplicationModel;
using Windows.Storage.Streams;
#endif
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.IO.IsolatedStorage;
using ContosoCookbook.Common;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace ContosoCookbook.Data
{
    /// <summary>
    /// Base class for <see cref="RecipeDataItem"/> and <see cref="RecipeDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [DataContract]
    public abstract class RecipeDataCommon : ContosoCookbook.Common.BindableBase
    {
        internal static Uri _baseUri = new Uri("ms-appx:///");

        public RecipeDataCommon(String uniqueId, String title, String shortTitle, String imagePath)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._shortTitle = shortTitle;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        [DataMember(Name = "key")]
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        [DataMember(Name = "title")]
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _shortTitle = string.Empty;
        [DataMember(Name = "shortTitle")]
        public string ShortTitle
        {
            get { return this._shortTitle; }
            set { this.SetProperty(ref this._shortTitle, value); }
        }

        private ImageSource _image = null;
        private string _imagePath = null;

        public Uri ImagePath
        {
            get
            {
                return new Uri(RecipeDataCommon._baseUri, this._imagePath);
            }
        }

        [DataMember(Name = "backgroundImage")]
        public string BackgroundImage
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
            }
        }
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(RecipeDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public string GetImageUri()
        {
            return _imagePath;
        }
    }

    /// <summary>
    /// Recipe item data model.
    /// </summary>
    [DataContract]
    public class RecipeDataItem : RecipeDataCommon
    {
        public RecipeDataItem()
            : base(String.Empty, String.Empty, String.Empty, String.Empty)
        {
        }

        public RecipeDataItem(String uniqueId, String title, String shortTitle, String imagePath, int preptime, String directions, ObservableCollection<string> ingredients, RecipeDataGroup group)
            : base(uniqueId, title, shortTitle, imagePath)
        {
            this._preptime = preptime;
            this._directions = directions;
            this._ingredients = ingredients;
            this._group = group;
        }

        private int _preptime = 0;
        [DataMember(Name = "preptime")]
        public int PrepTime
        {
            get { return this._preptime; }
            set { this.SetProperty(ref this._preptime, value); }
        }

        private string _directions = string.Empty;
        [DataMember(Name = "directions")]
        public string Directions
        {
            get { return this._directions; }
            set { this.SetProperty(ref this._directions, value); }
        }

        private ObservableCollection<string> _userImages;
        [DataMember(Name = "userImages")]
        public ObservableCollection<string> UserImages
        {
            get { return this._userImages; }
            set { this.SetProperty(ref this._userImages, value); }
        }

        private ObservableCollection<string> _ingredients;
        [DataMember(Name = "ingredients")]
        public ObservableCollection<string> Ingredients
        {
            get { return this._ingredients; }
            set { this.SetProperty(ref this._ingredients, value); }
        }

        private RecipeDataGroup _group;
        [DataMember(Name = "group")]
        public RecipeDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Recipe group data model.
    /// </summary>
    [DataContract]
    public class RecipeDataGroup : RecipeDataCommon
    {
        public RecipeDataGroup()
            : base(String.Empty, String.Empty, String.Empty, String.Empty)
        {
        }

        public RecipeDataGroup(String uniqueId, String title, String shortTitle, String imagePath, String description)
            : base(uniqueId, title, shortTitle, imagePath)
        {
        }

        private bool _licensedRequired;
        public bool LicensedRequired
        {
            get { return _licensedRequired; }
            set { _licensedRequired = value; }
        }

        private ObservableCollection<RecipeDataItem> _items = new ObservableCollection<RecipeDataItem>();
        [DataMember(Name = "group")]
        public ObservableCollection<RecipeDataItem> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        private string _description = string.Empty;
        [DataMember(Name = "description")]
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _groupImage;
        private string _groupImagePath;
        [DataMember(Name = "groupImage")]
        public string GroupImagePath
        {
            get
            {
                return _groupImagePath;
            }
            set
            {
                _groupImagePath = value;
            }
        }

        public ImageSource GroupImage
        {
            get
            {
                if (this._groupImage == null && this._groupImagePath != null)
                {
                    this._groupImage = new BitmapImage(new Uri(RecipeDataCommon._baseUri, this._groupImagePath));
                }
                return this._groupImage;
            }
            set
            {
                this._groupImagePath = null;
                this.SetProperty(ref this._groupImage, value);
            }
        }

        public int RecipesCount
        {
            get
            {
                return this.Items.Count;
            }
        }

        public void SetGroupImage(String path)
        {
            this._groupImage = null;
            this._groupImagePath = path;
            this.OnPropertyChanged("GroupImage");
        }
    }

    [DataContract]
    public class UserRecipeImages
    {
        public UserRecipeImages(string key, ObservableCollection<string> images)
        {
            this.UniqueId = key;
            this.Images = images;
        }

        [DataMember(Name = "key")]
        public string UniqueId { get; set; }

        [DataMember(Name = "images")]
        public ObservableCollection<string> Images { get; set; }
    }

    /// <summary>
    /// Creates a collection of groups and items.
    /// </summary>
    public sealed class RecipeDataSource
    {
        public event EventHandler RecipesLoaded;
        public bool IsLoaded = false;
        public List<UserRecipeImages> UserImages = new List<UserRecipeImages>();
        private ObservableCollection<RecipeDataGroup> _itemGroups = new ObservableCollection<RecipeDataGroup>();
        public ObservableCollection<RecipeDataGroup> ItemGroups
        {
            get { return this._itemGroups; }
        }

        public RecipeDataGroup FindGroup(string uniqueId)
        {
            return (from g in ItemGroups where g.UniqueId == uniqueId select g).SingleOrDefault();
        }

        public RecipeDataGroup FindGroupByName(string groupName)
        {
            return (from g in ItemGroups where g.Title == groupName select g).SingleOrDefault();
        }

        public RecipeDataItem FindRecipe(string uniqueId)
        {
            return ItemGroups.SelectMany(g => g.Items).SingleOrDefault(i => i.UniqueId == uniqueId);
        }

        public RecipeDataItem FindRecipeByText(string text)
        {
            return ItemGroups.SelectMany(g => g.Items).SingleOrDefault(i => i.Title.ToLower().Contains(text.ToLower()));
        }

        private void AssignedUserImages(RecipeDataItem recipe)
        {
            if (UserImages != null && UserImages.Any(item => item.UniqueId.Equals(recipe.UniqueId)))
            {
                var userImages = UserImages.Single(a => a.UniqueId.Equals(recipe.UniqueId));
                recipe.UserImages = userImages.Images;
            }
        }

        public async System.Threading.Tasks.Task LoadLocalDataAsync()
        {
            // Retrieve recipe data from Recipes.txt
            var sri = App.GetResourceStream(new Uri("Data\\Recipes.txt", UriKind.Relative));
            List<Type> types = new List<Type>();
            types.Add(typeof(RecipeDataItem));
            types.Add(typeof(RecipeDataGroup));
            types.Add(typeof(RecipeDataCommon));
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(IEnumerable<RecipeDataItem>), types);

            IEnumerable<RecipeDataItem> data = (IEnumerable<RecipeDataItem>)deserializer.ReadObject(sri.Stream);
            await LoadUserImagesLocalDataAsync();

            List<string> IDs = new List<string>();
            RecipeDataGroup group = null;
            foreach (var recipe in data)
            {
                if (!IDs.Contains(recipe.Group.UniqueId))
                {
                    group = recipe.Group;
                    group.Items = new ObservableCollection<RecipeDataItem>();
                    IDs.Add(recipe.Group.UniqueId);
                    _itemGroups.Add(group);
                }

                AssignedUserImages(recipe);

                group.LicensedRequired = false;

                group.Items.Add(recipe);
            }

            // Fire a RecipesLoaded event
            if (RecipesLoaded != null)
                RecipesLoaded(this, null);

            IsLoaded = true;
        }

        public async System.Threading.Tasks.Task LoadUserImagesLocalDataAsync()
        {
            try
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(List<UserRecipeImages>));
                var storage = IsolatedStorageFile.GetUserStoreForApplication();
                var fileStream = storage.OpenFile(Consts.UserImagesFile, FileMode.OpenOrCreate, FileAccess.Read);
                UserImages = (List<UserRecipeImages>)deserializer.ReadObject(fileStream);
                fileStream.Close();
            }
            catch (Exception ex)
            {
            }
        }

        public async System.Threading.Tasks.Task SaveUserImagesLocalDataAsync()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<UserRecipeImages>));
            try
            {
                var allItems = (from grp in ItemGroups
                                from item in grp.Items
                                where item.UserImages != null && item.UserImages.Count > 0
                                select new UserRecipeImages(item.UniqueId, item.UserImages)).ToList();

                var storage = IsolatedStorageFile.GetUserStoreForApplication();
                var fileStream = storage.OpenFile(Consts.UserImagesFile, FileMode.Create, FileAccess.ReadWrite);

                MemoryStream ms = new MemoryStream();
                ser.WriteObject(fileStream, allItems);

                fileStream.Close();
                ms.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }
}

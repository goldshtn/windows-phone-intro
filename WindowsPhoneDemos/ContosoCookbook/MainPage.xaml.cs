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
using Windows.Phone.Speech.Synthesis;

namespace ContosoCookbook
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Microsoft.Phone.Shell.ProgressIndicator pi;
       
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!App.Recipes.IsLoaded)
            {
                pi = new Microsoft.Phone.Shell.ProgressIndicator();
                pi.IsIndeterminate = true;
                pi.Text = "Loading data, please wait...";
                pi.IsVisible = true;

                Microsoft.Phone.Shell.SystemTray.SetIsVisible(this, true);
                Microsoft.Phone.Shell.SystemTray.SetProgressIndicator(this, pi);

                await App.Recipes.LoadLocalDataAsync();

                lstGroups.DataContext = App.Recipes.ItemGroups;

                pi.IsVisible = false;
                Microsoft.Phone.Shell.SystemTray.SetIsVisible(this, false);       
            }
            base.OnNavigatedTo(e);
        }

        private void lstGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstGroups.SelectedIndex > -1)
            {
                NavigationService.Navigate(new Uri("/GroupDetailPage.xaml?ID=" + (lstGroups.SelectedItem as RecipeDataGroup).UniqueId, UriKind.Relative));
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            App.speechRecognizerWithUI.Settings.ExampleText = "salad";
            App.speechRecognizerWithUI.Settings.ShowConfirmation = true;
            App.speechRecognizerWithUI.Settings.ListenText = "What are you looking for?";
            var result = await App.speechRecognizerWithUI.RecognizeWithUIAsync();

            if (result.ResultStatus == Windows.Phone.Speech.Recognition.SpeechRecognitionUIStatus.Succeeded)
            {
                SpeechSynthesizer synth = new SpeechSynthesizer();

                if (result.RecognitionResult.TextConfidence == Windows.Phone.Speech.Recognition.SpeechRecognitionConfidence.High ||
                    result.RecognitionResult.TextConfidence == Windows.Phone.Speech.Recognition.SpeechRecognitionConfidence.Medium)
                {
                    var recipe = App.Recipes.FindRecipeByText(result.RecognitionResult.Text);

                    if (null != recipe)
                        NavigationService.Navigate(new Uri("/RecipeDetailPage.xaml?ID=" + recipe.UniqueId, UriKind.Relative));
                    else
                        await synth.SpeakTextAsync(string.Format("Cannot find any {0}. I am sorry!", result.RecognitionResult.Text));
                }
                else
                {
                    await synth.SpeakTextAsync("I am not sure what you just said. Please try again!");
                }
            }
        }
    }
}
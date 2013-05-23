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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ContosoCookbook.Data;
using ContosoCookbook.Common;
using Windows.ApplicationModel;
using Windows.Phone.Speech.VoiceCommands;
using Windows.Phone.Speech.Recognition;

namespace ContosoCookbook
{
    public partial class App : Application
    {
        public static RecipeDataSource Recipes { get; set; }
        public static SpeechRecognizerUI speechRecognizerWithUI;

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            InitializeVoiceCommands();

            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

            Recipes = new RecipeDataSource();
            Recipes.RecipesLoaded += Recipes_RecipesLoaded;
        }

        void Recipes_RecipesLoaded(object sender, EventArgs e)
        {
            InitializeVoiceRecognition();
        }

        private void InitializeVoiceRecognition()
        {
            speechRecognizerWithUI = new SpeechRecognizerUI();
            List<string> searchTerms = ExtractSearchTerms();
            speechRecognizerWithUI.Recognizer.Grammars.AddGrammarFromList("SearchTerms", searchTerms);
        }

        private List<string> ExtractSearchTerms()
        {
            List<string> terms = new List<string>();

            terms.Add("shrimp");
            terms.Add("noodle");
            terms.Add("rice");
            terms.Add("dumpling");
            terms.Add("macaroons");
            terms.Add("bacon");
            terms.Add("fish");
            terms.Add("meatballs");
            terms.Add("cucumber");
            terms.Add("lamb");
            terms.Add("onion");
            terms.Add("chili");
            terms.Add("bean");
            terms.Add("oil");
            terms.Add("clams");
            terms.Add("lemon");
            terms.Add("risotto");
            terms.Add("taco");
            terms.Add("pork");
            terms.Add("salsa");

            return terms;
        }

        async private static void InitializeVoiceCommands()
        {
            var filename = "SupportedVoiceCommands.xml";

            try
            {
                var location = Package.Current.InstalledLocation.Path;
                var fileUriString = String.Format("file://{0}/{1}", location, filename);
                await VoiceCommandService.InstallCommandSetsFromFileAsync(new Uri(fileUriString));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private async void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            await Recipes.SaveUserImagesLocalDataAsync();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private async void Application_Closing(object sender, ClosingEventArgs e)
        {
            await Recipes.SaveUserImagesLocalDataAsync();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            RootFrame.UriMapper = new CookbookUriMapper();

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HelloWorld.Resources;

namespace HelloWorld
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            lstApartments.Items.Clear();
            lstApartments.ItemsSource = App.ApartmentStore.Apartments;
        }

        private void ApartmentSelected(object sender, SelectionChangedEventArgs args)
        {
            string uri = String.Format("/SecondPage.xaml?index={0}", lstApartments.SelectedIndex);
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }
    }
}
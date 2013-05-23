using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace HelloWorld
{
    public partial class SecondPage : PhoneApplicationPage
    {
        public SecondPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int apartmentIndex = int.Parse(NavigationContext.QueryString["index"]);
            Apartment apartment = App.ApartmentStore.Apartments[apartmentIndex];
            ContentPanel.DataContext = apartment;
        }
    }
}
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Neo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new PhotoPage
            {
                BackgroundColor = new Color(0, 0, 0, 0.7)
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

using System;
using System.Drawing.Imaging;
using System.IO;
using Neo.Services;
using Neo.Views;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Image = System.Drawing.Image;

namespace Neo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoPage : ContentPage
    {

        public Button TakePhoto { get; set; }
        
        public Grid GridWindow { get; set; }
        public CameraPreview Camera { get; set; }

        public Grid GridTakePhoto { get; set; }
        
        public Frame CaptureWindow { get; set; }

        private const int TakePhotoSizeBtn = 80;
        
        public PhotoPage()
        {
            InitializeComponent();
            
            SetGrids();
            SetCameraButtons();
        }
 
        private async void TakePhotoAsync(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                { 
                    Title = $"xamarin.{DateTime.Now:dd.MM.yyyy_hh.mm.ss}.png"
                });
 
                // save file to local storage
                var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                
                using (var readStream = await photo.OpenReadAsync())
                {
                    using (var writeStream = File.OpenWrite(newFile))
                        await readStream.CopyToAsync(writeStream);
                }

                var result = new Solver(Image.FromFile(newFile)).Result;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oh... something went wrong :(", ex.Message, "OK");
            }
        }

        private void SetGrids()
        {
            // Capture window
            Camera = new CameraPreview
            {
                BorderColor = Color.White,
                CornerRadius = 20,
                Options = CameraOptions.Back,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            GridWindow = new Grid
            {
                BackgroundColor = Color.Red,
                Children =
                {
                    Camera
                },
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition
                    {
                        Width = 200
                    },
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition
                    {
                        Height = 200
                    },
                },
            };

            // Take photo
            TakePhoto = new Button
            {
                CornerRadius = TakePhotoSizeBtn,
                BorderColor = Color.DimGray,
                BorderWidth = 5,
                BackgroundColor = System.Drawing.Color.Bisque
            };
            
            GridTakePhoto = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    TakePhoto
                },
                Margin = new Thickness(0, 350, 0, 0),
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition
                    {
                        Width = TakePhotoSizeBtn,
                    },
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition
                    {
                        Height = TakePhotoSizeBtn
                    },
                }
            };
        }
        
        private void SetCameraButtons()
        {
            // take photo
            TakePhoto.Clicked += TakePhotoAsync;
            
            Content = new StackLayout
            {
                Children = 
                {
                    GridWindow,
                    GridTakePhoto
                },
                Margin = new Thickness(0, 80, 0, 0),
                HorizontalOptions = LayoutOptions.Center
            };
        }
    }
}
using System;
using System.Drawing.Imaging;
using System.IO;
using Neo.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Image = System.Drawing.Image;

namespace Neo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoPage : ContentPage
    {
        public Image Image { get; set; }

        public Button TakePhoto { get; set; }
        
        public Grid GridWindow { get; set; }

        public Grid GridTakePhoto { get; set; }
        
        public Frame CaptureWindow { get; set; }

        public Button PickPhoto { get; }

        private const int TakePhotoSizeBtn = 80;
        
        public PhotoPage()
        {
            InitializeComponent();
            PickPhoto = new Button
            {
                Text = "Get photo"
            };
            
            SetGrids();
            SetCameraButtons();
        }
        
        private async void GetPhotoAsync(object sender, EventArgs e)
        {
            try
            {
                // select photo
                var photo = await MediaPicker.PickPhotoAsync();
                
                // load image to source
                Image = Image.FromFile(photo.FullPath);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oh... something went wrong :(", ex.Message, "OK");
            }
        }
 
        private async void TakePhotoAsync(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                { 
                    Title = $"xamarin.{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.png"
                });
 
                // save file to local storage
                var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                
                using (var readStream = await photo.OpenReadAsync())
                {
                    using (var writeStream = File.OpenWrite(newFile))
                        await readStream.CopyToAsync(writeStream);
                }
                Image.Save("test", ImageFormat.Png);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oh... something went wrong :(", ex.Message, "OK");
            }
        }

        private void SetGrids()
        {
            
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
                Children =
                {
                    TakePhoto
                },
                Margin = new Thickness(0, 0, 0, 40),
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Center,
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
            
            // Capture window
            
            CaptureWindow = new Frame
            {
                BorderColor = Color.White,
                CornerRadius = 20
            };
            
            GridWindow = new Grid
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    CaptureWindow
                },
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition
                    {
                        Width = 200
                    }
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition
                    {
                        Height = 200
                    }
                },
            };
        }
        
        private void SetCameraButtons()
        {
            // select photo
            PickPhoto.Clicked += GetPhotoAsync;
 
            // take photo
            TakePhoto.Clicked += TakePhotoAsync;
            
            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                
                Children = 
                {
                    GridWindow,
                    GridTakePhoto
                }
            };
        }
    }
}
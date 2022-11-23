using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Neo
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoPage : ContentPage
    {
        public Image Image { get; set; } = new Image();

        public Button TakePhoto { get; set; } = new Button()
        {
            Text = "Take photo"
        };

        public Button GetPhoto { get; set; } = new Button()
        {
            Text = "Get photo"
        };
        
        public PhotoPage()
        {
            InitializeComponent();
            SetCameraButtons();
        }
        
        private async void GetPhotoAsync(object sender, EventArgs e)
        {
            try
            {
                // выбираем фото
                var photo = await MediaPicker.PickPhotoAsync();
                // загружаем в ImageView
                Image.Source = ImageSource.FromFile(photo.FullPath);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Сообщение об ошибке", ex.Message, "OK");
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
 
                // для примера сохраняем файл в локальном хранилище
                var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using (var stream = await photo.OpenReadAsync())
                using (var newStream = File.OpenWrite(newFile))
                    await stream.CopyToAsync(newStream);
                
                Image.Source = ImageSource.FromFile(photo.FullPath);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Сообщение об ошибке", ex.Message, "OK");
            }
        }
        
        private void SetCameraButtons()
        {
            // select photo
            GetPhoto.Clicked += GetPhotoAsync;
 
            // take photo
            TakePhoto.Clicked += TakePhotoAsync;
 
            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Children = {
                    new StackLayout
                    {
                        Children =
                        {
                            TakePhoto,
                            GetPhoto
                        },
                        Orientation =StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.CenterAndExpand
                    },
                    Image
                }
            };
        }
    }
}
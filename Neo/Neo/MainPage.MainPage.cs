using System;
using System.Threading.Tasks;
using Neo.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace Neo;

public partial class MainPage
{
    public static MediaFile Photo { get; set; }

    private async void TakePhotoAsync(object sender, EventArgs e)
    {
        try
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Exception",
                    @$"{nameof(CrossMedia.Current.IsCameraAvailable)}: {CrossMedia.Current.IsCameraAvailable}
                                {nameof(CrossMedia.Current.IsTakePhotoSupported)}: {CrossMedia.Current.IsTakePhotoSupported}",
                    "OK");
            }


            if (!await SavePhotoAsync())
                await DisplayAlert("Exception", "didn't save photo.", "ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Oh... something went wrong :(",
                $"inner: {ex.InnerException?.Message}\nmessage: {ex.Message}",
                "OK");
        }
    }

    private static async Task<bool> SavePhotoAsync()
    {
        var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
        {
            Name = Guid.NewGuid().ToString(),
            DefaultCamera = CameraDevice.Rear,
            SaveToAlbum = false,
            SaveMetaData = false,
        });

        if (photo is null)
            return false;
        Photo = photo;
        DependencyService.Get<IMediaService>()
            .SavePicture(photo.OriginalFilename, photo.GetStream(), "Pictures");

        return true;
    }
}
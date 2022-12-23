using System;
using System.Threading.Tasks;
using IronOcr;
using Neo.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Tesseract;
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
            {
                await DisplayAlert("Exception", "didn't save photo.", "ok");
                return;
            }

            if (!_tesseractApi.Initialized)
                await _tesseractApi.Init("eng");

            var tessResult = _tesseractApi.SetImage(Photo?.GetStream()).Result;
            if (tessResult)
            {
                var solved = new Solver()
            }

            var a = 1;

            // var solver = new Solver(Photo?.GetStream());
            // var result = await solver.ReadAsync();
            // await DisplayAlert("Result", result.ToString(), "Ok");
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
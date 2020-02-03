
using Android.App;
using Android.Widget;
using LocalWeatherApp.Helpers;

namespace LocalWeatherApp.Droid.Helpers
{
    public class NativeCalls : INativeCalls
    {
        public void OpenToast(string text)
        {
            Toast.MakeText(Application.Context, text, ToastLength.Long).Show();
        }
    }
}
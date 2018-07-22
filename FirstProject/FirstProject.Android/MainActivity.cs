using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using FirstProject.Droid;
using static FirstProject.Droid.Resource.Layout;


[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
namespace FirstProject.Droid
{
    [Activity(Label = "FirstProject", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity,IPresenter
    {
        private static AlertDialog.Builder builder;
        private static View view;
        private static AlertDialog dialog;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            ActivityCompat.RequestPermissions(this,
                new String[] {
                    Manifest.Permission.ReadExternalStorage,
                    Manifest.Permission.WriteExternalStorage
                },
                requestCode:1);

            

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            builder = new AlertDialog.Builder(this);
            view = LayoutInflater.Inflate(layout1, null);
            var textView = FindViewById<EditText>(Resource.Id.message);
            builder.SetView(view);
            dialog = builder.Create();
        }

        public void clickButtonMenu(object sender, EventArgs e)
        {
            dialog.Show();
        }
    }
}


using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using FirstProject.Droid;
using FirstProject.Model;
using Java.IO;
using Xamarin.Forms;
using static FirstProject.Droid.Resource.Layout;
using Button = Android.Widget.Button;
using View = Android.Views.View;


[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]
namespace FirstProject.Droid
{
    [Activity(Label = "FirstProject", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity,IPresenter
    {
        private static AlertDialog dialog;
        private static View view;
        private static String Path;

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


            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            LayoutInflater inflaer = LayoutInflater.From(this);
            view = inflaer.Inflate(layout2, null);
            builder.SetView(view);
            dialog=builder.Create();

            var alertDialogButton_Ok = view.FindViewById<Button>(Resource.Id.buttonOk);

            alertDialogButton_Ok.Click += AlertDialogButton_Ok_Click;

        }

        private void AlertDialogButton_Ok_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this,Path,ToastLength.Short).Show();
        }

        public void clickButtonMenu(object sender, EventArgs e)
        {
            dialog.Show();
        }

        public void SetPath(string res)
        {
            Path = res;
        }
    }
}


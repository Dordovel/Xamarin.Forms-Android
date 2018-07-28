using System;
using System.Threading;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using FirstProject.Droid;
using FirstProject.Model;
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
        private static View musicViev;
        private static AlertDialog musicDialog;
        private static SeekBar seekBar;


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

            AlertDialogOpen();
            AlertDialogMusic();
            
        }

        private void AlertDialogOpen()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            LayoutInflater inflaer = LayoutInflater.From(this);
            view = inflaer.Inflate(layout2, null);
            builder.SetView(view);
            dialog = builder.Create();

            var alertDialogButton_Ok = view.FindViewById<Button>(Resource.Id.buttonOk);

            alertDialogButton_Ok.Click += AlertDialogButton_Ok_Click;
        }

        private void AlertDialogMusic()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            LayoutInflater inflater = LayoutInflater.From(this);
            musicViev = inflater.Inflate(Player, null);
            builder.SetView(musicViev);
            musicDialog = builder.Create();

            var buttonPlay = musicViev.FindViewById<ImageButton>(Resource.Id.buttonPlay);
            var buttonPause = musicViev.FindViewById<ImageButton>(Resource.Id.buttonPause);

            buttonPlay.Click += ButtonPlay_Click;
            buttonPause.Click += ButtonPause_Click;
        }

        private void ButtonPause_Click(object sender, EventArgs e)
        {
            MediaPlayer.Media_player.Pause();
        }

        private void ButtonPlay_Click(object sender, EventArgs e)
        {
            if (MediaPlayer.Media_player.isPause)
            {
                MediaPlayer.Media_player.Resume();
            }
            new Thread(tread).Start();
        }

        private void AlertDialogButton_Ok_Click(object sender, EventArgs e)
        {
            Model.Files temp = Model.Files.ResFile;
           
            var DirName = view.FindViewById<EditText>(Resource.Id.editTextDialogAlert).Text;
            if (DirName != null)
            {
                Toast.MakeText(this, temp.NewDirectory(DirName)?"Каталог: "+DirName+" успешно создан!":"Ошибка!", ToastLength.Long).Show();

                dialog.Cancel();
            }
        }

        public void clickButtonMenu(object sender, EventArgs e)
        {
            dialog.Show();
        }

        public void Item_Clicked(object sender, EventArgs e)
        {
            seekBar = musicViev.FindViewById<SeekBar>(Resource.Id.duration);
            var label = musicViev.FindViewById<TextView>(Resource.Id.MusicName);
            var performer = musicViev.FindViewById<TextView>(Resource.Id.Performer);
            var Album = musicViev.FindViewById<TextView>(Resource.Id.Aulbum_Year);
            var Gengres = musicViev.FindViewById<TextView>(Resource.Id.Gengre);
            var Bitreit = musicViev.FindViewById<TextView>(Resource.Id.Bitreit);
            if (MediaPlayer.Media_player.isPlay)
            {
                musicDialog.Show();
                label.Text ="Исполнитель "+MediaPlayer.Media_player.Tags()["Performer"];
                performer.Text ="Название "+MediaPlayer.Media_player.Tags()["Title"];
                Album.Text = "Альбом " + MediaPlayer.Media_player.Tags()["Album"]+" "+ MediaPlayer.Media_player.Tags()["Year"];
                Gengres.Text = "Жанр " + MediaPlayer.Media_player.Tags()["Gengres"];
                Bitreit.Text = "Битрейт " + MediaPlayer.Media_player.Tags()["Bitreit"];
                seekBar.Max = MediaPlayer.Media_player.Duration;
                seekBar.StopTrackingTouch += SeekBar_StopTrackingTouch;
                   new Thread(tread).Start();

            }
        }

        private void SeekBar_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            var duration = musicViev.FindViewById<SeekBar>(Resource.Id.duration);
            MediaPlayer.Media_player.CurrentPosition = duration.Progress;
        }
        

        private void tread()
        {
            while (MediaPlayer.Media_player.isPlay)
            {
                seekBar.Progress = MediaPlayer.Media_player.CurrentPosition;
                Thread.Sleep(500);
            }

            MediaPlayer.Media_player.isStop = true;
        }

        public override void OnBackPressed()
        {
            if (MediaPlayer.Media_player.isPlay)
            {
                MediaPlayer.Media_player.Dispose();
            }
            else
            {
                base.OnBackPressed();
            }
        }
    }
}


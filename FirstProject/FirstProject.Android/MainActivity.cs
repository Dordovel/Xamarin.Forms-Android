using System;
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using FirstProject.Droid;
using static FirstProject.Droid.Resource.Layout;
using Button = Android.Widget.Button;
using Uri = Android.Net.Uri;
using View = Android.Views.View;


[assembly: Xamarin.Forms.Dependency(typeof(MainActivity))]

namespace FirstProject.Droid
{
    [Activity(Label = "Project", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IPresenter
    {
        private static AlertDialog dialog;
        private static View view;
        private static View musicViev;
        private static AlertDialog musicDialog;
        private static SeekBar seekBar;
        private static View picturesView;
        private static AlertDialog picturesDialog;

        private static NotificationCompat.Builder builder;
        private static NotificationManager notificationManager1;

        private static AudioManager focus;

        public static AudioManager GetAudioManager
        {
            get { return focus; }
        }


        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            ActivityCompat.RequestPermissions(this,
                new String[]
                {
                    Manifest.Permission.ReadExternalStorage,
                    Manifest.Permission.WriteExternalStorage,
                },
                requestCode: 1);


            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            CreateNotificationChannel();

            focus = (AudioManager) GetSystemService(Context.AudioService);

            AlertDialogOpen();
            AlertDialogMusic();
            AlertDialogImage();

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

        private void AlertDialogImage()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            LayoutInflater inflaer = LayoutInflater.From(this);
            picturesView = inflaer.Inflate(Resource.Layout.Image, null);
            builder.SetView(picturesView);
            picturesDialog = builder.Create();

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
                Toast.MakeText(this,
                    temp.NewDirectory(DirName) ? "Каталог: " + DirName + " успешно создан!" : "Ошибка!",
                    ToastLength.Long).Show();

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
            if (MediaPlayer.Media_player.isPlay || MediaPlayer.Media_player.isPause)
            {
                musicDialog.Show();
                label.Text = "Исполнитель: \n\t\t" + MediaPlayer.Media_player.Tags()["Performer"];
                performer.Text = "Название: \n\t\t" + MediaPlayer.Media_player.Tags()["Title"];
                Album.Text = "Альбом: \n\t\t" + MediaPlayer.Media_player.Tags()["Album"] + " " +
                             MediaPlayer.Media_player.Tags()["Year"];
                Gengres.Text = "Жанр: \n\t\t" + MediaPlayer.Media_player.Tags()["Gengres"];
                Bitreit.Text = "Битрейт: \n\t\t" + MediaPlayer.Media_player.Tags()["Bitreit"];
                seekBar.Max = MediaPlayer.Media_player.Duration;
                seekBar.StopTrackingTouch += SeekBar_StopTrackingTouch;
                new Thread(tread).Start();

            }
        }


        public void ShowPictures(string Path)
        {
            ImageView image = picturesView.FindViewById<ImageView>(Resource.Id.imageView1);

            picturesDialog.Show();

            image.SetImageURI(Uri.Parse(Path));
        }

        private void SeekBar_StopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            var duration = musicViev.FindViewById<SeekBar>(Resource.Id.duration);
            MediaPlayer.Media_player.CurrentPosition = duration.Progress;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            var channel = new NotificationChannel("10000", "Notification", NotificationImportance.Default);

            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            builder = new NotificationCompat.Builder(this, "10000");
            notificationManager1 = GetSystemService(Context.NotificationService) as NotificationManager;

        }


        public void ShowNitification(string name)
        {
            builder
                .SetContentTitle(name)
                .SetSmallIcon(Resource.Drawable.music_play)
                .SetDefaults((int) NotificationDefaults.Vibrate);

            notificationManager1.Notify(0, builder.Build());
            //new Thread(ProgressNotification).Start();
        }

        private void ProgressNotification()
        {
            while (MediaPlayer.Media_player.isPlay)
            {
                builder.SetProgress(MediaPlayer.Media_player.Duration, MediaPlayer.Media_player.CurrentPosition,
                    false);

                notificationManager1.Notify(0, builder.Build());
            }
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

        protected override void OnPause()
        {
            if (MediaPlayer.Media_player.isPlay)
            {
                MediaPlayer.Media_player.Pause();
            }

            base.OnPause();
        }

        protected override void OnResume()
        {
            if (MediaPlayer.Media_player.isPause)
            {
                MediaPlayer.Media_player.Resume();
            }

            base.OnResume();
        }

    }

}


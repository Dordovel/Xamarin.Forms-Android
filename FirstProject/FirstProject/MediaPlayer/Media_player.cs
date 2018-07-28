using System.Collections.Generic;
using System.IO;
using Android.Graphics.Drawables;
using Xamarin.Forms;
using File = Java.IO.File;
using String = System.String;

namespace FirstProject.MediaPlayer
{
    class Media_player
    {
        private static File music_File;
        private static Android.Media.MediaPlayer player;
        public static bool isPlay=false;
        public static bool isPause=false;
        public static bool isStop=false;
        

        public static int Duration;

        public static int CurrentPosition
        {
            get
            {
                if (player.IsPlaying)
                {
                    return player.CurrentPosition;
                }

                return 0;
            }
            set
            {
                if (player.IsPlaying)
                {
                    player.SeekTo(value);
                }
            }
        }

        public static void Pause()
        {
            if (isPlay)
            {
                player.Pause();
                MainPage.itemMusic.Icon = new FileImageSource() {File = "pause.png"};
                isPlay = false;
                isPause = true;
            }
        }

        public static void Resume()
        {
            if (isPause)
            {
                Media_player.Start();
                isPlay = true;
                isPause = false;
            }
        }

        public Media_player(File file)
        {
            music_File = file;
            if (isPlay)
            {
                Dispose();
            }
                player = new Android.Media.MediaPlayer();
                player.SetDataSource(music_File.AbsolutePath);
                player.Prepare();
                Duration = player.Duration;
        }

        public static void Start()
        {
            if (player != null)
            {
                player.Start();
                isPlay = true;
                isPause = false;
                isStop = false;
                MainPage.itemMusic.Icon=new FileImageSource(){File="play.png"};
            }
        }

        public static void Dispose()
        {
            if (player != null)
            {
                isPlay = false;
                player.Stop();
                player.Dispose();
                MainPage.itemMusic.Icon = new FileImageSource() { File = "stop.png" };
            }
        }

        public static Dictionary<string, string> Tags()
        {
        
        var tag = TagLib.File.Create(music_File.AbsolutePath);

            Dictionary<string, string> MusicTags = new Dictionary<string, string>()
            {
                {"Title", tag.Tag.Title},
                {"Performer", String.Join(", ", tag.Tag.Performers)},
                {"Album", tag.Tag.Album},
                {"Year", (tag.Tag.Year).ToString()},
                {"Gengres",String.Join(", ",tag.Tag.Genres) },
                {"Bitreit",tag.Properties.AudioBitrate.ToString() }
            };

            return MusicTags;
        }

    }
}

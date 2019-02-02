using System;
using System.Collections.Generic;
using Android.Content;
using Android.Media;
using Android.Media.Session;
using Android.Support.V4.Media;
using FirstProject.Droid;
using File = Java.IO.File;
using String = System.String;

namespace FirstProject.MediaPlayer
{
    class Media_player
    {
        private static File music_File;
        private static Focus focus;
        private static Android.Media.MediaPlayer player;

        public static bool isPlay = false;
        public static bool isPause = false;
        public static bool isStop = false;

        public static readonly String[] supportedMediaFormats =
        {
            "FLAC",
            "MP3"
        };


        public static int Duration
        {
            get { return player.Duration; }
        }

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
                isPlay = false;
                isPause = true;
                MainActivity.GetAudioManager.AbandonAudioFocus(focus);
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
        }

        private static AudioFocusRequest getFocus()
        {
            focus = new Focus();

            return MainActivity.GetAudioManager.RequestAudioFocus(focus, Stream.Music, AudioFocus.Gain);
        }

        public static void Start()
        {
            if (getFocus()==AudioFocusRequest.Granted)
            {
                if (player != null)
                {
                    player.Start();
                    isPlay = true;
                    isPause = false;
                    isStop = false;
                }
            };
        }

        public static void Dispose()
        {
            if (player != null)
            {
                isPlay = false;
                player.Stop();
                player.Dispose();
                MainActivity.GetAudioManager.AbandonAudioFocus(focus);
            }
            
        }

        public static Dictionary<string, string> Tags()
        {

            var tag = TagLib.File.Create(music_File.AbsolutePath);

            Dictionary<string, string> MusicTags = new Dictionary<string, string>()
            {
                {"Title", music_File.Name},
                {"Performer", String.Join(", ", tag.Tag.Performers)},
                {"Album", tag.Tag.Album},
                {"Year", (tag.Tag.Year).ToString()},
                {"Gengres", String.Join(", ", tag.Tag.Genres)},
                {"Bitreit", tag.Properties.AudioBitrate.ToString()}
            };

            return MusicTags;
        }
    }

    class Focus : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener
    {

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public IntPtr Handle { get; }

        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.GainTransient:
                    break;
                case AudioFocus.LossTransient:
                    break;
            }
        }
    }
}

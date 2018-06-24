using System.Collections.Generic;
using System.IO;
using FirstProject.Model;
using Xamarin.Forms;
using File = Java.IO.File;

namespace FirstProject
{
    public partial class MainPage : ContentPage
    {
        private File file;
        private static string path;
        private List<Files> list;


        public MainPage()
        {
            InitializeComponent();
            path += "/";
            FilePrint();

        }

        public MainPage(File file)
        {
            InitializeComponent();

            this.file = file;

            FilePrint();
        }


        private void FilePrint()
        {
            list = new List<Files>();

            if (this.file == null)
            {
                this.file = new File("/");
            }

            path += file.Name;
            Count.Text = path;
            foreach (var VARIABLE in file.ListFiles())
            {
                if (VARIABLE.IsDirectory)
                {
                    list.Add(new Files() {FileName = VARIABLE.Name, Getfile = VARIABLE,Image = "folder.png"});
                }
                else if (VARIABLE.IsFile && Path.GetExtension(VARIABLE.Name).Equals("mp3"))
                {
                    list.Add(new Files() { FileName = VARIABLE.Name, Getfile = VARIABLE, Image = "music.png" });
                }
            }

            listView.ItemsSource = list;
        }

        private void Open()
        {
            Count.Text = path;

            path += ">";

            Navigation.PushAsync(new MainPage(this.file));
        }

        private async void ListView_OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            file = ((Files)listView.SelectedItem).Getfile;

            switch (await DisplayActionSheet(file.Name, "Cancel", null, "Open"))
            {
                case "Open": Open();break;
            }
        }
    }
}
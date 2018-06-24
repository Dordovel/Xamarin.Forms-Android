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
        private List<Files> list = new List<Files>();
        private static Files files;


        public MainPage()
        {
            files = new Files();
            InitializeComponent();
            files.Path += "/";
            Call();

        }

        public MainPage(File file)
        {
            InitializeComponent();

            this.file = file;
            Call();

        }

        private void Call()
        {
            listView.ItemsSource = files.FilePrint(this.file);
        }
       

        private void Open()
        {
            Count.Text = files.Path;

            files.Path += ">";

            Navigation.PushAsync(new MainPage(this.file));
        }

        private async void ListView_OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            file = ((Files)listView.SelectedItem).Getfile;

            if (!file.IsFile)
            {
                switch (await DisplayActionSheet(file.Name, "Cancel", null, "Open"))
                {
                    case "Open":
                        Open();
                        break;
                }
            }
            else
            {
                await DisplayAlert("Error", "Эта функция на данный момент не доступна", "Ok");
            }
        }
    }
}
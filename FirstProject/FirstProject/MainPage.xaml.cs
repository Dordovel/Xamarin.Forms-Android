using System.IO;
using FirstProject.Model;
using Xamarin.Forms;
using File = Java.IO.File;

namespace FirstProject
{
    public partial class MainPage : ContentPage
    {
        private File file;
        private static Files files;
        private string Path;
        private static bool mainPage;


        public MainPage()
        {
            InitializeComponent();
            files = new Files();
            Path += "/>";
            Call();

        }

        public MainPage(File file,string path)
        {
            InitializeComponent();
            this.file = file;
            Path = path;

            files = new Files();

            Call();

        }

        private void Call()
        {
            Count.Text = Path;

            listView.ItemsSource = files.FilePrint(this.file);
        }


        private void Open()
        {
            Path += file.Name+"/>";

            Navigation.PushAsync(new MainPage(this.file,this.Path));
            

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
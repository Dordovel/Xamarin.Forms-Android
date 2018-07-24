using System;
using Android.Widget;
using FirstProject.Model;
using Xamarin.Forms;
using File = Java.IO.File;

namespace FirstProject
{
    public partial class MainPage : ContentPage
    {
        private static Files PersonFile;
        private static ToolbarItem item;



        public MainPage()
        {
            InitializeComponent();

            PersonFile = new Files();
            Files.ResFile = PersonFile;
            Render();
            
        }

        public MainPage(File file, string path)
        {
            InitializeComponent();
            try
            {

                PersonFile = new Files(file, path);
                Files.ResFile = PersonFile;

                Count.Text = file.Name;


                Render();
            }
            catch (NullReferenceException e)
            {
                Count.Text = e.Message;
            }

        }

        private void Render()
        {
            Count.Text = Files.Path;

            if (item == null)
            {
                toolbar();
            }

            ToolbarItems.Add(item);

            listView.ItemsSource = PersonFile.FilePrint();
        }

        private void toolbar()
        {
            item = new ToolbarItem()
            {
                Text = "Новая папка",
                Order = ToolbarItemOrder.Default,
                Icon=new FileImageSource(){File="new_folder.png"}
            };
            item.Clicked += MenuItem_OnClicked;
        }


        private void Open(object obj)
        {
            File temp = obj as File;
            Files.Path += temp.Name + "/>";

            Navigation.PushAsync(new MainPage(temp, Files.Path), true);

        }

        public async void Delete(object obj)
        {
            File temp = obj as File;

            if (temp == null)
            {
                Count.Text = "File is null";
            }
            else
            {
                if (await DisplayAlert("Delete", "Вы действительно хотите удалить Файл/Папку?   "+temp.Name, "Yes", "No"))
                {

                    if (PersonFile.Delete(temp))
                    {
                        await DisplayAlert("", "Succeful", "Ok");
                    }
                }
            }
        }

        private async void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            File temp = ((Template)listView.SelectedItem).Getfile;
            switch (await DisplayActionSheet(PersonFile.file.Name, "Cancel", null, "Open", "Delete"))
            {
                case "Open":
                    if (!temp.IsFile)
                    {
                        listView.SelectedItem = null;
                        Open(temp);
                    }
                    else
                    {
                        await DisplayAlert("Error", "Эта функция на данный момент не доступна", "Ok");
                    }
                    break;

                case "Delete":

                    Delete(temp);

                    break;
            }

        }

        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(
                () => { DependencyService.Get<IPresenter>().clickButtonMenu(sender, e); });
        }
    }
}
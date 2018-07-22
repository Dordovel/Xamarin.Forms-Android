using System;
using System.Linq;
using System.Windows.Input;
using Android;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using FirstProject.Droid;
using FirstProject.Model;using Xamarin.Forms;
using Xamarin.Forms.Internals;
using AlertDialog = Android.App.AlertDialog;
using Environment = Android.OS.Environment;
using File = Java.IO.File;

namespace FirstProject
{
    public partial class MainPage : ContentPage
    {
        private File file;
        private static Files files;
        private string Path;
        private static bool mainPage;
        private static ToolbarItem item;



        public MainPage()
        {
            InitializeComponent();

            files = new Files();
            file = new File("/");
            Path += "/>";
            Render();


        }

        public MainPage(File file, string path)
        {
            InitializeComponent();
            this.file = file;
            Path = path;
            Count.Text = file.Name;

            files = new Files();

            Render();

        }

        private void Render()
        {
            Count.Text = Path;

            if (item == null)
            {
                toolbar();
            }

            ToolbarItems.Add(item);

            listView.ItemsSource = files.FilePrint(this.file);
        }

        private void toolbar()
        {
            item = new ToolbarItem()
            {
                Text = "Menu",
                Order = ToolbarItemOrder.Default

            };
        }


        private void Open(object obj)
        {
            File temp = obj as File;
            Path += temp.Name + "/>";

            Navigation.PushAsync(new MainPage(temp, this.Path), true);

        }

        public async void Delete(object obj)
        {
            File temp = obj as File;

            if (await DisplayAlert("Delete", "Вы действительно хотите удалить Файл/Папку?", "Yes", "No"))
            {

                if (files.Delete(temp as File))
                {
                    await DisplayAlert("", "Succeful", "Ok");
                }
            }
        }

        private async void ListView_OnItemSelected(object sender, ItemTappedEventArgs e)
        {

                /* File temp = ((Files)listView.SelectedItem).Getfile;
                     switch (await DisplayActionSheet(file.Name, "Cancel", null, "Open","Delete"))
                     {
                         case "Open":
                             if (!temp.IsFile)
                             {
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
                     */

        }

        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(
                () => { DependencyService.Get<IPresenter>().clickButtonMenu(sender, e); });
        }
    }
}
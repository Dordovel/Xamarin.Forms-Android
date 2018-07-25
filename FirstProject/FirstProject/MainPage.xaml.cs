using System;
using FirstProject.Model;
using Java.IO;
using Xamarin.Forms;
using View = Android.Views.View;

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

                PersonFile = new Files(file, path);

                Files.ResFile = PersonFile;

                Count.Text = file.Name;
            
                Render();

        }

        private void Render()
        {
            Count.Text = Files.Path;

            if (item == null)
            {
                toolbar();
            }

            ToolbarItems.Add(item);

            listViewMainPage.ItemsSource = PersonFile.FilePrint();
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

            if (temp == null)
            {
                Count.Text = "Error";
            }

            Navigation.PushAsync(new MainPage(temp, Files.Path), true);

        }

        public async void Delete(object obj)
        {
            File temp = (obj as Template).Getfile;

            if (temp == null)
            {
                Count.Text = "File is null";
            }
            else
            {
                if (await DisplayAlert("Delete", "Вы действительно хотите удалить Файл/Папку?   "+temp.Name, "Yes", "No"))
                {

                    if (PersonFile.Delete(obj as Template))
                    {
                        await DisplayAlert("", "Succeful", "Ok");
                    }
                }
            }
        }

        private async void ListView_OnItemSelected(object sender, ItemTappedEventArgs e)
        {
           File temp = ((Template)listViewMainPage.SelectedItem).Getfile;
                    if (!temp.IsFile && temp!=null)
                    {
                        listViewMainPage.SelectedItem = null;
                        Open(temp);
                    }
            else
            {
                await DisplayAlert("Error", "Эта функция на данный момент не доступна", "Ok");
            }
        
        }

        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(
                () => { DependencyService.Get<IPresenter>().clickButtonMenu(sender, e); });
        }

        private void MenuItem_OnClickedContextMenu(object sender, EventArgs e)
        {
            var menuItem = sender as MenuItem;
            Template template = menuItem.BindingContext as Template;
            switch (menuItem.Text)
            {
                case "Open":
                    Open(template);
                    break;

                case "Delete":
                    
                    Delete(template);
                    break;
            }
            
        }
        
    }
}
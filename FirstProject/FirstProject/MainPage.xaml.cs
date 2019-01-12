using System;
using System.IO;
using FirstProject.MediaPlayer;
using FirstProject.Model;
using Xamarin.Forms;
using File = Java.IO.File;

namespace FirstProject
{
    public partial class MainPage :ContentPage
    {
        private static Files PersonFile;
        private static ToolbarItem item;
        public static ToolbarItem itemMusic;
        public static ToolbarItem paste;

        #region Ctor

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

        #endregion
        
        #region Render

        private void Render()
        {
            Count.Text = Files.Path;
            
                toolbar();

            listViewMainPage.ItemsSource = PersonFile.FilePrint();
        }

        #endregion
        
        #region Toolbar

        private void toolbar()
        {
            if (Files.Move_copy_file != null)
            {
                paste = new ToolbarItem()
                {
                    Text = "Вставка",
                    Order = ToolbarItemOrder.Default
                };
                paste.Clicked += Item_Clicked1;

                ToolbarItems.Add(paste);
            }

            item = new ToolbarItem()
            {
                Text = "Новая папка",
                Order = ToolbarItemOrder.Default,
                Icon = new FileImageSource() { File = "new_folder.png" }
            };
            item.Clicked += MenuItem_OnClicked;

            itemMusic = new ToolbarItem()
            {
                Text = "Player",
                Order = ToolbarItemOrder.Default,
                Icon=new FileImageSource() { File="stop.png"}
            };
            itemMusic.Clicked += Item_Clicked;

            ToolbarItems.Add(item);
            ToolbarItems.Add(itemMusic);


        }

        private async void Item_Clicked1(object sender, EventArgs e)
        {
            if (!PersonFile.Paste(PersonFile.getTemplate()))
            { 
                await DisplayAlert("Error", "Ошибка при копировании файла "+PersonFile.getTemplate().Getfile.Name, "Ok");
            }
        }

        #endregion

        #region Event

        #region ToolBarMenuEvent


        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(
                () => { DependencyService.Get<IPresenter>().clickButtonMenu(sender, e); });
        }

       private void Item_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(
                () => { DependencyService.Get<IPresenter>().Item_Clicked(sender, e); });
        }

        #endregion

        private async void ListView_OnItemSelected(object sender, ItemTappedEventArgs e)
        {
            File temp = ((Template)listViewMainPage.SelectedItem).Getfile;
            if (!temp.IsFile && temp != null)
            {
                listViewMainPage.SelectedItem = null;
                Open(temp);
            }

            if (temp.IsFile)
            {
                for (int i = 0; i < Media_player.supportedMediaFormats.Length; i++)
                {
                    if (Path.GetExtension(temp.AbsolutePath).Contains(Media_player.supportedMediaFormats[i].ToLower()))
                    {
                        MediaPlayer.Media_player player = new Media_player(temp);
                        Media_player.Start();
                        listViewMainPage.SelectedItem = null;
                        return;
                    }

                }

                for (int i = 0; i < Files.imageSupported.Length; i++)
                {
                    if (Path.GetExtension(temp.AbsolutePath).Contains(Files.imageSupported[i].ToLower()))
                    {
                        Device.BeginInvokeOnMainThread(
                            () => { DependencyService.Get<IPresenter>().ShowPictures(temp.AbsolutePath); });

                            return;
                    }
                }

                await DisplayAlert("Error", "Эта функция на данный момент не доступна", "Ok");

            }

        }



        private void MenuItem_OnClickedContextMenu(object sender, EventArgs e)
        {
            Files files = new Files();
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

                case "Move":
                    files.Move(template);
                    break;

                case "Copy":
                    files.Copy(template);
                    break;
            }

        }


        #endregion

        #region FileSystemOperation

        private void Open(object obj)
        {
            File temp = obj as File;
            Files.Path = temp.AbsolutePath;

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
                if (await DisplayAlert("Delete", "Вы действительно хотите удалить Файл/Папку?   " + temp.Name, "Yes", "No"))
                {

                    if (PersonFile.Delete(obj as Template))
                    {
                        await DisplayAlert("", "Succeful", "Ok");
                    }

                    else
                    {
                        await DisplayAlert("", "Error", "Ok");
                    }
                }
            }
        }

        #endregion
        
    }
}
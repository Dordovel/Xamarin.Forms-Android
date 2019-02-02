using System;
using System.IO;
using FirstProject.MediaPlayer;
using FirstProject.Model;
using Xamarin.Forms;
using Button = Xamarin.Forms.Button;
using File = Java.IO.File;
using Label = Xamarin.Forms.Label;
using MenuItem = Xamarin.Forms.MenuItem;

namespace FirstProject
{
    public partial class MainPage :ContentPage
    {
        private static Files PersonFile;
        private static ToolbarItem item;
        private static ToolbarItem itemSearch;
        private static ToolbarItem itemMusic;
        public static ToolbarItem paste;

        private Editor seachEdit;
        private Label label;
        StackLayout stackLayout = null;

        private bool SearchFlag=false;

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

            PersonFile.Update();

            listViewMainPage.ItemsSource = PersonFile.getList();
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
                    Order = ToolbarItemOrder.Primary,
                    Icon = new FileImageSource() { File = "paste.png" }
                };
                paste.Clicked += Item_Clicked1;

                ToolbarItems.Add(paste);
            }

            item = new ToolbarItem()
            {
                Text = "Новая папка",
                Order = ToolbarItemOrder.Primary,
                Icon = new FileImageSource() { File = "new_folder.png" }
            };

            item.Clicked += MenuItem_OnClicked;

            itemSearch = new ToolbarItem()
            {
                Text = "Поиск",
                Order = ToolbarItemOrder.Primary,
                Icon = new FileImageSource() { File = "search.png" }
            };
            itemSearch.Clicked += ItemOnClicked;


            
            itemMusic = new ToolbarItem()
            {
                Text = "Player",
                Order = ToolbarItemOrder.Primary,
                Icon = new FileImageSource() { File = "music_play.png" }
            };
            itemMusic.Clicked += Item_Clicked;


            ToolbarItems.Add(itemMusic);

            ToolbarItems.Add(itemSearch);

            ToolbarItems.Add(item);
            
        }

        private void ItemOnClicked(object sender, EventArgs e)
        {
            if (!SearchFlag)
            {
                OpenSearchWindow();
            }
            else
            {
               CloseSearchWindow();
            }
        }

        private void OpenSearchWindow()
        {
            stackLayout = new StackLayout();

            Grid stack = new Grid();

            Button button = new Button()
            {
                HorizontalOptions = LayoutOptions.End,
                Image = "search.png",
                WidthRequest = 50,
                HeightRequest = 50,
                BorderRadius = 100,
                BackgroundColor = Color.Yellow

            };

            seachEdit = new Editor()
            {
                Text = "",
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            button.Clicked += Button_Search_Clicked;

            label = new Label() { TextColor = Color.Black };
            label.HorizontalOptions = LayoutOptions.Start;

            stack.Children.Add(seachEdit);

            stack.Children.Add(button);
            stackLayout.Children.Add(stack);
            stackLayout.Children.Add(label);

            Head.Children.Add(stackLayout);

            SearchFlag = true;
        }

        private void CloseSearchWindow()
        {
            Head.Children.Remove(stackLayout);
            SearchFlag = false;

            PersonFile.Update();

            listViewMainPage.ItemsSource = PersonFile.getList();
        }

        private async void Item_Clicked1(object sender, EventArgs e)
        {
            if (!PersonFile.Paste(PersonFile.getTemplate()))
            { 
                await DisplayAlert("Error", "Ошибка при копировании файла "+PersonFile.getTemplate().Getfile.AbsolutePath, "Ok");
            }

            ToolbarItems.Remove(paste);
        }
        

        private void Button_Search_Clicked(object sender, EventArgs e)
        {
            if (seachEdit.Text!="")
            {
                PersonFile.Search(seachEdit.Text, label);

                listViewMainPage.ItemsSource = PersonFile.getList();
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

                        Device.BeginInvokeOnMainThread(
                            () =>
                            {
                                DependencyService.Get<IPresenter>().ShowNitification(temp.Name);
                            });

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
                        listViewMainPage.SelectedItem = null;
                        return;
                    }
                }

                await DisplayAlert("Error", "Эта функция на данный момент не доступна", "Ok");
                listViewMainPage.SelectedItem = null;

            }

        }



        private void MenuItem_OnClickedContextMenu(object sender, EventArgs e)
        {
            Files files = new Files();
            var menuItem = sender as MenuItem;
            Template template = menuItem.BindingContext as Template;
            switch (menuItem.Text)
            {
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

            if (SearchFlag)
            {
                CloseSearchWindow();
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
                        await DisplayAlert("", temp.Name+"  Deleted", "Ok");
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
using System;
using System.Collections.Generic;
using System.Text;
using Java.IO;
using Xamarin.Forms;

namespace FirstProject
{
    interface IPresenter
    {
        void clickButtonMenu(object sender, EventArgs e);
        void Item_Clicked(object sender, EventArgs e);
        void ShowPictures(string path);
        void ShowNitification(string name);
    }
}

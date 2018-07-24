using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Android.Content.Res;
using Android.OS;
using File = Java.IO.File;

namespace FirstProject.Model
{
    public class Files
    {
        private List<Template> list;

        public static string Path;

        public File file { get; set; }
        public static Files ResFile { get; set; }

        public Files()
        {
            file = new File("/");
            Path = "/>";
            list = new List<Template>();
        }

        public Files(File file,string path)
        {
            this.file = file;
            Path = path;

            list = new List<Template>();
        }

        private File[] SortFile(File [] listFile)
        {

            for (int i = 0; i < listFile.Length; i++)
            {
                for (int g = listFile.Length - 1; g > i; g--)
                {

                    if (listFile[g].IsDirectory)
                    {

                        File temp = listFile[g];

                        listFile[g] = listFile[g - 1];

                        listFile[g - 1] = temp;

                    }

                }
            }
            return listFile;

        }

        public List<Template> FilePrint()
        {
            list.Clear();

            foreach (var VARIABLE in SortFile(file.ListFiles()))
            {
                if (VARIABLE.IsDirectory)
                {
                    list.Add(new Template() { FileName = VARIABLE.Name, Getfile = VARIABLE, Image = "folder.png" });
                }
                else if (VARIABLE.IsFile)
                {
                    if (System.IO.Path.GetExtension(VARIABLE.ToString()).Equals(".mp3"))
                    {
                        list.Add(new Template() { FileName = VARIABLE.Name, Getfile = VARIABLE, Image = "music.png" });
                    }
                    else
                    {
                        list.Add(new Template() { FileName = VARIABLE.Name, Getfile = VARIABLE, Image = "other.png" });
                    }
                }
            }

            return list;
        }

        public bool NewDirectory(string DirectoryName)
        {
            try
            {
                Directory.CreateDirectory(file.Path+"/"+DirectoryName);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }


        public bool Delete(File file)
        {
            try
            {
                switch (file.IsDirectory)
                {
                    case true:
                        Directory.Delete(file.Path);
                        break;
                    case false:
                        System.IO.File.Delete(file.Path);
                        break;
                }
            }
            catch (IOException e)
            {
                return false;
            }

            return true;
        }

    }
}

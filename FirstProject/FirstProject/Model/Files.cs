using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using File = Java.IO.File;

namespace FirstProject.Model
{
    public class Files
    {
        private ObservableCollection<Template> list;

        public static string Path;

        public File file { get; set; }
        public static Files ResFile { get; set; }

        public Files()
        {
            file = new File("/");
            Path = "/>";
            list = new ObservableCollection<Template>();
        }

        public Files(File file,string path)
        {
            this.file = file;
            Path = path;

            list = new ObservableCollection<Template>();
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

        

        public ObservableCollection<Template> FilePrint()
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
                string path=file.Path + "/" + DirectoryName;
                Directory.CreateDirectory(path);
                list.Add(new Template(){FileName =DirectoryName,Getfile = new File(path),Image = "folder.png"});
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }


        public bool Delete(Template template)
        {
            try
            {

                list.Remove(template);

                switch (template.Getfile.IsDirectory)
                {
                    case true:
                        Directory.Delete(template.Getfile.Path);
                        break;
                    case false:
                        System.IO.File.Delete(template.Getfile.Path);

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

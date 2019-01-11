using System;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using File = Java.IO.File;

namespace FirstProject.Model
{
    public class Files
    {
        private ObservableCollection<Template> list;

        public static string Path;

        public File file { get; set; }
        public static Files ResFile { get; set; }
        public static File Move_copy_file;
        private static Template template;

        public Files()
        {
            if (getAndroidVersion() > 7)
            {
                file = new File("/storage/emulated/0");
            }
            else
            {
                file = new File("/");
            }

            Path = file.AbsolutePath;

            list = new ObservableCollection<Template>();
        }

        public Files(File file,string path)
        {
            this.file = file;
            Path = file.AbsolutePath;

            list = new ObservableCollection<Template>();

        }


        public int getAndroidVersion()
        {
            return Convert.ToInt32(Android.OS.Build.VERSION.Release);
        }
       

        public ObservableCollection<Template> FilePrint()
        {
            list.Clear();

            foreach (var VARIABLE in file.ListFiles())
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
                switch (template.Getfile.IsDirectory)
                {
                    case true:
                       DeleteDirectory(template.Getfile);
                        break;
                    case false:
                        System.IO.File.Delete(template.Getfile.Path);

                        break;
                }

                list.Remove(template);

            }
            catch (IOException e)
            {
                return false;
            }

            return true;
        }

        private void DeleteDirectory(File tempFile)
        {
            foreach (var VARIABLE in tempFile.ListFiles())
            {
                if (VARIABLE.IsDirectory)
                {
                    DeleteDirectory(VARIABLE);
                }
                else
                {
                    System.IO.File.Delete(VARIABLE.AbsolutePath);
                }
            }

            if (tempFile.Exists())
            {
                Directory.Delete(tempFile.AbsolutePath);
            }
        }

        public bool Copy(Template template)
        {
            Move_copy_file = template.Getfile;
            Files.template = template;
            return false;
        }

        public bool Move(Template template)
        {
            Move_copy_file = template.Getfile;
            Files.template = template;
            list.Remove(template);
            return true;
        }

        public bool Paste(File temp)
        {
            bool flag = false;
            if (Move_copy_file.IsFile)
            {
                System.IO.File.Copy(Move_copy_file.AbsolutePath, temp.AbsolutePath + "/" + Move_copy_file.Name);
            }
            else if (Move_copy_file.IsDirectory)
            {
                string path = temp.AbsolutePath + "/" + Move_copy_file.Name;

               
                new DirectoryInfo(temp.AbsolutePath).CreateSubdirectory(Move_copy_file.Name);
                File newDirectory=new File(path);
                if (newDirectory.Exists()) { }

                flag=CopyDirectory(newDirectory);
            }

            if (flag)
            {
                Template tep = template;
                tep.Getfile = temp;
                ResFile.list.Add(tep);
                return true;
            }

            return false;
        }

        private bool CopyDirectory(File tempFile)
        {
            foreach (var VARIABLE in Move_copy_file.ListFiles())
            {
                Move_copy_file=VARIABLE;
                Paste(tempFile);
            }

            return true;
        }

    }
}

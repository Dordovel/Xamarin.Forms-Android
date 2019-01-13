using System;
using System.Collections.ObjectModel;
using System.IO;
using javaIO=Java.IO;
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
        private static bool move;

        public static readonly string[] imageSupported = new String[5]
        {
            "BMP",
            "GIF",
            "JPEG",
            "PNG",
            "JPG"
        };

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


        public File[] sort(File [] fileList)
        {
            int index = 0;

            for (int a = 0; a < fileList.Length; ++a)
            {
                if (fileList[a].IsDirectory)
                {
                    File temp = fileList[index];
                    fileList[index] = fileList[a];
                    fileList[a] = temp;
                    ++index;
                }
            }

            return fileList;

        }

        public Template getTemplate()
        {
            return template;
        }


        public int getAndroidVersion()
        {
            return Convert.ToInt32(Android.OS.Build.VERSION.Release);
        }
       

        public ObservableCollection<Template> FilePrint()
        {
            list.Clear();
            
            foreach (var VARIABLE in sort(file.ListFiles()))
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

            bool flag = false;
                switch (template.Getfile.IsDirectory)
                {
                    case true:
                       DeleteDirectory(template.Getfile);
                        break;
                    case false:
                        File temp = template.Getfile;

                        flag=temp.Delete();

                        break;
                }

                list.Remove(template);
            
            return flag;
        }

        private bool DeleteDirectory(File tempFile)
        {
            bool flag = false;

            foreach (var VARIABLE in tempFile.ListFiles())
            {
                if (VARIABLE.IsDirectory)
                {
                    DeleteDirectory(VARIABLE);
                }
                else
                {
                    File file = VARIABLE;
                    flag=file.Delete();
                }
            }

            if (tempFile.Exists())
            {
                Directory.Delete(tempFile.AbsolutePath);
            }

            return flag;
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
            move = true;
            return true;
        }

        public bool Paste(Template template)
        {
            string path = Path + "/" + Move_copy_file.Name;

            if (Move_copy_file.IsFile)
            {
                try
                {

                   javaIO.InputStream source=new javaIO.FileInputStream(Move_copy_file.AbsolutePath);
                    javaIO.OutputStream dest=new javaIO.FileOutputStream(path);
                    
                    byte[] buffer=new byte[1024];

                    int length;

                    while ((length = source.Read(buffer)) > 0)
                    {
                        dest.Write(buffer,0,length);
                    }

                    source.Close();
                    dest.Close();

                }
                catch (Exception e)
                {
                    return false;
                }

                if (move)
                {
                    Delete(template);
                    move = false;
                }
            }
            else if (Move_copy_file.IsDirectory)
            {
                new DirectoryInfo(template.Getfile.AbsolutePath).CreateSubdirectory(Move_copy_file.Name);
                File newDirectory=new File(path);
                if (newDirectory.Exists()) { }

                CopyDirectory(template);
            }

            template.Getfile = new File(path);

            ResFile.list.Add(template);

            Move_copy_file = null;

            return true;

        }

        private bool CopyDirectory(Template tempFile)
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

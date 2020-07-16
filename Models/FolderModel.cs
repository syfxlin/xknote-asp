using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace xknote.Models
{
    public class FolderModel
    {
        private string basePath { get; set; }
        
        public FolderModel(string basePath)
        {
            this.basePath = basePath;
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        public JContainer Get(string dir)
        {
            return this.Get(dir, "all");
        }
        
        public JContainer Get(string dir, string mode)
        {
            JObject re = new JObject();
            DirectoryInfo[] dirs = new DirectoryInfo(dir).GetDirectories();
            foreach (DirectoryInfo dirr in dirs)
            {
                re[dirr.Name] = JObject.FromObject(new
                {
                    type = "folder",
                    path = ReplacePath(dirr.FullName),
                    name = dirr.Name,
                    sub = this.Get(dirr.FullName, mode)
                });
            }

            if (mode == "all")
            {
                xknoteEntities entities = new xknoteEntities();
                Dictionary<string, config> config = entities.config.Select(i => i).ToDictionary(item => item.config_name);
                string documentExt = "." + config["document_ext"].config_value;
                string documentExtPreg = documentExt.Replace("|", "|.") + "$";
                FileInfo[] files = new DirectoryInfo(dir).GetFiles();
                foreach (FileInfo file in files)
                {
                    if (new Regex(documentExtPreg, RegexOptions.IgnoreCase).IsMatch(file.Name))
                    {
                        re[file.Name] = JObject.FromObject(new
                        {
                            type = "note",
                            path = ReplacePath(file.FullName),
                            name = file.Name
                        });
                    }
                }
            }

            return re;
        }

        public List<string> GetFlat(string dir)
        {
            DirectoryInfo[] dirs = new DirectoryInfo(dir).GetDirectories();
            List<string> list = new List<string>();
            foreach (DirectoryInfo info in dirs)
            {
                list.Add(ReplacePath(info.FullName));
            }

            return list;
        }

        public bool Exist(string path)
        {
            return Directory.Exists(path);
        }

        public int Create(string path)
        {
            if (this.Exist(path))
            {
                return 409;
            }
            Directory.CreateDirectory(path);
            return 200;
        }

        public int Delete(string path)
        {
            Directory.Delete(path);
            return 200;
        }

        public int Move(string newPath, string oldPath)
        {
            if (!this.Exist(oldPath))
            {
                return 404;
            }
            Directory.Move(oldPath, newPath);
            return 200;
        }

        public string ReplacePath(string path)
        {
            return path.Replace(basePath, "").Replace("\\", "/");
        }
    }
}
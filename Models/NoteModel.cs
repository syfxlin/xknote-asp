using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace xknote.Models
{
    public class NoteModel
    {
        public string basePath { get; set; }
        
        public NoteModel(string basePath)
        {
            this.basePath = basePath;
        }
        
        public dynamic Get(string path)
        {
            if (!this.Exist(path))
            {
                return 404;
            }
            string content = File.ReadAllText(path);
            string[] note = new string[2];
            if (content.Contains("===NoteInfo==="))
            {
                note = Regex.Split(content, "===NoteInfo===[\\r\\n]*");
            }
            else
            {
                note[1] = content;
                note[0] = "{\"title\": \"无标题\",\"created_at\": \"null\",\"updated_at\": \"null\",\"author\": \"null\"}";
            }
            JObject info = JObject.Parse(note[0]);
            info["content"] = note[1];
            return info;
        }

        public List<string> GetAll(string path)
        {
            string[] files = Directory.GetFiles(path);
            List<string> list = new List<string>();
            foreach (string file in files)
            {
                list.Add(ReplacePath(file));
            }
            return list;
        }

        private int Set(string path, JObject info, string content)
        {
            string contents = info + "===NoteInfo===\n\n" + content;
            File.WriteAllText(path, contents);
            return 200;
        }

        public int Create(string path, JObject info, string content)
        {
            if (this.Exist(path))
            {
                return 409;
            }
            this.Set(path, info, content);
            return 200;
        }

        public int Delete(string path)
        {
            File.Delete(path);
            return 200;
        }

        public int Edit(string path, JObject info, string content)
        {
            this.Set(path, info, content);
            return 200;
        }

        public int Move(string newPath, string oldPath)
        {
            if (!this.Exist(oldPath))
            {
                return 404;
            }
            File.Move(oldPath, newPath);
            return 200;
        }

        public JObject CheckStatus(string[] checkList, string[] pathList)
        {
            JObject reList = new JObject();
            for (int i = 0; i < checkList.Length; i++)
            {
                if (!this.Exist(checkList[i]))
                {
                    reList[pathList[i]] = JObject.FromObject(new
                    {
                        created_at = "No exists",
                        updated_at = "No exists"
                    });
                }
                else
                {
                    JObject note = this.Get(checkList[i]);
                    reList[pathList[i]] = JObject.FromObject(new
                    {
                        created_at = note["created_at"],
                        updated_at = note["updated_at"]
                    });
                }
            }
            return reList;
        }

        public bool Exist(string path)
        {
            return File.Exists(path);
        }
        
        public string ReplacePath(string path)
        {
            return path.Replace(basePath, "").Replace("\\", "/");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

namespace xknote.Models
{
    public class UserModel
    {
        public static JObject GetConfig(long id)
        {
            xknoteEntities entities = new xknoteEntities();
            user_config config = entities.user_config.Where(item => item.uid == id).FirstOrDefault();
            JObject result = new JObject();
            result["tinymceSetting"] = JObject.Parse(config.tinymce_setting);
            result["aceSetting"] = JObject.Parse(config.ace_setting);
            result["xkSetting"] = JObject.Parse(config.xk_setting);
            return result;
        }

        public static JObject GetDefaultConfig()
        {
            string path = HttpContext.Current.Server.MapPath("/Storage/setting.json");
            string json = File.ReadAllText(path);
            JObject config = JObject.Parse(json);
            return config;
        }

        public static void SetConfig(long id, JObject config)
        {
            xknoteEntities entities = new xknoteEntities();
            user_config userConfig = entities.user_config.Where(item => item.uid == id).FirstOrDefault();
            userConfig.tinymce_setting = config["tinymceSetting"].ToString();
            userConfig.ace_setting = config["aceSetting"].ToString();
            userConfig.xk_setting = config["xkSetting"].ToString();
            entities.SaveChanges();
        }

        public static IQueryable<users> GetAllUsers()
        {
            xknoteEntities entities = new xknoteEntities();
            IQueryable<users> users = entities.users.Select(i => i);
            return users;
        }

        public static int DeleteUser(long id)
        {
            xknoteEntities entities = new xknoteEntities();
            users user = entities.users.Where(item => item.id == id).FirstOrDefault();
            if (user != null)
            {
                entities.users.Remove(user);
                entities.SaveChanges();
                return 200;
            }

            return 404;
        }
    }
}
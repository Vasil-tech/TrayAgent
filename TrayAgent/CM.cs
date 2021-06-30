using System;
using System.Configuration;

namespace TrayAgent
{
    static class CM
    {   
        public static string GetString(string name, string defaultValue)
        {
            string result = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(result))
            {
                result = defaultValue;
            } 
            return result;
        }

        public static int GetInt32(string name, int defaultValue)
        {
            string textValue = ConfigurationManager.AppSettings[name];
            
            if (string.IsNullOrEmpty(textValue))
            {
                return defaultValue;
            }
            
            textValue = textValue.TrimStart('0'); //Убираем незначащие нули, оказалось, что они влияют
            if (string.IsNullOrEmpty(textValue))
            {
                return defaultValue;
            }
            
            int result;
            if (!int.TryParse(textValue, out result))
            {
                return defaultValue;
            }
            return result;
        }

        public static bool GetBoolean(string name, bool defaultValue)
        {
            string textValue = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(textValue))
            {
                return defaultValue;
            }

            bool result;
            if (!bool.TryParse(textValue, out result))
            {
                return defaultValue;
            }
            return result;
        }
    }
}

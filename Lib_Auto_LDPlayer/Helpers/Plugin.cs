using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System;
using Microsoft.VisualBasic;
using OtpNet;

namespace Auto_LDPlayer.Helpers
{
    public class Plugin
    {
        public static Dictionary<string, Bitmap> ImageClick = new Dictionary<string, Bitmap>();

        public static void LoadImages()
        {
            var directory = new DirectoryInfo(@"images\");
            var files = directory.GetFiles("*");
            foreach (var file in files)
            {
                try
                {
                    ImageClick.Add(file.Name, new Bitmap(file.FullName));
                }
                catch
                {
                }
            }
        }

        public static string Spin(ref string value)
        {
            string a = Regex.Replace(value, "{.*?}", new MatchEvaluator(Plugin.SpinEvaluator));
            return a;
        }

        public static string SpinEvaluator(Match match)
        {
            string text = match.ToString();
            string result;
            if (text.Contains("{"))
            {
                string[] array = text.Split(new char[] { '|' });
                string text2 = array[Plugin.RandomNumber(array.Length, 0)].Replace("{", "").Replace("}", "");
                result = text2;
            }
            else
            {
                result = text;
            }
            return result;
        }
        public static object Findmyfbid(string link)
        {
            return "";
        }
        public static object RegexIDGroup(string link)
        {
            object result;
            try
            {
                link = Strings.Mid(link, checked(link.IndexOf("groups/") + 8));
                if (link.Contains("?"))
                {
                    link = Strings.Mid(link, 1, link.IndexOf("?"));
                }
                else if (link.Contains("/"))
                {
                    link = Strings.Mid(link, 1, link.IndexOf("/"));
                }
                result = link;
            }
            catch
            {
                result = link;
            }
            return result;
        }

        public static int RandomNumber(int MaxNumber, int MinNumber = 0)
        {
            Random random = new Random();
            if (MinNumber > MaxNumber)
            {
                int num = MinNumber;
                MinNumber = MaxNumber;
                MaxNumber = num;
            }
            return random.Next(MinNumber, MaxNumber);
        }

        public static string GetKey2FA(string key)
        {
            try
            {
                try
                {
                    key = key.Trim().Replace(" ", "");
                }
                catch { }
                byte[] array = Base32Encoding.ToBytes(key);
                Totp totp = new Totp(array, 30, 0, 6, null);

                return totp.ComputeTotp(DateTime.UtcNow);
            }
            catch { }
            return null;
        }

        public static object TextToBase64(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

    }
}

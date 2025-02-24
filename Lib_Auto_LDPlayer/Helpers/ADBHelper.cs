using System.Drawing;
using Auto_LDPlayer.Enums;
using System.IO;
using KAutoHelper;

namespace Auto_LDPlayer.Helpers
{
    public class ADBHelper
    {
        public static bool ClickButton(LDType ldType, string nameOrId, Bitmap subBitmap, double percent = 0.9)
        {
            checked
            {
                bool result;
                try
                {
                    var point = FindButton(ldType, nameOrId, subBitmap);
                    if (point != null)
                    {
                        if ((point.X != 0) & (point.Y != 0))
                        {
                            LDPlayer.Tap(ldType, nameOrId, point.X + 10, point.Y + 10);
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
                catch
                {
                    result = false;
                }
                return result;
            }
        }

        public static void DeleteAllImage(LDType ldType, string nameOrID)
        {
            try
            {
                var images = LDPlayer.Adb(ldType, nameOrID, "shell ls /sdcard/DCIM").Split(new char[] { '\r' });
                foreach (var image in images)
                {
                    LDPlayer.Adb(ldType, nameOrID, $"shell rm -f \"\"/sdcard/DCIM/{image.Trim()}\"\"");
                }
            }
            catch
            {
            }
            try
            {
                LDPlayer.Adb(ldType, nameOrID, "shell rm -r /sdcard/Pictures");
            }
            catch
            {
            }
        }

        public static bool ExistImage(LDType ldType, string nameOrID, Bitmap btn, double percent = 0.9)
        {
            try
            {
                var point = FindButton(ldType, nameOrID, btn, percent);
                if (point != null)
                {
                    if ((point.X != 0) & (point.Y != 0))
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public static Point FindButton(LDType ldType, string nameOrID, Bitmap subBitmap, double percent = 0.9)
        {
            try
            {
                var mainBitmap = ScreenShoot(ldType, nameOrID, true, "screenShoot.png");
                var point = ImageScanOpenCV.FindOutPoint(mainBitmap, subBitmap, percent);
                if (point == null)
                    return default(Point);

                return new Point(point.Value.X, point.Value.Y);
            }
            catch
            {
                return default(Point);
            }
        }

        public static Bitmap ScreenShoot(LDType ldType, string nameOrID, bool isDeleteImageAfterCapture = true, string fileName = "screenShoot.png")
        {
            var fullFileName = Path.GetFileNameWithoutExtension(fileName) + nameOrID + Path.GetExtension(fileName);
            while (File.Exists(fullFileName))
            {
                try
                {
                    File.Delete(fullFileName);
                }
                catch
                {
                }
            }

            LDPlayer.Adb(ldType, nameOrID, $"shell screencap -p \"/sdcard/{fullFileName}\"");
            LDPlayer.Adb(ldType, nameOrID, $"pull \"/sdcard/{fullFileName}\"");
            LDPlayer.Adb(ldType, nameOrID, $"shell rm -f \"/sdcard/{fullFileName}\"");

            var result = new Bitmap(fullFileName);
            if (isDeleteImageAfterCapture)
            {
                try
                {
                    File.Delete(fullFileName);
                }
                catch
                {
                }
            }

            return result;
        }

        #region Private Methods
        #endregion
    }
}
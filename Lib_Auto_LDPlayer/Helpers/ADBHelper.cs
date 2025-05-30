using System.Drawing;
using Auto_LDPlayer.Enums;
using System.IO;
using KAutoHelper;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Web;
using System.Web.UI;
using System;
using Emgu.CV.CvEnum;

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
                        if ((point.Value.X != 0) & (point.Value.Y != 0))
                        {
                            LDPlayer.Tap(ldType, nameOrId, point.Value.X + 10, point.Value.Y + 10);
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
                    if ((point.Value.X != 0) & (point.Value.Y != 0))
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

        public static Point? FindButton(LDType ldType, string nameOrID, Bitmap subBitmap, double percent = 0.9)
        {
            try
            {
                var mainBitmap = ScreenShoot(ldType, nameOrID, true, "screenShoot.png");
                if (mainBitmap != null)
                {
                    var source = BitmapToImage(mainBitmap);
                    var temp = BitmapToImage(subBitmap);

                    var imageToShow = new Mat();
                    CvInvoke.MatchTemplate(source, temp, imageToShow, TemplateMatchingType.Ccoeff);

                    double minVal = 0, maxVal = 0;
                    Point minLoc = new Point(), maxLoc = new Point();
                    CvInvoke.MinMaxLoc(imageToShow, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                    if (maxVal < percent)
                        return default(Point);

                    return new Point(maxLoc.X, maxLoc.Y);
                    //var image = new Image<Bgr, byte>(mainBitmap);
                    //var image2 = new Image<Bgr, byte>(subBitmap);
                    //using (var image3 = image.MatchTemplate(image2, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
                    //{
                    //    double[] array1, array2;
                    //    Point[] array3, array4;
                    //    image3.MinMax(out array1, out array2, out array3, out array4);
                    //    if (array2[0] > percent)
                    //    {
                    //        result = new Point?(array4[0]);
                    //    }
                    //}
                }

                return default(Point);
            }
            catch
            {
                return default(Point);
            }
        }

        public static Bitmap ScreenShoot(LDType ldType, string nameOrID, bool isDeleteImageAfterCapture = true, string fileName = "screenShoot.png")
        {
            try
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

                Bitmap result;
                using (var bitmap = new Bitmap(fullFileName))
                {
                    result = new Bitmap(bitmap);
                }

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
            catch
            {
                return null;
            }
        }

        #region Private Methods
        private static Image<Bgr, byte> BitmapToImage(Bitmap bitmap)
        {
            try
            {
                var mat = BitmapExtension.ToMat(bitmap);

                return mat.ToImage<Bgr, byte>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        #endregion
    }
}
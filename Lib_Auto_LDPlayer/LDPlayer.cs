using Auto_LDPlayer.Constants;
using Auto_LDPlayer.Enums;
using Auto_LDPlayer.Extensions;
using Auto_LDPlayer.Helpers;
using Auto_LDPlayer.Helpers.Commons;
using Auto_LDPlayer.Helpers.IO;
using Auto_LDPlayer.Models;
using Auto_LDPlayer.Models.XML;
using Auto_LDPlayer.Properties;
using KAutoHelper;
using Microsoft.VisualBasic;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto_LDPlayer
{
    public class LDPlayer
    {
        private static Random Random = new Random();

        public static string PathLD = @"C:\LDPlayer\LDPlayer4.0\ldconsole.exe";
        public static string FolderLD = @"C:\LDPlayer\LDPlayer4.0";

        #region Control
        public static void Open(LDType ldType, string nameOrId)
        {
            ExecuteLD($"launch --{ldType.ToName()} {nameOrId}");
        }

        public static void OpenApp(LDType ldType, string nameOrId, string packageName)
        {
            ExecuteLD($"launchex --{ldType.ToName()} {nameOrId} --packagename {packageName}");
        }

        public static void Close(LDType ldType, string nameOrId)
        {
            ExecuteLD($"quit --{ldType.ToName()} {nameOrId}");
        }

        public static void CloseAll()
        {
            ExecuteLD("quitall");
        }

        public static bool IsAppOpened(LDType ldType, string nameOrId, string package)
        {
            var result = Adb(ldType, nameOrId, $"shell pidof {package}");
            return result != string.Empty;
        }
        #endregion

        #region Action LD
        public static void Create(string name)
        {
            ExecuteLD($"add --name {name}");
        }

        public static void Copy(string name, string fromNameOrId)
        {
            ExecuteLD($"copy --name {name} --from {fromNameOrId}");
        }

        public static void Delete(LDType ldType, string nameOrId)
        {
            ExecuteLD($"remove --{ldType.ToName()} {nameOrId}");
        }

        public static void Rename(LDType ldType, string nameOrId, string titleNew)
        {
            ExecuteLD($"rename --{ldType.ToName()} {nameOrId} --title {titleNew}");
        }
        #endregion

        #region Change Setting
        public static void InstallAppFile(LDType ldType, string nameOrId, string fileName)
        {
            ExecuteLD($@"installapp --{ldType.ToName()} {nameOrId} --filename ""{fileName}""");
        }

        public static void InstallAppPackage(LDType ldType, string nameOrId, string packageName)
        {
            ExecuteLD($"installapp --{ldType.ToName()} {nameOrId} --packagename {packageName}");
        }

        public static void UninstallApp(LDType ldType, string nameOrId, string packageName)
        {
            ExecuteLD($"uninstallapp --{ldType.ToName()} {nameOrId} --packagename {packageName}");
        }

        public static void RunApp(LDType ldType, string nameOrId, string packageName)
        {
            ExecuteLD($"runapp --{ldType.ToName()} {nameOrId} --packagename {packageName}");
        }

        public static void KillApp(LDType ldType, string nameOrId, string packageName)
        {
            ExecuteLD($"killapp --{ldType.ToName()} {nameOrId} --packagename {packageName}");
        }

        public static void KillApps(LDType ldType, string nameOrId, List<string> packages)
        {
            foreach(var package in packages)
            {
                KillApp(ldType, nameOrId, package);
            }
        }

        public static void Locate(LDType ldType, string nameOrId, string lng, string lat)
        {
            ExecuteLD($"locate --{ldType.ToName()} {nameOrId} --LLI {lng},{lat}");
        }

        public static void ChangeProperty(LDType ldType, string nameOrId, string cmd)
        {
            ExecuteLD($"modify --{ldType.ToName()} {nameOrId} {cmd}");
            //[--resolution ]
            //[--cpu < 1 | 2 | 3 | 4 >]
            //[--memory < 512 | 1024 | 2048 | 4096 | 8192 >]
            //[--manufacturer asus]
            //[--model ASUS_Z00DUO]
            //[--pnumber 13812345678]
            //[--imei ]
            //[--imsi ]    
            //[--simserial ]
            //[--androidid ]
            //[--mac ]
            //[--autorotate < 1 | 0 >]
            //[--lockwindow < 1 | 0 >]
        }

        public static void SetProp(LDType ldType, string nameOrId, string key, string value)
        {
            ExecuteLD($"setprop --{ldType.ToName()} {nameOrId} --key {key} --value {value}");
        }

        public static string GetProp(LDType ldType, string nameOrId, string key)
        {
            return ExecuteLDForResult($"getprop --{ldType.ToName()} {nameOrId} --key {key}");
        }

        public static string Adb(LDType ldType, string nameOrId, string cmd)
        {
            var command = $"adb --{ldType.ToName()} \"{nameOrId}\" --command \"{cmd}\"";
            var result = ExecuteLDForResult(command);
            if (result.Contains("error: failed to") || result.Contains("device not found") || result.Contains("device offline"))
            {
                Log.Information($"{command} => Adb() error: failed to || not found || device offline");
                Reboot(ldType, nameOrId);
                while (!IsDeviceRunning(ldType, nameOrId))
                {
                    Delay(10);
                }

                result = ExecuteLDForResult(command);
                if (!command.Contains("uiautomator dump"))
                {
                    Log.Information($"{command} => Adb() result2: {result}");
                }
            }
            else if (!command.Contains("uiautomator dump"))
            {
                Log.Information($"{command} => Adb() result1: {result}");
            }

            return result;
        }

        public static void AdbNoResult(LDType ldType, string nameOrId, string cmd)
        {
            ExecuteLD($"adb --{ldType.ToName()} \"{nameOrId}\" --command \"{cmd}\"");
        }

        public static void DownCpu(LDType ldType, string nameOrId, string rate)
        {
            ExecuteLD($"downcpu --{ldType.ToName()} {nameOrId} --rate {rate}");
        }

        public static void Backup(LDType ldType, string nameOrId, string filePath)
        {
            ExecuteLD($@"backup --{ldType.ToName()} {nameOrId} --file ""{filePath}""");
        }

        public static void Restore(LDType ldType, string nameOrId, string filePath)
        {
            ExecuteLD($@"restore --{ldType.ToName()} {nameOrId} --file ""{filePath}""");
        }

        public static void Action(LDType ldType, string nameOrId, string key, string value)
        {
            ExecuteLD($"action --{ldType.ToName()} {nameOrId} --key {key} --value {value}");
        }

        public static void Scan(LDType ldType, string nameOrId, string filePath)
        {
            ExecuteLD($"scan --{ldType.ToName()} {nameOrId} --file {filePath}");
        }

        public static void SortWnd()
        {
            ExecuteLD("sortWnd");
        }

        public static void ZoomIn(LDType ldType, string nameOrId)
        {
            ExecuteLD($"zoomIn --{ldType.ToName()} {nameOrId}");
        }

        public static void ZoomOut(LDType ldType, string nameOrId)
        {
            ExecuteLD($"zoomOut --{ldType.ToName()} {nameOrId}");
        }

        public static void Pull(LDType ldType, string nameOrId, string remoteFilePath, string localFilePath)
        {
            ExecuteLD($@"pull --{ldType.ToName()} {nameOrId} --remote ""{remoteFilePath}"" --local ""{localFilePath}""");
        }

        public static void Push(LDType ldType, string nameOrId, string remoteFilePath, string localFilePath)
        {
            ExecuteLD($@"push --{ldType.ToName()} {nameOrId} --remote ""{remoteFilePath}"" --local ""{localFilePath}""");
        }

        public static bool BackupApp(LDType ldType, string nameOrId, List<string> packages, string filePath, string fileName)
        {
            KillApps(ldType, nameOrId, packages);

            // Tạo lệnh tar để backup giữ nguyên quyền và metadata
            var cmd = $"shell tar -cvpf /data/local/tmp/{fileName}.vh " +
                "/data/data/com.facebook.katana/app_light_prefs/com.facebook.katana/* " +
                        "--exclude=*.so --exclude=*.dex --exclude=*.oat " +
                        "--exclude=/data/data/com.facebook.katana/app_state_logs/ " +
                        "--exclude=/data/data/com.facebook.katana/app_compactdisk/ " +
                        "--exclude=/data/data/com.facebook.katana/modules/ " +
                        "--exclude=/data/data/com.facebook.katana/cache/";

            // Thực hiện backup trên thiết bị
            var backupResult = Adb(ldType, nameOrId, cmd);
            Console.WriteLine("backupResult: " + backupResult);

            // Kéo file backup từ thiết bị về máy
            var isPullSuccess = PullOrPushFile(ldType, nameOrId, FileTransferAction.PULL, $"/data/local/tmp/{fileName}.vh", filePath);
            if (!isPullSuccess)
                return false;

            var deleteResult = Adb(ldType, nameOrId, $"shell rm -f /data/local/tmp/{fileName}.vh");
            Console.WriteLine("deleteResult: " + deleteResult);
            Console.WriteLine("✅ Backup hoàn tất!");

            return File.Exists(filePath);
        }

        public static bool RestoreApp(LDType ldType, string nameOrId, List<string> packages, string filePath, string fileName)
        {
            try
            {
                KillApps(ldType, nameOrId, packages);
                ClearApp(ldType, nameOrId, packages);

                var folder = filePath.Replace(".vh", string.Empty);
                try
                {
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{nameOrId} ❌ Lỗi tạo thư mục tạm: " + ex.Message);
                    return false;
                }

                try
                {
                    TarHelper.ExtractTGZ(filePath, folder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{nameOrId} ❌ Lỗi giải nén: " + ex.Message);
                    return false;
                }

                var pushResult = PullOrPushFile(ldType, nameOrId, FileTransferAction.PUSH, folder, "/");
                if (!pushResult)
                    return false;

                try
                {
                    Directory.Delete(folder, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{nameOrId} ⚠️ Lỗi xóa thư mục tạm: " + ex.Message);
                }
                Console.WriteLine($"{nameOrId} ✅ Khôi phục hoàn tất!");

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool PullOrPushFile(LDType ldType, string nameOrId, FileTransferAction action, string sourcePath, string destinationPath)
        {
            var arg = Enum.GetName(typeof(FileTransferAction), action).ToLower();
            var result = Adb(ldType, nameOrId, $"{arg} \"{sourcePath}\" \"{destinationPath}\"");
            if (string.IsNullOrEmpty(result) || result.Contains("error"))
            {
                Console.WriteLine("❌ Lỗi khi push dữ liệu lên thiết bị.");
                return false;
            }
            if (result.Contains("Timeout"))
            {
                result = Adb(ldType, nameOrId, $"{arg} \"{sourcePath}\" \"{destinationPath}\"");
                if (string.IsNullOrEmpty(result) || result.Contains("error"))
                {
                    Console.WriteLine("❌ Lỗi khi push dữ liệu lên thiết bị.");
                    return false;
                }
            }

            return true;
        }

        private static void SetIDApplication(LDType ldType, string nameOrId, string uid, string package)
        {
            Adb(ldType, nameOrId, $"shell su -c 'chown -R {uid}:{uid} /data/data/{package}'");
            Adb(ldType, nameOrId, $"shell su -c 'chown -R {uid} /data/user_de/0/{package}'");
            Adb(ldType, nameOrId, $"shell su -c 'chown -R {uid} /sdcard/Android/data/{package}'");

            //Adb(ldType, nameOrId, $"shell chown -R {uid} /sdcard/Android/obb/{package}/");
            Adb(ldType, nameOrId, $"shell su -c 'chown -R {uid} /data/misc/profiles/ref/{package}'");
            Adb(ldType, nameOrId, $"shell su -c 'chown -R {uid} /data/misc/profiles/cur/0/{package}'");
        }

        private static void WipePackages(LDType ldType, string nameOrId, List<string> packages)
        {
            foreach (var package in packages)
            {
                AdbNoResult(ldType, nameOrId, $"shell su -c 'rm -rf /data/data/{package} /data/user_de/0/{package} /sdcard/Android/data/{package} /data/misc/profiles/ref/{package} /data/misc/profiles/cur/0/{package}'");
            }
        }

        private static List<UID3rdPackage> GetIDApplications(LDType ldType, string nameOrId, List<string> packages)
        {
            var uids = new List<UID3rdPackage>();
            try
            {
                foreach (var package in packages)
                {
                    if (!string.IsNullOrWhiteSpace(package))
                    {
                        var uid = GetIDApplication(ldType, nameOrId, $"/data/data/{package}");
                        if (!string.IsNullOrWhiteSpace(uid))
                        {
                            uids.Add(new UID3rdPackage
                            {
                                PackageName = package,
                                UID = uid
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Information($"GetIDApplications EXCEPTION: {e}");
            }

            return uids;
        }

        private static string GetIDApplication(LDType ldType, string nameOrId, string package)
        {
            try
            {
                var result = Adb(ldType, nameOrId, $"shell stat -c '%u%' {package}");
                return result.Substring(0, result.LastIndexOf('?'));
            }
            catch (Exception)
            {
            }

            return string.Empty;
        }

        public static void GlobalConfig(LDType ldType, string nameOrId, string fps, string audio, string fastPlay, string cleanMode)
        {
            //  [--fps <0~60>] [--audio <1 | 0>] [--fastplay <1 | 0>] [--cleanmode <1 | 0>]
            ExecuteLD($"globalsetting --{ldType.ToName()} {nameOrId} --audio {audio} --fastplay {fastPlay} --cleanmode {cleanMode}");
        }

        public static List<string> GetDevices()
        {
            var devices = ExecuteLDForResult("list")?.Trim().Split('\n');
            if (devices == null)
                return new List<string>();

            for (var i = 0; i < devices.Length; i++)
            {
                if (devices[i] == "")
                    return new List<string>();
                devices[i] = devices[i].Trim();
            }

            return devices.ToList();
        }

        public static List<string> GetDevicesRunning()
        {
            var arr = ExecuteLDForResult("runninglist").Trim().Split('\n');
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] == "")
                    return new List<string>();
                arr[i] = arr[i].Trim();
            }

            return arr.ToList();
        }

        public static bool IsDeviceRunning(LDType ldType, string nameOrId)
        {
            var result = ExecuteLDForResult($"isrunning --{ldType.ToName()} {nameOrId}").Trim();
            return result == "running";
        }

        public static List<LDevice> GetDevices2()
        {
            try
            {
                var listLDPlayer = new List<LDevice>();
                var arr = ExecuteLDForResult("list2").Trim().Split('\n');
                foreach (var i in arr)
                {
                    var devices = new LDevice();
                    var aDetail = i.Trim().Split(',');
                    devices.index = int.Parse(aDetail[0]);
                    devices.name = aDetail[1];
                    devices.topHandle = new IntPtr(Convert.ToInt32(aDetail[2], 16));
                    devices.bindHandle = new IntPtr(Convert.ToInt32(aDetail[3], 16));
                    devices.androidState = int.Parse(aDetail[4]);
                    devices.dnplayerPID = int.Parse(aDetail[5]);
                    devices.vboxPID = int.Parse(aDetail[6]);
                    listLDPlayer.Add(devices);
                }

                return listLDPlayer;
            }
            catch
            {
                return new List<LDevice>();
            }
        }
        
        public static List<LDevice> GetDevices2Running()
        {
            try
            {
                var listLDPlayer = new List<LDevice>();
                var deviceRunning = GetDevicesRunning();
                var devices = ExecuteLDForResult("list2").Trim().Split('\n');
                foreach (var t in devices)
                {
                    var device = new LDevice();
                    var aDetail = t.Trim().Split(',');
                    device.index = int.Parse(aDetail[0]);
                    device.name = aDetail[1];
                    device.topHandle = new IntPtr(Convert.ToInt32(aDetail[2], 16));
                    device.bindHandle = new IntPtr(Convert.ToInt32(aDetail[3], 16));
                    device.androidState = int.Parse(aDetail[4]);
                    device.dnplayerPID = int.Parse(aDetail[5]);
                    device.vboxPID = int.Parse(aDetail[6]);
                    if (!deviceRunning.Contains(device.name)) continue;
                    listLDPlayer.Add(device);
                }

                return listLDPlayer;
            }
            catch
            {
                return new List<LDevice>();
            }
        }

        public static List<string> GetDevices3()
        {
            try
            {
                var listLDPlayer = new List<string>();
                var devices = ExecuteADB("devices").Trim().Split('\n');
                for (var i = 1; i < devices.Length; i++)
                {
                    var device = devices[i].Replace("\tdevice", string.Empty);
                    listLDPlayer.Add(device);
                }
                
                return listLDPlayer;
            }
            catch
            {
                return new List<string>();
            }
        }

        public static void ExecuteLD(string cmd)
        {
            try
            {
                DelayRandom(1, 2);
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = PathLD,
                        Arguments = cmd,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    };

                    process.Start();
                    process.WaitForExit();
                    process.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ExecuteLD() Error: {ex.Message}");
            }
        }

        public static string ExecuteLDForResult(string cmd, int timeout = 30000)
        {
            try
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = PathLD,
                        Arguments = cmd,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };

                    process.Start();
                    var outputTask = Task.Run(() => process.StandardOutput.ReadToEnd());
                    if (Task.WaitAny(new Task[] { outputTask }, timeout) == 0)
                    {
                        process.WaitForExit();

                        return outputTask.Result.Trim();
                    }
                    else
                    {
                        process.Kill();

                        return "Timeout";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"ExecuteLDForResult() Error: {ex.Message}";
            }
        }

        public static string ExecuteADB(string cmd, int timeout = 30000)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo($"{FolderLD}\\adb.exe")
                    {
                        WorkingDirectory = FolderLD,
                        Arguments = cmd,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
                };

                process.Start();
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit(timeout);

                return result;
            }
            catch
            {
                return "error";
            }
        }

        public static Point GetScreenResolution(LDType ldType, string nameOrId)
        {
            var str1 = Adb(ldType, nameOrId, "shell dumpsys display | grep \"mCurrentDisplayRect\"");
            var str2 = str1.Substring(str1.IndexOf("- ", StringComparison.Ordinal));
            var strArray = str2.Substring(str2.IndexOf(' '), str2.IndexOf(')') - str2.IndexOf(' ')).Split(',');
            return new Point(Convert.ToInt32(strArray[0].Trim()), Convert.ToInt32(strArray[1].Trim()));
        }

        public static void TapByPercent(LDType ldType, string nameOrId, double x, double y, int count = 1)
        {
            var screenResolution = GetScreenResolution(ldType, nameOrId);
            var num1 = (int) (x * (screenResolution.X * 1.0 / 100.0));
            var num2 = (int) (y * (screenResolution.Y * 1.0 / 100.0));
            Tap(ldType, nameOrId, num1, num2, count);
        }

        public static void Tap(LDType ldType, string nameOrId, int x, int y, int count = 1)
        {
            var cmdCommand = $"shell input tap {x} {y}";
            for (var index = 1; index < count; ++index)
                cmdCommand += (" && " + cmdCommand);
            Adb(ldType, nameOrId, cmdCommand);
        }

        public static void PressKey(LDType ldType, string nameOrId, LDKeyEvent key)
        {
            Adb(ldType, nameOrId, $"shell input keyevent {key}");
        }

        public static void SwipeByPercent(LDType ldType, string nameOrId, double x1, double y1, double x2, double y2, int duration = 100)
        {
            var screenResolution = GetScreenResolution(ldType, nameOrId);
            var num1 = (int) (x1 * (screenResolution.X * 1.0 / 100.0));
            var num2 = (int) (y1 * (screenResolution.Y * 1.0 / 100.0));
            var num3 = (int) (x2 * (screenResolution.X * 1.0 / 100.0));
            var num4 = (int) (y2 * (screenResolution.Y * 1.0 / 100.0));
            Swipe(ldType, nameOrId, num1, num2, num3, num4, duration);
        }

        public static void Swipe(LDType ldType, string nameOrId, int x1, int y1, int x2, int y2, int duration = 100)
        {
            Adb(ldType, nameOrId, $"shell input swipe {x1} {y1} {x2} {y2} {duration}");
        }

        public static void InputText(LDType ldType, string nameOrId, string text)
        {
            Adb(ldType, nameOrId,  $"shell input text \"{text.Replace(" ", "%s").Replace("&", "\\&").Replace("<", "\\<").Replace(">", "\\>").Replace("?", "\\?").Replace(":", "\\:").Replace("{", "\\{").Replace("}", "\\}").Replace("[", "\\[").Replace("]", "\\]").Replace("|", "\\|")}\"");
        }

        public static void LongPress(LDType ldType, string nameOrId, int x, int y, int duration = 100)
        {
            Swipe(ldType, nameOrId, x, y, x, y, duration);
        }

        public static Bitmap ScreenShoot(LDType ldType, string nameOrId, bool isDeleteImageAfterCapture = true, string fileName = "screenShoot.png")
        {
            var str1 = ldType + "_" + nameOrId;


            var path = Path.GetFileNameWithoutExtension(fileName) + str1 + Path.GetExtension(fileName);
            if (File.Exists(path))
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                    // ignored
                }

            var filename = Directory.GetCurrentDirectory() + "\\" + path;
            var str2 = $"\"{Directory.GetCurrentDirectory().Replace("\\\\", "\\")}\"";
            var cmdCommand1 = $"shell screencap -p \"/sdcard/{path}\"";
            var cmdCommand2 = $"pull /sdcard/{path} {str2}";
            Adb(ldType, nameOrId, cmdCommand1);
            Adb(ldType, nameOrId, cmdCommand2);
            Bitmap bitmap = null;
            try
            {
                using (var original = new Bitmap(filename))
                {
                    bitmap = new Bitmap(original);
                }
            }
            catch
            {
                // ignored
            }

            if (!isDeleteImageAfterCapture) return bitmap;
            try
            {
                File.Delete(path);
            }
            catch
            {
                // ignored
            }

            try
            {
                Adb(ldType, nameOrId, $"shell \"rm /sdcard/{path}\"");
            }
            catch
            {
                // ignored
            }

            return bitmap;
        }

        public static void PlanModeOn(LDType ldType, string nameOrId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            Adb(ldType, nameOrId, " settings put global airplane_mode_on 1");
            Adb(ldType, nameOrId, "am broadcast -a android.intent.action.AIRPLANE_MODE");
        }

        public static void PlanModeOff(LDType ldType, string nameOrId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            Adb(ldType, nameOrId, " settings put global airplane_mode_on 1");
            Adb(ldType, nameOrId, "am broadcast -a android.intent.action.AIRPLANE_MODE");
        }

        public static void Delay(double delayTime)
        {
            delayTime = delayTime * 1000;
            for (var num = 0.0; num < delayTime; num += 100.0)
                Thread.Sleep(100);
        }

        public static void DelayRandom(int delayTime1, int delayTime2)
        {
            Delay(Random.Next(delayTime1, delayTime2 + 1));
        }

        public static Point? FindImage(LDType ldType, string nameOrId, string imagePath, int count = 5)
        {
            var files = new DirectoryInfo(imagePath).GetFiles();
            do
            {
                Bitmap mainBitmap = null;
                var num = 3;
                do
                {
                    try
                    {
                        mainBitmap = ScreenShoot(ldType, nameOrId);
                        break;
                    }
                    catch (Exception)
                    {
                        --num;
                        Delay(1000.0);
                    }
                } while (num > 0);

                if (mainBitmap == null)
                    return new Point?();
                var image = new Point?();
                foreach (var fileSystemInfo in files)
                {
                    var subBitmap = (Bitmap)Image.FromFile(fileSystemInfo.FullName);
                    image = ImageScanOpenCV.FindOutPoint(mainBitmap, subBitmap);
                    if (image.HasValue)
                        break;
                }

                if (image.HasValue)
                    return image;
                Delay(2000.0);
                --count;
            } while (count > 0);

            return new Point?();
        }

        public static bool FindImageAndClick(LDType ldType, string nameOrId, string imagePath, int count = 5)
        {
            var point = FindImage(ldType, nameOrId, imagePath, count);
            if (!point.HasValue) return false;
            Tap(ldType, nameOrId, point.Value.X, point.Value.Y);
            return true;
        }

        public static void OffLocation(LDType ldType, string nameOrId)
        {
            AdbNoResult(ldType, nameOrId, "shell settings put secure location_mode 0");
            AdbNoResult(ldType, nameOrId, "shell settings put secure location_providers_allowed -gps");
            AdbNoResult(ldType, nameOrId, "shell settings put secure location_providers_allowed -network");
        }

        public static void ChangeTimeZone(LDType ldType, string nameOrId)
        {
            AdbNoResult(ldType, nameOrId, "shell settings put global auto_time_zone 0");
            AdbNoResult(ldType, nameOrId, "shell setprop persist.sys.country US");
            AdbNoResult(ldType, nameOrId, $"shell setprop persist.sys.timezone {LDCommon.RandomTimeZone()}");
        }

        public static void ChangeLanguage(LDType ldType, string nameOrId)
        {
            AdbNoResult(ldType, nameOrId, "shell settings put global auto_locale 0");
            AdbNoResult(ldType, nameOrId, "shell setprop persist.sys.locale en-US");
            AdbNoResult(ldType, nameOrId, "shell settings put system system_locales en-US");
        }

        public static void EnableADBRoot()
        {
            ExecuteADB("root");
        }

        public static void AllowPermission(LDType ldType, string nameOrId)
        {
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.ACCESS_COARSE_LOCATION");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.WAKE_LOCK");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.INTERNET");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.VIBRATE");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.READ_CONTACTS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.WRITE_CONTACTS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.GET_ACCOUNTS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.MANAGE_ACCOUNTS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.AUTHENTICATE_ACCOUNTS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.READ_SYNC_SETTINGS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.WRITE_SYNC_SETTINGS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.ACCESS_FINE_LOCATION");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.DOWNLOAD_WITHOUT_NOTIFICATION");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.CAMERA");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.SYSTEM_ALERT_WINDOW");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.RECORD_AUDIO");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.WRITE_EXTERNAL_STORAGE");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.ACCESS_NETWORK_STATE");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.ACCESS_WIFI_STATE");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.READ_PHONE_STATE");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.RECEIVE_BOOT_COMPLETED");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.GET_TASKS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.REORDER_TASKS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.CALL_PHONE");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.BATTERY_STATS");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.READ_EXTERNAL_STORAGE");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.READ_CALL_LOG");
            Adb(ldType, nameOrId, "shell pm grant com.facebook.katana android.permission.WRITE_CALL_LOG");
        }

        public static void OpenLink(LDType ldType, string nameOrId, string link)
        {
            link = link.Replace("&", "\\&");
            Adb(ldType, nameOrId, $"shell am start -a android.intent.action.VIEW -d \"{link}\"");
            Delay(1);
            ClickText(ldType, nameOrId, "Always", 0, 0);
        }

        public static bool ClickText(LDType ldType, string nameOrId, string text, int xC = 0, int yC = 0)
        {
            checked
            {
                try
                {
                    var result = DumpScreen(ldType, nameOrId);
                    if (result.Contains(text))
                    {
                        var arrStr = result.Split(new char[] { '>' });
                        foreach (var str in arrStr)
                        {
                            if (str.Contains(text))
                            {
                                var element = str;
                                element = Strings.Mid(element, element.IndexOf("bounds=") + 10);
                                element = Strings.Mid(element, 1, element.IndexOf("\"") - 1);
                                var array3 = element.Split(new char[] { ']' });
                                var array4 = array3[0].Split(new char[] { ',' });
                                var array5 = array3[1].Replace("[", "").Split(new char[] { ',' });
                                
                                var num = (int)Math.Round(unchecked((double)(Convert.ToInt16(array4[0]) + Convert.ToInt16(array5[0])) / 2.0 + 10.0));
                                var num2 = (int)Math.Round((double)(unchecked(Convert.ToInt16(array4[1]) + Convert.ToInt16(array5[1]))) / 2.0);
                                Tap(ldType, nameOrId, num + xC, num2 + yC);

                                return true;
                            }
                        }
                    }
                }
                catch
                {
                }
                return false;
            }
        }

        public static string DumpScreen(LDType ldType, string nameOrId)
        {
            try
            {
                var result = Adb(ldType, nameOrId, "shell uiautomator dump").ToString().Split(new char[] { ':' })[1].Trim();

                var path = $"{Directory.GetCurrentDirectory()}\\data\\account\\_x{nameOrId}";
                Adb(ldType, nameOrId, $"pull {result} {path}");

                var result2 = File.ReadAllText(path);
                File.Delete(path);

                return result2;
            }
            catch
            {
            }

            return string.Empty;
        }
        #endregion

        #region Navigation
        public static void Back(LDType ldType, string nameOrId)
        {
            PressKey(ldType, nameOrId, LDKeyEvent.KEYCODE_BACK);
        }

        public static void Home(LDType ldType, string nameOrId)
        {
            PressKey(ldType, nameOrId, LDKeyEvent.KEYCODE_HOME);
        }

        public static void Menu(LDType ldType, string nameOrId)
        {
            PressKey(ldType, nameOrId, LDKeyEvent.KEYCODE_APP_SWITCH);
        }
        #endregion

        #region OpenCV
        public static bool TapImg(LDType ldType, string nameOrId, Bitmap imgFind)
        {
            var bm = (Bitmap)imgFind.Clone();
            var screen = ScreenShoot(ldType, nameOrId);
            var point = ImageScanOpenCV.FindOutPoint(screen, bm);
            if (point == null) return false;

            Tap(ldType, nameOrId, point.Value.X, point.Value.Y);

            return true;
        }
        #endregion

        #region Change Proxy
        public static void ChangeProxy(LDType ldType, string nameOrId, string ipProxy, string portProxy)
        {
            AdbNoResult(ldType, nameOrId, $"shell settings put global http_proxy {ipProxy}:{portProxy}");
        }

        public static void RemoveProxy(LDType ldType, string nameOrId)
        {
            AdbNoResult(ldType, nameOrId, "shell settings put global http_proxy :0");
        }

        public static void ProxyON(LDType ldType, string nameOrId, NetIP netIP)
        {
            AdbNoResult(ldType, nameOrId, $"shell su -c 'settings put global http_proxy {netIP.HostNamePortforward}:{netIP.PortForward}'");
        }

        public static void ProxyOFF(LDType ldType, string nameOrId)
        {
            AdbNoResult(ldType, nameOrId, "shell su -c 'settings put global http_proxy :0'");
        }
        #endregion

        #region Text
        public static string ExportText(LDType ldType, string nameOrId)
        {
            var path = string.Concat(Path.GetTempPath(), "window_dump.xml");
            AdbNoResult(ldType, nameOrId, "shell uiautomator dump");
            AdbNoResult(ldType, nameOrId, $"pull /sdcard/window_dump.xml \"{path}\"");
            
            var end = string.Empty;
            try
            {
                var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    end = streamReader.ReadToEnd();
                }
                File.Delete(path);
            }
            catch (Exception)
            {
            }
            ExecuteLD("rm -rf sdcard/window_dump.xml");

            return end;
        }

        public static SearchText FindText(LDType ldType, string nameOrId, string text)
        {
            var exportText = ExportText(ldType, nameOrId);
            if (!string.IsNullOrEmpty(exportText))
            {
                text = text.Split(new char[] { '|' })[0];
                var listTextFromPhone = GetListTextFromPhone(exportText);

                return listTextFromPhone.FirstOrDefault(x => x.TEXT.Equals(text) || x.CONTENT_DESC.Equals(text) || x.RESOURCE_ID.Equals(text));
            }

            return null;
        }

        public static List<SearchText> GetListTextFromPhone(string fileDump)
        {
            Hierarchy hierarchy;
            List<SearchText> searchTexts = new List<SearchText>();
            var random = new Random();
            try
            {
                string str = fileDump.Replace("</node>", "").Replace("]\">", "]\"/>");
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Hierarchy));
                using (TextReader stringReader = new StringReader(str))
                {
                    hierarchy = xmlSerializer.Deserialize(stringReader) as Hierarchy;
                }
                for (int i = 0; i < hierarchy.Node.Count; i++)
                {
                    var item = hierarchy.Node[i];
                    if (!string.IsNullOrEmpty(item.Text) || !string.IsNullOrEmpty(item.ContentDesc) || !string.IsNullOrEmpty(item.ResourceId) || !string.IsNullOrEmpty(item.Password))
                    {
                        var searchText = new SearchText
                        {
                            TEXT = item.Text,
                            CONTENT_DESC = item.ContentDesc,
                            RESOURCE_ID = item.ResourceId,
                            PASSWORD = item.Password,
                            CHECKED = bool.Parse(item.Checked),
                            INDEX = i
                        };
                        string str1 = item.Bounds.Replace("][", "@");
                        str1 = str1.Replace(",", "@");
                        str1 = str1.Replace("[", "");
                        str1 = str1.Replace("]", "");
                        string[] strArrays = str1.Split(new char[] { '@' });
                        searchText.WIDTH_1 = strArrays[0];
                        searchText.HEIGHT_1 = strArrays[1];
                        searchText.WIDTH_2 = strArrays[2];
                        searchText.HEIGHT_2 = strArrays[3];
                        var x = (int.Parse(searchText.WIDTH_1) + int.Parse(searchText.WIDTH_2)) / 2;
                        var y = (int.Parse(searchText.HEIGHT_1) + int.Parse(searchText.HEIGHT_2)) / 2;
                        searchText.X = string.Concat(x.ToString(), ".0");
                        searchText.Y = string.Concat(y.ToString(), ".0");
                        searchText.X_NUM = x;
                        searchText.Y_NUM = y;

                        searchTexts.Add(searchText);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Information($"GetListTextFromPhone EXCEPTION: {ex.Message}");
            }
            return searchTexts;
        }
        #endregion

        #region Scripts
        public static void RunScript(LDType ldType, string nameOrId, string command)
        {
            var scriptFromSearchText = GetScriptFromSearchText(ldType, nameOrId, command);
            int num = 0;
            while (true)
            {
                if (scriptFromSearchText.Count == 0)
                {
                    if (num == 2)
                    {
                        break;
                    }
                    scriptFromSearchText = GetScriptFromSearchText(ldType, nameOrId, command);
                    num++;
                }
                else
                {
                    List<string>.Enumerator enumerator = scriptFromSearchText.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            try
                            {
                                string[] strArrays = current.Split(new char[] { '=' });
                                if (!strArrays[0].Equals(ScriptConstants.Wait.Replace("=", "")))
                                {
                                    AdbNoResult(ldType, nameOrId, CreateScript(command));
                                }
                                else
                                {
                                    Thread.Sleep(int.Parse(strArrays[1]));
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        break;
                    }
                    finally
                    {
                        ((IDisposable)enumerator).Dispose();
                    }
                }
            }
        }

        private static List<string> GetScriptFromSearchText(LDType ldType, string nameOrId, string script)
        {
            List<string> strs;
            try
            {
                var strs1 = new List<string>();
                var strArrays = script.Split(new char[] { '=' });
                if (ScriptConstants.SearchAndClick.IndexOf(strArrays[0]) >= 0 || ScriptConstants.SearchAndClickExactly.IndexOf(strArrays[0]) >= 0 || ScriptConstants.SearchAndClickStop.IndexOf(strArrays[0]) >= 0 || ScriptConstants.SearchAndRunScript.IndexOf(strArrays[0]) >= 0 || ScriptConstants.SmartSearchText.IndexOf(strArrays[0]) >= 0)
                {
                    if (strArrays.Length < 2)
                    {
                        strs = strs1;
                        return strs;
                    }
                    else
                    {
                        SearchText searchText = FindText(ldType, nameOrId, strArrays[1]);
                        string str = strArrays[1];
                        string[] strArrays1 = str.Split(new char[] { '|' });
                        int num = 0;
                        if (strArrays1.Length >= 2)
                        {
                            num = int.Parse(strArrays1[1]);
                        }
                        DateTime now = DateTime.Now;
                        TimeSpan timeSpan = DateTime.Now - now;
                        double totalMilliseconds = timeSpan.TotalMilliseconds;
                        while (searchText == null)
                        {
                            if (totalMilliseconds < (double)num)
                            {
                                searchText = FindText(ldType, nameOrId, strArrays[1]);
                                timeSpan = DateTime.Now - now;
                                totalMilliseconds = timeSpan.TotalMilliseconds;
                            }
                            else if (ScriptConstants.SmartSearchText.IndexOf(strArrays[0]) >= 0 && (int)strArrays1.Length >= 3)
                            {
                                script = string.Concat("ScriptFromFileNew=", strArrays1[2]);
                                strs1.Add(script);
                                strs = strs1;
                                return strs;
                            }
                            else if (ScriptConstants.SearchAndClick.IndexOf(strArrays[0]) < 0)
                            {
                                strs = strs1;
                                return strs;
                            }
                            else
                            {
                                strs = strs1;
                                return strs;
                            }
                        }
                        int num1 = int.Parse(searchText.WIDTH_1);
                        int num2 = int.Parse(searchText.HEIGHT_1);
                        int num3 = int.Parse(searchText.WIDTH_2);
                        int num4 = int.Parse(searchText.HEIGHT_2);
                        num1 = (num1 + num3) / 2;
                        num2 = (num2 + num4) / 2;
                        string str1 = string.Concat(num1.ToString(), ".0");
                        string str2 = string.Concat(num2.ToString(), ".0");
                        string.Concat(num3.ToString(), ".0");
                        string.Concat(num4.ToString(), ".0");
                        if (ScriptConstants.SearchAndClick.IndexOf(strArrays[0]) >= 0)
                        {
                            script = string.Concat("ClickXY=", str1, " ", str2);
                            strs1.Add(script);
                            strs = strs1;
                            return strs;
                        }
                        else if (ScriptConstants.SearchAndClickStop.IndexOf(strArrays[0]) >= 0)
                        {
                            script = string.Concat("ClickXY=", str1, " ", str2);
                            strs1.Add(script);
                            strs = strs1;
                            return strs;
                        }
                        else if (ScriptConstants.SearchAndRunScript.IndexOf(strArrays[0]) >= 0 && (int)strArrays1.Length >= 3)
                        {
                            script = string.Concat("ScriptSmall=", strArrays1[2]);
                            strs1.Add(script);
                            strs = strs1;
                            return strs;
                        }
                        else if (ScriptConstants.SmartSearchText.IndexOf(strArrays[0]) >= 0)
                        {
                            script = string.Concat("ClickXY=", str1, " ", str2);
                            strs1.Add(script);
                            strs = strs1;
                            return strs;
                        }
                        else if (ScriptConstants.SearchWaitClick.IndexOf(strArrays[0]) >= 0)
                        {
                            script = string.Concat(ScriptConstants.Wait, strArrays1[2]);
                            strs1.Add(script);
                            script = string.Concat("ClickXY=", str1, " ", str2);
                            strs1.Add(script);
                            strs = strs1;
                            return strs;
                        }
                    }
                }
                strs1.Add(script);
                strs = strs1;
            }
            catch (Exception ex)
            {
                strs = new List<string>();
            }
            return strs;
        }

        private static string CreateScript(string command)
        {
            var str = string.Empty;
            if (command.IndexOf(ScriptConstants.SendCommand) < 0)
            {
                string commandLine = GetCommandLine(command);
                str = (commandLine.IndexOf("shell") >= 0 || commandLine.IndexOf("pull") >= 0 || commandLine.IndexOf("reboot") >= 0 || commandLine.IndexOf("disconnect") >= 0 || commandLine.IndexOf("push") >= 0 ? string.Concat(str, commandLine) : string.Concat(str, "shell ", commandLine));
            }
            else
            {
                string[] strArrays = command.Split(new char[] { '=' });
                str = string.Concat(str, strArrays[1].Trim());
            }

            return str;
        }

        private static string GetCommandLine(string command)
        {
            string str = "";
            ScriptSetting scriptSetting = GetScriptSetting();
            bool flag = false;
            string[] strArrays = command.Split(new char[] { '=' });
            if (scriptSetting != null && (int)strArrays.Length >= 2)
            {
                ScriptKey[] sCRIPTKEY = scriptSetting.SCRIPTKEY;
                int num = 0;
                while (num < (int)sCRIPTKEY.Length)
                {
                    ScriptKey scriptKey = sCRIPTKEY[num];
                    if (strArrays[0].Equals(scriptKey.KEY))
                    {
                        string str1 = strArrays[1];
                        if (strArrays[0].Equals(ScriptConstants.Send_text.Replace("=", "")))
                        {
                            str1 = str1.Trim();
                            str1 = str1.Replace(" ", "%");
                        }
                        else if (strArrays[0].Equals(ScriptConstants.RandomFromFile.Replace("=", "")))
                        {
                            str1 = str1.Trim();
                            str1 = str1.Replace(" ", "%");
                        }
                        else if (strArrays[0].Equals(ScriptConstants.TextFromFile.Replace("=", "")))
                        {
                            str1 = str1.Trim();
                            str1 = str1.Replace(" ", "%");
                        }
                        str = string.Format(scriptKey.VALUE, str1);
                        flag = true;
                        if (!flag)
                        {
                            str = command;
                        }
                        return str;
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            if (!flag)
            {
                str = command;
            }
            return str;
        }

        private static ScriptSetting GetScriptSetting()
        {
            ScriptSetting scriptSetting;
            string str = Resources.ScriptSetting;
            try
            {
                MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(str));
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScriptSetting));
                scriptSetting = (ScriptSetting)xmlSerializer.Deserialize(new StreamReader(memoryStream));
            }
            catch (Exception ex)
            {
                scriptSetting = null;
            }
            return scriptSetting;
        }
        #endregion

        #region Actions
        public static bool WaitClickFindText(LDType ldType, string nameOrId, string text, int times = 15)
        {
            var xy = FindText(ldType, nameOrId, text);
            var i = 0;
            while (xy == null && i < times)
            {
                xy = FindText(ldType, nameOrId, text);
                Delay(1);
                i++;
            }
            if (xy != null)
            {
                RunScript(ldType, nameOrId, $"ClickXY={xy.X} {xy.Y}");

                return true;
            }

            return false;
        }

        public static bool WaitFindText(LDType ldType, string nameOrId, string text, int times = 15)
        {
            var xy = FindText(ldType, nameOrId, text);
            var i = 0;
            while (xy == null && i < times)
            {
                xy = FindText(ldType, nameOrId, text);
                Delay(1);
                i++;
            }

            return xy != null;
        }

        public static void ClearApp(LDType ldType, string nameOrId, List<string> packages)
        {
            foreach (var package in packages)
            {
                AdbNoResult(ldType, nameOrId, $"shell pm clear {package}");
            }
        }
        
        public static bool ClearDataLD(LDType ldType, string nameOrId)
        {
            var result = Adb(ldType, nameOrId, "shell rm -rf /data/* && reboot");

            return result.Contains("Success");
        }

        public static bool IsInstalledApp(string deviceName, string package)
        {
            var result = ExecuteLDForResult($"adb --name {deviceName} --command \"shell pm list packages\"");

            return result.Contains(package);
        }

        public static bool IsInstalledApps(LDType ldType, string nameOrId, List<string> packages)
        {
            var result = ExecuteLDForResult($"adb --{ldType.ToName()} {nameOrId} --command \"shell pm list packages\"");
            if (!result.Contains("not found"))
            {
                foreach (var package in packages)
                {
                    if (!result.Contains(package))
                        return false;
                }
            }

            return true;
        }

        public static void Reboot(LDType ldType, string nameOrId)
        {
            AdbNoResult(ldType, nameOrId, "shell reboot");
        }
        #endregion
    }
}
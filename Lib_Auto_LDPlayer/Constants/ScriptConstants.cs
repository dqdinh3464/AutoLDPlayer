using System.Collections.Generic;

namespace Auto_LDPlayer.Constants
{
    public class ScriptConstants
    {
        public static string ClickXY;

        public static string Send_key;

        public static string Wait;

        public static string Wait_Connect;

        public static string Open_app;

        public static string Close_app;

        public static string Clear_app;

        public static string Swipe;

        public static string Start_apk;

        public static string Send_text;

        public static string TextFromFile;

        public static string RandomFromFile;

        public static string RandomScriptFromFile;

        public static string ScriptFromFile;

        public static string SearchAndClick;

        public static string SearchWaitClick;

        public static string SearchAndClickExactly;

        public static string SearchAndClickStop;

        public static string SearchWaitClickStop;

        public static string SearchAndClickStopExactly;

        public static string SearchAndRunScript;

        public static string SmartSearchText;

        public static string SearchFileAndClick;

        public static string SearchFileWaitClick;

        public static string SearchFileAndClickExactly;

        public static string SearchFileAndClickStop;

        public static string SearchFileWaitClickStop;

        public static string SearchFileAndClickStopExactly;

        public static string SearchFileAndRunScript;

        public static string SendCommand;

        public static string BackupGmail;

        public static string RestoreGmail;

        public static string FakeDevice;

        public static string Return;

        public static string BypassCapChar;

        public static string Read_Gmail;

        public static string Read_Gmail_Restore;

        public static string Get_Gmail;

        public static string SendUser;

        public static string SendUserGet;

        public static string SendPassword;

        public static string SendPasswordGet;

        public static string SendMailRecover;

        public static string SendMailRecoverGet;

        public static string CheckSim;

        public static List<string> LIST_SCRIPT_SPEC;

        public static string ClickXY_CD;

        public static string Send_key_CD;

        public static string Wait_CD;

        public static string Open_app_CD;

        public static string Close_app_CD;

        public static string Swipe_CD;

        public static string Start_apk_CD;

        public static string Send_text_CD;

        public static string TextFromFile_CD;

        public static string RandomFromFile_CD;

        static ScriptConstants()
        {
            ClickXY = "ClickXY=x y";
            Send_key = "Send_key=";
            Wait = "Wait=";
            Wait_Connect = "Wait_Network=";
            Open_app = "Open_app=";
            Close_app = "Close_app=";
            Clear_app = "Clear_app=";
            Swipe = "Swipe=x y xv yv";
            Start_apk = "Start_apk=";
            Send_text = "Send_text=";
            TextFromFile = "TextFromFile=";
            RandomFromFile = "RandomFromFile=";
            RandomScriptFromFile = "RandomScriptFromFile=";
            ScriptFromFile = "ScriptFromFile=";
            SearchAndClick = "SearchAndClick=abc|3000";
            SearchWaitClick = "SearchWaitClick=abc|3000|1000";
            SearchAndClickExactly = "SearchAndClickExactly=abc|3000";
            SearchAndClickStop = "SearchAndClickStop=abc|3000";
            SearchWaitClickStop = "SearchWaitClickStop=abc|3000|1000";
            SearchAndClickStopExactly = "SearchAndClickStopExactly=abc|3000";
            SearchAndRunScript = "SearchAndRunScript=abc|3000|";
            SmartSearchText = "SmartSearchText=abc|30|script.xml";
            SearchFileAndClick = "SearchFileAndClick=script.txt|3000|-0 +0";
            SearchFileWaitClick = "SearchFileWaitClick=script.txt|3000|-0 +0|1000";
            SearchFileAndClickExactly = "SearchFileAndClickExactly=script.txt|3000|-0 +0";
            SearchFileAndClickStop = "SearchFileAndClickStop=script.txt|3000|-0 +0";
            SearchFileWaitClickStop = "SearchFileWaitClickStop=script.txt|3000|-0 +0|1000";
            SearchFileAndClickStopExactly = "SearchFileAndClickStopExactly=script.txt|3000|-0 +0";
            SearchFileAndRunScript = "SearchFileAndRunScript=script.txt|3000|";
            SendCommand = "SendCommand=";
            BackupGmail = "SendBackup()";
            RestoreGmail = "SendRestore()";
            FakeDevice = "FakeDevice=Phone|Maxis";
            Return = "Return()";
            BypassCapChar = "DeCapchar()";
            Read_Gmail = "ReadMail=";
            Read_Gmail_Restore = "ReadMailRestore=";
            Get_Gmail = "GetMail=";
            SendUser = "SendUser()";
            SendUserGet = "SendUserGet()";
            SendPassword = "SendPassword()";
            SendPasswordGet = "SendPasswordGet()";
            SendMailRecover = "SendRecover()";
            SendMailRecoverGet = "SendRecoverGet()";
            CheckSim = "CheckSim()";
            LIST_SCRIPT_SPEC = new List<string>()
            {
                SendUser,
                SendUserGet,
                SendPassword,
                SendPasswordGet,
                SendMailRecover,
                SendMailRecoverGet,
                BackupGmail,
                RestoreGmail,
                CheckSim,
                Return,
                BypassCapChar
            };
            ClickXY_CD = "adb shell input tap {0} {1}";
            Send_key_CD = "adb shell input keyevent {0}";
            Wait_CD = "adb shell sleep {0}";
            Open_app_CD = "adb shell am start {0}";
            Close_app_CD = "adb shell am force-stop {0}";
            Swipe_CD = "adb shell input touchscreen swipe {0} {1} {2} {3}";
            Start_apk_CD = "adb shell am start -a {0}";
            Send_text_CD = "adb shell input text {0}";
            TextFromFile_CD = "adb shell input text {0}";
            RandomFromFile_CD = "adb shell input text {0}";
        }

        public ScriptConstants()
        {
        }
    }
}

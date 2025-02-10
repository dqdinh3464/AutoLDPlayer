using Auto_LDPlayer.Enums;
using Auto_LDPlayer.Helpers.Commons;
using System;
using System.Threading;

namespace Auto_LDPlayer.Helpers
{
    public class KeyboardHelper
    {
        private static Random Random = new Random();

        public static void InputText(LDType ldType, string nameOrId, string text, int randomPercent = 5)
        {
            for (var i = 0; i < text.Length; i++)
            {
                LDPlayer.InputText(ldType, nameOrId, text[i].ToString());
                if (LDCommon.IsWin(randomPercent))
                {
                    DeleteText(ldType, nameOrId, ref i);
                }
                Thread.Sleep(Random.Next(100, 300));
            }
        }

        public static void DeleteText(LDType ldType, string nameOrId, ref int i)
        {
            var numberDeletedChars = Random.Next(0, i / 2 + 2);
            for (var j = 0; j < numberDeletedChars; j++)
            {
                LDPlayer.PressKey(ldType, nameOrId, LDKeyEvent.KEYCODE_DEL);
                i--;
                Thread.Sleep(Random.Next(100, 500));
            }
        }

        public static void DeleteText(LDType ldType, string nameOrId, int times)
        {
            for (var i = 0; i < times; i++)
            {
                LDPlayer.PressKey(ldType, nameOrId, LDKeyEvent.KEYCODE_DEL);
                Thread.Sleep(Random.Next(100, 500));
            }
        }
    }
}

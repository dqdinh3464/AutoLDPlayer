using Auto_LDPlayer.Properties;
using System;

namespace Auto_LDPlayer.Helpers.Commons
{
    public static class LDCommon
    {
        private static Random Random = new Random();

        public static bool IsWin(int percent)
        {
            if (percent == 0)
                return false;
            if (percent == 100)
                return true;

            return new Random().Next(0, 100) < percent;
        }

        public static string RandomTimeZone()
        {
            var timeZones = Resources.TimeZone.Split('\n');
            var index = Random.Next(0, timeZones.Length);

            return timeZones[index];
        }
    }
}
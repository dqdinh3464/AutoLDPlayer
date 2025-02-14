using System;

namespace Auto_LDPlayer
{
    public class LDevice
    {
        public int index;
        public string name;
        public IntPtr topHandle;
        public IntPtr bindHandle;
        public int androidState;
        public int dnplayerPID;
        public int vboxPID;

        public string Index
        {
            get => index.ToString();
            set => index = Convert.ToInt32(value);
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public IntPtr TopHandle
        {
            get => topHandle;
            set => topHandle = value;
        }

        public IntPtr BindHandle
        {
            get => bindHandle;
            set => bindHandle = value;
        }

        public int AndroidState
        {
            get => androidState;
            set => androidState = value;
        }

        public int DnplayerPid
        {
            get => dnplayerPID;
            set => dnplayerPID = value;
        }

        public int VboxPid
        {
            get => vboxPID;
            set => vboxPID = value;
        }
    }
}
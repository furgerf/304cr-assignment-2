using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace Ai2dShooter.Common
{
    public static class Constants
    {
        public static readonly Random Rnd = new Random();

        public const int ScaleFactor = 32;
        
        public const int Visibility = 3;

        public static readonly Dictionary<PlayerController, string> PlayerControllerNames = new Dictionary<PlayerController, string> {{PlayerController.Human, "Human"}, {PlayerController.AiFsm, "AI: FSM"}};

        public const int Framerate = 50;

        public const int MsPerCell = 200;

        public const int AiMoveTimeout = 1000;

        public static readonly SoundPlayer HeadshotSound;

        static Constants()
        {
            Stream str = Properties.Resources.headshot;
            HeadshotSound = new SoundPlayer(str);
        }
    }
}

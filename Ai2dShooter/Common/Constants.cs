using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;

namespace Ai2dShooter.Common
{
    public static class Constants
    {
        public static readonly Random Rnd = new Random();

        public const int ScaleFactor = 32;
        
        public const int Visibility = 3;

        public static readonly Dictionary<PlayerController, string> PlayerControllerNames = new Dictionary<PlayerController, string> {{PlayerController.Human, "Human"}, {PlayerController.AiFsm, "AI: FSM"}};

        public const int Framerate = 50;

        public static readonly object MovementLock = new object();
        //public static readonly Semaphore MovementSemaphore = new Semaphore(1, 1);

        public const int AiMoveTimeout = 100;

        public const int ShootingTimeout = 500; //(int)(1.5*AiMoveTimeout);

        public static readonly SoundPlayer HeadshotSound;
        public static readonly SoundPlayer PlaySound;
        public static readonly SoundPlayer FirstBloodSound;
        public static readonly SoundPlayer PerfectSound;

        public static readonly SoundPlayer LowHitSound;
        public static readonly SoundPlayer MediumHitSound;
        public static readonly SoundPlayer HardHitSound;

        static Constants()
        {
            HeadshotSound = new SoundPlayer(Properties.Resources.headshot);
            PlaySound = new SoundPlayer(Properties.Resources.play);
            FirstBloodSound = new SoundPlayer(Properties.Resources.firstblood);
            PerfectSound = new SoundPlayer(Properties.Resources.perfect);

            LowHitSound = new SoundPlayer(Properties.Resources.usp1);
            MediumHitSound = new SoundPlayer(Properties.Resources.elite_1);
            HardHitSound = new SoundPlayer(Properties.Resources.deagle_1);
        }
    }
}

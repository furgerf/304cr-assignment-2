using System;
using System.Collections.Generic;
using System.Media;

namespace Ai2dShooter.Common
{
    public static class Constants
    {
        public static readonly Random Rnd = new Random();

        public const int ScaleFactor = 32;
        
        public const int Visibility = 4;

        public static readonly Dictionary<PlayerController, string> PlayerControllerNames = new Dictionary<PlayerController, string> {{PlayerController.Human, "Human"}, {PlayerController.AiFsm, "AI: FSM"}};

        public const int Framerate = 200; // FPS
        public const int Framelength = 1000/Framerate; // ms

        public static readonly object MovementLock = new object();

        public static readonly object ShootingLock = new object();

        //public static readonly object HumanMovementLock = new object();

        public const int AiMoveTimeout = 50;

        public const int ShootingTimeout = 400; //(int)(1.5*AiMoveTimeout);

        public static readonly SoundPlayer HeadshotSound;
        public static readonly SoundPlayer PlaySound;
        public static readonly SoundPlayer FirstBloodSound;
        public static readonly SoundPlayer PerfectSound;
        public static readonly SoundPlayer DeathSound;
        public static readonly SoundPlayer TripleKillSound;

        public static readonly SoundPlayer MissSound;
        public static readonly SoundPlayer LowHitSound;
        public static readonly SoundPlayer MediumHitSound;
        public static readonly SoundPlayer HardHitSound;
        public static readonly SoundPlayer KnifeHitSound;
        public static readonly SoundPlayer KnifeMissSound;

        public static readonly SoundPlayer Reload1Sound;
        public static readonly SoundPlayer Reload2Sound;
        public static readonly SoundPlayer Reload3Sound;

        public static readonly SoundPlayer[] ReloadSounds;

        public const int DeadAlpha = 64;

        static Constants()
        {
            HeadshotSound = new SoundPlayer(Properties.Resources.headshot);
            PlaySound = new SoundPlayer(Properties.Resources.play);
            FirstBloodSound = new SoundPlayer(Properties.Resources.firstblood);
            PerfectSound = new SoundPlayer(Properties.Resources.perfect);
            DeathSound = new SoundPlayer(Properties.Resources.death6);
            TripleKillSound = new SoundPlayer(Properties.Resources.triplekill);

            MissSound = new SoundPlayer(Properties.Resources.bulletltor07);
            LowHitSound = new SoundPlayer(Properties.Resources.usp1);
            MediumHitSound = new SoundPlayer(Properties.Resources.elite_1);
            HardHitSound = new SoundPlayer(Properties.Resources.deagle_1);
            KnifeHitSound = new SoundPlayer(Properties.Resources.knife_hit3);
            KnifeMissSound = new SoundPlayer(Properties.Resources.knife_slash1);

            Reload1Sound = new SoundPlayer(Properties.Resources.ak47_clipout);
            Reload2Sound = new SoundPlayer(Properties.Resources.ak47_clipin);
            Reload3Sound = new SoundPlayer(Properties.Resources.ak47_boltpull);
            ReloadSounds = new[] {Reload1Sound, Reload2Sound, Reload3Sound};
        }
    }
}

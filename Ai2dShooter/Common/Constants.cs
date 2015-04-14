using System;
using System.Media;

namespace Ai2dShooter.Common
{
    /// <summary>
    /// Contains commonly used constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Object that generates random numbers.
        /// </summary>
        public static readonly Random Rnd = new Random();

        /// <summary>
        /// Scale factor for everything that is drawn.
        /// </summary>
        public const int ScaleFactor = 32;
        
        /// <summary>
        /// Number of fields of vision in each direction for all players.
        /// </summary>
        public const int Visibility = 4;

        /// <summary>
        /// Maps readable strings to the enum members.
        /// </summary>
        public static readonly string[] PlayerControllerNames = { "Human", "AI: FSM", "AI: DT"};

        /// <summary>
        /// Opacity with which to draw dead players.
        /// </summary>
        public const int DeadAlpha = 64;

        /// <summary>
        /// Graphic redraw rate.
        /// </summary>
        public const int Framerate = 200; // FPS

        /// <summary>
        /// Time between graphic redraws.
        /// </summary>
        public const int Framelength = 1000/Framerate; // ms

        /// <summary>
        /// Lock that has to be aquired for any sort of movement. Can be used for pausing.
        /// </summary>
        public static readonly object MovementLock = new object();

        /// <summary>
        /// Lock that has to be aquired when shooting. Can be used for pausing.
        /// </summary>
        public static readonly object ShootingLock = new object();

        /// <summary>
        /// Time that each AI player waits after making a decision.
        /// </summary>
        public const int AiMoveTimeout = 50;

        /// <summary>
        /// Time that passes after each fired shot.
        /// </summary>
        public const int ShootingTimeout = 400;

        /// <summary>
        /// Time that passes between each step of the reloading process.
        /// </summary>
        public const int ReloadTimeout = 1250; // AiMoveTimeout * 25;

        /// <summary>
        /// Sounds.
        /// </summary>
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

        static Constants()
        {
            // load sounds from properties
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

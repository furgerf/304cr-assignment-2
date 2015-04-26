using System;
using System.Windows.Media;

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
        public const int Visibility = 5;

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
        public static readonly MediaPlayer HeadshotSound = new MediaPlayer();
        public static readonly MediaPlayer PlaySound = new MediaPlayer();
        public static readonly MediaPlayer FirstBloodSound = new MediaPlayer();
        public static readonly MediaPlayer PerfectSound = new MediaPlayer();
        public static readonly MediaPlayer DeathSound = new MediaPlayer();
        public static readonly MediaPlayer TripleKillSound = new MediaPlayer();

        public static readonly MediaPlayer MissSound = new MediaPlayer();
        public static readonly MediaPlayer LowHitSound = new MediaPlayer();
        public static readonly MediaPlayer MediumHitSound = new MediaPlayer();
        public static readonly MediaPlayer HardHitSound = new MediaPlayer();
        public static readonly MediaPlayer KnifeHitSound = new MediaPlayer();
        public static readonly MediaPlayer KnifeMissSound = new MediaPlayer();

        public static readonly MediaPlayer Reload1Sound = new MediaPlayer();
        public static readonly MediaPlayer Reload2Sound = new MediaPlayer();
        public static readonly MediaPlayer Reload3Sound = new MediaPlayer();

        public static readonly MediaPlayer[] ReloadSounds = {Reload1Sound, Reload2Sound, Reload3Sound};

        static Constants()
        {
            // load sounds from properties
            HeadshotSound.Open(new Uri(@"../../Resources/headshot.wav", UriKind.Relative));
            HeadshotSound.MediaEnded += (o, e) =>
            {
                HeadshotSound.Stop();
                HeadshotSound.Open(new Uri(@"../../Resources/headshot.wav", UriKind.Relative));
            };
            PlaySound.Open(new Uri(@"../../Resources/play.wav", UriKind.Relative));
            PlaySound.MediaEnded += (o, e) =>
            {
                PlaySound.Stop();
                PlaySound.Open(new Uri(@"../../Resources/play.wav", UriKind.Relative));
            };
            FirstBloodSound.Open(new Uri(@"../../Resources/firstblood.wav", UriKind.Relative));
            FirstBloodSound.MediaEnded += (o, e) =>
            {
                FirstBloodSound.Stop();
                FirstBloodSound.Open(new Uri(@"../../Resources/firstblood.wav", UriKind.Relative));
            };
            PerfectSound.Open(new Uri(@"../../Resources/perfect.wav", UriKind.Relative));
            PerfectSound.MediaEnded += (o, e) =>
            {
                PerfectSound.Stop();
                PerfectSound.Open(new Uri(@"../../Resources/perfect.wav", UriKind.Relative));
            };
            DeathSound.Open(new Uri(@"../../Resources/death6.wav", UriKind.Relative));
            DeathSound.MediaEnded += (o, e) =>
            {
                DeathSound.Stop();
                DeathSound.Open(new Uri(@"../../Resources/death6.wav", UriKind.Relative));
            };
            TripleKillSound.Open(new Uri(@"../../Resources/triplekill.wav", UriKind.Relative));
            TripleKillSound.MediaEnded += (o, e) =>
            {
                TripleKillSound.Stop();
                TripleKillSound.Open(new Uri(@"../../Resources/triplekill.wav", UriKind.Relative));
            };

            MissSound.Open(new Uri(@"../../Resources/bulletltor07.wav", UriKind.Relative));
            MissSound.MediaEnded += (o, e) =>
            {
                MissSound.Stop();
                MissSound.Open(new Uri(@"../../Resources/bulletltor07.wav", UriKind.Relative));
            }; 
            LowHitSound.Open(new Uri(@"../../Resources/usp1.wav", UriKind.Relative));
            LowHitSound.MediaEnded += (o, e) =>
            {
                LowHitSound.Stop();
                LowHitSound.Open(new Uri(@"../../Resources/usp1.wav", UriKind.Relative));
            }; 
            MediumHitSound.Open(new Uri(@"../../Resources/elite-1.wav", UriKind.Relative));
            MediumHitSound.MediaEnded += (o, e) =>
            {
                MediumHitSound.Stop();
                MediumHitSound.Open(new Uri(@"../../Resources/elite-1.wav", UriKind.Relative));
            }; 
            HardHitSound.Open(new Uri(@"../../Resources/deagle-1.wav", UriKind.Relative));
            HardHitSound.MediaEnded += (o, e) =>
            {
                HardHitSound.Stop();
                HardHitSound.Open(new Uri(@"../../Resources/deagle-1.wav", UriKind.Relative));
            }; 
            KnifeHitSound.Open(new Uri(@"../../Resources/knife_hit3.wav", UriKind.Relative));
            KnifeHitSound.MediaEnded += (o, e) =>
            {
                KnifeHitSound.Stop();
                KnifeHitSound.Open(new Uri(@"../../Resources/knife_hit3.wav", UriKind.Relative));
            };
            KnifeMissSound.Open(new Uri(@"../../Resources/knife_slash1.wav", UriKind.Relative));
            KnifeMissSound.MediaEnded += (o, e) =>
            {
                KnifeMissSound.Stop();
                KnifeMissSound.Open(new Uri(@"../../Resources/knife_slash1.wav", UriKind.Relative));
            };

            Reload1Sound.Open(new Uri(@"../../Resources/ak47_clipout.wav", UriKind.Relative));
            Reload1Sound.MediaEnded += (o, e) =>
            {
                Reload1Sound.Stop();
                Reload1Sound.Open(new Uri(@"../../Resources/ak47_clipout.wav", UriKind.Relative));
            };
            Reload2Sound.Open(new Uri(@"../../Resources/ak47_clipin.wav", UriKind.Relative));
            Reload2Sound.MediaEnded += (o, e) =>
            {
                Reload2Sound.Stop();
                Reload2Sound.Open(new Uri(@"../../Resources/ak47_clipin.wav", UriKind.Relative));
            };
            Reload3Sound.Open(new Uri(@"../../Resources/ak47_boltpull.wav", UriKind.Relative));
            Reload3Sound.MediaEnded += (o, e) =>
            {
                Reload3Sound.Stop();
                Reload3Sound.Open(new Uri(@"../../Resources/ak47_boltpull.wav", UriKind.Relative));
            };
        }
    }
}

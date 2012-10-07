using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    partial class CloudberryKingdomGame
    {
        private void BenchmarkAll()
        {
            // Load art
            Start2();
            
            Start();
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Environments");
            long LoadEnviros = Stop();

            Start();
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Bob");
            long LoadBob = Stop();

            Start();
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Menu");
            long LoadMenus = Stop();

            Start();
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Coins");
            //Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Effects");
            long LoadEffects = Stop();

            Start();
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Buttons");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Characters");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "HeroItems");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "LoadScreen_Initial");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "LoadScreen_Level");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Old Art Holdover");
            Tools.TextureWad.LoadFolder(Tools.GameClass.Content, "Title");
            long LoadRest = Stop();

            Tools.Write("ArtMusic done...");

            // Load the infowad and boxes
            Start();
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            LoadInfo();
            long Info = Stop();
            Tools.Write("Infowad done...");

            Start();
            TileSets.Init();
            long Tiles = Stop();

            Start();
            Fireball.InitRenderTargets(MyGraphicsDevice, MyGraphicsDevice.PresentationParameters, 300, 200);
            ParticleEffects.Init();
            long Particle = Stop();

            Start();
            PlayerManager.Init();
            Awardments.Init();
            long Players = Stop();

            // Load saved files
            Start();
            SaveGroup.Initialize();
            long LoadSaves = Stop();

            Start();
            Prototypes.LoadObjects();
            long Protos = Stop();

            Start();
            ObjectIcon.InitIcons();
            long Icons = Stop();

            Tools.Write("Stickmen done...");

            Start();
            Tools.padState = new GamePadState[4];
            Tools.PrevpadState = new GamePadState[4];
            long GamePads = Stop();

            long Total = Stop2();

            Tools.Write("Textures done...");

            //Console.WriteLine("Pre load       {0}", PreLoad);
            Console.WriteLine("Load Bob       {0}", LoadBob);
            Console.WriteLine("Load enviros   {0}", LoadEnviros);
            Console.WriteLine("Load effects   {0}", LoadEffects);
            Console.WriteLine("Load menus     {0}", LoadMenus);
            Console.WriteLine("Load rest      {0}", LoadRest);
            Console.WriteLine("Load info      {0}", Info);
            Console.WriteLine("Load tiles     {0}", Tiles);
            Console.WriteLine("Load particles {0}", Particle);
            Console.WriteLine("Load players   {0}", Players);
            Console.WriteLine("Load saves     {0}", LoadSaves);
            Console.WriteLine("Load Protos    {0}", Protos);
            Console.WriteLine("Load Icons     {0}", Icons);
            Console.WriteLine("Load Small     {0}", GamePads);
            Console.WriteLine("-----------------------", GamePads);
            Console.WriteLine("Total          {0}", Total);
            Console.WriteLine("");
        }

        static System.Diagnostics.Stopwatch stopwatch;
        static void Start()
        {
            stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
        }
        static long Stop()
        {
            stopwatch.Stop();
            return stopwatch.ElapsedTicks;
        }

        static System.Diagnostics.Stopwatch stopwatch2;
        static void Start2()
        {
            stopwatch2 = new System.Diagnostics.Stopwatch();
            stopwatch2.Start();
        }
        static long Stop2()
        {
            stopwatch2.Stop();
            return stopwatch2.ElapsedTicks;
        }


        private static void BenchmarkLoadSize()
        {
            Console.WriteLine(Tools.GameClass.Content.RootDirectory);

            Tools.GameClass.Content.Load<Texture>("Art\\Environments\\Snow");


            long big = 0; long small = 0;

            Start();
            for (int i = 0; i < 1; i++)
            {
                Tools.GameClass.Content.Load<Texture>("Art\\Bob\\Classic\\v1\\Stand\\Bob_Stand_0001");
                Tools.GameClass.Content.Load<Texture>("Art\\Bob\\Classic\\v1\\Stand\\Bob_Stand_0002");
                Tools.GameClass.Content.Load<Texture>("Art\\Bob\\Classic\\v1\\Stand\\Bob_Stand_0003");
                Tools.GameClass.Content.Load<Texture>("Art\\Bob\\Classic\\v1\\Stand\\Bob_Stand_0004");
            }
            small = Stop();

            Start();
            for (int i = 0; i < 1; i++)
            {
                Tools.GameClass.Content.Load<Texture>("Art\\Environments\\Castle\\Pillars\\Pillar_Castle_1000");
                Tools.GameClass.Content.Load<Texture>("Art\\Environments\\Cave\\Pillars\\Pillar_Cave_1000");
                Tools.GameClass.Content.Load<Texture>("Art\\Environments\\Cloud\\Pillars\\Pillar_Cloud_1000");
                Tools.GameClass.Content.Load<Texture>("Art\\Environments\\Forest\\Pillars\\Pillar_Forest_1000");
            }
            big = Stop();

            Console.WriteLine("big took {0}", big);
            Console.WriteLine("small took {0}", small);
            Console.WriteLine("");
        }
    }
}
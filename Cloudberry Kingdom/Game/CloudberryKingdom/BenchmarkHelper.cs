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
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
    partial class CloudberryKingdomGame
    {
        private void BenchmarkAll()
        {
			/*
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
            Tools.GamepadState = new GamePadState[4];
            Tools.PrevGamepadState = new GamePadState[4];
            long GamePads = Stop();

            long Total = Stop2();

            Tools.Write("Textures done...");

            //Tools.Write("Pre load       {0}", PreLoad);
            Tools.Write("Load Bob       {0}", LoadBob);
            Tools.Write("Load enviros   {0}", LoadEnviros);
            Tools.Write("Load effects   {0}", LoadEffects);
            Tools.Write("Load menus     {0}", LoadMenus);
            Tools.Write("Load rest      {0}", LoadRest);
            Tools.Write("Load tiles     {0}", Tiles);
            Tools.Write("Load particles {0}", Particle);
            Tools.Write("Load players   {0}", Players);
            Tools.Write("Load saves     {0}", LoadSaves);
            Tools.Write("Load Protos    {0}", Protos);
            Tools.Write("Load Icons     {0}", Icons);
            Tools.Write("Load Small     {0}", GamePads);
            Tools.Write("-----------------------", GamePads);
            Tools.Write("Total          {0}", Total);
            Tools.Write("");
			 */
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
            Tools.Write(Tools.GameClass.Content.RootDirectory);

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

            Tools.Write("big took {0}", big);
            Tools.Write("small took {0}", small);
            Tools.Write("");
        }
    }
}
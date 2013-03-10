using System;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;
#if WINDOWS
using System.Windows.Forms;
#endif

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Levels;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Awards;

#if XBOX
#else
using CloudberryKingdom.Viewer;
#endif

#if WINDOWS
using Forms = System.Windows.Forms;
#endif

namespace CloudberryKingdom
{
    public partial class XnaGameClass : Game
    {
        CloudberryKingdomGame MyGame;
        
        public XnaGameClass()
        {
            Tools.GameClass = this;

#if XBOX
            Components.Add(new GamerServicesComponent(this));
#endif
            Content.RootDirectory = "Content";

            
            // Volume control
            Tools.SoundVolume = new WrappedFloat();
            Tools.SoundVolume.MinVal = 0;
            Tools.SoundVolume.MaxVal = 1;
            Tools.SoundVolume.Val = .7f;

            Tools.MusicVolume = new WrappedFloat();
            Tools.MusicVolume.MinVal = 0;
            Tools.MusicVolume.MaxVal = 1;
            Tools.MusicVolume.Val = 1;
            Tools.MusicVolume.SetCallback = () => Tools.UpdateVolume();

#if DEBUG || INCLUDE_EDITOR
            Tools.SoundVolume.Val = 0;
            Tools.MusicVolume.Val = 0;
#endif

            MyGame = new CloudberryKingdomGame();
            MyGame.InitialResolution();
        }

        protected override void Initialize()
        {
            MyGame.Initialize();

            Window.Title = "Cloudberry Kingdom ";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            MyGame.LoadContent();

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            this.IsFixedTimeStep = Tools.FixedTimeStep;

            MyGame.RunningSlowly = gameTime.IsRunningSlowly;

            MyGame.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
#if DEBUG
			MyGame.Draw(gameTime);

			//try
			//{
			//    MyGame.Draw(gameTime);
			//}
			//catch (Exception e)
			//{
			//    Tools.Write(e.Message);
			//}
#else
			try
			{
				MyGame.Draw(gameTime);
			}
			catch (Exception e)
			{
				Tools.Write(e.Message);
			}
#endif

            base.Draw(gameTime);
        }

#if WINDOWS
        public void SetBorder(bool Show)
        {
            IntPtr hWnd = Tools.GameClass.Window.Handle;
            var control = Control.FromHandle(hWnd);
            var form = control.FindForm();
            form.FormBorderStyle = Show ? FormBorderStyle.FixedSingle : FormBorderStyle.None;
        }
#endif
    }
}

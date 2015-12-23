using System;

#if WINDOWS && !MONO && !SDL2
using System.Windows.Forms;
#endif

using Microsoft.Xna.Framework;

#if XBOX
using Microsoft.Xna.Framework.GamerServices;
#endif

#if XBOX
#elif !MONO && !SDL2
#endif

namespace CloudberryKingdom
{
    public partial class XnaGameClass : Game
    {
        CloudberryKingdomGame MyGame;

        public XnaGameClass()
        {
            Tools.GameClass = this;

            Tools.Write("XnaGameClass Constructor");
			
#if XBOX
            Components.Add(new GamerServicesComponent(this));
            Tools.Write("GamerService added");
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
            Tools.Write("Volume created");

#if DEBUG || INCLUDE_EDITOR
            Tools.SoundVolume.Val = 0;
            Tools.MusicVolume.Val = 0;
#endif

            MyGame = new CloudberryKingdomGame();
            Tools.Write("MyGame created");

            MyGame.InitialResolution();
            Tools.Write("InitialResolutions created.");
        }

        protected override void Initialize()
        {
            Tools.Write("XnaGameClass Initialize");
            Tools.Write("MyGame is null? " + (MyGame == null));
            MyGame.Initialize();

            Window.Title = "Cloudberry Kingdom ";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Tools.Write("XnaGameClass LoadContent");
            Tools.Write("MyGame is null? " + (MyGame == null));
            MyGame.LoadContent();

            Tools.Write("XnaGameClass LoadContent Done");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
			base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
			try
			{
				this.IsFixedTimeStep = Tools.FixedTimeStep;

				MyGame.RunningSlowly = gameTime.IsRunningSlowly;

				MyGame.Update();

				base.Update(gameTime);
			}
			catch (Exception e)
			{
				Tools.Log("Exception on XnaGameClass.Update\n" + Tools.ExceptionStr(e));
			}
        }

        protected override void Draw(GameTime gameTime)
        {
#if DEBUG
			MyGame.Draw(gameTime);
#else
			try
			{
				MyGame.Draw(gameTime);
			}
			catch (Exception e)
			{
				Tools.Log(string.Format("Stack trace\n\n{0}\n\n\nInner Exception\n\n{1}\n\n\nMessage\n\n{2}", e.StackTrace, e.InnerException, e.Message));

                Tools.Write(e.StackTrace);
                Tools.Write(e.InnerException);
				Tools.Write(e.Message);
			}
#endif

            base.Draw(gameTime);
        }

#if WINDOWS
		public static bool WindowModeSet = false;

		public void FakeFull()
		{
			if (WindowModeSet) return;
			WindowModeSet = true;

			IntPtr hWnd = Tools.GameClass.Window.Handle;
			var control = Control.FromHandle(hWnd);
			var form = control.FindForm();

			switch (Tools.Mode)
			{
				case WindowMode.Borderless:
					control.Show();
					control.Location = new System.Drawing.Point(0, 0);

					form.TopMost = true;

					form.FormBorderStyle = FormBorderStyle.None;
					form.WindowState = FormWindowState.Maximized;
					break;

				case WindowMode.Windowed:
					form.TopMost = false;

					form.FormBorderStyle = FormBorderStyle.FixedSingle;
					form.WindowState = FormWindowState.Normal;

					break;
			}
		}

		public void FakeTab()
		{
			if (!WindowModeSet) return;
			WindowModeSet = false;

			IntPtr hWnd = Tools.GameClass.Window.Handle;
			var control = Control.FromHandle(hWnd);
			var form = control.FindForm();

			switch (Tools.Mode)
			{
				case WindowMode.Borderless:
					form.TopMost = false;

					form.FormBorderStyle = FormBorderStyle.None;
					form.WindowState = FormWindowState.Minimized;
					break;

				case WindowMode.Windowed:
					break;
			}
		}

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

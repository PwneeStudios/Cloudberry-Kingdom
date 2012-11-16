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
#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
using Microsoft.Xna.Framework.GamerServices;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;








#if XBOX
#else

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
            MyGame = new CloudberryKingdomGame();

#if PC_VERSION
#elif XBOX || XBOX_SIGNIN
            Components.Add(new GamerServicesComponent(this));
#endif
            Content.RootDirectory = "Content";
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
            MyGame.Draw(gameTime);

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

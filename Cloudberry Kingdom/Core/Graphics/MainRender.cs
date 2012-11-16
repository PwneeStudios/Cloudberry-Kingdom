using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CloudberryKingdom
{
    public class MainRender
    {
        GraphicsDevice MyGraphicsDevice;
        public float SpriteScaling = 1f;

        public Viewport MainViewport;

        public bool UsingSpriteBatch = false;
        public SpriteBatch MySpriteBatch;


        public MainRender(GraphicsDevice Device)
        {
            MyGraphicsDevice = Device;
        }

        /// <summary>
        /// Sets the standard render states.
        /// </summary>
        public void SetStandardRenderStates()
        {
            Tools.QDrawer.SetInitialState();

            MyGraphicsDevice.RasterizerState = RasterizerState.CullNone;
            MyGraphicsDevice.BlendState = BlendState.AlphaBlend;
            MyGraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            ResetViewport();
        }

        public void ResetViewport()
        {
            Tools.TheGame.MyGraphicsDevice.Viewport = Tools.Render.MainViewport;
        }

        /// <summary>
        /// If the aspect ratio of the game (1280:720) doesn't match the window, use a letterbox viewport.
        /// </summary>
        public void MakeInnerViewport()
        {
            float targetAspectRatio = 1280f / 720f;
            // figure out the largest area that fits in this resolution at the desired aspect ratio
            int width = MyGraphicsDevice.PresentationParameters.BackBufferWidth;
            SpriteScaling = width / 1280f;
            int height = (int)(width / targetAspectRatio + .5f);
            if (height > MyGraphicsDevice.PresentationParameters.BackBufferHeight)
            {
                height = MyGraphicsDevice.PresentationParameters.BackBufferHeight;
                width = (int)(height * targetAspectRatio + .5f);
            }

            // set up the new viewport centered in the backbuffer
            MainViewport = MyGraphicsDevice.Viewport = new Viewport
            {
                X = MyGraphicsDevice.PresentationParameters.BackBufferWidth / 2 - width / 2,
                Y = MyGraphicsDevice.PresentationParameters.BackBufferHeight / 2 - height / 2,
                Width = width,
                Height = height,
                MinDepth = 0,
                MaxDepth = 1
            };
        }

        /// <summary>
        /// Ends the SpriteBatch, if in use, and resets standard render states.
        /// </summary>
        public void EndSpriteBatch()
        {
            if (UsingSpriteBatch)
            {
                UsingSpriteBatch = false;

                MySpriteBatch.End();

                Tools.Render.SetStandardRenderStates();
            }
        }
    }
}
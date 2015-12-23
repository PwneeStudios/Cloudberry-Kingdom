using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CloudberryKingdom
{
    public struct ResolutionGroup
    {
        public static DisplayMode LastSetMode;

        public static IntVector2 SafeResolution(int width, int height)
        {
            bool MatchingFound = false;
            int SuitableY = -1;

            // Make sure this is a supported resolution
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Width == width)
                {
                    if (mode.Height == height)
                    {
                        MatchingFound = true;
                        break;
                    }
                    else
                        SuitableY = mode.Height;
                }
            }

            if (!MatchingFound)
            {
                if (SuitableY > 0)
                    height = SuitableY;
                else
                {
                    width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                }
            }

            return new IntVector2(width, height);
        }

        public static void Use(DisplayMode mode)
        {
            LastSetMode = mode;
            Use(mode.Width, mode.Height);
        }

        public static void Use(int width, int height) { Use(width, height, true); }
        public static void Use(int width, int height, bool AllowModifications)
        {
#if WINDOWS
            var save = Tools.MousePos;
#endif
            if (AllowModifications)
                if (Tools.TheGame.MyGraphicsDeviceManager.IsFullScreen)
                {
                    var safe = SafeResolution(width, height);
                    width = safe.X;
                    height = safe.Y;
                }
                else
                    // Trim excess
                    height = (int)((720f / 1280f) * width);

            Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferWidth = width;
            Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferHeight = height;
            Tools.TheGame.MyGraphicsDeviceManager.ApplyChanges();
            Tools.Render.MakeInnerViewport();
           
#if WINDOWS 
            Tools.MousePos = save;
#endif
        }

        public void Use()
        {
            int width = Backbuffer.X, height = Backbuffer.Y;
            if (!Tools.TheGame.MyGraphicsDeviceManager.IsFullScreen)
            {
                height = (int)((720f / 1280f) * width);
            }

            Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferWidth = width;
            Tools.TheGame.MyGraphicsDeviceManager.PreferredBackBufferHeight = height;
            Tools.TheGame.MyGraphicsDeviceManager.ApplyChanges();
            Tools.Render.MakeInnerViewport();
        }

        public IntVector2 Backbuffer, Bob;

        public Vector2 TextOrigin;
        public float LineHeightMod;

        public override string ToString()
        {
            return Backbuffer.X + " x " + Backbuffer.Y;
        }

        public void CopyTo(ref ResolutionGroup dest)
        {
            dest.Backbuffer = Backbuffer;
            dest.Bob = Bob;

            dest.TextOrigin = TextOrigin;
            dest.LineHeightMod = LineHeightMod;
        }

        public void CopyTo(ref ResolutionGroup dest, Vector2 scale)
        {
            dest.Backbuffer = Backbuffer * scale;
            dest.Bob = Bob * scale;

            dest.TextOrigin = TextOrigin - scale;
            dest.LineHeightMod = LineHeightMod * scale.Y;
        }
    }
}
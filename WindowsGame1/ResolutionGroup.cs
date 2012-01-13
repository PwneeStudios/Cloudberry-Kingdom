using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CloudberryKingdom;

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
            var save = Tools.MousePos;

            if (AllowModifications)
                if (Tools.TheGame.graphics.IsFullScreen)
                {
                    var safe = SafeResolution(width, height);
                    width = safe.X;
                    height = safe.Y;
                }
                else
                    // Trim excess
                    height = (int)((720f / 1280f) * width);

            Tools.TheGame.graphics.PreferredBackBufferWidth = width;
            Tools.TheGame.graphics.PreferredBackBufferHeight = height;
            Tools.TheGame.graphics.ApplyChanges();
            Tools.TheGame.MakeInnerViewport();
            
            Tools.MousePos = save;
        }

        public void Use()
        {
            int width = Backbuffer.X, height = Backbuffer.Y;
            if (!Tools.TheGame.graphics.IsFullScreen)
            {
                height = (int)((720f / 1280f) * width);
            }

            Tools.TheGame.graphics.PreferredBackBufferWidth = width;
            Tools.TheGame.graphics.PreferredBackBufferHeight = height;
            Tools.TheGame.graphics.ApplyChanges();
            Tools.TheGame.MakeInnerViewport();
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
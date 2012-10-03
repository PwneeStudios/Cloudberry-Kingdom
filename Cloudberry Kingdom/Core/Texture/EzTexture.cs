﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CloudberryKingdom;

namespace CoreEngine
{
    public class EzTexture
    {
        Texture2D _Tex;
        public Texture2D Tex 
        {
            get { return _Tex; }
            set { _Tex = value; }
        }

        public float AspectRatio
        {
            get
            {
                return (float)_Tex.Width / (float)_Tex.Height;
            }
        }

        public string Path, Name;

        /// <summary>
        /// If true this texture is a sub-image from a packed collection.
        /// </summary>
        public bool FromPacked = false;

        public EzTexture Packed;

        /// <summary>
        /// If true this texture was loaded dynamically after the game loaded, not from a packed XNA file.
        /// </summary>
        public bool Dynamic = false;

        /// <summary>
        /// If true this texture is created via code, not from an asset file.
        /// </summary>
        public bool FromCode = false;

        /// <summary>
        /// Texture coordinates in the atlas.
        /// </summary>
        public Vector2 BL = Vector2.Zero, TR = Vector2.One;

#if EDITOR
        public static Game game;
        public bool Load()
        {
            if (Tex == null && Path != null)
                Tex = game.Content.Load<Texture2D>(Path);

            return Tex != null;
        }
#else
        public bool Load()
        {
            if (_Tex == null && Path != null)
                _Tex = Tools.GameClass.Content.Load<Texture2D>(Path);

            return _Tex != null;
        }
#endif

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator EzTexture(string name)
        {
            return Tools.Texture(name);
        }
    }
}
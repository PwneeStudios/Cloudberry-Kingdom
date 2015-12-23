using Microsoft.Xna.Framework;

using CoreEngine;

namespace CloudberryKingdom
{
    public class LineSpriteInfo
    {
        public bool Wrap = false;

        public TextureOrAnim Sprite;
        public float Width, RepeatWidth;
        public int Dir;
        public Vector4 Tint;
        public float BlendAddRatio;

        public bool DrawEndPoints = false;
        public TextureOrAnim End1, End2;

        public LineSpriteInfo(TextureOrAnim End1, TextureOrAnim Body, TextureOrAnim End2, float Width)
        {
            DrawEndPoints = true;
            this.End1 = End1;
            this.End2 = End2;
            this.Sprite = Body;

            this.Width = Width;
            this.RepeatWidth = Width * (float)Body.MyTexture.Height / (float)Body.MyTexture.Width;

            this.Dir = 0;
            this.Tint = Vector4.One;
            this.BlendAddRatio = 0;
        }

        public LineSpriteInfo(TextureOrAnim Sprite, float Width, float RepeatWidth)
        {
            this.Sprite = Sprite;
            this.Width = Width;
            this.RepeatWidth = RepeatWidth;
            this.Dir = 0;
            this.Tint = Vector4.One;
            this.BlendAddRatio = 0;
        }

        public LineSpriteInfo(TextureOrAnim Sprite, float Width, float RepeatWidth, int Dir, Vector4 Tint)
        {
            this.Sprite = Sprite;
            this.Width = Width;
            this.RepeatWidth = RepeatWidth;
            this.Dir = Dir;
            this.Tint = Tint;
            this.BlendAddRatio = 0;
        }

        public LineSpriteInfo(TextureOrAnim Sprite, float Width, float RepeatWidth, int Dir, Vector4 Tint, float BlendAddRatio)
        {
            this.Sprite = Sprite;
            this.Width = Width;
            this.RepeatWidth = RepeatWidth;
            this.Dir = Dir;
            this.Tint = Tint;
            this.BlendAddRatio = BlendAddRatio;
        }
    }

    /// <summary>
    /// Struct for holding information about a sprite.
    /// </summary>
    public class SpriteInfo
    {
        public TextureOrAnim Sprite;
        public Vector2 Size, Offset;
        public Color Tint;
        public bool RelativeOffset;
        public float Degrees = 0;
        
        public Matrix ColorMatrix = Matrix.Identity;

        public SpriteInfo(TextureOrAnim Sprite)
        {
            this.Sprite = Sprite;
            this.Size = Vector2.One;
            this.Offset = Vector2.Zero;
            this.Tint = Color.White;
            this.RelativeOffset = false;
        }

        public SpriteInfo(TextureOrAnim Sprite, Vector2 Size)
        {
            this.Sprite = Sprite;
            this.Size = Size;
            this.Offset = Vector2.Zero;
            this.Tint = Color.White;
            this.RelativeOffset = false;
        }

        public SpriteInfo(TextureOrAnim Sprite, Vector2 Size, Vector2 Offset, Color Tint)
        {
            this.Sprite = Sprite;
            this.Size = Size;
            this.Offset = Offset;
            this.Tint = Tint;
            this.RelativeOffset = false;
        }

        public SpriteInfo(TextureOrAnim Sprite, Vector2 Size, Vector2 Offset, Color Tint, bool RelativeOffset)
        {
            this.Sprite = Sprite;
            this.Size = Size;
            this.Offset = Offset;
            this.Tint = Tint;
            this.RelativeOffset = RelativeOffset;
        }
    }
}
using System;
using System.IO;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.Coins
{
    public class Coin : IObject
    {
        public void TextDraw() { }

        static EzSound MySound;

        static Particle DieTemplate;
        static bool TemplateInitialized;

        public bool Touched;

        public AABox Box;
        public SimpleQuad MyQuad;
        public BasePoint Base;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public int MyValue, MyScoreValue;

        public void Release()
        {
            Core.Release();
        }

        public void MakeNew()
        {
            Core.Init();

            MyValue = 1;
            MyScoreValue = 50;

            Core.MyType = ObjectType.Coin;
            Core.DrawLayer = 5;
            //Core.EditHoldable = true;

            Init();
        }

        public Coin(bool BoxesOnly)
        {
            CoreData = new ObjectData();

            Box = new AABox();

            if (!BoxesOnly)
                MyQuad = new SimpleQuad();

            MakeNew();

            Core.BoxesOnly = BoxesOnly;
        }

        public void Die()
        {
            if (Core.MyLevel.PlayMode != 0) return;

            Core.Active = false;
            int i;

            MySound.Play(.65f, .1f, 0);

            for (int j = 0; j < 3; j++)
            {
                i = Core.MyLevel.MainEmitter.GetNextSlot();
                Core.MyLevel.MainEmitter.Particles[i] = DieTemplate;
                Core.MyLevel.MainEmitter.Particles[i].Data.Position = Core.Data.Position + Tools.RndDir(35);
                Core.MyLevel.MainEmitter.Particles[i].MyQuad.MyTexture = Tools.TextureWad.FindByName("Pop");
            }

            // Text float
            int val = CalcScoreValue();
            TextFloat text = new TextFloat("+" + val.ToString(), Core.Data.Position + new Vector2(21, 22.5f));
            text.MyText.MyFloatColor = Core.MyLevel.MyTileSetInfo.CoinScoreColor.ToVector4();
            text.Core.DrawLayer = 8;
            Core.MyLevel.MyGame.AddGameObject(text);
        }

        GameData MyGame { get { return Core.MyLevel.MyGame; } }

        public int CalcScoreValue()
        {
            return (int)(MyScoreValue * MyGame.CoinScoreMultiplier * MyGame.ScoreMultiplier);
        }

        public enum CoinType { Blue, Red };
        public CoinType MyType;

        static EzTexture Blue, Red;
        public void Init()
        {
            if (!TemplateInitialized)
            {
                TemplateInitialized = true;

                Blue = Tools.TextureWad.FindByName("CoinBlue");
                Red = Tools.TextureWad.FindByName("CoinRed");

                MySound = InfoWad.GetSound("GetCoin_Sound");

                DieTemplate = new Particle();
                DieTemplate.MyQuad.Init();
                DieTemplate.MyQuad.MyEffect = Tools.BasicEffect;
                DieTemplate.MyQuad.MyTexture = Tools.TextureWad.FindByName("Coin");

                DieTemplate.SetSize(45);
                DieTemplate.SizeSpeed = new Vector2(10, 10);
                DieTemplate.AngleSpeed = .013f;
                DieTemplate.Life = 20;
                DieTemplate.MyColor = new Vector4(1f, 1f, 1f, .75f);
                DieTemplate.ColorVel = new Vector4(0, 0, 0, -.065f);
            }

            Box.Initialize(Core.Data.Position, new Vector2(40, 40));

            if (!Core.BoxesOnly)
            {
                MyQuad.MyEffect = Tools.BasicEffect;


                SetType(CoinType.Blue);
                MyQuad.SetColor(InfoWad.GetColor("CoinColor"));

                MyQuad.Init();
            }

            Vector2 Size = InfoWad.GetVec("CoinSize");
            Base.e1 = new Vector2(Size.X, 0);
            Base.e2 = new Vector2(0, Size.Y);

            Update();
        }

        public void SetType(CoinType type)
        {
            MyType = type;
            switch (MyType)
            {
                case CoinType.Blue:
                    AlwaysActive = false;
                    MyQuad.MyTexture = Blue; break;
                case CoinType.Red:
                    AlwaysActive = true;
                    MyQuad.MyTexture = Red; break;
            }
        }

        bool AlwaysActive = false;
        public int Offset = 0, Period = 95;
        Vector2 Radii = new Vector2(0, 120);
        public Vector2 GetPos()
        {
            double t = 2 * Math.PI * (Core.GetPhsxStep() + Offset) / (float)Period;
            //return Tools.AngleToDir(t) * (float)Math.Cos(2*t) * Radii + Core.StartData.Position;
            //return Tools.AngleToDir(t) * Radii + Core.StartData.Position;
            return new Vector2((float)Math.Cos(t)) * Radii + Core.StartData.Position;
        }

        public void PhsxStep()
        {
            if (!Core.Active) return;

            if (!AlwaysActive)
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 200))
            {
                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            if (MyType == CoinType.Red)
                Core.Data.Position = GetPos();
            else
                Core.Data.Position = Core.StartData.Position;
            Core.Data.Position += new Vector2(0, 4.65f * (float)Math.Sin(.075f * (Core.MyLevel.CurPhsxStep) + Core.AddedTimeStamp));

            Box.SetTarget(Core.Data.Position, Box.Current.Size);

            if (Core.WakeUpRequirements)
            {
                Box.SwapToCurrent();
                Core.WakeUpRequirements = false;
            }
        }

        public void PhsxStep2()
        {
            if (!Core.Active) return;
            if (Core.SkippedPhsx) return;

            Box.SwapToCurrent();

            Update();
        }


        public void Update()
        {
            Base.Origin = Core.Data.Position;
            if (!Core.BoxesOnly)
                MyQuad.Update(ref Base);
        }

        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            Core.Data.Position = Core.StartData.Position;

            Update();
        }

        public void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;

            Box.Move(shift);
        }

        public void Interact(Bob bob)
        {
            if (!Core.Active) return;
            if (Core.MyLevel.SuppressCheckpoints && !Core.MyLevel.ShowCoinsInReplay) return;

            //ColType Col = Phsx.CollisionTest(bob.Box2, Box);
            //if (Col != ColType.NoCol)
            bool Col = Phsx.BoxBoxOverlap(bob.Box2, Box);
            if (Col)
            {
                Die();

                if (Core.MyLevel.PlayMode == 0 && !Core.MyLevel.Watching)
                {
                    Core.InteractingBob = bob;

                    GameData game = Core.MyLevel.MyGame;

                    // Add the value to the player's stats
                    int CurScoreValue = CalcScoreValue();
                    if (game.TakeOnce || game.AlwaysGiveCoinScore)
                    {
                        bob.MyStats.Score += CurScoreValue;
                        bob.MyStats.Coins += MyValue;
                    }
                    else
                    {
                        bob.MyTempStats.Score += CurScoreValue;
                        bob.MyTempStats.Coins += MyValue;
                    }

                    // Fire the games OnCoinGrab event
                    game.CoinGrabEvent(this);

                    // Remove the coin permanently if it can only be grabbed once
                    if (game.TakeOnce)
                        Core.Recycle.CollectObject(this);
                }
            }
        }


        static Vector2 DrawGrace = new Vector2(50, 50);
        public void Draw()
        {
            if (!Core.Active) return;
            if (Core.MyLevel.SuppressCheckpoints && !Core.MyLevel.ShowCoinsInReplay) return;

            if (Box.Current.BL.X > Core.MyLevel.MainCamera.TR.X + DrawGrace.X || Box.Current.BL.Y > Core.MyLevel.MainCamera.TR.Y + DrawGrace.Y)
                return;
            if (Box.Current.TR.X < Core.MyLevel.MainCamera.BL.X - DrawGrace.X || Box.Current.TR.Y < Core.MyLevel.MainCamera.BL.Y - DrawGrace.Y)
                return;

            if (Tools.DrawGraphics && !Core.BoxesOnly)
                Tools.QDrawer.DrawQuad(MyQuad);
            if (Tools.DrawBoxes)
                Box.Draw(Tools.QDrawer, Color.Bisque, 10);
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            Coin CoinA = A as Coin;

            Box.SetTarget(CoinA.Box.Target.Center, CoinA.Box.Target.Size);
            Box.SetCurrent(CoinA.Box.Current.Center, CoinA.Box.Current.Size);

            MyValue = CoinA.MyValue;
            MyScoreValue = CoinA.MyScoreValue;

            SetType(CoinA.MyType);

            Period = CoinA.Period;
            Offset = CoinA.Offset;
            Radii = CoinA.Radii;
        }

        public void Write(BinaryWriter writer)
        {
            Core.Write(writer);
        }
        public void Read(BinaryReader reader) { Core.Read(reader); }
//StubStubStubStart
public void OnUsed() { }
public void OnMarkedForDeletion() { }
public void OnAttachedToBlock() { }
public bool PermissionToUse() { return true; }
public Vector2 Pos { get { return Core.Data.Position; } set { Core.Data.Position = value; } }
public GameData Game { get { return Core.MyLevel.MyGame; } }
public void Smash(Bob bob) { }
//StubStubStubEnd6
    }
}
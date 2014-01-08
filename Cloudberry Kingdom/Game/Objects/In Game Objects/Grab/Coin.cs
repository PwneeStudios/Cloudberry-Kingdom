using System;
using System.IO;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Particles;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom.InGameObjects
{
    public class Coin : ObjectBase
    {
        public class CoinTileInfo : TileInfoBase
        {
            public SpriteInfo Sprite = new SpriteInfo("CoinShimmer", new Vector2(105, -1));

            public Vector2 BoxSize = new Vector2(52.5f, 65);
            public Color Color = new Color(255, 255, 255, 255);
            public bool ShowEffect = true, ShowText = true, ShowCoin = true;

            public EzSound MySound = Tools.NewSound("Coin", .75f);
        }

        public bool Touched;

        public AABox Box;
        public QuadClass MyQuad;

        public int MyValue, MyScoreValue;

        public override void MakeNew()
        {
            CoreData.Init();

            MyValue = 1;
            MyScoreValue = 50;

            CoreData.MyType = ObjectType.Coin;
            CoreData.DrawLayer = 5;

            Box.Initialize(Vector2.Zero, Vector2.One);

            if (MyQuad != null)
            {
                MyQuad.Quad.t = 0;
                MyQuad.Quad.Playing = false;
                MyQuad.Quad.Loop = false;

            }
        }

        public override void Release()
        {
            base.Release();

            if (MyQuad != null) MyQuad.Release(); MyQuad = null;
        }

        public Coin(bool BoxesOnly)
        {
            Box = new AABox();

            if (!BoxesOnly)
                MyQuad = new QuadClass();

            MakeNew();

            CoreData.BoxesOnly = BoxesOnly;
        }

        public static Vector2 PosOfLastCoinGrabbed;
        public void Die()
        {
            if (CoreData.MyLevel.PlayMode != 0) return;

            PosOfLastCoinGrabbed = CoreData.Data.Position;

            CoreData.Active = false;

            Info.Coins.MySound.Play(.4f, .1f, 0);

            // Effect
            if (Info.Coins.ShowEffect)
            {
                ParticleEffects.CoinDie_New(MyLevel, CoreData.Data.Position);
                ParticleEffects.CoinDie_Old(MyLevel, CoreData.Data.Position);
            }

            // Text float
            if (Info.Coins.ShowText && CoreData.MyLevel.MyGame.MyBankType != GameData.BankType.Campaign)
            {
				MyGame.CalculateCoinScoreMultiplier();
                int val = CalcScoreValue();
                TextFloat text = new TextFloat("+" + val.ToString(), CoreData.Data.Position + new Vector2(21, 22.5f));
                text.CoreData.DrawLayer = 8;
                CoreData.MyLevel.MyGame.AddGameObject(text);
            }
			else if (CoreData.MyLevel.MyGame.MyBankType == GameData.BankType.Campaign)
			{
				ParticleEffects.CoinDie_Campaign(MyGame.MyLevel, Coin.PosOfLastCoinGrabbed);
			}
        }

        GameData MyGame { get { return CoreData.MyLevel.MyGame; } }

        public int CalcScoreValue()
        {
            return (int)(MyScoreValue * MyGame.CoinScoreMultiplier * MyGame.ScoreMultiplier);
        }

        public enum CoinType { Blue, Red };
        public CoinType MyType;

        public void SetType(CoinType type)
        {
            MyType = type;

            Box.Initialize(CoreData.Data.Position, Info.Coins.BoxSize);

            if (!CoreData.BoxesOnly)
            switch (MyType)
            {
                case CoinType.Blue:
                    AlwaysActive = false;
                    MyQuad.Set(Info  .Coins.Sprite);
                    MyQuad.Quad.Playing = false;
                    MyQuad.Quad.Loop = false;
                    break;
                default: break;
                //case CoinType.Red:
                //    AlwaysActive = true;
                //    MyQuad.MyTexture = Info.Coins.Red; break;
            }
        }

        bool AlwaysActive = false;
        public int Offset = 0, Period = 95;
        Vector2 Radii = new Vector2(0, 120);
        public Vector2 GetPos()
        {
            double t = 2 * Math.PI * (CoreData.GetPhsxStep() + Offset) / (float)Period;
            return new Vector2((float)Math.Cos(t)) * Radii + CoreData.StartData.Position;
        }

        public override void PhsxStep()
        {
            if (!CoreData.Active) return;

            if (!AlwaysActive)
            if (!CoreData.MyLevel.MainCamera.OnScreen(CoreData.Data.Position, 200))
            {
                if (!MyLevel.BoxesOnly)
                {
                    MyQuad.Quad.Playing = false;
                    MyQuad.Quad.Loop = false;
                    MyQuad.Quad.t = 0;
                }

                CoreData.SkippedPhsx = true;
                CoreData.WakeUpRequirements = true;
                return;
            }
            CoreData.SkippedPhsx = false;

            // Shimmer
            if (!MyLevel.BoxesOnly)
            {
                MyQuad.Quad.Playing = true;
                MyQuad.Quad.Loop = false;
                MyQuad.Quad.t = MyLevel.CurPhsxStep % 110;
            }

            if (MyType == CoinType.Red)
                CoreData.Data.Position = GetPos();
            else
                CoreData.Data.Position = CoreData.StartData.Position;
            CoreData.Data.Position += new Vector2(0, 4.65f * (float)Math.Sin(.075f * (CoreData.MyLevel.CurPhsxStep) + CoreData.AddedTimeStamp));

            Box.SetTarget(CoreData.Data.Position, Box.Current.Size);

            if (CoreData.WakeUpRequirements)
            {
                Box.SwapToCurrent();
                CoreData.WakeUpRequirements = false;
            }
        }

        public override void PhsxStep2()
        {
            if (!CoreData.Active) return;
            if (CoreData.SkippedPhsx) return;

            Box.SwapToCurrent();
        }

        public override void Reset(bool BoxesOnly)
        {
            CoreData.Active = true;

            CoreData.Data.Position = CoreData.StartData.Position;
        }

        public override void Move(Vector2 shift)
        {
            CoreData.StartData.Position += shift;
            CoreData.Data.Position += shift;

            Box.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            if (!CoreData.Active) return;
            if (CoreData.MyLevel.SuppressCheckpoints && !CoreData.MyLevel.ShowCoinsInReplay) return;

            bool Col = Phsx.BoxBoxOverlap(bob.Box2, Box);
            if (Col)
            {
                Die();

                if (CoreData.MyLevel.PlayMode == 0 && !CoreData.MyLevel.Watching)
                {
                    CoreData.InteractingBob = bob;

                    GameData game = CoreData.MyLevel.MyGame;

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
                        CoreData.Recycle.CollectObject(this);
                }
            }
        }

        static Vector2 DrawGrace = new Vector2(50, 50);
        public override void Draw()
        {
            if (!CoreData.Active) return;
            if (CoreData.MyLevel.SuppressCheckpoints && !CoreData.MyLevel.ShowCoinsInReplay) return;

            if (Box.Current.BL.X > CoreData.MyLevel.MainCamera.TR.X + DrawGrace.X || Box.Current.BL.Y > CoreData.MyLevel.MainCamera.TR.Y + DrawGrace.Y)
                return;
            if (Box.Current.TR.X < CoreData.MyLevel.MainCamera.BL.X - DrawGrace.X || Box.Current.TR.Y < CoreData.MyLevel.MainCamera.BL.Y - DrawGrace.Y)
                return;

            if (Tools.DrawGraphics && Info.Coins.ShowCoin && !CoreData.BoxesOnly)
            {
                MyQuad.Pos = CoreData.Data.Position;
                MyQuad.Draw();
            }

			if (Tools.DrawBoxes)
			{
				Tools.QDrawer.DrawCircle(CoreData.Data.Position, 38, new Color(255, 134, 26, 235));

				//Box.DrawFilled(Tools.QDrawer, new Color(255, 134, 26, 220));
				//Box.Draw(Tools.QDrawer, Color.Bisque, 10);
			}
        }

        public override void Clone(ObjectBase A)
        {
            CoreData.Clone(A.CoreData);

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
    }
}
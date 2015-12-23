using System;
using Microsoft.Xna.Framework;

using CoreEngine;
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

            public CoreSound MySound = Tools.NewSound("Coin", .75f);
        }

        public bool Touched;

        public AABox Box;
        public QuadClass MyQuad;

        public int MyValue, MyScoreValue;

        public override void MakeNew()
        {
            Core.Init();

            MyValue = 1;
            MyScoreValue = 50;

            Core.MyType = ObjectType.Coin;
            Core.DrawLayer = 5;

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

            Core.BoxesOnly = BoxesOnly;
        }

        public static Vector2 PosOfLastCoinGrabbed;
        public void Die()
        {
            if (Core.MyLevel.PlayMode != 0) return;

            PosOfLastCoinGrabbed = Pos;

            Core.Active = false;

            Info.Coins.MySound.Play(.4f, .1f, 0);

            // Effect
            if (Info.Coins.ShowEffect)
            {
                ParticleEffects.CoinDie_New(MyLevel, Pos);
                ParticleEffects.CoinDie_Old(MyLevel, Pos);
            }

            // Text float
            if (Info.Coins.ShowText && Core.MyLevel.MyGame.MyBankType != GameData.BankType.Campaign && !Game.SuppresCoinText)
            {
				MyGame.CalculateCoinScoreMultiplier();
                int val = CalcScoreValue();
                TextFloat text = new TextFloat("+" + val.ToString(), Core.Data.Position + new Vector2(21, 22.5f));
                text.Core.DrawLayer = 8;
                Core.MyLevel.MyGame.AddGameObject(text);
            }
            else if (Core.MyLevel.MyGame.MyBankType == GameData.BankType.Campaign || Game.SuppresCoinText)
			{
				ParticleEffects.CoinDie_Campaign(MyGame.MyLevel, Coin.PosOfLastCoinGrabbed);
			}
        }

        GameData MyGame { get { return Core.MyLevel.MyGame; } }

        public int CalcScoreValue()
        {
            return (int)(MyScoreValue * MyGame.CoinScoreMultiplier * MyGame.ScoreMultiplier);
        }

        public enum CoinType { Blue, Red };
        public CoinType MyType;

        public void SetType(CoinType type)
        {
            MyType = type;

            Box.Initialize(Core.Data.Position, Info.Coins.BoxSize);

            if (!Core.BoxesOnly)
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
            double t = 2 * Math.PI * (Core.GetPhsxStep() + Offset) / (float)Period;
            return new Vector2((float)Math.Cos(t)) * Radii + Core.StartData.Position;
        }

        public override void PhsxStep()
        {
            if (!Core.Active) return;

            if (!AlwaysActive)
            if (!Core.MyLevel.MainCamera.OnScreen(Core.Data.Position, 200))
            {
                if (!MyLevel.BoxesOnly)
                {
                    MyQuad.Quad.Playing = false;
                    MyQuad.Quad.Loop = false;
                    MyQuad.Quad.t = 0;
                }

                Core.SkippedPhsx = true;
                Core.WakeUpRequirements = true;
                return;
            }
            Core.SkippedPhsx = false;

            // Shimmer
            if (!MyLevel.BoxesOnly)
            {
                MyQuad.Quad.Playing = true;
                MyQuad.Quad.Loop = false;
                MyQuad.Quad.t = MyLevel.CurPhsxStep % 110;
            }

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

        public override void PhsxStep2()
        {
            if (!Core.Active) return;
            if (Core.SkippedPhsx) return;

            Box.SwapToCurrent();
        }

        public override void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            Pos = Core.StartData.Position;
        }

        public override void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Pos += shift;

            Box.Move(shift);
        }

        public override void Interact(Bob bob)
        {
            if (!Core.Active) return;
            if (Core.MyLevel.SuppressCheckpoints && !Core.MyLevel.ShowCoinsInReplay) return;

			bool Col = false;
			if (bob.MyPhsx.Transcendent)
				Col = Phsx.BoxBoxOverlap(bob.Box, Box);
			else
				Col = Phsx.BoxBoxOverlap(bob.Box2, Box);

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
        public override void Draw()
        {
            if (!Core.Active) return;
            if (Core.MyLevel.SuppressCheckpoints && !Core.MyLevel.ShowCoinsInReplay) return;

            if (Box.Current.BL.X > Core.MyLevel.MainCamera.TR.X + DrawGrace.X || Box.Current.BL.Y > Core.MyLevel.MainCamera.TR.Y + DrawGrace.Y)
                return;
            if (Box.Current.TR.X < Core.MyLevel.MainCamera.BL.X - DrawGrace.X || Box.Current.TR.Y < Core.MyLevel.MainCamera.BL.Y - DrawGrace.Y)
                return;

            if (Tools.DrawGraphics && Info.Coins.ShowCoin && !Core.BoxesOnly)
            {
                MyQuad.Pos = Pos;
                MyQuad.Draw();
            }

			if (Tools.DrawBoxes)
			{
				Tools.QDrawer.DrawCircle(Pos, 38, new Color(255, 134, 26, 235));

				//Box.DrawFilled(Tools.QDrawer, new Color(255, 134, 26, 220));
				//Box.Draw(Tools.QDrawer, Color.Bisque, 10);
			}
        }

        public override void Clone(ObjectBase A)
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
    }
}
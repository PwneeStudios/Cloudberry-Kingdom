using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Levels;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public class IntroCinematic : GameData
    {
        Bob Bob1;
        Seed seed;
        QuadClass FBlock;
        bool FBlockShake = false;
        Vector2 FBlockOffset = Vector2.Zero;
        QuadClass BackgroundQuad;

        ScenePart Part_ZoomedInBob, Part_ZoomedOut, Part_ZoomedInSeed, Part_Explosion, CurPart;

        public IntroCinematic()
        {
            Init();
        }

        public Level MakeLevel()
        {
            Level level = new Level();
            level.MainCamera = new Camera();
            level.MainCamera.SetPhsxType(Camera.PhsxType.Fixed);
            level.MainCamera.Update();

            level.MainEmitter = new CloudberryKingdom.Particles.ParticleEmitter(10000);
            
            seed = (Seed)Recycle.GetObject(ObjectType.Seed, false);
            seed.Core.Data.Position = new Vector2(-700, 0);
            seed.InteractionDist = 2500;
            seed.SetState(Seed.State.On);
            seed.MyDrawType = Seed.DrawType.Chaos;
            seed.MyType = Seed.Type.Cinematic;
            level.AddObject(seed);

            for (int i = 0; i < 4; i++)
            {
                //Generic.PlayerData[i].PlayerExists = true;

                if (PlayerManager.Get(i).Exists)
                {
                    Bob1 = new Bob(BobPhsxNormal.Instance, false);
                    Bob1.MyPlayerIndex = PlayerManager.Get(i).MyPlayerIndex;
                    Bob1.SetToCinematic();
                    Bob1.AffectsCamera = false;
                    Bob1.Immortal = true;
                    Bob1.CompControl = true;
                    //Bob1.SetColorScheme(Generic.ColorSchemes[i]);
                    Bob1.SetColorScheme(PlayerManager.Get(i).ColorScheme);
                    level.AddBob(Bob1);

                    PhsxData start = new PhsxData();
                    start.Position = new Vector2(1200, 0);
                    Bob1.Init(false, start, this);
                    Bob1.MyCape.Gravity = Vector2.Zero;
                    Bob1.PlayerObject.AnimQueue.Clear();
                    Bob1.PlayerObject.EnqueueAnimation("Intro_HoldOn", .4f * i, true, true, 1, 3.3f, false);
                }
            }
            Bob1 = level.Bobs[0];
            
            level.MyBackground = Background.Get(BackgroundType.Gray);
            level.MyBackground.Init(level);
            level.BL = level.MainCamera.BL - new Vector2(1500, 0);
            level.TR = level.MainCamera.TR;
            level.MyBackground.AddSpan(level.BL, level.TR);
            
            level.PhsxStep(false);
            ((BobPhsxCinematic)Bob1.MyPhsx).FancyPos.RelVal = Bob1.Core.Data.Position;

            return level;
        }

        public override void Init()
        {
            base.Init();

            MyLevel = MakeLevel();
            Tools.CurGameData = this;

            BackgroundQuad = new QuadClass();
            //BackgroundQuad.Quad.SetColor(new Color(new Vector3(.5f, 0, .5f) * .5f));
            BackgroundQuad.Quad.SetColor(new Color(new Vector3(.5f, .5f, .5f) * .4f));
            BackgroundQuad.SetToDefault();
            BackgroundQuad.FullScreen(MyLevel.MainCamera);
            BackgroundQuad.Scale(1.25f);
            BackgroundQuad.Update();

            FBlock = new QuadClass();
            FBlock.SetToDefault();
            FBlock.Quad.MyTexture = Tools.TextureWad.FindByName("FallingBlock1");
            //FBlock.Quad.MyTexture = Tools.TextureWad.FindByName("Block_Medium");
            FBlock.Scale(100);
            FBlock.Pos = ((Quad)Bob1.PlayerObject.FindQuad("Hand_Right")).Center.Pos
                            + new Vector2(-FBlock.Size.X * .55f, -FBlock.Size.Y * 1.27f);

            // Zoomed in on seed
            Part_ZoomedInSeed = new ScenePart();
            Part_ZoomedInSeed.MyBegin = delegate()
            {
                MyLevel.MainCamera.Data.Position = seed.Core.Data.Position;
                MyLevel.MainCamera.Zoom = 2 * new Vector2(.001f, .001f);

                FadeIn(.02f);
            };

            Part_ZoomedInSeed.MyPhsxStep = delegate(int Step)
            {
                if (Step == 20)//37)
                    Tools.SoundWad.FindByName("Angels").Play();

                if (Step == 37 + 215 + 18)
                {
                    CurPart = Part_ZoomedInBob;
                    CurPart.Begin();
                }
            };

            // Zoomed in on Bob
            Part_ZoomedInBob = new ScenePart();
            Part_ZoomedInBob.MyBegin = delegate()
            {
                MyLevel.MainCamera.Data.Position = Bob1.Core.Data.Position + new Vector2(-300, 0);
                MyLevel.MainCamera.Zoom = 2.35f * new Vector2(.001f, .001f);

                ((BobPhsxCinematic)Bob1.MyPhsx).UseFancy = true;
            };

            Part_ZoomedInBob.MyPhsxStep = delegate(int Step)
            {
                if (Step == 170 + 16)
                {
                    CurPart = Part_ZoomedOut;
                    CurPart.Begin();
                }
            };

            // Zoomed out
            Part_ZoomedOut = new ScenePart();
            Part_ZoomedOut.MyBegin = delegate()
            {
                MyLevel.MainCamera.Data.Position = new Vector2(75, 0);
                MyLevel.MainCamera.Zoom = new Vector2(.001f, .001f);

                ((BobPhsxCinematic)Bob1.MyPhsx).UseFancy = true;
            };

            int Slips = 0;
            const int SlipDelay = 82;
            int[] SlipTimes = { 2 * SlipDelay - 40, 3 * SlipDelay - 40, 4 * SlipDelay - 40, 5 * SlipDelay - 40, 7 * SlipDelay + 30 };
            Part_ZoomedOut.MyPhsxStep = delegate(int Step)
            {
                if (Slips < SlipTimes.Length && Step == SlipTimes[Slips])
                {
                    if (Slips == 4)
                    {
                        ((BobPhsxCinematic)Bob1.MyPhsx).FancyPos.LerpTo(seed.Core.Data.Position, 30);
                        Bob1.PlayerObject.EnqueueAnimation("Intro_Flail", 0, true, true, 1, 3.3f, false);
                        //FBlock.Quad.MyTexture = Tools.TextureWad.FindByName("FallingBlock3");
                        //FBlockShake = false;
                    }
                    else if (Slips < 4)
                    {
                        //FBlock.Quad.MyTexture = Tools.TextureWad.FindByName("FallingBlock2");
                        ((BobPhsxCinematic)Bob1.MyPhsx).FancyPos.LerpTo(Bob1.Core.Data.Position + new Vector2(-35, 0), 5);
                    }

                    if (Slips == 3)
                    {
                        FBlock.Quad.MyTexture = Tools.TextureWad.FindByName("FallingBlock2");
                        FBlockShake = true;
                    }

                    Slips++;
                }

                // FBlock falls in
                if (Step == SlipTimes[SlipTimes.Length - 1] + 37)
                {
                    FBlock.MakeFancyPos();
                    FBlock.FancyPos.LerpTo(seed.Core.Data.Position, 30);
                    FBlock.FancyAngle = new FancyVector2();
                    FBlock.FancyAngle.LerpTo(new Vector2(100, 0), 30);
                }

                /*
                // Center camera on seed
                if (Step == SlipTimes[SlipTimes.Length - 1] + 105)
                {
                    MyLevel.MainCamera.MakeFancyPos();
                    MyLevel.MainCamera.FancyPos.LerpTo(seed.Core.Data.Position, 150);
                }*/

                // Explosion
                if (Step == SlipTimes[SlipTimes.Length - 1] + 120 + 3 + 36 + 10)
                //if (Step == SlipTimes[SlipTimes.Length - 1] + 300 + 3 + 36)
                {
                    CurPart = Part_Explosion;
                    CurPart.Begin();
                }
            };

            // Explosion
            Part_Explosion = new ScenePart();
            Part_Explosion.MyBegin = delegate()
            {
                Part_ZoomedOut.Begin();
            };

            Part_Explosion.MyPhsxStep = delegate(int Step)
            {
                if (Step == 3)
                {
                    //Tools.SoundWad.FindByName("ExplosionLong").Play();
                    Tools.SoundWad.FindByName("Piece Explosion").Play();
                    seed.Core.MarkedForDeletion = true;
                }
                if (Step >= 0)
                {
                    ParticleEffects.PieceExplosion(MyLevel, seed.Core.Data.Position, MyLevel.CurPhsxStep, 1);
                }
                if (Step == 140)
                {
                    FadeColor = new FancyColor();
                    FadeColor.LerpTo(new Vector4(1f, 1f, 1f, 0f), Color.White.ToVector4(), 60);
                }
                //if (Step == 4 * 60)
                //{
                //    FadeColor.LerpTo(Color.Black.ToVector4(), 60);
                //}

                if (Step == 350)
                {
                    //Tools.CurGameType = GameType.DifficultySelect;
                    //Tools.CurGameData = new MainWorldGameData();

                    StringWorldGameData StringWorld = new StringWorldGameData();
                    Tools.WorldMap = Tools.CurGameData = StringWorld;
                }
                
                //if (Step * 60 % 70 == 0)
                      //  Tools.SoundWad.FindByName("ExplosionLong").Play(.3f);
            };

            //CurPart = Part_Explosion;
            CurPart = Part_ZoomedInSeed;
            //CurPart = Part_ZoomedOut;
            CurPart.Begin();
        }

        public void Start()
        {
            Tools.CurLevel = MyLevel;
            MyLevel.MyGame = this;
        }

        public override void PhsxStep()
        {            
            base.PhsxStep();

            CurPart.PhsxStep();

            Bob PrevBob = Bob1;
            foreach (Bob bob in MyLevel.Bobs)
            {
                if (bob != Bob1)
                {
                    Vector2 HandPos = ((Quad)bob.PlayerObject.FindQuad("Hand_Right")).Center.Pos;
                    Vector2 HandPos2 = ((Quad)PrevBob.PlayerObject.FindQuad("Hand_Left")).Center.Pos;
                    Vector2 LegPos = ((BendableQuad)Bob1.PlayerObject.FindQuad("Leg_Left")).MySpline.Node[0].Pos;
                    //Bob2.Move(LegPos - HandPos);
                    bob.Move(HandPos2 - HandPos + new Vector2(-47, 11));
                }
                PrevBob = bob;
            }

            FBlock.Pos -= FBlockOffset;
            if (FBlockShake && MyLevel.CurPhsxStep % 2 == 0)
            {
                FBlockOffset = new Vector2(MyLevel.Rnd.Rnd.Next(-10, 10), MyLevel.Rnd.Rnd.Next(-10, 10));
            }
            FBlock.Pos += FBlockOffset;

            //FBlock.Pos = ((Quad)Bob1.PlayerObject.FindQuad("Hand_Right")).Center.Pos
            //                + 1.05f * new Vector2(FBlock.Size.X, -FBlock.Size.Y * 1.3f) / 2;

            //Bob1.CapeWind = new Vector2(-2, 0);
            seed.Interact(Bob1);

            /*
            if (MyLevel.CurPhsxStep < 300)
                ParticleEffects.PieceOrb(MyLevel, new Vector2(-7500,0), MyLevel.CurPhsxStep, 1);
            else
                ParticleEffects.PieceExplosion(MyLevel, new Vector2(-7500, 0), MyLevel.CurPhsxStep, 1);
            */
        }

        public override void PreDraw()
        {
            base.PreDraw();

            //BackgroundQuad.Draw(false);
            FBlock.Draw();
        }

        public override void PostDraw()
        {
            base.PostDraw();
        }
    }
}
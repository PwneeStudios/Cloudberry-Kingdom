using System;
using Microsoft.Xna.Framework;

namespace CloudberryKingdom.Levels
{
    public class MakeFinalSeed : MakeThing
    {
        Level MyLevel;

        Vector2 SeedPos = Vector2.Zero;
        float SchwarzschildRadius = 625;
        float DistanceToSeed = 900;

        public MakeFinalSeed(Level level)
        {
            MyLevel = level;
        }

        /// <summary>
        /// Add some extra vanilla blocks and set the tentative position for the seed.
        /// </summary>
        public override void Phase1()
        {
            base.Phase1();

            MyLevel.EndBuffer = 6000;

            float NewRight = MyLevel.VanillaFill(new Vector2(MyLevel.MaxRight + 450, MyLevel.MainCamera.BL.Y), new Vector2(MyLevel.MaxRight + 3000, MyLevel.MainCamera.TR.Y - 1000), 400);
            SeedPos = new Vector2(NewRight + DistanceToSeed, MyLevel.MainCamera.Data.Position.Y + 200);//400);
        }

        /// <summary>
        /// Based on the actual path of the computer, calculate an accessible location for the seed, as close to the original tentative position as possible.
        /// </summary>
        public override void Phase2()
        {
            base.Phase2();

            int Earliest = 100000;
            Vector2 pos = SeedPos;
            float Closest = -1;
            int NewLastStep = MyLevel.LastStep;
            for (int i = 0; i < MyLevel.CurPiece.NumBobs; i++)
                for (int j = MyLevel.LastStep - 1; j > 0; j--)//LastStep - 240; j--)
                {
                    Vector2 BobPos = MyLevel.CurPiece.Recording[i].AutoLocs[j];
                    float Dist = (BobPos - SeedPos).Length();
                    //if (Closest == -1 || Dist < SchwarzschildRadius)
                    //  Earliest = Math.Min(Earliest, j - 10);
                    if (Closest == -1 || Dist < Closest)
                    {
                        Earliest = Math.Min(Earliest, j);
                        Closest = Dist;
                        pos = BobPos;
                        NewLastStep = j;
                    }
                }
            Vector2 dif = SeedPos - pos;

            dif.Normalize();
            SeedPos = pos + dif * Math.Min(SchwarzschildRadius - 50, (SeedPos - pos).Length());
            MyLevel.LastStep = Earliest; // NewLastStep;
        }

        /// <summary>
        /// Create the actual seed.
        /// Add a suitable camera zone so that the camera centers on the seed when the player is near.
        /// </summary>
        public override void Phase3()
        {
            base.Phase3();

            //Stage1FinalPlats(new Vector2(MaxLeft, MainCamera.BL.Y + 50), new Vector2(NewRight + 230, MainCamera.TR.Y - 50), ObjectType.NormalBlock, ref CurMakeData);
            Seed seed = (Seed)MyLevel.Recycle.GetObject(ObjectType.Seed, false);
            seed.Core.StartData.Position = seed.Core.Data.Position = SeedPos;
            seed.Intensity = 1f;//.85f;
            seed.InteractionDist = 2300f;
            seed.SchwarzschildRadius = SchwarzschildRadius;
            seed.SetState(Seed.State.On);
            seed.MyDrawType = Seed.DrawType.Cloud;
            seed.MyType = Seed.Type.EndOfLevel;
            seed.Core.EditorCode1 = LevelConnector.EndOfLevelCode;
            MyLevel.AddObject(seed);

            // Add random blocks after seed
            MyLevel.RandomBlocks(new Vector2(SeedPos.X + MyLevel.Rnd.RndFloat(600 + DistanceToSeed, 1500), MyLevel.MainCamera.BL.Y), new Vector2(SeedPos.X + 4000, MyLevel.MainCamera.TR.Y), ref MyLevel.CurMakeData);

            // Add CameraZone
            CameraZone CamZone = (CameraZone)MyLevel.Recycle.GetObject(ObjectType.CameraZone, false);
            Vector2 Size = new Vector2(MyLevel.MainCamera.GetWidth() / 2, MyLevel.MainCamera.GetHeight() / 2);
            CamZone.Init(new Vector2(seed.Core.Data.Position.X, MyLevel.CurMakeData.PieceSeed.End.Y), Size);
            CamZone.Start = seed.Core.Data.Position;
            CamZone.End = seed.Core.Data.Position;
            CamZone.CameraType = Camera.PhsxType.Center;
            CamZone.CameraSpeed = 30f;
            MyLevel.AddObject(CamZone);
        }
    }
}

using System;
using System.Linq;
using Microsoft.Xna.Framework;

using Drawing;
using System.IO;
using CloudberryKingdom.Bobs;

namespace CloudberryKingdom
{
    public partial class Seed : IObject, ILevelConnector
    {
        LevelSeedData _NextLevelSeedData;
        public LevelSeedData NextLevelSeedData { get { return _NextLevelSeedData; } set { _NextLevelSeedData = value; } }

        public void TextDraw() { }
        public void Release()
        {
            Core.Release();
        }

        public enum DrawType { Chaos, Cloud };
        public DrawType MyDrawType;
        public enum State { Off, On, TurningOn, TurningOff, Launch, DownLaunch };
        public State MyState;
        public enum Type { Cinematic, EndOfLevel };
        public Type MyType;

        public float InteractionDist = 2300;
        public float SchwarzschildRadius = 755;
        public float Intensity = 1f;

        static bool TemplateInitialized;
        static EzSound MySound;

        public bool SkipPhsx;

        public ObjectData CoreData;
        public ObjectData Core { get { return CoreData; } }

        public void MakeNew()
        {
            Core.Init();
            Core.MyType = ObjectType.Seed;
            Core.DrawLayer = 8;

            Core.ResetOnlyOnReset = true;

            Init();
        }

        public Seed()
        {
            CoreData = new ObjectData();

            MakeNew();

            Core.BoxesOnly = false;
        }

        public void SetState(State NewState)
        {
            if (NewState != MyState)
            {
                switch (NewState)
                {
                    case State.TurningOn:
                        Start_Init();
                        break;

                    case State.TurningOff:
                        End_Init();
                        break;

                    case State.Off:
                        foreach (Bob bob in Core.MyLevel.Bobs)
                            if (bob.SuckedIn && bob.SuckedInSeed == this)
                            {
                                bob.EndSuckedIn();
                            }
                        break;

                    case State.Launch:
                        Start_Launch();
                        break;

                    case State.DownLaunch:
                        Start_DownLaunch();
                        break;
                }

                MyState = NewState;
            }
        }



        public void Die()
        {
            if (Core.MyLevel.PlayMode != 0) return;

            MySound.Play();
        }

        public void Init()
        {
            if (!TemplateInitialized)
            {
                TemplateInitialized = true;

                MySound = InfoWad.GetSound("Checkpoint_Sound");
            }
        }

        public void PhsxStep()
        {
            //if (!Panning && Core.MyLevel.CurPhsxStep == 60) StartPan();
            //if (Panning) PanPhsx();


            if (!Core.Active) return;

            switch (MyState)
            {
                case State.Off:
                    SkipPhsx = true;
                    return;

                case State.On:
                    On_PhsxStep();
                    break;

                case State.TurningOn:
                    Start_PhsxStep();
                    break;

                case State.TurningOff:
                    End_PhsxStep();
                    break;

                case State.Launch:
                    Launch_PhsxStep();
                    break;

                case State.DownLaunch:
                    DownLaunch_PhsxStep();
                    break;
            }
        }

        int WhispySound_Count = 0;
        int WhispySound_Period = (int)InfoWad.GetFloat("Maelstrom_Regular_Period");
        void WhispySound_Phsx()
        {
            if (WhispySound_Count == 0)
                Tools.SoundWad.FindByName("Maelstrom_Regular").Play();

            WhispySound_Count++;
            if (WhispySound_Count > WhispySound_Period)
                WhispySound_Period = 0;
        }

        int EndOfLevelCount = 0;
        void On_PhsxStep()
        {
            if (MyType == Type.Cinematic) return;

            if (Core.Data.Position.X > Core.MyLevel.MainCamera.TR.X + 1250 ||
                Core.Data.Position.X < Core.MyLevel.MainCamera.BL.X - 1250 ||
                Core.Data.Position.Y > Core.MyLevel.MainCamera.TR.Y + 1250 ||
                Core.Data.Position.Y < Core.MyLevel.MainCamera.BL.Y - 1250)
            {
                SkipPhsx = true;
                return;
            }

            WhispySound_Phsx();

            // Check for end of level
            if (Core.MyLevel.Finished)
            {
                EndOfLevelCount++;
                if (EndOfLevelCount > 42)
                {
                    StringWorldGameData world = Tools.WorldMap as StringWorldGameData;
                    if (null == world || world.GetSeed(world.CurLevelIndex + 1).MyBackgroundType == Core.MyLevel.MyBackground.MyType)
                        SetState(State.Launch);
                    else
                        SetState(State.DownLaunch);
                }
            }
            else
            {
                bool AllInside = Core.MyLevel.Bobs.All(delegate(Bob bob) { return bob.SuckedIn || bob.Dead; });
                if (AllInside && !PlayerManager.AllDead())
                {
                    // End the level
                    Core.MyLevel.EndLevel();

                    // Give bonus to completing players
                    Core.MyLevel.EndOfLevelBonus(null);
                }
            }

            SkipPhsx = false;
        }

        public void PhsxStep2() { }
       
        public void Reset(bool BoxesOnly)
        {
            Core.Active = true;

            Core.Data.Position = Core.StartData.Position;
        }

        public void Move(Vector2 shift)
        {
            Core.StartData.Position += shift;
            Core.Data.Position += shift;
        }

        public void Interact(Bob bob)
        {
            if (!Core.Active) return;
            //if (Core.MyLevel.SuppressCheckpoints || Core.MyLevel.GhostCheckpoints) return;

            float CapeInteraction = 0;
            bool CanSuckIn = false;
            if (MyState == State.On) { CapeInteraction = 1; CanSuckIn = true; }
            if (MyState == State.TurningOn) CapeInteraction = Math.Min(1f, .05f * StartStep);
            if (MyState == State.TurningOff) CapeInteraction = 1;// Math.Max(0f, 1f - .0175f * EndStep);
            if (MyState == State.Off) return;

            if (bob.SuckedIn) return;

            // Suction
            Vector2 Dir = Core.Data.Position - bob.Core.Data.Position;
            float Length = Dir.Length();
            if (Length < InteractionDist)
            {
                if (CanSuckIn && Length < SchwarzschildRadius)
                {
                    Tools.SoundWad.FindByName("Maelstrom_SuckIn").Play();

                    bob.SuckedIn = true;
                    bob.SuckedInSeed = this;

#if XBOX
            Tools.SetVibration(bob.MyPlayerIndex, .5f, .5f, 30);
#endif
                }

                Length *= .8f;

                Dir.Normalize();
                float Period = 3 * Math.Max(.35f, 1.5f * Length / 1000f);

                bob.CapeWind = 2350f / Math.Max(400, Length) * Cape.SineWind(Dir, .75f + .15f * Length / 1000f, Period * .7f, Core.MyLevel.CurPhsxStep);
                bob.CapeWind *= CapeInteraction;
            }
        }

        public void Draw()
        {
            if (!Core.Active) return;
            //if (Core.MyLevel.SuppressCheckpoints && !Core.MyLevel.GhostCheckpoints) return;
            if (SkipPhsx) return;

            if (Tools.DrawGraphics && !Core.BoxesOnly && Core.MyLevel.MainEmitter.IsActive()
                && MyState == State.On)
            {
                switch (MyDrawType)
                {
                    case DrawType.Chaos:
                        ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.BigRnd, Core.Data.Position, Core.MyLevel.CurPhsxStep, Intensity);
                        break;

                    case DrawType.Cloud:
                        ParticleEffects.Coalesce(Core.MyLevel, Core.Data.Position);
                        ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.Cloud, Core.Data.Position, Core.MyLevel.CurPhsxStep, Intensity);
                        break;
                }

                //ParticleEffects.PieceOrb(Core.MyLevel, ParticleEffects.PieceOrbStyle.BigRnd, Core.Data.Position, Core.MyLevel.CurPhsxStep, Intensity);
            }
        }

        public void Clone(IObject A)
        {
            Core.Clone(A.Core);

            Seed SeedA = A as Seed;
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
public bool PreDecision(Bob bob) { return false; }
//StubStubStubEnd7
    }
}
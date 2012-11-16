using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;




namespace CloudberryKingdom
{
    public class Recording
    {
        /// <summary>
        /// The file this recording was loaded from.
        /// </summary>
        public string SourceFile;

        /// <summary>
        /// Returns the current working directory for where .rec files are stored.
        /// Do not save here if you wish to override a .rec file in future builds.
        /// </summary>
        public static string DefaultRecordingDirectory()
        {
            return Path.Combine(Globals.ContentDirectory, "Recordings");
        }

        /// <summary>
        /// Returns the directory where the source .rec files are stored.
        /// Save here if you wish to override a .rec file in future builds.
        /// </summary>
        /// <returns></returns>
        public static string SourceRecordingDirectory()
        {
            return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))), "Content\\Recordings");
        }

        /// <summary>
        /// Save the recording to a .rec file
        /// </summary>
        /// <param name="Bin">Whether the file is saved to the bin or the original project content directory.</param>
        public void Save(String file, bool Bin)
        {
            // First move to standard directory for .rec files
            string fullpath;
            if (Bin)
                fullpath = Path.Combine(DefaultRecordingDirectory(), file);
            else
                fullpath = Path.Combine(SourceRecordingDirectory(), file);

            // Now write to file
            Tools.UseInvariantCulture();
            FileStream stream = File.Open(fullpath, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);
            Write(writer);
            writer.Close();
            stream.Close();
        }

        /// <summary>
        /// Load into the level information from a .rec file.
        /// </summary>
        public void Load(String file)
        {
            SourceFile = file;

            // First move to standard directory for .rec files
            string fullpath = Path.Combine(DefaultRecordingDirectory(), file);

            // Now read the file
            Tools.UseInvariantCulture();
            FileStream stream = File.Open(fullpath, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            Read(reader);
            reader.Close();
            stream.Close();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(NumBobs);
            writer.Write(Length);

            for (int i = 0; i < NumBobs; i++)
                Recordings[i].Write(writer, Length);
        }
        public void Read(BinaryReader reader)
        {
            NumBobs = reader.ReadInt32();
            Length = reader.ReadInt32();
            Init(NumBobs, Length);

            for (int i = 0; i < NumBobs; i++)
                Recordings[i].Read(reader, Length);
        }

        public int NumBobs;
        public ComputerRecording[] Recordings;
        public int Length;

        public Vector2 BoxSize;

        public void Draw(QuadClass BobQuad, int Step, Level level, SpriteAnimGroup[] AnimGroup, List<BobLink> BobLinks)
        {
            if (level.MyGame.MyGameFlags.IsTethered && Step < Length - 1)
            {
                foreach (BobLink link in BobLinks)
                {
                    if (level.DefaultHeroType is BobPhsxSpaceship &&
                        (!Recordings[link._j].GetAlive(Step) || !Recordings[link._k].GetAlive(Step)))
                        continue;
                    link.Draw(Recordings[link._j].GetBoxCenter(Step), Recordings[link._k].GetBoxCenter(Step));
                }
            }

            for (int i = 0; i < NumBobs; i++)
            {
                if (i >= level.Bobs.Count) continue;
                if (Step < Length - 1)
                {
                    if (level.DefaultHeroType is BobPhsxSpaceship)
                        if (Step > 0 && !Recordings[i].GetAlive(Step))
                        {
                            if (Recordings[i].GetAlive(Step - 1))
                            {
                                ParticleEffects.AddPop(level, Recordings[i].GetBoxCenter(Step));
                            }
                            continue;
                        }

                    Vector2 padding = Vector2.Zero;
                    int anim = (int)Recordings[i].GetAnim(Step);
                    if (anim > 200) continue;

                    BobQuad.Quad.MyTexture.Tex = AnimGroup[i].Get(anim, Recordings[i].Gett(Step), ref padding);

                    BobQuad.Base.e1 = new Vector2(BoxSize.X + padding.X, 0);
                    BobQuad.Base.e2 = new Vector2(0, BoxSize.Y + padding.Y);
                    BobQuad.Base.Origin = Recordings[i].GetBoxCenter(Step);
                    if (BobQuad.Base.Origin == Vector2.Zero) continue;

                    BobQuad.Draw();
                    Tools.QDrawer.Flush();
                }
                else
                    if (Step == Length - 1 && !level.ReplayPaused && !(level.DefaultHeroType is BobPhsxSpaceship && !Recordings[i].GetAlive(Length - 1)))
                        ParticleEffects.AddPop(level, Recordings[i].GetBoxCenter(Length - 1));
            }
        }

        public void ConvertToSuperSparse()
        {
            for (int i = 0; i < Recordings.Length; i++)
            {
                Recordings[i].ConvertToSuperSparse();
            }
        }

        public void Release()
        {
            if (Recordings != null)
            for (int i = 0; i < Recordings.Length; i++)
            {
                Recordings[i].Release();
            }
            Recordings = null;
        }

        public Recording(int NumBobs, int Length) { Init(NumBobs, Length); }
        public void Init(int NumBobs, int Length)
        {
            this.NumBobs = NumBobs;

            Recordings = new ComputerRecording[NumBobs];
            for (int i = 0; i < NumBobs; i++)
            {
                Recordings[i] = new ComputerRecording();
                Recordings[i].Init(Length, true);
            }
        }

        public void Record(Level level)
        {
            if (level.Bobs.Count <= 0) return;
            if (level.PlayMode != 0 || level.Watching) return;
            if (level.CurPhsxStep < 0) return;

            BoxSize = level.Bobs[0].PlayerObject.BoxList[0].Size() / 2;

            Length = level.CurPhsxStep;
            for (int i = 0; i < NumBobs; i++)
            {
                if (i >= level.Bobs.Count) continue;

                Recordings[i].Anim[level.CurPhsxStep] = (byte)level.Bobs[i].PlayerObject.anim;

                Recordings[i].t[level.CurPhsxStep] = level.Bobs[i].PlayerObject.t;
                Recordings[i].BoxCenter[level.CurPhsxStep] = level.Bobs[i].PlayerObject.BoxList[0].Center();
                Recordings[i].AutoLocs[level.CurPhsxStep] = level.Bobs[i].Core.Data.Position;
                Recordings[i].AutoVel[level.CurPhsxStep] = level.Bobs[i].Core.Data.Velocity;
                Recordings[i].Input[level.CurPhsxStep] = level.Bobs[i].CurInput;
                Recordings[i].Alive[level.CurPhsxStep] = !(level.Bobs[i].Dead || level.Bobs[i].Dying);

                if (!level.Bobs[i].Core.Show)
                {
                    Recordings[i].Anim[level.CurPhsxStep] = (byte)(255);
                    Recordings[i].BoxCenter[level.CurPhsxStep] =
                    Recordings[i].AutoLocs[level.CurPhsxStep] = level.MainCamera.Data.Position;
                }
            }
        }
 
        public void MarkEnd(Level level)
        {
            Length = level.CurPhsxStep;
        }
    }
}
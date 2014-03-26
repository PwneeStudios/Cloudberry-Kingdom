using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CoreEngine;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Blocks;
using CloudberryKingdom.Obstacles;

namespace CloudberryKingdom
{
    public class Prototypes
    {
        static ObjectClass LoadAnimObj = null;
        public static void LoadAnimation(string path)
        {
            //if (!path.Contains("double")) return;
            //if (path.Contains("double")) return;
            if (!path.Contains("bob_v2_trimmed")) return;

            Tools.UseInvariantCulture();
            FileStream rstream = File.Open("Content\\Objects\\TigarBob.smo", FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader rreader = new BinaryReader(rstream, Encoding.UTF8);
            var obj = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.Device.PresentationParameters, 100, 100, Tools.BasicEffect, Tools.TextureWad.FindByName("White"));
            obj.ReadFile(rreader, Tools.EffectWad, Tools.TextureWad);
            rreader.Close();
            rstream.Close();

            LoadAnimObj = obj;

            SetTigarLoaded(obj);
            obj.ParentQuad.Scale(new Vector2(260, 260));

            bool BrandNew = false;


            





            // Find path
            Tools.UseInvariantCulture();
            FileStream stream = null;
            string original_path = path;
            try
            {
                stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch
            {
                try
                {
                    path = Path.Combine(Globals.ContentDirectory, original_path);
                    stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch
                {
                    try
                    {
                        path = Path.Combine(Globals.ContentDirectory, Path.Combine("DynamicLoad", original_path));
                        stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                    }
                    catch
                    {
                        Tools.Log(string.Format("Attempting to load a .anim file. Path <{0}> not found."));
                        return;
                    }
                }
            }

            ObjectClass p;
            if (BrandNew)
            {
                p = MakeObj();
                p.LoadingRunSpeed = .85f;
                p.ParentQuad.Scale(new Vector2(260, 260));
                p.ParentQuad.MyEffect = Tools.BasicEffect;
            }
            else
            {
                p = obj;
            }

            p.Play = true;
            foreach (BaseQuad quad in p.QuadList)
                quad.MyDrawOrder = ObjectDrawOrder.AfterOutline;

            var h = p.FindQuad("Head");
            h.Show = false;

            var q = p.FindQuad("MainQuad");
            q.MyEffect = Tools.BasicEffect;
            q.MyDrawOrder = ObjectDrawOrder.AfterOutline;
            q.SetColor(Color.White);
            Quad _quad = q as Quad;
            q.TextureAnim = new AnimationData_Texture();
            q.TextureAnim.Anims = new OneAnim_Texture[20];

            var ToAnim = new Dictionary<string, int>();
            ToAnim.Add("Stand", 0);
            ToAnim.Add("Run", 1);
            ToAnim.Add("Jump", 2);
            ToAnim.Add("DoubleJump", 29);
            ToAnim.Add("Fall", 3);
            ToAnim.Add("Duck", 4);
            ToAnim.Add("Turn", 5);

            ToAnim.Add("Wheel", 25);
            ToAnim.Add("Bouncy", 24);
            ToAnim.Add("Jump2", 29);
            ToAnim.Add("Flip", 16);
            ToAnim.Add("Wave", 13);
            ToAnim.Add("Box_Stand", 6);
            ToAnim.Add("Box_Jump", 7);
            ToAnim.Add("Box_Duck", 8);

            ToAnim.Add("Cart_Stand", 17);
            ToAnim.Add("Cart_Jump", 18);
            ToAnim.Add("Cart_Duck", 19);

            ToAnim.Add("Dead", 9);

            LoadAnimObj = p;

            StreamReader reader = new StreamReader(stream);

            string line;
            Vector2 shift = Vector2.Zero;
            float scale = 1;

            line = reader.ReadLine();
            while (line != null)
            {
                var bits = Tools.GetBitsFromLine(line);

                if (bits.Count > 1)
                {
                    var name = bits[0];

                    // Try to load line as an animation
                    if (ToAnim.ContainsKey(name))
                    {
                        int anim = ToAnim[name];

                        // Name, file, start frame, end frame
                        string root = bits[1];
                        int start_frame = int.Parse(bits[2]);
                        int end_frame = int.Parse(bits[3]);

                        // Speed or frame length
                        bool _use_speed = false;
                        int frame_length = 1;
                        float speed = 1;
                        if (bits[4] == "speed")
                        {
                            speed = float.Parse(bits[5]);
                            _use_speed = true;
                        }
                        else
                            frame_length = int.Parse(bits[4]);

                        // Reverse
                        bool reverse = false;
                        if (bits.Contains("reverse"))
                            reverse = true;

                        q.TextureAnim.ClearAnim(anim);

                        //if (name.Contains("ounc")) Tools.Write("!");

                        int num_frames = end_frame > start_frame ? end_frame - start_frame + 1 : start_frame - end_frame + 1;
                        for (int i = 0; i < num_frames; i++)
                        {
                            int frame = end_frame > start_frame ? start_frame + i : start_frame - i;

                            // Get the texture for this frame.
                            EzTexture texture = Tools.Texture(string.Format("{0}_{1}", root, frame));
                            if (texture == Tools.TextureWad.DefaultTexture)
                                texture = Tools.Texture(string.Format("{0}_0000{1}", root, frame));
                            if (texture == Tools.TextureWad.DefaultTexture)
                                texture = Tools.Texture(string.Format("{0}_000{1}", root, frame));
                            if (texture == Tools.TextureWad.DefaultTexture)
                                texture = Tools.Texture(string.Format("{0}_00{1}", root, frame));
                            if (texture == Tools.TextureWad.DefaultTexture)
                                texture = Tools.Texture(string.Format("{0}_0{1}", root, frame));

                            // Record object quad positions
                            int anim_to_mimick = 0;
                            if (anim == 4)
                                anim_to_mimick = 4;
                            p.Read_NoTexture(anim_to_mimick, 0);

                            //if (BrandNew)
                            if (true)
                            {
                                _quad.Corner[0].RelPos = new Vector2(-1, 1);
                                _quad.Corner[1].RelPos = new Vector2(1, 1);
                                _quad.Corner[2].RelPos = new Vector2(-1, -1);
                                _quad.Corner[3].RelPos = new Vector2(1, -1);
                                _quad.xAxis.RelPos = new Vector2(1, 0);
                                _quad.yAxis.RelPos = new Vector2(0, 1);

                                //_quad.xAxis.RelPos.X = _quad.yAxis.RelPos.Y * (float)texture.Width / (float)texture.Height;
                                //if (reverse)
                                //    _quad.xAxis.RelPos.X = -System.Math.Abs(_quad.xAxis.RelPos.X);

                                _quad.Center.RelPos = shift / scale;
                                _quad.xAxis.RelPos.X = scale * (float)texture.Width / (float)texture.Height;
                                if (reverse)
                                    _quad.xAxis.RelPos.X = -System.Math.Abs(_quad.xAxis.RelPos.X);
                                _quad.yAxis.RelPos.Y = scale;

                                if (BrandNew)
                                    p.Record(anim, i, true);
                                else
                                    q.Record(anim, i, true);
                            }
                            else
                                q.Record(anim, i, true);

                            p.AnimLength[anim] = num_frames - 1;

                            // Record the texture
                            q.TextureAnim.AddFrame(texture, anim);
                        }

                        if (BrandNew)
                        {
                            _quad.Center.RelPos += shift / scale;

                            _quad.Corner[0].RelPos = new Vector2(-1, 1);
                            _quad.Corner[1].RelPos = new Vector2(1, 1);
                            _quad.Corner[2].RelPos = new Vector2(-1, -1);
                            _quad.Corner[3].RelPos = new Vector2(1, -1);

                            _quad.xAxis.RelPos = new Vector2(scale, 0);
                            _quad.yAxis.RelPos = new Vector2(0, scale);

                            _quad.ModifyAllRecords(0, 0, ChangeMode.All);
                            p.Record(0, 0, true);
                        }

                        if (_use_speed)
                            p.AnimSpeed[anim] = speed;
                        else
                            p.AnimSpeed[anim] = 1f / frame_length;
                    }
                    else
                    switch (bits[0])
                    {
                        case "Scale":
                            scale = float.Parse(bits[1]);
                            break;

                        case "Shift":
                            shift = Tools.ParseToVector2(bits[1], bits[2]);
                            break;

                        default:
                            Tools.Break();
                            //Tools.ReadLineToObj(BobPhsxMario.Instance, bits);
                            break;
                    }
                }

                line = reader.ReadLine();
            }

            // Shift
            p.Read(0, 0);

            //if (BrandNew)
            //{
            //    _quad.Center.RelPos += shift / scale;

            //    _quad.Corner[0].RelPos = new Vector2(-1, 1);
            //    _quad.Corner[1].RelPos = new Vector2(1, 1);
            //    _quad.Corner[2].RelPos = new Vector2(-1, -1);
            //    _quad.Corner[3].RelPos = new Vector2(1, -1);

            //    _quad.xAxis.RelPos = new Vector2(scale, 0);
            //    _quad.yAxis.RelPos = new Vector2(0, scale);

            //    _quad.ModifyAllRecords(0, 0, ChangeMode.All);
            //    p.Record(0, 0, true);
            //}

            reader.Close();
            stream.Close();

            p.Read(0, 0);
            p.PlayUpdate(0);

            // Use object
            if (Tools.CurLevel != null && Tools.CurLevel.Bobs.Count > 0)// && Tools.CurLevel.DefaultHeroType == BobPhsxNormal.Instance)
            {
                Tools.CurLevel.Bobs[0].PlayerObject = new ObjectClass(p, false, false);
                Tools.CurLevel.Bobs[0].PlayerObject.AnimQueue.Clear();
                Tools.CurLevel.Bobs[0].PlayerObject.EnqueueAnimation(0, 0, true);
                //Tools.CurLevel.Bobs[0].MyPhsx.Prototype.PlayerObject = p;
                //BobPhsxNormal.Instance.Prototype.PlayerObject = p;
            }
            else
                Hero = p;

            ////return;

            // Save object
#if DEBUG
            Tools.UseInvariantCulture();
            FileStream fstream = File.Open("C:\\Users\\Ezra\\Desktop\\TigarBob.smo", FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter writer = new BinaryWriter(fstream, Encoding.UTF8);
            p.Write(writer);
            writer.Close();
            fstream.Close();
#endif
        }

        private static void SetTigarLoaded(ObjectClass obj)
        {
            obj.LoadingRunSpeed = .85f;
            obj.CapeThickness = 8;
            obj.p1_Left = new Vector2(-64, -64);
            obj.p2_Left = new Vector2(-24, -42);
            obj.p1_Right = obj.p1_Left; obj.p1_Right.X *= -1;
            obj.p2_Right = obj.p2_Left; obj.p2_Right.X *= -1;

            var mq = obj.FindQuad("MainQuad");
            if (mq != null)
                mq.MyEffect = Tools.EffectWad.FindByName("Hsl_Green");
            var rp = obj.FindQuad("Rocket");
            if (rp != null)
                rp.MyEffect = Tools.EffectWad.FindByName("Hsl");
            obj.UpdateEffectList();
        }

        static ObjectClass MakeObj()
        {
            var path = "Objects\\SpriteHeroTemplate.smo";
            //var path = "Objects\\stickman.smo";
            path = Path.Combine(Globals.ContentDirectory, path);
            
            var obj = LoadObject(path);
            return obj;
        }

        public static FlyingBlob FlyingBlobObj;
        public static FlyingBlob goomba;
        public static Dictionary<BobPhsx, Bob> bob;
        public static Spike SpikeObj;
        public static SimpleObject GhostBlockObj, CheckpointObj, Door, GrassDoor, ArrowObj;

        public static ObjectClass Hero;

        public static ObjectClass PlaceBob;

        public static SimpleObject LoadSimple(string file)
        {
            ObjectClass SourceObject;
            Tools.UseInvariantCulture();
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            SourceObject = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.Device.PresentationParameters, 100, 100, Tools.EffectWad.FindByName("BasicEffect"), Tools.TextureWad.FindByName("White"));
            SourceObject.ReadFile(reader, Tools.EffectWad, Tools.TextureWad);
            reader.Close();
            stream.Close();

            SourceObject.ConvertForSimple();
            return new SimpleObject(SourceObject);
        }

        public static ObjectClass LoadObject(string file)
        {
            ObjectClass obj;
            Tools.UseInvariantCulture();
            FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);
            obj = new ObjectClass(Tools.QDrawer, Tools.Device, Tools.Device.PresentationParameters, 100, 100, Tools.EffectWad.FindByName("BasicEffect"), Tools.TextureWad.FindByName("White"));
            obj.ReadFile(reader, Tools.EffectWad, Tools.TextureWad);
            reader.Close();
            stream.Close();

            return obj;
        }

        public static void LoadObjects()
        {
            Prototypes.CheckpointObj = Prototypes.LoadSimple(Path.Combine(Globals.ContentDirectory, "Objects\\FlyingCoin_v2.smo"));
            
            Prototypes.ArrowObj = Prototypes.LoadSimple(Path.Combine(Globals.ContentDirectory, "Objects\\Arrow.smo"));

            Prototypes.GhostBlockObj = Prototypes.LoadSimple(Path.Combine(Globals.ContentDirectory, "Objects\\GhostBlock.smo"));

            Prototypes.FlyingBlobObj = new FlyingBlob(Path.Combine(Globals.ContentDirectory, "Objects\\Blob.smo"), Tools.EffectWad, Tools.TextureWad);
			Prototypes.FlyingBlobObj.Core.MyType = ObjectType.FlyingBlob;
            Vector2 BlobSize = new Vector2(1.11f, 1.11f);
            Prototypes.FlyingBlobObj.MyObject.Base.e1 *= BlobSize.X;
            Prototypes.FlyingBlobObj.MyObject.Base.e2 *= BlobSize.Y;

            Prototypes.SpikeObj = new Spike(Path.Combine(Globals.ContentDirectory, "Objects\\regular_spike.smo"), Tools.EffectWad, Tools.TextureWad);

            // Create all the stickmen hero prototypes
            bob = new Dictionary<BobPhsx, Bob>();
            Bob NewBob;

            //// Bezier base object
            //NewBob = new Bob(Path.Combine(Globals.ContentDirectory, "Objects\\stickman.smo"), Tools.EffectWad, Tools.TextureWad);
            //NewBob.MyObjectType = BobPhsxNormal.Instance;
            //NewBob.DrawOutline = true;
            //NewBob.CanHaveCape = true;
            //NewBob.PlayerObject.ParentQuad.MyEffect = Tools.BasicEffect;
            //NewBob.PlayerObject.FindQuad("Hat_FireHead").MyTexture = Fireball.FlameTexture;

            // Tigar base object
            NewBob = new Bob(Path.Combine(Globals.ContentDirectory, "Objects\\TigarBob.smo"), Tools.EffectWad, Tools.TextureWad);
            SetTigarLoaded(NewBob.PlayerObject);
            NewBob.IsSpriteBased = true;
            NewBob.MyObjectType = BobPhsxNormal.Instance;
            NewBob.CanHaveCape = true;
            NewBob.PlayerObject.Linear = true;
            NewBob.PlayerObject.ParentQuad.MyEffect = Tools.BasicEffect;
            NewBob.PlayerObject.FindQuad("Hat_FireHead").MyTexture = Fireball.FlameTexture;

            // Classic Bob
            BobPhsxNormal.Instance.Prototype = NewBob;
            bob.Add(BobPhsxNormal.Instance, NewBob);

            // Invert
            BobPhsxInvert.Instance.Prototype = BobPhsxNormal.Instance.Prototype;
            bob.Add(BobPhsxInvert.Instance, NewBob);

            // Upside Down
            BobPhsxUpsideDown.Instance.Prototype = BobPhsxNormal.Instance.Prototype;
            bob.Add(BobPhsxUpsideDown.Instance, NewBob);

            // Time
            BobPhsxTime.Instance.Prototype = BobPhsxNormal.Instance.Prototype;
            bob.Add(BobPhsxTime.Instance, NewBob);

            // Tiny Bob
            BobPhsxSmall.Instance.Prototype = BobPhsxNormal.Instance.Prototype;
            bob.Add(BobPhsxSmall.Instance, NewBob);

            // Big
            BobPhsxBig.Instance.Prototype = BobPhsxNormal.Instance.Prototype;
            bob.Add(BobPhsxBig.Instance, NewBob);

            // Phase
            BobPhsxScale.Instance.Prototype = NewBob;
            bob.Add(BobPhsxScale.Instance, NewBob);

            // Double Jump
            BobPhsxDouble.Instance.Prototype = NewBob;
            bob.Add(BobPhsxDouble.Instance, NewBob);

            // Rocket
            BobPhsxJetman.Instance.Prototype = NewBob;
            bob.Add(BobPhsxJetman.Instance, NewBob);

            // Bouncy
            NewBob = new Bob(BobPhsxNormal.Instance, false);
            NewBob.MyObjectType = BobPhsxBouncy.Instance;
            NewBob.CanHaveCape = true;
            if (!NewBob.IsSpriteBased)
                NewBob.PlayerObject.FindQuad("Bouncy").Show = true;

            BobPhsxBouncy.Instance.Prototype = NewBob;
            bob.Add(BobPhsxBouncy.Instance, NewBob);

            // Wheelie
            NewBob = new Bob(BobPhsxNormal.Instance, false);
            NewBob.MyObjectType = BobPhsxWheel.Instance;
            NewBob.CanHaveCape = false;
            if (!NewBob.IsSpriteBased)
                NewBob.PlayerObject.FindQuad("Wheel").Show = true;

            BobPhsxWheel.Instance.Prototype = NewBob;
            bob.Add(BobPhsxWheel.Instance, NewBob);

			// Four-Way
			NewBob = new Bob(BobPhsxNormal.Instance, false);
			NewBob.MyObjectType = BobPhsxFourWay.Instance;
			NewBob.CanHaveCape = false;
			if (!NewBob.IsSpriteBased)
				NewBob.PlayerObject.FindQuad("Wheel").Show = true;

			BobPhsxFourWay.Instance.Prototype = NewBob;
			bob.Add(BobPhsxFourWay.Instance, NewBob);

            // Hero in a Box
            NewBob = new Bob(BobPhsxNormal.Instance, false);
            NewBob.MyObjectType = BobPhsxBox.Instance;
            if (!NewBob.IsSpriteBased)
            {
                NewBob.PlayerObject.FindQuad("Box_Back").Show = true;
                NewBob.PlayerObject.FindQuad("Box_Front").Show = true;
                NewBob.PlayerObject.FindQuad("Leg_Left").Show = false;
                NewBob.PlayerObject.FindQuad("Leg_Right").Show = false;
            }

            BobPhsxBox.Instance.Prototype = NewBob;
            bob.Add(BobPhsxBox.Instance, NewBob);

            // Rocketbox
            NewBob = new Bob(BobPhsxBox.Instance, false);
            NewBob.MyObjectType = BobPhsxBox.Instance;
            if (!NewBob.IsSpriteBased)
            {
                NewBob.PlayerObject.FindQuad("Wheel_Left").Show = true;
                NewBob.PlayerObject.FindQuad("Wheel_Right").Show = true;
            }

            BobPhsxRocketbox.Instance.Prototype = NewBob;
            bob.Add(BobPhsxRocketbox.Instance, NewBob);

            // Spaceship 
            NewBob = new Bob(Path.Combine(Globals.ContentDirectory, "Objects\\Spaceship.smo"), Tools.EffectWad, Tools.TextureWad, BobPhsxSpaceship.Instance, false);
            NewBob.MyObjectType = BobPhsxSpaceship.Instance;
            NewBob.PlayerObject.ParentQuad.Scale(new Vector2(3.5f, 3.5f));
            foreach (BaseQuad quad in NewBob.PlayerObject.QuadList)
                quad.MyDrawOrder = ObjectDrawOrder.AfterOutline;
            NewBob.CanHaveCape = false;
            NewBob.CanHaveHat = false;
            NewBob.PlayerObject.ParentQuad.MyEffect = Tools.BasicEffect;

            Quad spaceship = (Quad)NewBob.PlayerObject.QuadList[1];
            spaceship.MyTexture = Tools.Texture("Spaceship_Paper");
            spaceship.Resize();
            NewBob.PlayerObject.QuadList[2].Show = false;


            BobPhsxSpaceship.Instance.Prototype = NewBob;
            bob.Add(BobPhsxSpaceship.Instance, NewBob);

            BobPhsxTimeship.Instance.Prototype = NewBob;
            bob.Add(BobPhsxTimeship.Instance, NewBob);


	
			// Blobby
			NewBob = new Bob(Path.Combine(Globals.ContentDirectory, "Objects\\Spaceship.smo"), Tools.EffectWad, Tools.TextureWad, BobPhsxSpaceship.Instance, false);
			NewBob.MyObjectType = BobPhsxBlobby.Instance;
			NewBob.PlayerObject.ParentQuad.Scale(new Vector2(3.5f, 3.5f));
			foreach (BaseQuad quad in NewBob.PlayerObject.QuadList)
				quad.MyDrawOrder = ObjectDrawOrder.AfterOutline;
			NewBob.CanHaveCape = false;
			NewBob.CanHaveHat = false;
			NewBob.PlayerObject.ParentQuad.MyEffect = Tools.BasicEffect;

			spaceship = (Quad)NewBob.PlayerObject.QuadList[1];
			spaceship.MyTexture = Tools.Texture("blob_cave");
			spaceship.Resize();
			spaceship.MirrorUV_Horizontal();
			spaceship.Scale(new Vector2(1.25f));
			NewBob.PlayerObject.QuadList[2].Show = false;

			BobPhsxBlobby.Instance.Prototype = NewBob;
			bob.Add(BobPhsxBlobby.Instance, NewBob);



            // Meat
            NewBob = new Bob(Path.Combine(Globals.ContentDirectory, "Objects\\MeatBoy.smo"), Tools.EffectWad, Tools.TextureWad, BobPhsxMeat.Instance, true);
            NewBob.IsSpriteBased = false;
            NewBob.MyObjectType = BobPhsxMeat.Instance;
            NewBob.PlayerObject.ParentQuad.Scale(new Vector2(2.75f, 2.75f));
            foreach (BaseQuad quad in NewBob.PlayerObject.QuadList)
                quad.MyDrawOrder = ObjectDrawOrder.AfterOutline;
            NewBob.CanHaveCape = true;
            NewBob.CanHaveHat = true;
            NewBob.PlayerObject.ParentQuad.MyEffect = Tools.BasicEffect;

            BobPhsxMeat.Instance.Prototype = NewBob;
            bob.Add(BobPhsxMeat.Instance, NewBob);

            
            // Freeplay Heroes
            CustomLevel_GUI.FreeplayHeroes = new List<BobPhsx>(Bob.HeroTypes);

            // Associate the BobPhsx with each prototype
            foreach (BobPhsx HeroType in Bob.HeroTypes)
                Prototypes.bob[HeroType].MyHeroType = HeroType;

            // Place bob
            //PlaceBob = LoadObject(Path.Combine(Globals.ContentDirectory, "Objects\\place_bob.smo"));

            Resources.ResourceLoadedCountRef.MyFloat++;

            ArcadeMenu.StaticInit();
        }
    }
}
﻿using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Drawing;

using CloudberryKingdom.Bobs;
using CloudberryKingdom.Goombas;
using CloudberryKingdom.Spikes;

namespace CloudberryKingdom
{
    public class Prototypes
    {
        public static Balrog balrog;
        public static Goomba flyinggoomba;
        public static Goomba goomba;
        public static Dictionary<BobPhsx, Bob> bob;
        public static Spike spike;
        public static Floater_Core floater;
        public static SimpleObject GhostBlockObj, FlyingBlockObj, Checkpoint, Door, GrassDoor, Arrow;

        public static ObjectClass PlaceBob;

        public static SimpleObject LoadSimple(string file)
        {
            ObjectClass SourceObject;
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
            Prototypes.Checkpoint = Prototypes.LoadSimple(Path.Combine(Globals.ContentDirectory, "Objects\\FlyingCoin.smo"));
            
            Prototypes.Door = Prototypes.LoadSimple(Path.Combine(Globals.ContentDirectory, "Objects\\Door.smo"));
            Prototypes.GrassDoor = Prototypes.LoadSimple(Path.Combine(Globals.ContentDirectory, "Objects\\GrassDoor.smo"));

            Prototypes.Arrow = Prototypes.LoadSimple(Path.Combine(Globals.ContentDirectory, "Objects\\Arrow.smo"));

            Prototypes.GhostBlockObj = Prototypes.LoadSimple(Path.Combine(Globals.ContentDirectory, "Objects\\GhostBlock.smo"));
            Prototypes.FlyingBlockObj = Prototypes.LoadSimple(Path.Combine(Globals.ContentDirectory, "Objects\\FlyingBlock.smo"));

            Prototypes.flyinggoomba = new Goomba(Path.Combine(Globals.ContentDirectory, "Objects\\Blob.smo"), Tools.EffectWad, Tools.TextureWad);
            Prototypes.flyinggoomba.Core.MyType = ObjectType.FlyingBlob;
            Vector2 BlobSize = new Vector2(1.11f, 1.11f);
            Prototypes.flyinggoomba.MyObject.Base.e1 *= BlobSize.X;
            Prototypes.flyinggoomba.MyObject.Base.e2 *= BlobSize.Y;

            Prototypes.spike = new Spike(Path.Combine(Globals.ContentDirectory, "Objects\\regular_spike.smo"), Tools.EffectWad, Tools.TextureWad);

            Prototypes.floater = new Floater_Core(Path.Combine(Globals.ContentDirectory, "Objects\\SpikeGuy.smo"));
            Vector2 SpikeyGuySize = new Vector2(1.55f, 1.55f);
            Prototypes.floater.MyObject.Base.e1 *= SpikeyGuySize.X;
            Prototypes.floater.MyObject.Base.e2 *= SpikeyGuySize.Y;


            // Create all the stickmen hero prototypes
            bob = new Dictionary<BobPhsx, Bob>();
            Bob NewBob;

            // Stickman base object
            NewBob = new Bob(Path.Combine(Globals.ContentDirectory, "Objects\\stickman.smo"), Tools.EffectWad, Tools.TextureWad);
            NewBob.MyObjectType = BobPhsxNormal.Instance;
            NewBob.DrawOutline = true;
            NewBob.CanHaveCape = true;
            NewBob.PlayerObject.ParentQuad.MyEffect = Tools.BasicEffect;

            NewBob.PlayerObject.FindQuad("Hat_FireHead").MyTexture = Fireball.FlameTexture;

            BobPhsxNormal.Instance.Prototype = NewBob;
            bob.Add(BobPhsxNormal.Instance, NewBob);

            // Invert
            BobPhsxInvert.Instance.Prototype = BobPhsxNormal.Instance.Prototype;
            bob.Add(BobPhsxInvert.Instance, NewBob);

            // Braid
            BobPhsxBraid.Instance.Prototype = BobPhsxNormal.Instance.Prototype;
            bob.Add(BobPhsxBraid.Instance, NewBob);

            // Tiny Bob
            BobPhsxSmall.Instance.Prototype = BobPhsxNormal.Instance.Prototype;
            bob.Add(BobPhsxSmall.Instance, NewBob);
            //NewBob = new Bob(Prototypes.bob[BobPhsxNormal.Instance], false);
            //NewBob.MyObjectType = BobPhsxSmall.Instance;
            //NewBob.PlayerObject.ParentQuad.Scale(new Vector2(.6f, .65f));
            //BobPhsxSmall.Instance.Prototype = NewBob;
            //bob.Add(BobPhsxSmall.Instance, NewBob);


            // Big
            BobPhsxBig.Instance.Prototype = BobPhsxNormal.Instance.Prototype;
            bob.Add(BobPhsxBig.Instance, NewBob);
            //NewBob = new Bob(Prototypes.bob[BobPhsxNormal.Instance], false);
            //NewBob.MyObjectType = BobPhsxBig.Instance;
            //NewBob.PlayerObject.ParentQuad.Scale(new Vector2(1.7f, 1.4f));
            //BobPhsxBig.Instance.Prototype = NewBob;
            //bob.Add(BobPhsxBig.Instance, NewBob);

            // Phase
            BobPhsxScale.Instance.Prototype = NewBob;
            bob.Add(BobPhsxScale.Instance, NewBob);


            // Double Jump
            BobPhsxDouble.Instance.Prototype = NewBob;
            bob.Add(BobPhsxDouble.Instance, NewBob);

            //NewBob = new Bob(Prototypes.bob[BobPhsxNormal.Instance], false);
            //NewBob.MyObjectType = BobPhsxDouble.Instance;
            ////NewBob.PlayerObject.FindQuad("Hand_Left").Show = false;
            ////NewBob.PlayerObject.FindQuad("Hand_Right").Show = false;
            //NewBob.PlayerObject.FindQuad("Arm_Right").Show = false;
            //NewBob.PlayerObject.FindQuad("Arm_Left").Show = false;
            //NewBob.PlayerObject.FindQuad("Leg_Left").Show = false;
            //NewBob.PlayerObject.FindQuad("Leg_Right").Show = false;
            //NewBob.PlayerObject.FindQuad("Foot_Left").Show = true;
            //NewBob.PlayerObject.FindQuad("Foot_Right").Show = true;
            //NewBob.PlayerObject.FindQuad("Body").Show = false;
            //BobPhsxDouble.Instance.Prototype = NewBob;
            //bob.Add(BobPhsxDouble.Instance, NewBob);

            // Rocket
            BobPhsxJetman.Instance.Prototype = NewBob;
            bob.Add(BobPhsxJetman.Instance, NewBob);

            //NewBob = new Bob(Prototypes.bob[BobPhsxNormal.Instance], false);
            //NewBob.MyObjectType = BobPhsxJetman.Instance;
            //NewBob.CanHaveCape = false;
            //NewBob.PlayerObject.FindQuad("Rocket").Show = true;
            //BobPhsxJetman.Instance.Prototype = NewBob;
            //bob.Add(BobPhsxJetman.Instance, NewBob);

            // Bouncy
            NewBob = new Bob(BobPhsxNormal.Instance, false);
            NewBob.MyObjectType = BobPhsxBouncy.Instance;
            NewBob.CanHaveCape = true;
            NewBob.PlayerObject.FindQuad("Bouncy").Show = true;

            BobPhsxBouncy.Instance.Prototype = NewBob;
            bob.Add(BobPhsxBouncy.Instance, NewBob);

            // Wheelie
            NewBob = new Bob(BobPhsxNormal.Instance, false);
            NewBob.MyObjectType = BobPhsxWheel.Instance;
            NewBob.CanHaveCape = false;
            NewBob.PlayerObject.FindQuad("Wheel").Show = true;

            BobPhsxWheel.Instance.Prototype = NewBob;
            bob.Add(BobPhsxWheel.Instance, NewBob);

            // Hero in a Box
            NewBob = new Bob(BobPhsxNormal.Instance, false);
            NewBob.MyObjectType = BobPhsxBox.Instance;
            NewBob.PlayerObject.FindQuad("Box_Back").Show = true;
            NewBob.PlayerObject.FindQuad("Box_Front").Show = true;
            NewBob.PlayerObject.FindQuad("Leg_Left").Show = false;
            NewBob.PlayerObject.FindQuad("Leg_Right").Show = false;

            BobPhsxBox.Instance.Prototype = NewBob;
            bob.Add(BobPhsxBox.Instance, NewBob);

            // Rocketbox
            NewBob = new Bob(BobPhsxBox.Instance, false);
            NewBob.MyObjectType = BobPhsxBox.Instance;
            NewBob.PlayerObject.FindQuad("Wheel_Left").Show = true;
            NewBob.PlayerObject.FindQuad("Wheel_Right").Show = true;

            BobPhsxRocketbox.Instance.Prototype = NewBob;
            bob.Add(BobPhsxRocketbox.Instance, NewBob);

            // Spaceship 
            NewBob = new Bob(Path.Combine(Globals.ContentDirectory, "Objects\\Spaceship.smo"), Tools.EffectWad, Tools.TextureWad, BobPhsxSpaceship.Instance, false);
            NewBob.MyObjectType = BobPhsxSpaceship.Instance;
            NewBob.PlayerObject.ParentQuad.Scale(new Vector2(3.5f, 3.5f));
            NewBob.DrawOutline = true;
            foreach (BaseQuad quad in NewBob.PlayerObject.QuadList)
                quad.MyDrawOrder = ObjectDrawOrder.AfterOutline;
            NewBob.CanHaveCape = false;
            NewBob.CanHaveHat = false;
            NewBob.PlayerObject.ParentQuad.MyEffect = Tools.BasicEffect;

            BobPhsxSpaceship.Instance.Prototype = NewBob;
            bob.Add(BobPhsxSpaceship.Instance, NewBob);


            // Meat
            NewBob = new Bob(Path.Combine(Globals.ContentDirectory, "Objects\\MeatBoy.smo"), Tools.EffectWad, Tools.TextureWad, BobPhsxMeat.Instance, true);
            NewBob.MyObjectType = BobPhsxMeat.Instance;
            NewBob.PlayerObject.ParentQuad.Scale(new Vector2(2.75f, 2.75f));
            NewBob.DrawOutline = true;
            foreach (BaseQuad quad in NewBob.PlayerObject.QuadList)
                quad.MyDrawOrder = ObjectDrawOrder.AfterOutline;
            NewBob.CanHaveCape = true;
            NewBob.CanHaveHat = true;
            NewBob.PlayerObject.ParentQuad.MyEffect = Tools.BasicEffect;
            NewBob.PlayerObject.QuadList[0].SetTexture("FallingBlock1", Tools.TextureWad);
            NewBob.PlayerObject.QuadList[1].SetTexture("FallingBlock1", Tools.TextureWad);

            BobPhsxMeat.Instance.Prototype = NewBob;
            bob.Add(BobPhsxMeat.Instance, NewBob);



            // Mario
#if DEBUG
            bool Mario = true;
#else
            bool Mario = false;
#endif
            if (Mario)
            {
                NewBob = new Bob(Path.Combine(Globals.ContentDirectory, "Objects\\SpriteHeroTemplate.smo"), Tools.EffectWad, Tools.TextureWad, BobPhsxSpaceship.Instance, true);
                NewBob.MyObjectType = BobPhsxMario.Instance;
                NewBob.PlayerObject.ParentQuad.Scale(new Vector2(1.8f, 1.7f));
                NewBob.MyCapeType = Cape.CapeType.None;
                NewBob.DrawOutline = true;
                foreach (BaseQuad quad in NewBob.PlayerObject.QuadList)
                    quad.MyDrawOrder = ObjectDrawOrder.AfterOutline;
                NewBob.CanHaveCape = true;
                NewBob.CanHaveHat = true;
                NewBob.PlayerObject.ParentQuad.MyEffect = Tools.BasicEffect;

                var p = NewBob.PlayerObject;
                var q = p.FindQuad("Head");
                q.TextureAnim = new AnimationData_Texture();
                EzTexture i_stand_1 = Tools.Texture("Mario_Run_3"),
                          i_run_1 = Tools.Texture("Mario_Run_1"),
                          i_run_2 = Tools.Texture("Mario_Run_2"),
                          i_run_3 = Tools.Texture("Mario_Run_3"),
                          i_jump_1 = Tools.Texture("Mario_Jump_1"),
                          i_fall_1 = Tools.Texture("Mario_Fall_1"),
                          i_turn_1 = Tools.Texture("Mario_Turn_1"),
                          i_duck_1 = Tools.Texture("Mario_Duck_1");

                //var Q = q as Quad;
                //Q.MirrorUV_Horizontal();

                q.TextureAnim.Anims = new OneAnim_Texture[20];
                q.TextureAnim.AddFrame(i_stand_1, 0);
                q.TextureAnim.AddFrame(i_run_1, 1);
                q.TextureAnim.AddFrame(i_run_2, 1);
                q.TextureAnim.AddFrame(i_run_3, 1);
                q.TextureAnim.AddFrame(i_run_3, 1);
                q.TextureAnim.AddFrame(i_jump_1, 2);
                q.TextureAnim.AddFrame(i_fall_1, 3);
                q.TextureAnim.AddFrame(i_duck_1, 4);
                q.TextureAnim.AddFrame(i_turn_1, 5);
                p.Read(0, 0);
                p.Record(5, 0, true);
                p.Record(5, 1, true);

                BobPhsxMario.Instance.Prototype = NewBob;
                bob.Add(BobPhsxMario.Instance, NewBob);
            }

            // Associate the BobPhsx with each prototype
            foreach (BobPhsx HeroType in Bob.HeroTypes)
                Prototypes.bob[HeroType].MyHeroType = HeroType;

            // Place bob
            PlaceBob = LoadObject(Path.Combine(Globals.ContentDirectory, "Objects\\place_bob.smo"));

            Tools.TheGame.ResourceLoadedCountRef.MyFloat++;
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using CloudberryKingdom.Levels;
using Drawing;

namespace CloudberryKingdom
{
    public class BackgroundNode : BackgroundFloater
    {
        public string MapName = "";
        public int Number = 0;

        public Background MyBackground;

        public int Guid_N; public BackgroundNode Connect_N { get { if (Guid_N <= 0) return null; if (MyBackground.NodeLookup.ContainsKey(Guid_N)) return MyBackground.NodeLookup[Guid_N]; else { Guid_N = -1; return null; } } }
        public int Guid_E; public BackgroundNode Connect_E { get { if (Guid_E <= 0) return null; if (MyBackground.NodeLookup.ContainsKey(Guid_E)) return MyBackground.NodeLookup[Guid_E]; else { Guid_E = -1; return null; } } }
        public int Guid_S; public BackgroundNode Connect_S { get { if (Guid_S <= 0) return null; if (MyBackground.NodeLookup.ContainsKey(Guid_S)) return MyBackground.NodeLookup[Guid_S]; else { Guid_S = -1; return null; } } }
        public int Guid_W; public BackgroundNode Connect_W { get { if (Guid_W <= 0) return null; if (MyBackground.NodeLookup.ContainsKey(Guid_W)) return MyBackground.NodeLookup[Guid_W]; else { Guid_W = -1; return null; } } }

        public int Guid;

        public override void Release()
        {
 	        base.Release();
            MyBackground.NodeLookup.Remove(Guid);
            MyBackground = null;
        }

        public override void SetBackground(Background b)
        {
            MyBackground = b;
            MyBackground.NodeLookup.AddOrOverwrite(Guid, this);
        }

        public BackgroundNode()
            : base()
        {
        }

        public BackgroundNode(Level level)
            : base(level)
        {
        }

        public BackgroundNode(Level level, string Root)
            : base(level, Root)
        {
        }

        public override void PhsxStep(BackgroundFloaterList list)
        {
            // Warning: We shouldn't ever need to use this method, it should just be data to be loaded from the save file.
            Tools.Warning();
            //SetAsArcadeNode();
            SetAsWorldNode();

            base.PhsxStep(list);
        }

        public EzTexture PathTexture_Horizontal, PathTexture_Vertical;
        public void SetAsArcadeNode()
        {
            PathTexture_Horizontal = "Path_Horizontal_Arcade";
            PathTexture_Vertical = "Path_Vertical_Arcade";
            MyQuad.TextureName = "Node_Arcade" + (Number > 0 ? "_" + Number.ToString() : "");
            MyQuad.ScaleYToMatchRatio(270);
        }

        public void SetAsWorldNode()
        {
            PathTexture_Horizontal = "WorldMap_Bridge_Horizontal";
            PathTexture_Vertical = "WorldMap_Bridge_Vertical";

            //PathTexture_Horizontal = "Path_Horizontal_Arcade";
            //PathTexture_Vertical = "Path_Vertical_Arcade";
            
            //MyQuad.TextureName = "WorldMap_Castle_" + (Number > 0 ? "_" + Number.ToString() : "");
            //MyQuad.ScaleYToMatchRatio(270);
        }

        public override void Draw()
        {
            base.Draw();
        }

        static string[] _bits_to_save = new string[] { "Name", "MyQuad", "uv_speed", "uv_offset", "Data", "StartData",
                                                        "Guid", "Guid_N", "Guid_E", "Guid_S", "Guid_W", "MapName", "Number" };
        public override string[] GetViewables() { return _bits_to_save; }
    }
}
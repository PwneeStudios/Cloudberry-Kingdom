using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CloudberryKingdom
{
    partial class CloudberryKingdomGame
    {
        public static void PreloadArt()
        {
            _PreloadArt();
            //_PreloadArt_Preprocess();
            //PreprocessArt();
        }

        static void _PreloadArt()
        {
            String path = Path.Combine(Globals.ContentDirectory, "Art");
            string[] files = Tools.GetFiles(path, true);

            foreach (String file in files)
            {
                if (Tools.GetFileExt(path, file) == "xnb")
                {
                    Tools.TextureWad.AddTexture(null, "Art\\" + Tools.GetFileName(path, file));
                }
            }
        }

        public static void PreprocessArt()
        {
            //String path = Path.Combine(Globals.ContentDirectory, "Art");
            String path = "C://Users//Ezra//Desktop//Dir//Pwnee//CK//Cloudberry Kingdom//Cloudberry Kingdom//Content//";

            string[] files = Tools.GetFiles(path, true);

            Tools.Write("");
            Tools.Write("");

            foreach (String file in files)
            {
                //if (Tools.GetFileExt(path, file) == "xnb")
                if (Tools.GetFileExt(path, file) == "png")
                {
                    var t = Tools.TextureWad.AddTexture(null, "Art\\" + Tools.GetFileName(path, file));
                    //t.Load();

                    string FilePath = t.Path;
                    string StrippedName = Tools.StripPath(FilePath);
                    string LowerName = FilePath.ToLower();
                    string LowerPath = FilePath.ToLower();
                    string BigName = Tools.GetFileBigName(FilePath).ToLower();
                    string Folder = Tools.FirstFolder(FilePath, "Art\\");

                    //Tools.Write("Tools.TextureWad.AddTexture_Fast(null, \"{0}\", {1}, {2}, {3}, {4}, {5}, {6}, {7});", Tools.ToCode(FilePath), t.Width, t.Height,
                    //    Tools.ToCode(StrippedName), Tools.ToCode(LowerName), Tools.ToCode(LowerPath), Tools.ToCode(BigName), Tools.ToCode(Folder));

                    var c_path = FilePath.Replace("\\", "/");
                    Tools.Write(c_path + ".png");
                }
            }

            Tools.Write("");
            Tools.Write("");
        }

        static void _PreloadArt_Preprocess()
        {
            var w = Tools.TextureWad;
        }
    }
}
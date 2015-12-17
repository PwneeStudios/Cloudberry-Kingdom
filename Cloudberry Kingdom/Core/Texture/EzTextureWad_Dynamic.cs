using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using CloudberryKingdom;

namespace CoreEngine
{
    public partial class EzTextureWad
    {
#if WINDOWS
        public void KillDynamic()
        {
            foreach (EzTexture texture in TextureList)
                if (texture.Dynamic)
                    texture.Tex.Dispose();

            TextureList.RemoveAll(t => t.Dynamic);
            NameDict.RemoveAll(kp => kp.Value.Dynamic);
        }

        public enum WhatToLoad { Art, Backgrounds, Animations, Tilesets };
        public void LoadModRoot(Stream stream, WhatToLoad what)
        {
            Tools.UseInvariantCulture();
            StreamReader reader = new StreamReader(stream);

            String line;

            line = reader.ReadLine();
            while (line != null)
            {
                var bits = Tools.GetBitsFromLine(line);

                if (bits.Count > 0)
                {
                    var first = bits[0];

                    switch (first)
                    {
                        case "Load":
                            LoadFolder(bits[1], what);
                            break;

                        case "TileSetToTest":
                            CloudberryKingdomGame.TileSetToTest = bits[1];
                            break;

                        default: break;
                    }
                }

                line = reader.ReadLine();
            }

            reader.Close();
            stream.Close();
        }

        /// <summary>
        /// Dynamically load a folder (assuming it is inside DynamicLoad).
        /// </summary>
        public void LoadFolder(string path, WhatToLoad what)
        {
            path = Path.Combine(Globals.ContentDirectory, Path.Combine("DynamicLoad", path));

            string[] files = Tools.GetFiles(path, true);

            foreach (String file in files)
            {
                string extension = Tools.GetFileExt(path, file);

                // Load sprites
                if (what == WhatToLoad.Art)
                    if (extension == "png" || extension == "jpg" || extension == "jpeg" || extension == "bmp")
                    {
                        string filename = Tools.GetFileName(path, file);
                        Stream stream = File.Open(file, FileMode.Open);
                        var tex = Texture2D.FromStream(Tools.Device, stream);
                        stream.Close();

                        //if (file.Contains("600")) Tools.Write("");
                        ConvertToPreMultipliedAlpha(tex);

                        var texture = Tools.TextureWad.AddTexture(tex, filename);
                        texture.Dynamic = true;
                    }

                // Load tileset
                if (what == WhatToLoad.Tilesets)
                    if (extension == "tileset")
                        TileSets.LoadTileSet(file);

                // Load background
                if (what == WhatToLoad.Backgrounds)
                    if (extension == "bkg")
                        BackgroundType.Load(file);

                // Load animation
                if (what == WhatToLoad.Animations)
                    if (extension == "anim")
                        Prototypes.LoadAnimation(file);
            }
        }

        public static Texture2D ConvertToPreMultipliedAlpha(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(data, 0, data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Color(new Vector4(data[i].ToVector3() * (data[i].A / 255f), (data[i].A / 255f)));
            }
            texture.SetData<Color>(data, 0, data.Length);

            return texture;
        }

        public void LoadAllDynamic(ContentManager Content, WhatToLoad what)
        {
            String path = Path.Combine(Globals.ContentDirectory, "DynamicLoad");

            // Find top level files
            string[] files = Tools.GetFiles(path, false);

            // Process files that are .modroots
            foreach (String file in files)
            {
                // Only load the specified mod root
                var name = Tools.GetFileName(path, file);
                if (string.Compare(name, CloudberryKingdomGame.ModRoot, true) != 0) continue;

                string extension = Tools.GetFileExt(path, file);
                if (extension != "modroot") continue;

                // Load the modroot
                Stream stream = File.Open(file, FileMode.Open);
                LoadModRoot(stream, what);
                stream.Close();
            }
        }
#endif
    }
}
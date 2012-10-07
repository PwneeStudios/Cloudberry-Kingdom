using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CloudberryKingdom;

namespace CoreEngine
{
    public class EzTextureWad
    {
        public TextureOrAnim FindTextureOrAnim(string name)
        {
            if (name == "null") return null;

            var t_or_a = new TextureOrAnim();
            t_or_a.Set(name);
            return t_or_a;
        }

        /// <summary>
        /// The texture returned when a texture isn't found in the wad.
        /// </summary>
        public EzTexture DefaultTexture;

        public List<EzTexture> TextureList;
        public Dictionary<string, List<EzTexture>> TextureListByFolder;

        public Dictionary<string, AnimationData_Texture> AnimationDict;
        public void Add(AnimationData_Texture anim, string name)
        {
            AnimationDict.AddOrOverwrite(name, anim);
        }


        public Dictionary<string, EzTexture> PathDict, NameDict, BigNameDict;
        public void Add(PackedTexture packed)
        {
            foreach (PackedTexture.SubTexture sub in packed.SubTextures)
            {
                EzTexture texture = FindByName(sub.name);
                texture.FromPacked = true;
                texture.TR = sub.TR;
                texture.BL = sub.BL;
                texture.Packed = packed.MyTexture;
            }
        }


        public Dictionary<string, PackedTexture> PackedDict = new Dictionary<string, PackedTexture>();

        public EzTextureWad()
        {
            const int Size = 2000;

            TextureList = new List<EzTexture>(Size);
            TextureListByFolder = new Dictionary<string, List<EzTexture>>(Size);

            AnimationDict = new Dictionary<string, AnimationData_Texture>(Size, StringComparer.CurrentCultureIgnoreCase);

            PathDict = new Dictionary<string, EzTexture>(Size, StringComparer.CurrentCultureIgnoreCase);
            NameDict = new Dictionary<string, EzTexture>(Size, StringComparer.CurrentCultureIgnoreCase);
            BigNameDict = new Dictionary<string, EzTexture>(Size, StringComparer.CurrentCultureIgnoreCase);
        }

        public void LoadFolder(ContentManager Content, string Folder)
        {
            foreach (EzTexture Tex in TextureListByFolder[Folder])
            {
                // If texture hasn't been loaded yet, load it
                if (Tex.Tex == null && !Tex.FromCode)
                {
                    Tex.Tex = Content.Load<Texture2D>(Tex.Path);
                    Tex.Width = Tex.Tex.Width;
                    Tex.Height = Tex.Tex.Height;

#if EDITOR
#else
                    Tools.TheGame.ResourceLoadedCountRef.Val++;
#endif
                }
            }
        }

        public void KillDynamic()
        {
            foreach (EzTexture texture in TextureList)
                if (texture.Dynamic)
                    texture.Tex.Dispose();

            TextureList.RemoveAll(t => t.Dynamic);
            BigNameDict.RemoveAll(kp => kp.Value.Dynamic);
            NameDict.RemoveAll(kp => kp.Value.Dynamic);
            //PackedDict.RemoveAll(kp => kp.Value.Dynamic);
            PathDict.RemoveAll(kp => kp.Value.Dynamic);
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

        public EzTexture FindOrLoad(ContentManager Content, string name)
        {
            EzTexture texture = FindByName(name);

            if (texture != Tools.TextureWad.DefaultTexture) return texture;

            return Tools.TextureWad.AddTexture(Content.Load<Texture2D>(name), name);
        }

        /// <summary>
        /// Accepts a path. If the path does not exist, the name is used instead.
        /// </summary>
        public EzTexture FindByPathOrName(string path)
        {
            // Look for the texture with the full path
            var PathTexture = FindByName(path);

            // If nothing but white was found
            if (PathTexture == null || PathTexture == TextureList[0])
            {
                // Get the name from the path
                int i = path.LastIndexOf("\\");
                
                // If the name is the path, return what we found
                if (i <= 0) return PathTexture;

                // Otherwise find the name and return the result
                string name = path.Substring(i + 1);
                return FindByName(name);
            }
            else
                return PathTexture;
        }

        public EzTexture FindByName(string name)
        {
            EzTexture texture = _FindByName(name);

            if (texture == null) return null;

            if (texture.Tex == null)
            {
                texture.Load();
            }

            return texture;
        }
        public EzTexture _FindByName(string name)
        {
            if (name == null)
                return DefaultTexture;

            return Find(name);
        }

        public EzTexture Find(string name)
        {
            if (name.Contains("\\") && BigNameDict.ContainsKey(name))
                return BigNameDict[name];
            else if (PathDict.ContainsKey(name))
                return PathDict[name];
            else if (NameDict.ContainsKey(name))
                return NameDict[name];
            
            return DefaultTexture;
        }

        public void AddEzTexture(EzTexture NewTex)
        {
            TextureList.Add(NewTex);

            string name = NewTex.Name.ToLower();
            if (!NameDict.ContainsKey(name))
                NameDict.AddOrOverwrite(name, NewTex);

            if (NewTex.Path != null)
            {
                PathDict.AddOrOverwrite(NewTex.Path.ToLower(), NewTex);
                BigNameDict.AddOrOverwrite(Tools.GetFileBigName(NewTex.Path).ToLower(), NewTex);
            }
        }

        public EzTexture AddTexture(Texture2D Tex, string Name)
        {
            if (Tex == null)
                return AddTexture(Tex, Name, 0, 0);
            else
                return AddTexture(Tex, Name, Tex.Width, Tex.Height);
        }
        public EzTexture AddTexture(Texture2D Tex, string Name, int Width, int Height)
        {
            EzTexture NewTex = null;

            if (TextureList.Exists(match => string.Compare(match.Path, Name, StringComparison.OrdinalIgnoreCase) == 0))
            {
                NewTex = FindByName(Name);

                // Override pre-existing texture?
                if (Tex != null)
                {
                    // Get rid of old texture if it was dynamic.
                    if (NewTex.Dynamic && NewTex.Tex != null && !NewTex.Tex.IsDisposed)
                        NewTex.Tex.Dispose();

                    NewTex.Tex = Tex;
                }
            }
            else
            {
                NewTex = new EzTexture();
                NewTex.Path = Name;
                NewTex.Tex = Tex;

                NewTex.Name = Tools.StripPath(Name);

                TextureList.Add(NewTex);

                string name = NewTex.Name.ToLower();
                if (!NameDict.ContainsKey(name))
                    NameDict.AddOrOverwrite(name, NewTex);
                PathDict.AddOrOverwrite(NewTex.Path.ToLower(), NewTex);

                BigNameDict.AddOrOverwrite(Tools.GetFileBigName(NewTex.Path).ToLower(), NewTex);

                // Add to folder
                string folder = Tools.FirstFolder(Name, "Art\\");
                if (!TextureListByFolder.ContainsKey(folder))
                    TextureListByFolder.Add(folder, new List<EzTexture>());
                TextureListByFolder[folder].Add(NewTex);
            }

            NewTex.Width = Width;
            NewTex.Height = Height;

            return NewTex;
        }

        public EzTexture AddTexture_Fast(Texture2D Tex, string Name, int Width, int Height)
        {
            return null;
        }

        public EzTexture AddTexture_Fast(Texture2D Tex, string Name, int Width, int Height,
                                         string StrippedName, string LowerName, string LowerPath, string BigName, string Folder)
        {
            EzTexture NewTex = null;

            NewTex = new EzTexture();
            NewTex.Path = Name;
            NewTex.Tex = Tex;

            NewTex.Name = StrippedName;

            TextureList.Add(NewTex);

            NameDict.Add(LowerName, NewTex);
            PathDict.Add(LowerPath, NewTex);

            BigNameDict.Add(BigName, NewTex);

            // Add to folder
            if (!TextureListByFolder.ContainsKey(Folder))
                TextureListByFolder.Add(Folder, new List<EzTexture>());
            TextureListByFolder[Folder].Add(NewTex);

            NewTex.Width = Width;
            NewTex.Height = Height;

            return NewTex;
        }
    }
}
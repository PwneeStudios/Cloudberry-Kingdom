using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CloudberryKingdom;

namespace Drawing
{
    public class EzTexture
    {
        //public Texture2D Tex;
        Texture2D _Tex;
        public Texture2D Tex 
        {
            get
            {
                //if (_Tex == null)
                //    Load();
                return _Tex;
            }
            set { _Tex = value; }
        }

        public string Path, Name;

        /// <summary>
        /// If true this texture is a sub-image from a packed collection.
        /// </summary>
        public bool FromPacked = false;

        public EzTexture Packed;

        /// <summary>
        /// If true this texture was loaded dynamically after the game loaded, not from a packed XNA file.
        /// </summary>
        public bool Dynamic = false;

        /// <summary>
        /// Texture coordintes within a bigger packed collection
        /// </summary>
        public Vector2 BL, TR;

#if EDITOR
        public static Game game;
        public bool Load()
        {
            if (Tex == null && Path != null)
                Tex = game.Content.Load<Texture2D>(Path);

            return Tex != null;
        }
#else
        public bool Load()
        {
            if (_Tex == null && Path != null)
                _Tex = Tools.TheGame.Content.Load<Texture2D>(Path);

            return _Tex != null;
        }
#endif
        //public EzTexture() { }
    }

    public class PackedTexture
    {
        public EzTexture MyTexture;

        public struct SubTexture
        {
            public string name;
            public Vector2 BL, TR;
        }
        public List<SubTexture> SubTextures = new List<SubTexture>();
        public PackedTexture(string name)
        {
            MyTexture = Tools.TextureWad.FindByName(name);
        }

        public void Add(string name, float x1, float y1, float x2, float y2)
        {
            SubTexture sub;
            sub.name = name;
            sub.BL = new Vector2(x1, y2);
            sub.TR = new Vector2(x2, y1);

            SubTextures.Add(sub);
        }
    }

    public class EzTextureWad
    {
        /// <summary>
        /// The texture returned when a texture isn't found in the wad.
        /// </summary>
        public EzTexture DefaultTexture;

        public List<EzTexture> TextureList;
        public LockableBool AllLoaded;

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
            AllLoaded = new LockableBool();
            TextureList = new List<EzTexture>();

            PathDict = new Dictionary<string, EzTexture>(StringComparer.CurrentCultureIgnoreCase);
            NameDict = new Dictionary<string, EzTexture>(StringComparer.CurrentCultureIgnoreCase);
            BigNameDict = new Dictionary<string, EzTexture>(StringComparer.CurrentCultureIgnoreCase);
        }

        /*
        public void _LoadAll_frac(ContentManager Content, int m)
        {
            EzTexture Tex;
            for (int i = 0; i < TextureList.Count; i++)
            {
                if (i % 3 == m)
                {
                    {
                        Tex = TextureList[i];

                        // If texture hasn't been loaded yet, load it
                        if (Tex.Tex == null)
                        {
                            AllLoaded.val = false;
                            Tex.Tex = Content.Load<Texture2D>(Tex.Path);

                            {
                                Tools.TheGame.ResourceLoadedCountRef.Val++;
                            }
                        }
                    }
                }
            }

            AllLoaded.val = true;
        }
        public Thread[] threads = new Thread[3];
        public void LoadAll_frac(int m, int thread)
        {
            ContentManager manager = new ContentManager(Tools.TheGame.Content.ServiceProvider, Tools.TheGame.Content.RootDirectory);

            Thread LoadThread = threads[m] = new Thread(
                new ThreadStart(
                    delegate
                    {
#if XBOX
                        Thread.CurrentThread.SetProcessorAffinity(new[] { thread });
#endif
                        LoadThread = Thread.CurrentThread;
                        EventHandler<EventArgs> abort = (s, e) =>
                        {
                            if (LoadThread != null)
                            {
                                LoadThread.Abort();
                            }
                        };
                        Tools.TheGame.Exiting += abort;

                        _LoadAll_frac(manager, m);

                        Tools.TheGame.Exiting -= abort;
                    }))
            {
                Name = "LoadArtThread",
#if WINDOWS
                Priority = ThreadPriority.Highest,
#endif
            };

            LoadThread.Start();
        }

        public Thread LoadThread;
        public void LoadAll(ContentManager Content)
        {
            LoadThread = new Thread(
                new ThreadStart(
                    delegate
                    {
#if XBOX
                        Thread.CurrentThread.SetProcessorAffinity(new[] { 5 });
#endif
                        LoadThread = Thread.CurrentThread;
                        EventHandler<EventArgs> abort = (s, e) =>
                        {
                            if (LoadThread != null)
                            {
                                LoadThread.Abort();
                            }
                        };
                        Tools.TheGame.Exiting += abort;

                        _LoadAll(Content);

                        Tools.TheGame.Exiting -= abort;
                    }))
            {
                Name = "LoadArtThread",
#if WINDOWS
                Priority = ThreadPriority.Highest,
#endif
            };

            LoadThread.Start();
        }

        public void _LoadAll(ContentManager Content)
        {
            EzTexture Tex;
            for (int i = 0; i < TextureList.Count; i++)
            {
                int j = i + 20;
                lock (Content)
                //lock (TextureList[i])
                {
                    for (; i < TextureList.Count && i < j; i++)
                    {
                        Tex = TextureList[i];

                        // If texture hasn't been loaded yet, load it
                        if (Tex.Tex == null)
                        {
                            AllLoaded.val = false;
                            Tex.Tex = Content.Load<Texture2D>(Tex.Path);

                            lock (Tools.TheGame.ResourceLoadedCountRef)
                            {
                                Tools.TheGame.ResourceLoadedCountRef.Val++;
                            }
                        }
                    }

                    Thread.Sleep(10);
                }
            }

            AllLoaded.val = true;
        }
        */
        public static float PercentToLoad =
                    1f;
                    //.2f;
        public void LoadAllDirect(ContentManager Content)
        {
            EzTexture Tex;
            int n = (int)(TextureList.Count * PercentToLoad);
            for (int i = 0; i < n; i++)
            {
                Tex = TextureList[i];

                // If texture hasn't been loaded yet, load it
                if (Tex.Tex == null)
                {
                    AllLoaded.val = false;
                    Tex.Tex = Content.Load<Texture2D>(Tex.Path);

#if EDITOR
#else
                    Tools.TheGame.ResourceLoadedCountRef.Val++;
#endif
                }
            }

            if (PercentToLoad == 1f)
                AllLoaded.val = true;
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

        public enum WhatToLoad { Art, Tilesets };
        public void LoadModRoot(Stream stream, WhatToLoad what)
        {
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
                string extension = Tools.GetFileExt(path, file);
                if (extension == "modroot")
                {
                    Stream stream = File.Open(file, FileMode.Open);

                    LoadModRoot(stream, what);

                    stream.Close();
                }
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

            if (AllLoaded.val) return texture;
            else
            {
                //lock (texture)
                //lock (Tools.TheGame.Content)
                {
                    if (texture.Tex == null)
                    {
                        texture.Load();
                    }
                }
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

        public EzTexture AddTexture(Texture2D Tex, string Name)
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
                //int LastSlash = Name.LastIndexOf("\\");
                //if (LastSlash < 0)
                //    NewTex.Name = Name;
                //else
                //    NewTex.Name = Name.Substring(LastSlash + 1);

                //lock (AllLoaded)
                {
                    TextureList.Add(NewTex);

                    string name = NewTex.Name.ToLower();
                    if (!NameDict.ContainsKey(name))
                        NameDict.Add(name, NewTex);
                    PathDict.Add(NewTex.Path.ToLower(), NewTex);

                    BigNameDict.Add(Tools.GetFileBigName(NewTex.Path).ToLower(), NewTex);
                }
            }

            return NewTex;
        }
    }
}
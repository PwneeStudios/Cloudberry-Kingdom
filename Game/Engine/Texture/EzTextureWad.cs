using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using CloudberryKingdom;

namespace CoreEngine
{
    public partial class CoreTextureWad
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
        public CoreTexture DefaultTexture;

        public List<CoreTexture> TextureList;
        public Dictionary<string, List<CoreTexture>> TextureListByFolder;

        public Dictionary<string, AnimationData_Texture> AnimationDict;
        public void Add(AnimationData_Texture anim, string name)
        {
            AnimationDict.AddOrOverwrite(name, anim);
        }


        public Dictionary<string, CoreTexture> NameDict;
        public void Add(PackedTexture packed)
        {
            foreach (PackedTexture.SubTexture sub in packed.SubTextures)
            {
                CoreTexture texture = FindByName(sub.name);
                texture.FromPacked = true;
                texture.TR = sub.TR;
                texture.BL = sub.BL;
                texture.Packed = packed.MyTexture;
            }
        }


        public Dictionary<string, PackedTexture> PackedDict = new Dictionary<string, PackedTexture>();

        public CoreTextureWad()
        {
            const int Size = 2000;

            TextureList = new List<CoreTexture>(Size);
            TextureListByFolder = new Dictionary<string, List<CoreTexture>>(Size);

            AnimationDict = new Dictionary<string, AnimationData_Texture>(Size, StringComparer.CurrentCultureIgnoreCase);

            NameDict = new Dictionary<string, CoreTexture>(Size, StringComparer.CurrentCultureIgnoreCase);
        }

//        public void LoadFolder(ContentManager Content, string Folder)
//        {
//            foreach (CoreTexture Tex in TextureListByFolder[Folder])
//            {
//                // If texture hasn't been loaded yet, load it
//                if (Tex.Tex == null && !Tex.FromCode)
//                {
//                    Tex.Tex = Tools.Transparent.Tex;
//                    //Tex.Tex = TextureList[0].Tex;

//                    //Tex.Tex = Content.Load<Texture2D>(Tex.Path);
//                    //Tex.Width = Tex.Tex.Width;
//                    //Tex.Height = Tex.Tex.Height;

//#if EDITOR
//#else
//                    Resources.ResourceLoadedCountRef.Val++;
//#endif
//                }
//            }
//        }

		//public void LoadFolder_Real(ContentManager Content, string Folder)
		//{
		//    Texture2D transparent = Tools.Transparent.Tex;

		//    foreach (CoreTexture Tex in TextureListByFolder[Folder])
		//    {
		//        // If texture hasn't been loaded yet, load it
		//        if ((Tex.Tex == null || Tex.Tex == transparent) && !Tex.FromCode)
		//        {
		//            Tex.Tex = Content.Load<Texture2D>(Tex.Path);

		//            Thread.Sleep(100);
		//        }
		//    }
		//}

        public CoreTexture FindOrLoad(ContentManager Content, string name, string path)
        {
            CoreTexture texture = FindByName(name);

            if (texture != Tools.TextureWad.DefaultTexture) return texture;

			return Tools.TextureWad.AddTexture(Content.LoadTillSuccess<Texture2D>(path), name);
        }

        /// <summary>
        /// Accepts a path. If the path does not exist, the name is used instead.
        /// </summary>
        public CoreTexture FindByPathOrName(string path)
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

        public CoreTexture FindByName(string name)
        {
            CoreTexture texture = _FindByName(name);

            if (texture == null) return null;

            if (texture.Tex == null)
            {
                if (CloudberryKingdomGame.LoadResources)
                {
                    texture.Load();
                }
                else
                {
                    texture.Tex = DefaultTexture.Tex;
                }
            }

            return texture;
        }
        public CoreTexture _FindByName(string name)
        {
            if (name == null)
                return DefaultTexture;

            return Find(name);
        }

        public CoreTexture Find(string name)
        {
			//if (name.Contains("\\") && BigNameDict.ContainsKey(name))
			//    return BigNameDict[name];
			//else if (PathDict.ContainsKey(name))
			//    return PathDict[name];
            //else
			if (NameDict.ContainsKey(name))
                return NameDict[name];
            
            return DefaultTexture;
        }

        public void AddCoreTexture(CoreTexture NewTex)
        {
            TextureList.Add(NewTex);

            string name = NewTex.Name.ToLower(CultureInfo.InvariantCulture);
            if (!NameDict.ContainsKey(name))
                NameDict.AddOrOverwrite(name, NewTex);
        }

        public CoreTexture AddTexture(Texture2D Tex, string Name)
        {
            if (Tex == null)
                return AddTexture(Tex, Name, 0, 0);
            else
                return AddTexture(Tex, Name, Tex.Width, Tex.Height);
        }
        public CoreTexture AddTexture(Texture2D Tex, string Name, int Width, int Height)
        {
            CoreTexture NewTex = null;

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
                NewTex = new CoreTexture();
                NewTex.Path = Name;
                NewTex.Tex = Tex;

                NewTex.Name = Tools.StripPath(Name);

                TextureList.Add(NewTex);

                string name = NewTex.Name.ToLower(CultureInfo.InvariantCulture);
                if (!NameDict.ContainsKey(name))
                    NameDict.AddOrOverwrite(name, NewTex);

                // Add to folder
                string folder = Tools.FirstFolder(Name, "Art\\");
                if (!TextureListByFolder.ContainsKey(folder))
                    TextureListByFolder.Add(folder, new List<CoreTexture>());
                TextureListByFolder[folder].Add(NewTex);
            }

            NewTex.Width = Width;
            NewTex.Height = Height;

            return NewTex;
        }

        public CoreTexture AddTexture_Fast(Texture2D Tex, string Name, int Width, int Height)
        {
            return null;
        }

        public CoreTexture AddTexture_Fast(Texture2D Tex, string Name, int Width, int Height,
                                         string StrippedName, string LowerName, string Folder)
        {
            CoreTexture NewTex = null;

            NewTex = new CoreTexture();
            NewTex.Path = Name;
            NewTex.Tex = Tex;

            NewTex.Name = StrippedName;

            TextureList.Add(NewTex);

            NameDict.Add(LowerName, NewTex);

            // Add to folder
            if (!TextureListByFolder.ContainsKey(Folder))
                TextureListByFolder.Add(Folder, new List<CoreTexture>());
            TextureListByFolder[Folder].Add(NewTex);

            NewTex.Width = Width;
            NewTex.Height = Height;

            return NewTex;
        }
    }
}
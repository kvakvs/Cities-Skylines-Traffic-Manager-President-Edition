namespace TrafficManager.UI.Textures {
    using CSUtil.Commons;
    using System.IO;
    using System.Reflection;
    using System;
    using ColossalFramework.UI;
    using TrafficManager.State.ConfigData;
    using UnityEngine;
    using Debug = System.Diagnostics.Debug;

    public static class TextureResources {
        public static readonly Texture2D MainMenuButtonTexture2D;
        public static readonly Texture2D MainMenuButtonsTexture2D;
        public static readonly Texture2D NoImageTexture2D;
        public static readonly Texture2D RemoveButtonTexture2D;
        public static readonly Texture2D WindowBackgroundTexture2D;

        static TextureResources() {
            // missing image
            NoImageTexture2D = LoadDllResource("noimage.png", 64, 64);

            // main menu icon
            MainMenuButtonTexture2D = LoadDllResource("MenuButton.png", 300, 50);
            MainMenuButtonTexture2D.name = "TMPE_MainMenuButtonIcon";

            // main menu buttons
            MainMenuButtonsTexture2D = LoadDllResource("mainmenu-btns.png", 960, 30);
            MainMenuButtonsTexture2D.name = "TMPE_MainMenuButtons";

            RemoveButtonTexture2D = LoadDllResource("remove-btn.png", 150, 30);

            WindowBackgroundTexture2D = LoadDllResource("WindowBackground.png", 16, 60);
        }

        internal static Texture2D LoadDllResource(string resourceName, int width, int height)
        {
#if DEBUG
            bool debug = DebugSwitch.JunctionRestrictions.Get();
#endif
            try {
#if DEBUG
                if (debug) {
                    Log._Debug($"Loading DllResource {resourceName}");
                }
#endif
                Assembly myAssembly = Assembly.GetExecutingAssembly();
                Stream myStream = myAssembly.GetManifestResourceStream("TrafficManager.Resources."
                                                                       + resourceName);

                Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);

                texture.LoadImage(ReadToEnd(myStream));

                return texture;
            } catch (Exception e) {
                Log.Error(e.StackTrace.ToString());
                return null;
            }
        }

        static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead != readBuffer.Length) {
                        continue;
                    }

                    int nextByte = stream.ReadByte();
                    if (nextByte == -1) {
                        continue;
                    }

                    byte[] temp = new byte[readBuffer.Length * 2];
                    Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                    Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                    readBuffer = temp;
                    totalBytesRead++;
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length == totalBytesRead) {
                    return buffer;
                }

                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                return buffer;
            }
            catch (Exception e) {
                Log.Error(e.StackTrace.ToString());
                return null;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// From a list of sprite names create UI texture atlas for CO.UI.
        /// </summary>
        /// <param name="atlasName">Atlas will have a name.</param>
        /// <param name="spriteNames">List of texture names.</param>
        /// <param name="assemblyPath">Path to the texture directory using dots as separators.</param>
        /// <returns>New atlas.</returns>
        public static UITextureAtlas CreateTextureAtlas(string atlasName,
                                                        string[] spriteNames,
                                                        string assemblyPath)
        {
            int maxSize = 1024;
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            Texture2D[] textures = new Texture2D[spriteNames.Length];
            Rect[] regions = new Rect[spriteNames.Length];

            for (int i = 0; i < spriteNames.Length; i++) {
                textures[i] = LoadTextureFromAssembly(assemblyPath + spriteNames[i] + ".png");
            }

            regions = texture2D.PackTextures(textures, 2, maxSize);

            UITextureAtlas textureAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            Material material = UnityEngine.Object.Instantiate<Material>(UIView.GetAView().defaultAtlas.material);
            material.mainTexture = texture2D;
            textureAtlas.material = material;
            textureAtlas.name = atlasName;

            for (int i = 0; i < spriteNames.Length; i++)
            {
                UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo
                {
                    name = spriteNames[i],
                    texture = textures[i],
                    region = regions[i],
                };

                textureAtlas.AddSprite(item);
            }

            return textureAtlas;
        }

        public static void AddTexturesInAtlas(UITextureAtlas atlas,
                                              Texture2D[] newTextures,
                                              bool locked = false) {
            Texture2D[] textures = new Texture2D[atlas.count + newTextures.Length];

            for (int i = 0; i < atlas.count; i++) {
                Texture2D texture2D = atlas.sprites[i].texture;

                if (locked) {
                    // Locked textures workaround
                    RenderTexture renderTexture = RenderTexture.GetTemporary(
                        texture2D.width,
                        texture2D.height,
                        0);
                    Graphics.Blit(texture2D, renderTexture);

                    RenderTexture active = RenderTexture.active;
                    texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                    RenderTexture.active = renderTexture;
                    texture2D.ReadPixels(
                        new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height),
                        0,
                        0);
                    texture2D.Apply();
                    RenderTexture.active = active;

                    RenderTexture.ReleaseTemporary(renderTexture);
                }

                textures[i] = texture2D;
                textures[i].name = atlas.sprites[i].name;
            }

            for (int i = 0; i < newTextures.Length; i++) {
                textures[atlas.count + i] = newTextures[i];
            }

            Rect[] regions = atlas.texture.PackTextures(textures, atlas.padding, 4096, false);

            atlas.sprites.Clear();

            for (int i = 0; i < textures.Length; i++) {
                UITextureAtlas.SpriteInfo spriteInfo = atlas[textures[i].name];
                atlas.sprites.Add(
                    new UITextureAtlas.SpriteInfo {
                        texture = textures[i],
                        name = textures[i].name,
                        border = (spriteInfo != null) ? spriteInfo.border : new RectOffset(),
                        region = regions[i],
                    });
            }

            atlas.RebuildIndexes();
        }

        /// <summary>Retrieve an atlas by name.</summary>
        /// <param name="name">Name.</param>
        /// <returns>The atlas which has been found or a default one.</returns>
        public static UITextureAtlas GetAtlas(string name)
        {
            UITextureAtlas[] atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
            Debug.Assert(atlases != null, nameof(atlases) + " != null");

            foreach (UITextureAtlas t in atlases) {
                if (t.name == name) {
                    return t;
                }
            }

            return UIView.GetAView().defaultAtlas;
        }

        private static Texture2D LoadTextureFromAssembly(string path)
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);

            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, array.Length);

            Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            texture2D.LoadImage(array);

            return texture2D;
        }

        // public static Texture2D ConvertRenderTexture(RenderTexture renderTexture)
        // {
        //     RenderTexture active = RenderTexture.active;
        //     Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
        //     RenderTexture.active = renderTexture;
        //     texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
        //     texture2D.Apply();
        //     RenderTexture.active = active;
        //
        //     return texture2D;
        // }

        // public static void ResizeTexture(Texture2D texture, int width, int height)
        // {
        //     RenderTexture active = RenderTexture.active;
        //
        //     texture.filterMode = FilterMode.Trilinear;
        //     RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);
        //     renderTexture.filterMode = FilterMode.Trilinear;
        //
        //     RenderTexture.active = renderTexture;
        //     Graphics.Blit(texture, renderTexture);
        //     texture.Resize(width, height);
        //     texture.ReadPixels(new Rect(0, 0, width, width), 0, 0);
        //     texture.Apply();
        //
        //     RenderTexture.active = active;
        //     RenderTexture.ReleaseTemporary(renderTexture);
        // }

        // public static void CopyTexture(Texture2D texture2D, Texture2D dest)
        // {
        //     RenderTexture renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0);
        //     Graphics.Blit(texture2D, renderTexture);
        //
        //     RenderTexture active = RenderTexture.active;
        //     RenderTexture.active = renderTexture;
        //     dest.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
        //     dest.Apply();
        //     RenderTexture.active = active;
        //
        //     RenderTexture.ReleaseTemporary(renderTexture);
        // }
    } // end class
}
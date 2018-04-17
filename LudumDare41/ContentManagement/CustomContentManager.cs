using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LudumDare41.ContentManagement
{
    public class CustomContentManager
    {
        Dictionary<string, SoundEffect> soundEffects;
        Dictionary<string, Song> songs;
        Dictionary<string, Texture2D> textures;
        Dictionary<string, TrueTypeFontLoader> spriteFonts;

        public CustomContentManager(GraphicsDevice graphics, string contentBasePath)
        {
            //soundEffects = new Dictionary<string, SoundEffect>();
            songs = new Dictionary<string, Song>();
            textures = new Dictionary<string, Texture2D>();

            soundEffects = load<SoundEffect>(Path.Combine(contentBasePath, "sfx"), loader =>
            {
                loader.Map(".wav", n => SoundEffect.FromStream(n.GetStream()));
                loader.Map(".ogg", n => OggSoundEffectLoader.LoadFromStream(n.GetStream()));
            });

            textures = load<Texture2D>(Path.Combine(contentBasePath, "gfx"), loader =>
            {
                loader.Map(".png", n => {
                    var tex = Texture2D.FromStream(graphics, n.GetStream());

                    // pre-multiply the alpha
                    var tmp = new Color[tex.Width * tex.Height];
                    tex.GetData(tmp);

                    Parallel.For(0, tmp.Length, p =>
                    {
                        Color color = tmp[p];
                        tmp[p] = Color.FromNonPremultiplied(color.R, color.G, color.B, color.A);
                    });
                    tex.SetData(tmp);

                    return tex;
                });
            });

            songs = load<Song>(Path.Combine(contentBasePath, "songs"), loader =>
            {
                loader.Map(".ogg", n => Song.FromUri(n.Key, new Uri(n.FilePath, UriKind.Relative)));
            });

            spriteFonts = load<TrueTypeFontLoader>(Path.Combine(contentBasePath, "fonts"), loader =>
            {
                loader.Map(".ttf", n => new TrueTypeFontLoader(graphics, n.FilePath));
            });
        }

        public ContentReference<T> Get<T>(string name) where T : class
        {
            if (typeof(T) == typeof(SoundEffect))
            {
                if (soundEffects.TryGetValue(name, out var sfx))
                {
                    return new ContentReference<T> { Content = sfx as T };
                }

                return null;
            }
            else if (typeof(T) == typeof(Texture2D))
            {
                if (textures.TryGetValue(name, out var tex))
                {
                    return new ContentReference<T> { Content = tex as T };
                }

                return null;
            }
            else if (typeof(T) == typeof(Song))
            {
                if (songs.TryGetValue(name, out var song))
                {
                    return new ContentReference<T> { Content = song as T };
                }

                return null;
            }
            else if (typeof(T) == typeof(TrueTypeFontLoader))
            {
                if (spriteFonts.TryGetValue(name, out var font))
                {
                    return new ContentReference<T> { Content = font as T };
                }

                return null;
            }
            else
            {
                return null;
            }
        }
            

        private Dictionary<string, T> load<T>(string path, Action<LoadContext<T>> context) where T : class
        {
            var dictionary = new Dictionary<string, T>();

            var loadContext = new LoadContext<T>();
            context(loadContext);

            Console.WriteLine($"Loading {typeof(T).FullName}...");

            foreach (var file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    var key = file.Substring(path.Length + 1).ToLowerInvariant().Replace('\\', '/');
                    key = key.Substring(0, key.Length - 4);
                    

                    Console.WriteLine($"\tLoading {key}...");
                    var obj = loadContext.Load(file, key);

                    if (obj != null)
                    {
                        dictionary.Add(key, obj);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\t\tError loading {file} of type {typeof(T).FullName}: {ex}");
                }
            }

            return dictionary;
        }

        private class LoadContext<T> where T : class
        {
            Dictionary<string, Func<LoadArgs, T>> loaders;

            public LoadContext()
            {
                loaders = new Dictionary<string, Func<LoadArgs, T>>();
            }

            public void Map(string ext, Func<LoadArgs, T> loader)
            {
                loaders.Add(ext, loader);
            }

            public T Load(string fileName, string key)
            {
                var ext = Path.GetExtension(fileName);

                if (loaders.TryGetValue(ext, out var value))
                {
                    using (var args = new LoadArgs
                    {
                        FilePath = fileName,
                        Key = key
                    })
                    {
                        return value(args);
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        
        internal class LoadArgs : IDisposable
        {
            private Stream stream;

            public string FilePath { get; set; }
            public string Key { get; set; }
            
            public void Dispose()
            {
                stream?.Dispose();
            }

            public Stream GetStream()
            {
                return stream = File.OpenRead(FilePath);
            }
        }
    }

    public class ContentReference<T>
    {
        public T Content { get; set; }
    }
}
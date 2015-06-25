using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;


namespace LittleFlame.Functions
{
    /// <summary>
    /// This class can be used to save all content you load through the ContentManager. Just save all the assets through this class and load them when you need
    /// the assets.
    /// The positive thing by save all your assets at the start is that you only have to load your assets once. Though this won't work if you have
    /// alot of huge assets or just an huge amount of assets. In that case it's better to load those through the ContentManager yourself and unload those
    /// assets once you're done with them.
    /// Currently supported assets:
    /// - Texture2D
    /// - TextureCube
    /// - Song
    /// - SoundEffect
    /// - Model
    /// - Effect
    /// - SpriteFont
    /// </summary>
    public class AssetHolder
    {
        private Dictionary<string, Texture2D> textures;
        private Dictionary<string, TextureCube> texcubes;
        private Dictionary<string, Song> songs;
        private Dictionary<string, SoundEffect> soundEffects;
        private Dictionary<string, Model> models;
        private Dictionary<string, Effect> effects;
        private Dictionary<string, SpriteFont> fonts;
        private ContentManager content;

        /// <summary>
        /// Create an object which will save the assets you want to use throughout the game.
        /// </summary>
        /// <param name="content">The ContentManager which will be used to save the assets into this object.</param>
        public AssetHolder(ContentManager content)
        {
            this.textures = new Dictionary<string, Texture2D>();
            this.texcubes = new Dictionary<string, TextureCube>();
            this.soundEffects = new Dictionary<string, SoundEffect>();
            this.songs = new Dictionary<string, Song>();
            this.models = new Dictionary<string, Model>();
            this.effects = new Dictionary<string, Effect>();
            this.fonts = new Dictionary<string, SpriteFont>();
            this.content = content;
        }

        /// <summary>
        /// Save an key into this object's memory.
        /// Note: You can save multiple assets with the same key if the assets are different types (say a Texture2D and a SoundEffect).
        /// Warning: this method does not check if a value is already saved.
        /// </summary>
        /// <typeparam name="T">The type of key you want to load.</typeparam>
        /// <param name="key">The key you want to use to load the key later on.</param>
        /// <param name="assetLocation">The path to the key in the ContentProject.</param>
        public void SaveAsset<T>(string key, string assetLocation)
        {
            //Save the typeof(T) into a variable so you don't have to reuse typeof() all the time.
            Type type = typeof(T);
            //Load the right key using the ContentManager and save it to the corrosponding dictionary.
            if (type == typeof(Texture2D)) {
                this.textures.Add(key, content.Load<Texture2D>(assetLocation));
            } else if (type == typeof(Song)) {
                this.songs.Add(key, content.Load<Song>(assetLocation));
            } else if (type == typeof(SoundEffect)) {
                this.soundEffects.Add(key, content.Load<SoundEffect>(assetLocation));
            } else if (type == typeof(Model)) {
                this.models.Add(key, content.Load<Model>(assetLocation));
            } else if (type == typeof(Effect)) {
                this.effects.Add(key, content.Load<Effect>(assetLocation));
            } else if (type == typeof(TextureCube)) {
                this.texcubes.Add(key, content.Load<TextureCube>(assetLocation));
            } else if (type == typeof(SpriteFont)) {
                this.fonts.Add(key, content.Load<SpriteFont>(assetLocation));
            } else {
                //This exceptions means that you have to implement a specific type to this class.
                throw new NotImplementedException("The type " + typeof(T) + " is not yet supported.");
            }
        }

        /// <summary>
        /// Load a saved key from this object's momery. Do not use this method in an update loop. It could kill your game's performance.
        /// </summary>
        /// <typeparam name="T">The type of key you want to load.</typeparam>
        /// <param name="key">The which is used to save the key with.</param>
        /// <returns></returns>
        public T Load<T>(string key)
        {
            //Save the typeof(T) into a variable so you don't have to reuse typeof() all the time.
            Type type = typeof(T);
            //To return the right value you have to cast the key to an object and cast that object to (T).
            if (type == typeof(Texture2D) && this.textures.ContainsKey(key)) {
                return (T)((object)(this.textures[key]));
            } else if (type == typeof(Song) && this.songs.ContainsKey(key)) {
                return (T)((object)(this.songs[key]));
            } else if (type == typeof(SoundEffect) && this.soundEffects.ContainsKey(key)) {
                return (T)((object)(this.soundEffects[key]));
            } else if (type == typeof(Model) && this.models.ContainsKey(key)) {
                return (T)((object)(this.models[key]));
            } else if (type == typeof(Effect) && this.effects.ContainsKey(key)) {
                return (T)((object)(this.effects[key]));
            } else if (type == typeof(TextureCube) && this.texcubes.ContainsKey(key)) {
                return (T)((object)(this.texcubes[key]));
            } else if (type == typeof(SpriteFont) && this.fonts.ContainsKey(key)) {
                return (T)((object)(this.fonts[key]));
            } else {
                //Throw an ArgumentNullException if the key is not used with the given type.
                throw new ArgumentNullException("key", "There is no " + typeof(T) + " with the key you're using.");
            }
        }

        /// <summary>
        /// Check if this memory already uses a certain key and if the key is a certain type.
        /// Example: (the arrow brackets are replaced by the [] brackets because of XML code issues)
        /// if you use ContainsKey[Texture2D]("key"); then this method checks all Texture2Ds for a Texture2D with the argument's key.
        /// </summary>
        /// <typeparam name="T">The type of key you want to check.</typeparam>
        /// <param name="key">The key which might be used to save an key.</param>
        /// <returns>True means that a spot is taken by a certain type.</returns>
        public bool ContainsKey<T>(string key)
        {
            Type type = typeof(T);
            if (type == typeof(Texture2D)) {
                return this.textures.ContainsKey(key);
            } else if (type == typeof(Song)) {
                return this.songs.ContainsKey(key);
            } else if (type == typeof(SoundEffect)) {
                return this.soundEffects.ContainsKey(key);
            } else if (type == typeof(Model)) {
                return this.models.ContainsKey(key);
            } else if (type == typeof(Effect)) {
                return this.effects.ContainsKey(key);
            } else if (type == typeof(TextureCube)) {
                return this.texcubes.ContainsKey(key);
            } else if (type == typeof(SpriteFont)) {
                return this.fonts.ContainsKey(key);
            } else {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LittleFlame.States
{
    public class State
    {
        // Game
        private Game1 game;
        protected Game1 Game
        {
            get { return game; }
            set { game = value; }
        }
        
        // Graphics
        SpriteBatch spriteBatch;
        protected SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        // Effect
        Effect effect;
        protected Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        protected GameComponent[] componentList;
        
        public State(Game1 _game)
        {
            this.game = _game;

            spriteBatch = new SpriteBatch(Game.Graphics.GraphicsDevice);
            //effect = Game.Content.Load<Effect>("Effects\\Effect");
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void LoadContent()
        {
            
        }

        public virtual void UnloadContent()
        {
            if (Game.Components.Count != 0)
            {
                componentList = new GameComponent[Game.Components.Count];
                for (int i = Game.Components.Count - 1; i >= 0; i--)
                    componentList[i] = (GameComponent)Game.Components.ElementAt(i);
                Game.Components.Clear();
            }
        }

        public virtual void ReloadContent()
        {
            if (componentList != null)
                for (int i = 0; i < componentList.Length; i++)
                    if (componentList[i] != null && !(componentList[i] is GamerServicesComponent))
                        Game.Components.Add(componentList[i]);
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }
    }
}

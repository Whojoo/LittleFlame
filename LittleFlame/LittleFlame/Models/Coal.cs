using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace LittleFlame.Models
{
    class Coal : LFModel
    {
        private Terrain terrain;
        
        public Coal(Game game, Model model, Vector3 position, Vector3 rotation, Vector3 scale)
            : base(game, model, position, rotation, scale)
        {
            rangeDistance = 2.5f;
            terrain = (Terrain)Game.Services.GetService(typeof(Terrain));
        }
    }
}

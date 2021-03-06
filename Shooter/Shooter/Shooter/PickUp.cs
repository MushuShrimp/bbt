﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    class PickUp
    {

        public Texture2D Texture;

        public Vector2 Position;

        public bool Active;

        public enum pickUpType { TYPE1, TYPE2, TYPE3, TYPE4, TYPE5, TYPE6, NONE };
        /*
         * TYPE1 = projectile size up
         * TYPE2 = enemy projectile size down
         * TYPE3 = player move speed up
         * TYPE4 = enemy move speed down
         * TYPE5 = player projectile speed up
         * TYPE6 = enemy projectile speed down
         * */
        public pickUpType PickUpType;
        Viewport viewport;
        public int Width
        {
            get
            {
                return Texture.Width;
            }
        }

        public int Height
        {
            get
            {
                return Texture.Height;
            }
        }

        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position, pickUpType pickuptype)
        {
            PickUpType = pickuptype;

            Active = true;
            this.viewport = viewport;
            Texture = texture;

            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f,
            new Vector2(Width / 2, Height / 2), 0.05f, SpriteEffects.None, 0f);
        }
    }
}

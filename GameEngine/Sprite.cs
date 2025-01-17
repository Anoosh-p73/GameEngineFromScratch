using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public class Sprite
    {
        // PROPERTIES //

        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; }
        public Single Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects Effect { get; set; }
        public Single Layer{ get; set; }

        public Sprite(Texture2D texture)
        {
            Texture = texture;
            Position = new Vector2(0, 0);
            Source = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Color = Color.White;
            Rotation = 0;
            Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            Scale = new Vector2(1, 1);
            Effect = SpriteEffects.None;
            Layer = 1;
        }

        // UPDATE AND DRAW //
        public virtual void Update() { }
        public virtual void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Texture, Position, Source, Color, Rotation, Origin, Scale, Effect, Layer);
        }


    }
}

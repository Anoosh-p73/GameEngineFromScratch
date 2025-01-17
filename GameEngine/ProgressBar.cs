using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class ProgressBar : Sprite
    {
        public ProgressBar(Texture2D texture, Color fillColor) : base(texture)
        {
            FillColor = fillColor;
        }

        public Color FillColor { get; set; }
        public float Value { get; set; } // Value between 0 and 1

        private int WidthMaker() {
            if (Value >= 0 && Value <= 1) {
                return (int) (Texture.Width * Value);
            }
            return 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch); // let the sprite do its work
            spriteBatch.Draw(Texture, 
                Position, 
                new Rectangle(0, 0, WidthMaker(), Texture.Height), 
                FillColor, 
                Rotation, 
                Origin, 
                Scale, 
                Effect, 
                Layer);
        }


    }
}

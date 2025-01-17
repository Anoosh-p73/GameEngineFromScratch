using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CPI311.GameEngine
{
    public class AnimatedSprite : Sprite
    {
        public int Frames { get; set; }
        public float Frame { get; set; }
        public float Speed { get; set; }
        public int Layer { get; set; }
        public bool Idle { get; set; }

        public AnimatedSprite(Texture2D texture, int frames = 1): base(texture)
        {
            Frames = frames;
            Frame = 0;
            Speed = 1;
        }

        public override void Update()
        {
            if (!Idle) {
                Frame += Speed * Time.ElapsedGameTime;
                Source = new Rectangle(((int)Frame * 32) % (Frames * 32), Layer * 32, 32, 32);
            }
        }


    }
}

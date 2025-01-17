using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPI311.Labs
{
    public class Lab1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SpriteFont font;

        Fraction a = new Fraction(2, 5);
        Fraction b = new Fraction(2, 5);
        Fraction c;

        public Lab1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, a + " * " + b + " = " + (a * b), new Vector2(100, 50), Color.Black);
            _spriteBatch.DrawString(font, a + " + " + b + " = " + (a + b), new Vector2(100, 100), Color.Black);
            _spriteBatch.DrawString(font, a + " - " + b + " = " + (a - b), new Vector2(100, 150), Color.Black);
            _spriteBatch.DrawString(font, a + " / " + b + " = " + (a / b), new Vector2(100, 200), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab02
{
    public class Lab02 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Sprite sprite;
        SpiralMover SpiralMover;
        

        public Lab02()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // sprite = new Sprite(Content.Load<Texture2D>("Square"));

            SpiralMover = new SpiralMover(Content.Load<Texture2D>("Square"), Vector2.Zero);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            SpiralMover.Update();

            /* KeyboardState currentState = Keyboard.GetState();
             * if (InputManager.IsKeyPressed(Keys.Right)) {
                   sprite.Position += Vector2.UnitX * 5;
               }
               if (InputManager.IsKeyPressed(Keys.Left))
               {
                   sprite.Position += Vector2.UnitX * -5;
               }
               if (InputManager.IsKeyPressed(Keys.Up))
               {
                   sprite.Position += Vector2.UnitY * -5;
               }
               if (InputManager.IsKeyPressed(Keys.Down))
               {
                   sprite.Position += Vector2.UnitY * 5;
               }
               if (InputManager.IsKeyDown(Keys.Space))
                   sprite.Rotation += 0.05f; */

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            //sprite.Draw(_spriteBatch);
            SpiralMover.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
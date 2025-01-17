using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Assignment01
{
    public class Assignment01 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private AnimatedSprite player;
        private float playerSpeed = 1;
        private int dir = 1; // 0 = UP, 1 = DOWN, 2 = LEFT, 3 = RIGHT

        private ProgressBar timeBar;
        private ProgressBar distBar;

        private float distMult = 0.005f;

        private bool sanityCheck = false;

        public Assignment01()
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

            player = new AnimatedSprite(Content.Load<Texture2D>("explorer"), 8);
            timeBar = new ProgressBar(Content.Load<Texture2D>("Square"), Color.Red);
            distBar = new ProgressBar(Content.Load<Texture2D>("Square"), Color.Green);

            player.Position = new Vector2(200, 200);
            timeBar.Position = new Vector2(100, 50);
            timeBar.Scale = new Vector2(5, 1);
            timeBar.Value = 1;
            
            distBar.Position = new Vector2(400, 50);
            distBar.Scale = new Vector2(5, 1);
            distBar.Value = 0;

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            Move();
            TimerBar();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            player.Draw(_spriteBatch);
            timeBar.Draw(_spriteBatch);
            distBar.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Move() {
            player.Idle = true;

            if (InputManager.IsKeyDown(Keys.Right))
            {
                player.Position += Vector2.UnitX * playerSpeed;
                dir = 3;
                player.Idle = false;
                distBar.Value += distMult;
            }
            if (InputManager.IsKeyDown(Keys.Left))
            {
                player.Position += Vector2.UnitX * -playerSpeed;
                dir = 2;
                player.Idle = false;
                distBar.Value += distMult;
            }
            if (InputManager.IsKeyDown(Keys.Up))
            {
                player.Position += Vector2.UnitY * -playerSpeed;
                dir = 0;
                player.Idle = false;
                distBar.Value += distMult;
            }
            if (InputManager.IsKeyDown(Keys.Down))
            {
                player.Position += Vector2.UnitY * playerSpeed;
                dir = 1;
                player.Idle = false;
                distBar.Value += distMult;
            }

            if (!sanityCheck)
            {
                player.Idle = false;
                sanityCheck = true;
            }

            player.Layer = dir;
            player.Update();
        }
        private void TimerBar() {
            timeBar.Value -= (Time.ElapsedGameTime * 0.1f);
        }

    }
}
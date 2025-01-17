using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lab03
{
    public class Lab03 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Model model;
        private SpriteFont font;

        Matrix world;
        Matrix view;
        Matrix projection;

        Vector3 cameraPos = new Vector3(0, 0, 5);
        Vector3 modelPos = new Vector3(0, 0, 0);

        float yaw = 0f;
        float pitch = 0f;
        float roll = 0f;

        float scale = 1f;

        bool orderToggle = false;
        bool orthoPOVToggle = false;

        float left = 3f;
        float right = -3f;
        float bottom = 1.25f;
        float top = -1f;

        public Lab03()
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

            model = Content.Load<Model>("Torus");
            font = Content.Load<SpriteFont>("Font");

            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            // CAMERA CONTROL //

            if (InputManager.IsKeyDown(Keys.W)) 
                cameraPos += Vector3.Up * Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.S)) 
                cameraPos += Vector3.Down * Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.A)) 
                cameraPos += Vector3.Left * Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.D)) 
                cameraPos += Vector3.Right * Time.ElapsedGameTime * 5;

            // MODEL CONTROL //

            if (InputManager.IsKeyDown(Keys.Up))
                modelPos += Vector3.Up * Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.Down))
                modelPos += Vector3.Down * Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.Left))
                modelPos += Vector3.Left * Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.Right))
                modelPos += Vector3.Right * Time.ElapsedGameTime * 5;
            
            if (InputManager.IsKeyDown(Keys.Insert))
                yaw +=  Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.Delete))
                yaw -= Time.ElapsedGameTime * 5;

            if (InputManager.IsKeyDown(Keys.Home))
                pitch += Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.End))
                pitch -= Time.ElapsedGameTime * 5;

            if (InputManager.IsKeyDown(Keys.PageUp))
                roll += Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.PageDown))
                roll -= Time.ElapsedGameTime * 5;

            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Up))
                scale += Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.Down))
                scale -= Time.ElapsedGameTime * 5;

            // TOGGLES //

            if (InputManager.IsKeyPressed(Keys.Space))
                orderToggle = !orderToggle;

            if (InputManager.IsKeyPressed(Keys.Tab))
                orthoPOVToggle = !orthoPOVToggle;

            // PROJ CONTROLS //

            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.W))
                top += Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.S))
                top -= Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.A))
                left += Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.LeftShift) && InputManager.IsKeyDown(Keys.D))
                left -=  Time.ElapsedGameTime * 5;

            if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.W))
                bottom -= Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.S))
                bottom += Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.A))
                right -= Time.ElapsedGameTime * 5;
            if (InputManager.IsKeyDown(Keys.LeftControl) && InputManager.IsKeyDown(Keys.D))
                right += Time.ElapsedGameTime * 5;

            if (!orderToggle)
                world = Matrix.CreateScale(scale) * Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateTranslation(modelPos);
            else
                world = Matrix.CreateTranslation(modelPos) * Matrix.CreateFromYawPitchRoll(yaw, pitch, roll) * Matrix.CreateScale(scale);

            view = Matrix.CreateLookAt(
                cameraPos,
                new Vector3(0, 0, -1),
                new Vector3(0, 1, 0));

            if (!orthoPOVToggle)
                /*projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver2,
                    1.33f,
                    0.1f,
                    1000f
                    );*/
                projection = Matrix.CreatePerspectiveOffCenter(
                    left,
                    right,
                    bottom,
                    top,
                    0.1f,
                    1000f);
            else
                projection = Matrix.CreateOrthographicOffCenter(
                left,
                right,
                bottom,
                top,
                0.1f,
                1000f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            model.Draw(world, view, projection);

            _spriteBatch.Begin();

            _spriteBatch.DrawString(font, "Arrow keys to move the object (x- and y- directions only, ignore the z- direction)", new Vector2(10, 10), Color.Black);
            _spriteBatch.DrawString(font, "Insert/Delete, Home/End, PageUp/PageDown for yaw, pitch, roll, respectively", new Vector2(10, 25), Color.Black);
            _spriteBatch.DrawString(font, "Shift + Up/Down to increase/decrease the scale of the model", new Vector2(10, 40), Color.Black);
            _spriteBatch.DrawString(font, "Tab to toggle order of world matrix", new Vector2(10, 55), Color.Black);
            _spriteBatch.DrawString(font, "Use WASD keys to move the camera", new Vector2(10, 70), Color.Black);
            _spriteBatch.DrawString(font, "Use the Tab key to toggle the Orthographic and Perspective mode", new Vector2(10, 85), Color.Black);
            _spriteBatch.DrawString(font, "Shift + WASD to move the center (so, the width/height don't change)", new Vector2(10, 100), Color.Black);
            _spriteBatch.DrawString(font, "Ctrl + WASD keys to change the width/height", new Vector2(10, 115), Color.Black);

            // _spriteBatch.DrawString(font, $"{left}, {right}, {bottom}, {top}", new Vector2(10, 130), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

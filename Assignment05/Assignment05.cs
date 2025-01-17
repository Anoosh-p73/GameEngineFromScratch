using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;

namespace Assignment05
{
    public class Assignment05 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        TerrainRenderer terrainRenderer;
        Effect effect;

        Camera camera;
        Light light;

        Player player;
        Agent[] agents = new Agent[3];

        SpriteFont font;

        int collisions = 0;

        public Assignment05()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            ScreenManager.Initialize(_graphics);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");

            terrainRenderer = new TerrainRenderer(Content.Load<Texture2D>("mazeH2"), Vector2.One * 100, Vector2.One * 200);
            terrainRenderer.NormalMap = Content.Load<Texture2D>("mazeN2");
            float height = terrainRenderer.GetHeight(Vector2.One * 0.5f);
            terrainRenderer.Transform = new Transform();
            terrainRenderer.Transform.LocalScale *= new Vector3(1, 5, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.3f, 0.3f, 0.3f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Up * 60;
            camera.Transform.Rotate(Vector3.Left, MathHelper.PiOver2);
            
            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Backward * 5 + Vector3.Right * 5 + Vector3.Up * 5; ;

            player = new Player(terrainRenderer, Content, camera, GraphicsDevice, light);
            for (int i = 0; i < 3; i++) {
                agents[i] = new Agent(terrainRenderer, Content, camera, GraphicsDevice, light);
            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Update(gameTime);
            InputManager.Update();

            if (InputManager.IsKeyDown(Keys.Up)) camera.Transform.Rotate(Vector3.Right, Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Down)) camera.Transform.Rotate(Vector3.Left, Time.ElapsedGameTime);

            player.Update();
            foreach (Agent agent in agents) agent.Update();

            foreach (Agent agent in agents)
            {
                Vector3 temp;
                if (agent.Collider.Collides(player.Collider, out temp)) { 
                    agent.CollidedWithPlayer();
                    collisions++;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, $"Elapsed Time: {Time.TotalGameTime.Seconds}", Vector2.One * 50, Color.Black);
            _spriteBatch.DrawString(font, $"Aliens caught: {collisions}", new Vector2(50, 80), Color.Black);
            _spriteBatch.End();

            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["World"].SetValue(terrainRenderer.Transform.World);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
            effect.Parameters["NormalMap"].SetValue(terrainRenderer.NormalMap);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                terrainRenderer.Draw();
                player.Draw();
                foreach (Agent agent in agents) agent.Draw();
            }
            base.Draw(gameTime);
        }
    }
}
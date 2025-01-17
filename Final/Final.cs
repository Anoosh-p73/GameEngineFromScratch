using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading;

namespace Final
{
    public class Final : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        TerrainRenderer Terrain;
        Effect effect;

        Camera camera;
        Light light;

        Player player;
        Agent[] agents = new Agent[3];

        Color background = Color.White;
        Texture2D texture;

        SpriteFont font;

        Dictionary<String, Scene> scenes;
        List<GUIElement> guiElements;
        Scene currentScene;

        List<Coin> coins;

        float Score = 0;
        int HP = 3;
        float totalTime = 2000f;
        bool ended = false;

        bool haveThreadRunning = false;

        public Final()
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

            scenes = new Dictionary<string, Scene>();
            guiElements = new List<GUIElement>();

            coins = new List<Coin>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            texture = Content.Load<Texture2D>("Square");

            Terrain = new TerrainRenderer(Content.Load<Texture2D>("FinalMap"), Vector2.One * 170, Vector2.One * 200);
            Terrain.NormalMap = Content.Load<Texture2D>("FinalNormalMap");
            float height = Terrain.GetHeight(Vector2.One * 0.5f);
            Terrain.Transform = new Transform();
            Terrain.Transform.LocalScale *= new Vector3(1, 5, 1);

            effect = Content.Load<Effect>("TerrainShader");
            effect.Parameters["AmbientColor"].SetValue(new Vector3(0.4f, 0.4f, 0.4f));
            effect.Parameters["DiffuseColor"].SetValue(new Vector3(0.1f, 0.1f, 0.1f));
            effect.Parameters["SpecularColor"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            effect.Parameters["Shininess"].SetValue(20f);

            camera = new Camera();
            camera.Transform = new Transform();
            camera.Transform.LocalPosition = Vector3.Up * 100;
            camera.Transform.Rotate(Vector3.Left, MathHelper.PiOver2);

            light = new Light();
            light.Transform = new Transform();
            light.Transform.LocalPosition = Vector3.Up * 25;

            player = new Player(Terrain, Content, camera, GraphicsDevice, light);
            player.Transform.LocalPosition = new Vector3(-10, 0, -24);
            player.Transform.LocalScale = Vector3.One * 1.5f;

            for (int i = 0; i < agents.Length; i++)
            {
                agents[i] = new Agent(Terrain, Content, camera, GraphicsDevice, light);
                //agents[i].Transform.LocalPosition = new Vector3(-6, 0, -6);
            }

            Button box = new Button();
            box.Texture = texture;
            box.Text = "    Play";
            box.Bounds = new Rectangle(50, 50, 300, 50);
            box.Action += SwitchScenes;
            guiElements.Add(box);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];

            InitializeCoins();

        }

        private void InitializeCoins()
        {
            int gridSize = 32;
            AStarSearch search = new AStarSearch(gridSize, gridSize);
            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;

            int counter = 0;

            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - Terrain.size.X / 2, 0, gridH * j + gridH / 2 - Terrain.size.Y / 2);
                    Random random = new Random();
                    if (Terrain.GetAltitude(pos) == 0)
                    {
                        double rand = random.NextDouble();
                        coins.Add(new Coin(Content, camera, GraphicsDevice, light, rand > 0.95 ? true : false));
                        coins[counter].Transform.LocalScale = 0.5f * Vector3.One;
                        coins[counter].Transform.LocalPosition = pos;
                        if (coins[counter].Special) coins[counter].Transform.LocalScale = Vector3.One * 1.25f;
                        counter++;
                    }
                    // Debug.WriteLine($"For Node {j},{i} the value is {search.Nodes[j, i].Passable}, and the height is {Terrain.GetAltitude(pos)}");
                }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Update(gameTime);
            InputManager.Update();

            currentScene.Update();

            base.Update(gameTime);
        }

        void TeleportAcross() {
            if (player.Transform.LocalPosition.X < -85) 
                player.Transform.LocalPosition = new Vector3(85, 0, -8);
            if (player.Transform.LocalPosition.X > 85)
                player.Transform.LocalPosition = new Vector3(-85, 0, -8);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            currentScene.Draw();

            base.Draw(gameTime);
        }
        void SwitchScenes(GUIElement element)
        {
            currentScene = (currentScene == scenes["Play"] ? scenes["Menu"] : scenes["Play"]);
        }

        void MainMenuUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }
        void MainMenuDraw()
        {
            _spriteBatch.Begin();
            foreach (GUIElement element in guiElements)
                element.Draw(_spriteBatch, font);
            _spriteBatch.DrawString(font, "Controls: " +
                "\n WASD for movement and P to Pause\n" +
                "Information:\n" +
                "You need to avoid the enemies, you have 3 lives and each time you'll respawn at the center\n" +
                "Collect coins to gain points\n" +
                "Larger coins grant immunity for 3s and allows you to reset the enemies incase you are in a sticky situation\n" +
                "Try to get as many points before the timer runs out!", 
                new Vector2(50, 100), Color.Black);
            _spriteBatch.End();
        }
        void PlayUpdate()
        {

            if (HP < 1 || totalTime < 1)
                EndGame();

            if(InputManager.IsKeyReleased(Keys.P))
                currentScene = scenes["Menu"];

            if (!ended) {
                player.Update();
                foreach (Agent agent in agents) agent.Update();
            }

            foreach (Agent agent in agents)
            {
                Vector3 temp;
                if (agent.Collider.Collides(player.Collider, out temp))
                {
                    if (!player.isSpecial)
                    {
                        HP--;
                        player.Transform.LocalPosition = new Vector3(-6, 0, -6);
                    }
                    else {
                        agent.CollidedWithPlayer();
                    }
                }
            }

            foreach (Coin coin in coins) { 
                Vector3 temp;
                if (coin.Collider.Collides(player.Collider, out temp)) {
                    if (!coin.Special)
                    {
                        Score += 10;
                        coin.CollidedWithPlayer();
                        foreach (Agent agent in agents)
                        {
                            if (Score > 200)
                            {
                                int speed = (int)(Score - 200) / 50;
                                agent.speed = 5 + speed;
                            }
                        }
                    }
                    else {
                        Score += 100;
                        coin.CollidedWithPlayer();
                        player.isSpecial = true;
                        haveThreadRunning = true;
                        Debug.WriteLine("Thread Started");
                        ThreadPool.QueueUserWorkItem(new WaitCallback(PoweredUp));
                    }
                }
            }

            if (player.isSpecial)
            {
                player.Transform.LocalScale = Vector3.One * 2.5f;
            }
            else
            {
                player.Transform.LocalScale = Vector3.One * 1.5f;
            }

            TeleportAcross();

            if (totalTime > 0)
                totalTime -= Time.ElapsedGameTime * 20;
            // Score += Time.ElapsedGameTime * 0.01f;
        }
        void PlayDraw()
        {
            effect.Parameters["View"].SetValue(camera.View);
            effect.Parameters["Projection"].SetValue(camera.Projection);
            effect.Parameters["World"].SetValue(Terrain.Transform.World);
            effect.Parameters["CameraPosition"].SetValue(camera.Transform.Position);
            effect.Parameters["LightPosition"].SetValue(light.Transform.Position);
            effect.Parameters["NormalMap"].SetValue(Terrain.NormalMap);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Terrain.Draw();
                player.Draw();
                foreach (Agent agent in agents) agent.Draw();
                foreach (Coin coin in coins) 
                    if (!coin.isCollected)
                        coin.Draw();
            }

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, $"Score: {(int) Score}", new Vector2(50, 20), Color.Black);
            _spriteBatch.DrawString(font, $"Time Left: {(int)totalTime}", new Vector2(50, 80), Color.Black);
            _spriteBatch.DrawString(font, $"Lives Left: {HP}", new Vector2(50, 110), Color.Black);
            _spriteBatch.DrawString(font, $"Press P to go back to the Menu", new Vector2(50, 50), Color.Black);
            // _spriteBatch.DrawString(font, $"Location: {player.Transform.LocalPosition}", new Vector2(50, 160), Color.Black);

            if (ended) {
                _spriteBatch.DrawString(font, "GAME ENDED", new Vector2(ScreenManager.Width / 2, ScreenManager.Height / 2), Color.Black);
            }

            _spriteBatch.End();
        }

        void EndGame() {
            ended = true;
        }
        private void PoweredUp(Object obj)
        {
            int timer = 0;
            while (haveThreadRunning)
            {
                if (timer < 3) timer++;
                else haveThreadRunning = false;

                player.isSpecial = false;
                Debug.WriteLine($"{timer}");
                Thread.Sleep(3000);
            }
        }

    }
}
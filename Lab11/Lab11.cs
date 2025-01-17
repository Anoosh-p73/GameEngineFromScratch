using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Lab11
{
    public class Lab11 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D texture;
        // Button exitButton;

        Color background = Color.White;

        SpriteFont font;

        Dictionary<String, Scene> scenes;
        List<GUIElement> guiElements;
        Scene currentScene;

        bool isChecked = false;
        bool isFullScreen = false;

        public Lab11()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();

            scenes = new Dictionary<string, Scene>();
            guiElements = new List<GUIElement>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            /*System.Diagnostics.Debug.WriteLine("It's me, Bugxor!!!");*/
            texture = Content.Load<Texture2D>("Square");
            font = Content.Load<SpriteFont>("Font");

            /* exitButton = new Button();
            exitButton.Texture = texture;
            exitButton.Text = "Exit";
            exitButton.Bounds = new Rectangle(50, 50, 300, 20);
            exitButton.Action += ExitGame;*/

            

            /*for (int i = 0; i < 16; i++) {
                Button tempButton = new Button();
                tempButton.Text = ((i % 2 == 0)? "On" : "Off");

                tempButton.Texture = texture;

                int width = _graphics.PreferredBackBufferWidth/16;
                int height = _graphics.PreferredBackBufferHeight/16;

                tempButton.Bounds = new Rectangle(width * i, height * i, width, height);



                guiElements.Add(tempButton);

            }*/

            Button Start = new Button();
            Start.Texture = texture;
            Start.Bounds = new Rectangle(50, 50, 80, 30);
            Start.Text = "";
            Start.Action += StartGame;

            guiElements.Add(Start);

            Button FullScreen = new Button();
            FullScreen.Texture = texture;
            FullScreen.Bounds = new Rectangle(250, 50, 200, 30);
            FullScreen.Text = "   FullScreen Mode Toggle ";
            FullScreen.Action += FullScreenMode;

            guiElements.Add(FullScreen);

            scenes.Add("Menu", new Scene(MainMenuUpdate, MainMenuDraw));
            scenes.Add("Play", new Scene(PlayUpdate, PlayDraw));
            currentScene = scenes["Menu"];

        }

        /* void ExitGame(GUIElement element)
        {
            System.Diagnostics.Debug.WriteLine("It's me, Bugxor!");
            background = (background == Color.White ? Color.Blue : Color.White);
        }*/

        void FullScreenMode(GUIElement element) {
            isFullScreen = !isFullScreen;
            System.Diagnostics.Debug.WriteLine($"isFullScreen is {isFullScreen}");
            _graphics.IsFullScreen = isFullScreen;
            _graphics.ApplyChanges();

        }

        void StartGame(GUIElement element) {
            isChecked = !isChecked;
            // System.Diagnostics.Debug.WriteLine($"Checked is {Checked}");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            /*exitButton.Update();*/

            if (isChecked) currentScene = scenes["Play"];
            else currentScene = scenes["Menu"];

            currentScene.Update();

            Time.Update(gameTime);
            InputManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);
            currentScene.Draw();
            /*_spriteBatch.Begin();
            exitButton.Draw(_spriteBatch, font);
            _spriteBatch.End();*/


            _spriteBatch.Begin();
            _spriteBatch.Draw(texture, new Rectangle(50, 50, 30, 30),
            isChecked ? Color.Red : Color.White);
            _spriteBatch.DrawString(font, $"{(isChecked? "Play" : "Menu")}", new Vector2(80,
            50), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
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
            _spriteBatch.End();
        }
        void PlayUpdate()
        {
            foreach (GUIElement element in guiElements)
                element.Update();
        }
        void PlayDraw()
        {
            _spriteBatch.Begin();
            foreach (GUIElement element in guiElements)
                element.Draw(_spriteBatch, font);
            _spriteBatch.End();
        }

    }
}
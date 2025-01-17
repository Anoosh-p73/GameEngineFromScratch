using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using CPI311.GameEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Assignment03
{
    public class Assignment03 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Random random;
        Light light;
        Camera camera;
        List<GameObject> gameObjects;
        Model model;
        SpriteFont font;

        BoxCollider boxCollider;

        int numberCollisions;
        bool showColor = false;
        bool showTexture = true;
        bool toWrite = true;

        List<int> collisionCounter;
        bool haveThreadRunning = false;
        int lastSecondCollision = 0;

        public Assignment03()
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

            random = new Random();

            boxCollider = new BoxCollider();
            boxCollider.Size = 10;

            gameObjects = new List<GameObject>();
            collisionCounter = new List<int>();

            for(int i = 0; i < 5; i++)
                collisionCounter.Add(0);

            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Sphere");
            Transform cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            light = new Light();
            Transform lightTr = new Transform();
            lightTr.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
            light.Transform = lightTr;
            font = Content.Load<SpriteFont>("Font");

            AddGameObject();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyPressed(Keys.Up)) AddGameObject();
            if (InputManager.IsKeyPressed(Keys.Down)) RemoveGameObject();
            if (InputManager.IsKeyPressed(Keys.Space)) showColor = !showColor;
            if (InputManager.IsKeyPressed(Keys.LeftAlt)) showTexture = !showTexture;
            if (InputManager.IsKeyPressed(Keys.LeftShift)) toWrite = !toWrite;

            if (showColor) {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    float speed = gameObjects[i].Rigidbody.Velocity.Length();
                    float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                    (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                    new Vector3(speedValue, speedValue, 1);
                }
            }

            Vector3 normal;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (boxCollider.Collides(gameObjects[i].Collider, out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) < 0)
                        gameObjects[i].Rigidbody.Impulse +=
                           Vector3.Dot(normal, gameObjects[i].Rigidbody.Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < gameObjects.Count; j++)
                {
                    if (gameObjects[i].Collider.Collides(gameObjects[j].Collider, out normal))
                        numberCollisions++;
                    Vector3 velocityNormal = Vector3.Dot(normal,
                        gameObjects[i].Rigidbody.Velocity - gameObjects[j].Rigidbody.Velocity) * -2
                           * normal * gameObjects[j].Rigidbody.Mass * gameObjects[j].Rigidbody.Mass;
                    gameObjects[i].Rigidbody.Impulse += velocityNormal / 2;
                    gameObjects[j].Rigidbody.Impulse += -velocityNormal / 2;
                }
            }

            foreach (GameObject gameObject in gameObjects)
                gameObject.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (showColor)
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    float speed = gameObjects[i].Rigidbody.Velocity.Length();
                    float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                    (gameObjects[i].Renderer.ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = new Vector3(speedValue, speedValue, 1);
                    model.Draw(gameObjects[i].Transform.World, camera.View, camera.Projection);
                }
            }
            else if (!showTexture)
            {
                for (int i = 0; i < gameObjects.Count; i++)
                {
                    (gameObjects[i].Renderer.ObjectModel.Meshes[0].Effects[0] as BasicEffect).DiffuseColor = new Vector3(0, 0, 1);
                    model.Draw(gameObjects[i].Transform.World, camera.View, camera.Projection);
                }
            }

            if (!showColor && showTexture) {
                for (int i = 0; i < gameObjects.Count; i++)
                    gameObjects[i].Draw();
            }

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, $"Use Left Shift to hide info", new Vector2(50, 10), Color.Black);
            _spriteBatch.DrawString(font, $"Number Collisions: {numberCollisions}", Vector2.One * 50, Color.Black);
            _spriteBatch.DrawString(font, $"Use Up and Down to spawn and despawn spheres", new Vector2(50, 90), Color.Black);
            _spriteBatch.DrawString(font, $"Use Space to toggle color mode: {showColor}", new Vector2(50, 130), Color.Black);
            _spriteBatch.DrawString(font, $"Use Left Alt to toggle texture mode: {showTexture}", new Vector2(50, 170), Color.Black);

            if (toWrite) {
                _spriteBatch.DrawString(font, $"Current number of spheres: {gameObjects.Count}", new Vector2(50, 210), Color.Black);
                _spriteBatch.DrawString(font, $"Average collisions over last 5 iterations using multithreading: " +
                    $"{collisionCounter.Average()}", new Vector2(50, 250), Color.Black);
            }

            _spriteBatch.DrawString(font, $"Note: if color mode is toggled, textures will not be visible regardless of texture mode",
                new Vector2(50, 290), Color.Black);
            _spriteBatch.End();
         

            base.Draw(gameTime);
        }

        private void CollisionReset(Object obj)
        {
            while (haveThreadRunning)
            {
                lastSecondCollision = numberCollisions;
                numberCollisions = 0;

                collisionCounter.RemoveAt(0);
                collisionCounter.Add(lastSecondCollision);

                Thread.Sleep(1000);
            }
        }

        private void RemoveGameObject() {
            if (gameObjects.Count > 0) gameObjects.RemoveAt(0);
        }

        private void AddGameObject()
        {
            GameObject gameObject = new GameObject();

            Transform transform = new Transform();
            transform.LocalPosition += Vector3.Right * 10 * (float)random.NextDouble();

            gameObject.Add<Transform>(transform);

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;
            rigidbody.Mass = (float) random.NextDouble() + 0.1f;
            Vector3 direction = new Vector3(
              (float)random.NextDouble(), (float)random.NextDouble(),
              (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);

            gameObject.Add<Rigidbody>(rigidbody);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;

            gameObject.Add<Collider>(sphereCollider);

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(model, gameObject.Transform, camera, Content,
            GraphicsDevice, light, 1, "SimpleShading", 20f, texture);

            gameObject.Add<Renderer>(renderer);

            gameObjects.Add(gameObject);

        }

    }
}
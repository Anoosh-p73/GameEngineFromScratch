
using CPI311.GameEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Lab07
{
    public class Lab07 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        BoxCollider boxCollider;
        SphereCollider sphere1, sphere2;
        // *** Not using GameObject[] But List<Transform> List<Collider>, 
        List<Renderer> renderers;
        List<Transform> transforms;
        List<Collider> colliders;
        List<Rigidbody> rigidbodies;
        int numberCollisions;
        Random random;
        Light light;
        // *** from Lab 4
        Model model;
        Camera camera;
        Transform cameraTransform;
        bool haveThreadRunning = false;
        int lastSecondCollision = 0;
        SpriteFont font;
        public Lab07()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();
            random = new Random();
            transforms = new List<Transform>();
            rigidbodies = new List<Rigidbody>();
            colliders = new List<Collider>();
            boxCollider = new BoxCollider();
            renderers = new List<Renderer>();
            boxCollider.Size = 10;
            /*for (int i = 0; i < 5; i++)
            {
                AddSphere();
            }*/
            haveThreadRunning = true;
            ThreadPool.QueueUserWorkItem(new WaitCallback(CollisionReset));

            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // *** Copy from Lab 4
            model = Content.Load<Model>("Sphere");
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Backward * 20;
            camera = new Camera();
            camera.Transform = cameraTransform;

            light = new Light();
            Transform lightTr = new Transform();
            lightTr.LocalPosition = Vector3.Backward * 10 + Vector3.Right * 5;
            light.Transform = lightTr;
            font = Content.Load<SpriteFont>("Font");
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.Update();
            Time.Update(gameTime);
            
            if(InputManager.IsKeyPressed(Keys.Space)) AddSphere();

            foreach (Rigidbody rigidbody in rigidbodies) rigidbody.Update();
            Vector3 normal; 
            for (int i = 0; i < transforms.Count; i++)
            {
                if (boxCollider.Collides(colliders[i], out normal))
                {
                    numberCollisions++;
                    if (Vector3.Dot(normal, rigidbodies[i].Velocity) < 0)
                        rigidbodies[i].Impulse +=
                           Vector3.Dot(normal, rigidbodies[i].Velocity) * -2 * normal;
                }
                for (int j = i + 1; j < transforms.Count; j++)
                {
                    if (colliders[i].Collides(colliders[j], out normal))
                        numberCollisions++;
                    Vector3 velocityNormal = Vector3.Dot(normal,
                        rigidbodies[i].Velocity - rigidbodies[j].Velocity) * -2
                           * normal * rigidbodies[i].Mass * rigidbodies[j].Mass;
                    rigidbodies[i].Impulse += velocityNormal / 2;
                    rigidbodies[j].Impulse += -velocityNormal / 2;
                }
            }
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            for (int i = 0; i < transforms.Count; i++)
            {
                float speed = rigidbodies[i].Velocity.Length();
                float speedValue = MathHelper.Clamp(speed / 20f, 0, 1);
                (model.Meshes[0].Effects[0] as BasicEffect).DiffuseColor =
                new Vector3(speedValue, speedValue, 1);
                model.Draw(transforms[i].World, camera.View, camera.Projection);
            }

            /*for (int i = 0; i < renderers.Count; i++) 
                renderers[i].Draw();*/

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, $"Last Second Collisions: {lastSecondCollision}", Vector2.One * 100, Color.Black);
            _spriteBatch.DrawString(font, $"Use Spacebar to spawn spheres", new Vector2(100, 140), Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void CollisionReset(Object obj) {
            while (haveThreadRunning) {
                lastSecondCollision = numberCollisions;
                numberCollisions = 0;
                Thread.Sleep(1000);
            }
        }

        private void AddSphere()
        {
            Transform transform = new Transform();
            transform.LocalPosition += Vector3.Right * 10 * (float)random.NextDouble();

            transforms.Add(transform);

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = transform;
            rigidbody.Mass = 1;
            Vector3 direction = new Vector3(
              (float)random.NextDouble(), (float)random.NextDouble(),
              (float)random.NextDouble());
            direction.Normalize();
            rigidbody.Velocity = direction * ((float)random.NextDouble() * 5 + 5);

            rigidbodies.Add(rigidbody);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1.0f * transform.LocalScale.Y;
            sphereCollider.Transform = transform;

            colliders.Add(sphereCollider);

            Texture2D texture = Content.Load<Texture2D>("Square");
            Renderer renderer = new Renderer(model, transform, camera, Content,
            GraphicsDevice, light, 1, "SimpleShading", 20f, texture);

            renderers.Add(renderer);
        }


    }
}

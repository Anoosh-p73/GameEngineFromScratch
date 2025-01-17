using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace Assignment02
{
    public class Ass2 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Model sun, merc, earth, moon;
        private Transform suntr, merctr, earthtr, moontr;

        private Model plane;
        private Transform planetr;

        private Camera camera;
        private Transform cameraTransform;

        // INPUTS
        private bool fps = false;
        private bool tabpressed = false;
        private float animSpeed = 1f;

        public Ass2()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Time.Initialize();
            InputManager.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            sun = Content.Load<Model>("Sphere");
            merc = Content.Load<Model>("Sphere");
            earth = Content.Load<Model>("Sphere");
            moon = Content.Load<Model>("Sphere");

            plane = Content.Load<Model>("Plane");
            planetr = new Transform();

            suntr = new Transform();
            merctr = new Transform();
            earthtr = new Transform();
            moontr = new Transform();

            merctr.Parent = suntr;
            earthtr.Parent = suntr;
            moontr.Parent = earthtr;

            suntr.LocalPosition = Vector3.Zero;
            suntr.LocalScale = Vector3.One * 5;

            merctr.LocalPosition = Vector3.Right * 3;
            merctr.LocalScale = Vector3.One * 0.4f;

            earthtr.LocalPosition = Vector3.Right * 7;
            earthtr.LocalScale = Vector3.One * 0.6f;

            moontr.LocalPosition = Vector3.Right * 2;
            moontr.LocalScale = Vector3.One * 0.33f;

            camera = new Camera();
            cameraTransform = new Transform();
            cameraTransform.LocalPosition = Vector3.Up * 70;
            cameraTransform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Left, 1.57f);
            camera.Transform = cameraTransform;

            planetr.LocalPosition = Vector3.Down * 5;
            planetr.LocalScale = Vector3.One * 3;

            foreach (ModelMesh mesh in sun.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }

            foreach (ModelMesh mesh in merc.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
            foreach (ModelMesh mesh in earth.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }

            foreach (ModelMesh mesh in moon.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            // INPUTS
            if (InputManager.IsKeyPressed(Keys.Tab))
            {
                fps = !fps;
                tabpressed = true;
            }
            if (!InputManager.IsKeyPressed(Keys.Tab))
                tabpressed = false;

            if (InputManager.IsKeyDown(Keys.W)) camera.Transform.LocalPosition += Vector3.Forward * 10f * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.A)) camera.Transform.LocalPosition += Vector3.Left * 10f * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.S)) camera.Transform.LocalPosition += Vector3.Backward * 10f * Time.ElapsedGameTime;
            if (InputManager.IsKeyDown(Keys.D)) camera.Transform.LocalPosition += Vector3.Right * 10f * Time.ElapsedGameTime;

            if (InputManager.IsKeyDown(Keys.Up)) camera.Transform.Rotate(Vector3.Up, 1f * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Left)) camera.Transform.Rotate(Vector3.Backward, 1f * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Down)) camera.Transform.Rotate(Vector3.Down, 1f * Time.ElapsedGameTime);
            if (InputManager.IsKeyDown(Keys.Right)) camera.Transform.Rotate(Vector3.Forward, 1f * Time.ElapsedGameTime);

            if(InputManager.IsKeyDown(Keys.PageUp)) animSpeed += 1f;
            if(InputManager.IsKeyDown(Keys.PageDown)) animSpeed -= 1f;

            //

            if (fps && tabpressed) {
                cameraTransform.LocalPosition = Vector3.Backward * 70;
                cameraTransform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Left, 0);
                camera.Transform = cameraTransform;
            }

            if (!fps && tabpressed) {
                cameraTransform.LocalPosition = Vector3.Up * 70;
                cameraTransform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Left, 1.57f);
                camera.Transform = cameraTransform;
            }

            suntr.Rotate(Vector3.Up, 1f * Time.ElapsedGameTime * animSpeed);
            earthtr.Rotate(Vector3.Up, 2f * Time.ElapsedGameTime * animSpeed);
            moontr.Rotate(Vector3.Up, 4f * Time.ElapsedGameTime * animSpeed);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sun.Draw(suntr.World, camera.View, camera.Projection);
            merc.Draw(merctr.World, camera.View, camera.Projection);
            earth.Draw(earthtr.World, camera.View, camera.Projection);
            moon.Draw(moontr.World, camera.View, camera.Projection);
            plane.Draw(planetr.World, camera.View, camera.Projection);

            base.Draw(gameTime);
        }

        

    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI311.GameEngine;
using System.Collections.Generic;
using System;

namespace Lab09
{
    public class Lab09 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Model cube;
        Model sphere;
        AStarSearch search;
        List<Vector3> path;

        Camera cam;
        Transform camTr;

        int size = 100;

        Random random = new Random();

        public Lab09()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            InputManager.Initialize();
            Time.Initialize();
            ScreenManager.Initialize(_graphics);

            search = new AStarSearch(size, size); // size of grid 
            foreach (AStarNode node in search.Nodes)
                if (random.NextDouble() < 0.2)
                    search.Nodes[random.Next(size), random.Next(size)].Passable = false;

            search.Start = search.Nodes[0, 0];
            search.Start.Passable = true;
            search.End = search.Nodes[size - 1, size - 1];
            search.End.Passable = true;

            search.Search(); // A search is made here.
            path = new List<Vector3>();
            AStarNode current = search.End;
            while (current != null)
            {
                path.Insert(0, current.Position);
                current = current.Parent;
            }


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            cube = Content.Load<Model>("cube");
            sphere = Content.Load<Model>("Sphere");
            camTr = new Transform();
            cam = new Camera();
            cam.Transform = camTr;
            cam.Transform.LocalPosition = Vector3.Right * size/2 + Vector3.Backward * size/2 + Vector3.Up * (int) (size * 0.8);
            cam.Transform.Rotate(Vector3.Right, -MathHelper.PiOver2);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            Time.Update(gameTime);

            if (InputManager.IsKeyPressed(Keys.Space))
            {
                /*search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]; // assign a random start node (passable)
                search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]; // assign a random end node (passable)*/

                while (!(search.Start = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;
                while (!(search.End = search.Nodes[random.Next(search.Cols), random.Next(search.Rows)]).Passable) ;

                search.Search();
                path.Clear();
                AStarNode current = search.End;
                while (current != null)
                {
                    path.Insert(0, current.Position);
                    current = current.Parent;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            foreach (AStarNode node in search.Nodes)
                if (!node.Passable)
                    cube.Draw(Matrix.CreateScale(0.5f, 0.05f, 0.5f) *
                    Matrix.CreateTranslation(node.Position), cam.View, cam.Projection);
            foreach (Vector3 position in path)
                sphere.Draw(Matrix.CreateScale(0.3f, 0.3f, 0.3f) * Matrix.CreateTranslation(position), cam.View, cam.Projection);



            base.Draw(gameTime);
        }
    }
}
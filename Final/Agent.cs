using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using CPI311.GameEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Final
{
    public class Agent : GameObject
    {
        public AStarSearch search;
        List<Vector3> path;
        public float speed = 5f;   //moving speed
        private int gridSize = 32;  //grid size
        private TerrainRenderer Terrain;

        public Agent(TerrainRenderer terrain, ContentManager Content, Camera camera, 
            GraphicsDevice graphicsDevice, Light light) : base() {
            
            Terrain = terrain;
            path = null;
            search = new AStarSearch(gridSize, gridSize);

            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);

            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = 1f;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);

            Texture2D texture = Content.Load<Texture2D>("Square");

            Renderer renderer = new Renderer(Content.Load<Model>("Sphere"), Transform, camera, Content, graphicsDevice,
                light, 1, "SimpleShading", 20f, texture);
            Add<Renderer>(renderer);

            float gridW = Terrain.size.X / gridSize;
            float gridH = Terrain.size.Y / gridSize;
            
            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                {
                    Vector3 pos = new Vector3(gridW * i + gridW / 2 - terrain.size.X / 2, 0, gridH * j + gridH / 2 - terrain.size.Y / 2);
                    if (Terrain.GetAltitude(pos) > 0) { 
                        search.Nodes[j, i].Passable = false;
                    }
                    // Debug.WriteLine($"For Node {j},{i} the value is {search.Nodes[j, i].Passable}, and the height is {Terrain.GetAltitude(pos)}");    
                }
            StartPoint();
        }

        public override void Update()
        {
            TrackPlayer();

            Transform.Update();
            base.Update();
        }

        private void ResetToSpawn() { 
            
        }

        private void TrackPlayer() {

            if (path != null && path.Count > 0)
            {
                // Debug.WriteLine($"Current Destination: {GetGridPosition(path[0])}");
                // Move to the destination along the path
                Vector3 currP = Transform.LocalPosition;
                Vector3 destP = GetGridPosition(path[0]);

                currP.Y = 0;
                destP.Y = 0;

                Vector3 dir = Vector3.Distance(currP, destP) == 0 ? Vector3.Zero : Vector3.Normalize(destP - currP);
                this.Rigidbody.Velocity = new Vector3(dir.X, 0, dir.Z) * speed;

                if (Vector3.Distance(currP, destP) < 1f) // if it reaches to a point, go to the next in path
                {
                    path.RemoveAt(0);
                    if (path.Count == 0) // if it reached to the goal
                    {
                        path = null;
                        return;
                    }
                }
            }
            else {
                // Rigidbody.Velocity = Vector3.Zero;
                StartPoint();
            }

            this.Transform.LocalPosition = new Vector3(
                this.Transform.LocalPosition.X,
                Terrain.GetAltitude(this.Transform.LocalPosition),
                this.Transform.LocalPosition.Z) + Vector3.Up;
        }

        private Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2, 0, gridH * gridPos.Z + gridH / 2 - Terrain.size.Y / 2);
        }

        void StartPoint() {
            Random random = new Random();
            // while (!(search.Start = search.Nodes[random.Next(search.Rows), random.Next(search.Cols)]).Passable);
            search.Start = search.Nodes[14, 14];
            while (!(search.End = search.Nodes[random.Next(search.Rows),
                random.Next(search.Cols)]).Passable) ;

            // Debug.WriteLine($"Start point: {search.Start.Position}, End point: {search.End.Position}");

            search.Search();
            path = new List<Vector3>();
            AStarNode current = search.End;
            var count = 0;
            while (current != null)
            {
                count++;
                path.Insert(0, current.Position);
                current = current.Parent;

            }
        }

        public void CollidedWithPlayer()
        {
            if (path.Count > 0) path.RemoveAt(0);
        }
    }
}

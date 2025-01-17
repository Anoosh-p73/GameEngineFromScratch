using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CPI311.GameEngine;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment05
{
    public class Agent : GameObject
    {
        public AStarSearch search;
        List<Vector3> path;
        private float speed = 5f; //moving speed
        private int gridSize = 20; //grid size
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
                    if (Terrain.GetAltitude(pos) > 1.0)
                        search.Nodes[j, i].Passable = false;
                }
        }

        public override void Update()
        {
            if (path != null && path.Count > 0)
            {
                // Move to the destination along the path
                Vector3 currP = Transform.LocalPosition;
                Vector3 destP = GetGridPosition(path[0]);

                currP.Y = 0;
                destP.Y = 0;

                Vector3 dir = Vector3.Distance(currP, destP) == 0 ? Vector3.Zero : Vector3.Normalize(destP - currP);
                this.Rigidbody.Velocity = new Vector3(dir.X, 0, dir.Z) * 10;

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
            else // When agent reaches the goal
            {
                RandomPathFinding(); // Search again
                Transform.LocalPosition = GetGridPosition(path[0]);
            }
            this.Transform.LocalPosition = new Vector3(
            this.Transform.LocalPosition.X,
            Terrain.GetAltitude(this.Transform.LocalPosition),
            this.Transform.LocalPosition.Z) + Vector3.Up;
            Transform.Update();
            base.Update();
        }

        private Vector3 GetGridPosition(Vector3 gridPos)
        {
            float gridW = Terrain.size.X / search.Cols;
            float gridH = Terrain.size.Y / search.Rows;
            return new Vector3(gridW * gridPos.X + gridW / 2 - Terrain.size.X / 2, 0, gridH * gridPos.Z + gridH / 2 - Terrain.size.Y / 2);
        }
        private void RandomPathFinding()
        {
            Random random = new Random();
            while (!(search.Start = search.Nodes[random.Next(search.Rows),
                random.Next(search.Cols)]).Passable) ;
            search.End = search.Nodes[search.Rows / 2, search.Cols / 2];
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

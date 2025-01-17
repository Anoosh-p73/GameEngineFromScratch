using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment04
{
    public class Asteroid : GameObject
    {

        public bool isActive;

        public Asteroid(ContentManager Content, Camera camera, GraphicsDevice graphicsDevice, Light light) : base()
        {
            // *** Add Rigidbody
            Rigidbody rigidbody = new Rigidbody();
            rigidbody.Transform = Transform;
            rigidbody.Mass = 1;
            Add<Rigidbody>(rigidbody);
            // *** Add Renderer
            Texture2D texture = Content.Load<Texture2D>("asteroid1");
            Renderer renderer = new Renderer(Content.Load<Model>("asteroid4"),
            Transform, camera, Content, graphicsDevice, light, 1, null, 20f, texture);
            Add<Renderer>(renderer);
            // *** Add collider
            SphereCollider sphereCollider = new SphereCollider();
            sphereCollider.Radius = renderer.ObjectModel.Meshes[0].BoundingSphere.Radius;
            sphereCollider.Transform = Transform;
            Add<Collider>(sphereCollider);
            //*** Additional Property (for Asteroid, isActive = true)
            isActive = false;
            Transform.LocalScale = 0.1f * Vector3.One;
        }
        public override void Update()
        {
            if (!isActive) return;

            if (Transform.Position.X > GameConstants.PlayfieldSizeX ||
            Transform.Position.X < -GameConstants.PlayfieldSizeX ||
            Transform.Position.Z > GameConstants.PlayfieldSizeY ||
            Transform.Position.Z < -GameConstants.PlayfieldSizeY)
            {
                isActive = false;
                Rigidbody.Velocity = Vector3.Zero; // stop moving
            }

            base.Update();
        }
        public override void Draw()
        {
            if (isActive) base.Draw();
        }
    }
}

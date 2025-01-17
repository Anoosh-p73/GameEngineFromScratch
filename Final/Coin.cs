using CPI311.GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Final
{
    public class Coin : GameObject
    {

        public bool isCollected = false;
        public bool Special = false;

        public Coin(ContentManager Content, Camera camera,
            GraphicsDevice graphicsDevice, Light light, bool isSpecial) : base()
        {

            Special = isSpecial;

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

            // Debug.WriteLine("Coin Created");

        }
        public void CollidedWithPlayer()
        {
            this.Transform.LocalPosition = Vector3.One * 100; // Sent to oblivion
            isCollected = true;
        }
    }
}

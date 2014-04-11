using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment_3D
{
    class Projectile : ModelObject
    {
        private float speed = 30f;
        private Vector3 direction;
        public bool destroy { get;set; }

        public Projectile(ContentManager Content) : base(Content)
        {
            this.destroy = false;
        }

        public override void Load(string ModelName)
        {
            base.Load(ModelName);
        }

        public override void Load(string ModelName, string ModelTexture)
        {
            base.Load(ModelName, ModelTexture);
        }


        public void SetDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        public bool isColliding(BoundingBox box)
        {
            for (int I = 0; I < this.model.Meshes.Count; ++I)
            {
                BoundingSphere sphere1 = this.model.Meshes[I].BoundingSphere;
                sphere1 = sphere1.Transform(this.World);

                if (box.Contains(sphere1) == ContainmentType.Intersects)
                    return true;
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {
            if (!this.destroy)
            this.Position += this.direction * this.speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public override void Draw(Matrix View, Matrix Projection)
        {
            if (this.destroy) return;
            this.GetWorldMatrix();

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = transforms[mesh.ParentBone.Index] * this.World;
                    effect.View = View;
                    effect.Projection = Projection;

                    effect.EnableDefaultLighting();

                    if (this.texture != null)
                    {
                        effect.TextureEnabled = true;
                        effect.Texture = this.texture;
                    }
                }
                mesh.Draw();
            }
        }
    }
}

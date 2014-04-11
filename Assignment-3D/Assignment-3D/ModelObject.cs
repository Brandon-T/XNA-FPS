using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Assignment_3D
{
    class ModelObject
    {
        public Model model = null;
        protected ContentManager Content = null;
        protected Matrix[] transforms = null;
        public Vector3 Position { get; set; }
        public float Rotation_X { get; set; }
        public float Rotation_Y { get; set; }
        public float Rotation_Z { get; set; }
        public Vector3 Scale { get; set; }
        protected Matrix World = Matrix.Identity;
        protected Texture2D texture = null;

        public ModelObject(ContentManager Content)
        {
            this.Content = Content;
            this.Scale = new Vector3(1, 1, 1);
            this.Position = Vector3.Zero;
        }

        public virtual void Load(string ModelName)
        {
            this.model = this.Content.Load<Model>(ModelName);
            this.transforms = new Matrix[this.model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
        }

        public virtual void Load(string ModelName, string ModelTexture)
        {
            this.Load(ModelName);
            if (ModelTexture != null)
            {
                this.texture = this.Content.Load<Texture2D>(ModelTexture);
            }
        }

        public bool IsColliding(BoundingSphere sphere)
        {
            for (int I = 0; I < this.model.Meshes.Count; ++I)
            {
                BoundingSphere sphere1 = this.model.Meshes[I].BoundingSphere;
                sphere1 = sphere1.Transform(this.World);

                if (sphere.Intersects(sphere1))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsColliding(BoundingBox box)
        {
            for (int I = 0; I < this.model.Meshes.Count; ++I)
            {
                BoundingSphere sphere1 = this.model.Meshes[I].BoundingSphere;
                sphere1 = sphere1.Transform(this.World);

                if (box.Intersects(sphere1))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsColliding(ModelObject modeltwo)
        {
            for (int I = 0; I < this.model.Meshes.Count; ++I)
            {
                BoundingSphere sphere1 = this.model.Meshes[I].BoundingSphere;
                sphere1 = sphere1.Transform(this.World);

                for (int J = 0; J < modeltwo.model.Meshes.Count; ++J)
                {
                    BoundingSphere sphere2 = modeltwo.model.Meshes[J].BoundingSphere;
                    sphere2 = sphere2.Transform(modeltwo.World);

                    if (sphere1.Intersects(sphere2))
                        return true;
                }
            }
            return false;
        }

        public bool Contains(BoundingBox box)
        {
            for (int I = 0; I < this.model.Meshes.Count; ++I)
            {
                BoundingSphere sphere1 = this.model.Meshes[I].BoundingSphere;
                sphere1 = sphere1.Transform(this.World);

                if (sphere1.Contains(box) == ContainmentType.Contains)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(BoundingSphere sphere)
        {
            for (int I = 0; I < this.model.Meshes.Count; ++I)
            {
                BoundingSphere sphere1 = this.model.Meshes[I].BoundingSphere;
                sphere1 = sphere1.Transform(this.World);

                if (sphere1.Contains(sphere) == ContainmentType.Contains)
                {
                    return true;
                }
            }
            return false;
        }

        protected void GetWorldMatrix()
        {
            this.World = Matrix.CreateScale(this.Scale)
                       * Matrix.CreateRotationX(MathHelper.ToRadians(this.Rotation_X))
                       * Matrix.CreateRotationY(MathHelper.ToRadians(this.Rotation_Y))
                       * Matrix.CreateRotationZ(MathHelper.ToRadians(this.Rotation_Z))
                       * Matrix.CreateTranslation(this.Position);
        }

        public virtual void Draw(Matrix View, Matrix Projection)
        {
            this.GetWorldMatrix();

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = transforms[mesh.ParentBone.Index] * this.World;
                    effect.View = View;
                    effect.Projection = Projection;

                    effect.LightingEnabled = true;
                    effect.AmbientLightColor = Color.White.ToVector3();
                    effect.PreferPerPixelLighting = true;

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

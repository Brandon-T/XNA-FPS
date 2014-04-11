using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Assignment_3D
{
    class GunModel : ModelObject
    {
        public Projectile[] projectiles { get; set; }
        public float XOffset { get; set; }
        public float YOffset { get; set; }
        public float ZOffset { get; set; }
        public Crosshair crosshair { get; set; }
        private int ammocount = 0;
        private int clipsize = 0;
        private SoundEffect sound = null;
        public List<Projectile> bullets = new List<Projectile>();

        public GunModel(ContentManager Content)
            : base(Content)
        {
            this.Scale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        public void Load(string ModelName, string bulletModel, int bulletCount)
        {
            base.Load(ModelName);
            this.projectiles = new Projectile[bulletCount];

            this.ammocount = bulletCount - 1;
            this.clipsize = this.ammocount;
            for (int i = 0; i < projectiles.Length; ++i)
            {
                this.projectiles[i] = new Projectile(base.Content);
                this.projectiles[i].Load(bulletModel);
                this.projectiles[i].Scale = new Vector3(0.01f, 0.01f, 0.01f);
            }
        }

        public void Load(string ModelName, string bulletModel, string shotName, int bulletCount)
        {
            base.Load(ModelName);
            this.projectiles = new Projectile[bulletCount];
            this.sound = Content.Load<SoundEffect>(shotName);

            this.ammocount = bulletCount - 1;
            this.clipsize = this.ammocount;
            for (int i = 0; i < projectiles.Length; ++i)
            {
                this.projectiles[i] = new Projectile(base.Content);
                this.projectiles[i].Load(bulletModel);
                this.projectiles[i].Scale = new Vector3(0.01f, 0.01f, 0.01f);
            }
        }

        public void SetUpInfo(float ScaleX, float ScaleY, float ScaleZ, float XOffset, float YOffset, float ZOffset)
        {
            this.Scale = new Vector3(ScaleX, ScaleY, ScaleZ);
            this.XOffset = XOffset;
            this.YOffset = YOffset;
            this.ZOffset = ZOffset;
        }

        public void ResetAmmo()
        {
            this.ammocount = this.clipsize;
            foreach (Projectile bullet in projectiles)
            {
                bullet.destroy = false;
            }
        }

        public void Fire(Camera cam)
        {
            if (ammocount > 0)
            {
                Projectile bullet = projectiles[ammocount--];
                bullet.Position = cam.GetTarget;
                Vector3 dir = cam.GetTarget - cam.GetPosition;
                dir.Normalize();
                bullet.SetDirection(dir);
                float angle = (float)Math.Atan2(-dir.X, dir.Y);
                bullet.Rotation_X = -90f;
                bullet.Rotation_Y = MathHelper.ToDegrees(cam.Angle_Y);
                this.bullets.Add(bullet);
                this.sound.Play();
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Projectile bullet in bullets)
            {
                bullet.Update(gameTime);
            }
        }

        public Matrix WeaponWorldMatrix(Matrix ViewMatrix, Vector3 Position, float updown, float leftright)
        {
            Vector3 xAxis = Vector3.Zero;
            Vector3 yAxis = Vector3.Zero;
            Vector3 zAxis = Vector3.Zero;

            xAxis.X = ViewMatrix.M11;
            xAxis.Y = ViewMatrix.M21;
            xAxis.Z = ViewMatrix.M31;

            yAxis.X = ViewMatrix.M12;
            yAxis.Y = ViewMatrix.M22;
            yAxis.Z = ViewMatrix.M32;

            zAxis.X = ViewMatrix.M13;
            zAxis.Y = ViewMatrix.M23;
            zAxis.Z = ViewMatrix.M33;
            
            Position += xAxis * XOffset;                             //X axis offset
            Position += -yAxis * YOffset;                            //Y axis offset
            Position += zAxis * ZOffset;                             //Z Axis offset
            return Matrix.CreateScale(this.Scale)                    //Weapon scaling                  
                * Matrix.CreateFromYawPitchRoll(0, 0, 0)             //Rotation offset
                * Matrix.CreateRotationX(updown)
                * Matrix.CreateRotationY(leftright)
                * Matrix.CreateTranslation(Position);
        }

        public virtual void Draw(Matrix View, Matrix Projection, Vector3 Target, float pitch, float yaw)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = /*transforms[mesh.ParentBone.Index] * */WeaponWorldMatrix(View, Position, pitch, yaw);
                    effect.View = View;
                    effect.Projection = Projection;
                }
                mesh.Draw();
            }

            for (int i = 0; i < bullets.Count; ++i)
            {
                bullets[i].Draw(View, Projection);

                if (bullets[i].destroy)
                {
                    bullets.RemoveAt(i);
                    --i;
                }
            }
        }
    }
}

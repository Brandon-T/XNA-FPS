using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Assignment_3D
{
    public class Engine : GameComponent
    {
        /****************************
         *     LOOK AT VECTORS
         ****************************/
        private Vector3 Target = Vector3.Zero;
        private Vector3 UpVector = Vector3.Up;
        private Vector3 Position = new Vector3(0.0f, 0.0f, 1.0f);

        /****************************
         *  MODEL VIEW PROJECTION
         ****************************/
        private Matrix Projection = Matrix.Identity;
        private Matrix View = Matrix.Identity;
        private Matrix World = Matrix.Identity;

        /****************************
         *          ROTATION
         ****************************/
        private float angle_X = 0.0f;
        private float angle_Y = 0.0f;
        private float angle_Z = 0.0f;

        private Matrix Rotation_X = Matrix.Identity;
        private Matrix Rotation_Y = Matrix.Identity;
        private Matrix Rotation_Z = Matrix.Identity;

        /****************************
         *   TRANSLATION & SCALE
         ****************************/
        private float scale = 1.0f;

        private float translation_X = 0.0f;
        private float translation_Y = 0.0f;
        private float translation_Z = 0.0f;

        private Matrix Scale_Matrix = Matrix.Identity;
        private Matrix Translation_Matrix = Matrix.Identity;

        /****************************
         *          OTHER
         ****************************/
        private BasicEffect Effects = null;
        private GraphicsDevice graphics = null;



        /****************************
         *     IMPLEMENTATION
         ****************************/

        public float Scale
        {
            get { return this.scale; }
            set
            {
                this.scale = value;
                this.Scale_Matrix = Matrix.CreateScale(this.scale);
            }
        }

        public float Angle_X
        {
            get { return this.angle_X; }

            set
            {
                this.angle_X = value;
                this.Rotation_X = Matrix.CreateRotationX(MathHelper.ToRadians(this.angle_X));
            }
        }

        public float Angle_Y
        {
            get { return this.angle_Y; }
            set
            {
                this.angle_Y = value;
                this.Rotation_Y = Matrix.CreateRotationY(MathHelper.ToRadians(this.angle_Y));
            }
        }

        public float Angle_Z
        {
            get { return this.angle_Z; }
            set
            {
                this.angle_Z = value;
                this.Rotation_Z = Matrix.CreateRotationZ(MathHelper.ToRadians(this.angle_Z));
            }
        }

        public float Translation_X
        {
            get { return this.translation_X; }
            set
            {
                this.translation_X = value;
                this.Translation_Matrix = Matrix.CreateTranslation(this.translation_X, this.translation_Y, this.translation_Z);
            }
        }

        public float Translation_Y
        {
            get { return this.translation_Y; }
            set
            {
                this.translation_Y = value;
                this.Translation_Matrix = Matrix.CreateTranslation(this.translation_X, this.translation_Y, this.translation_Z);
            }
        }

        public float Translation_Z
        {
            get { return this.translation_Z; }
            set
            {
                this.translation_Z = value;
                this.Translation_Matrix = Matrix.CreateTranslation(this.translation_X, this.translation_Y, this.translation_Z);
            }
        }

        public BasicEffect Effect
        {
            get { return this.Effects; }
        }



        public Engine(Game game): base(game)
        {
            this.graphics = game.GraphicsDevice;
        }

        public override void Initialize()
        {
            this.graphics.RasterizerState = RasterizerState.CullNone;
            this.Effects = new BasicEffect(this.graphics);

            base.Initialize();
        }

        public void LookAt(float FOV_Degrees, float NearPlaneDistance, float FarPlaneDistance, Vector3 position)
        {
            this.LookAt(FOV_Degrees, NearPlaneDistance, FarPlaneDistance, position, this.Target, this.UpVector);
        }

        public void LookAt(float FOV_Degrees, float NearPlaneDistance, float FarPlaneDistance, Vector3 position, Vector3 target, Vector3 up)
        {
            this.Position = position;
            this.Target = target;
            this.UpVector = up;
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV_Degrees), this.graphics.Viewport.AspectRatio, NearPlaneDistance, FarPlaneDistance);
            this.View = Matrix.CreateLookAt(Position, Target, UpVector);
        }

        public override void Update(GameTime gameTime)
        {
            this.Effects.View = View;
            this.Effects.Projection = Projection;
            this.Effects.World = Matrix.Identity * this.Rotation_X * this.Rotation_Y * this.Rotation_Z * this.Translation_Matrix * this.Scale_Matrix;
            base.Update(gameTime);
        }
    }
}

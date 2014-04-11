using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment_3D
{
    class Camera
    {
        public event EventHandler onMouseClick = null;
        public event EventHandler onMousePress = null;
        public event EventHandler onKeyPress = null;
        private MouseState OldMouseState;
        private Vector3 Target = Vector3.Zero;
        private Vector3 UpVector = Vector3.Up;
        private Vector3 Position = Vector3.UnitZ;
        private Matrix ViewMatrix = Matrix.Identity;
        private Matrix ProjectionMatrix = Matrix.Identity;
        private BasicEffect Effects = null;
        private float Pitch = 0.0f;
        private float Yaw = 0.0f;
        private float Roll = 0.0f;
        private float Speed = 10.0f;
        private float WalkingSpeed = 2.0f;
        private float GroundHeight = 1.3f;
        private float RotationSpeed = 0.05f;
        private Game game = null;

        public float Angle_X
        {
            get { return this.Pitch; }
        }

        public float Angle_Y
        {
            get { return this.Yaw; }
        }

        public float Angle_Z
        {
            get { return this.Roll; }
        }

        public BasicEffect Effect
        {
            get { return this.Effects; }
        }

        public Matrix View
        {
            get { return this.ViewMatrix; }
        }

        public Matrix Projection
        {
            get { return this.ProjectionMatrix; }
        }

        public Vector3 GetTarget
        {
            get { return this.Target; }
        }

        public Vector3 GetPosition
        {
            get { return this.Position; }
        }

        public Camera(Game game)
        {
            this.game = game;
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            this.Effects = new BasicEffect(game.GraphicsDevice);

            Mouse.SetPosition(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2);
            this.OldMouseState = Mouse.GetState();
        }

        public void LookAt(float FOV_Degrees, float NearPlaneDistance, float FarPlaneDistance, Vector3 Position, Vector3 Target, Vector3 UpVector)
        {
            this.Position = Position;
            this.UpVector = UpVector;
            this.Target = Target + Vector3.Forward;
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FOV_Degrees), this.game.GraphicsDevice.Viewport.AspectRatio, NearPlaneDistance, FarPlaneDistance);
            this.ViewMatrix = Matrix.CreateLookAt(Position, Target, UpVector);
        }

        public Vector3 ProcessKeyboardInput(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            Vector3 velocity = Vector3.Zero;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float moveFactor = deltaTime * (keyboard.IsKeyDown(Keys.LeftShift) ? this.WalkingSpeed : this.Speed);

            if (keyboard.IsKeyDown(Keys.W)) velocity.Z -= moveFactor;
            if (keyboard.IsKeyDown(Keys.S)) velocity.Z += moveFactor;
            if (keyboard.IsKeyDown(Keys.A)) velocity.X -= moveFactor;
            if (keyboard.IsKeyDown(Keys.D)) velocity.X += moveFactor;

            if (velocity.LengthSquared() != 0) //So we don't move faster diagonally.
            {
                velocity.Normalize();
                velocity *= moveFactor;
            }

            if (keyboard.GetPressedKeys() != null && keyboard.GetPressedKeys().Length > 0)
            {
                if (this.onKeyPress != null)
                {
                    this.onKeyPress(this, EventArgs.Empty);
                }
            }

            return velocity;
        }

        private void ProcessMouseInput(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float mouseX = mouse.X - (this.game.GraphicsDevice.Viewport.Width / 2);
            float mouseY = mouse.Y - (this.game.GraphicsDevice.Viewport.Height / 2);

            this.Yaw -= (mouseX * RotationSpeed) * deltaTime;
            this.Pitch -= (mouseY * RotationSpeed) * deltaTime;
            this.Pitch = MathHelper.Clamp(this.Pitch, MathHelper.ToRadians(-90), MathHelper.ToRadians(90));
            Mouse.SetPosition(this.game.GraphicsDevice.Viewport.Width / 2, this.game.GraphicsDevice.Viewport.Height / 2);

            if (mouse.LeftButton == ButtonState.Pressed && this.OldMouseState.LeftButton == ButtonState.Released)
            {
                if (this.onMousePress != null)
                {
                    this.onMousePress(this, EventArgs.Empty);
                }
            }

            if (mouse.LeftButton == ButtonState.Released && this.OldMouseState.LeftButton == ButtonState.Pressed)
            {
                if (this.onMouseClick != null)
                {
                    this.onMouseClick(this, EventArgs.Empty);
                }
            }

            OldMouseState = mouse;
        }

        private void ProcessInput(GameTime gameTime, Vector3 velocity)
        {
            ProcessMouseInput(gameTime);

            Matrix RotationMatrix = Matrix.CreateRotationX(this.Pitch) * Matrix.CreateRotationY(this.Yaw);
            Vector3 TransformedCamera = Vector3.Transform(Vector3.Forward, RotationMatrix);
            this.Position += Vector3.Transform(velocity, RotationMatrix);

            this.Position += (Vector3.Down * 9.81f * (float)gameTime.ElapsedGameTime.TotalSeconds); //gravity.
            if (this.Position.Y < GroundHeight) this.Position.Y = GroundHeight;                     //cannot fall through the ground.
            this.Target = TransformedCamera + this.Position;

            Vector3 TransformedUp = Vector3.Transform(this.UpVector, RotationMatrix);
            this.ViewMatrix = Matrix.CreateLookAt(this.Position, this.Target, TransformedUp);
        }

        public void Update(GameTime gameTime, Vector3 velocity)
        {
            ProcessInput(gameTime, velocity);
            this.Effects.View = ViewMatrix;
            this.Effects.Projection = ProjectionMatrix;
            this.Effects.World = Matrix.Identity;
        }

    }
}

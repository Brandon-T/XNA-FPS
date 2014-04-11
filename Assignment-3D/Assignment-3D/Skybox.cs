using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Assignment_3D
{
    class Skybox
    {
        private Game game = null;
        private Effect skyeffect = null;
        private TextureCube skycube = null;
        private VertexBuffer vertices = null;
        private IndexBuffer indices = null;

        public Vector3 Position { get; set; }
        public float Rotation_X { get; set; }
        public float Rotation_Y { get; set; }
        public float Rotation_Z { get; set; }
        public Vector3 Scale { get; set; }

        public Skybox(Game game)
        {
            this.game = game;
            this.Position = Vector3.Zero;
            this.Scale = new Vector3(1, 1, 1);

            this.skyeffect = this.game.Content.Load<Effect>("Shaders/SkyEffect2");
            this.skycube = this.game.Content.Load<TextureCube>("Skyboxes/Mountains");
            this.skyeffect.Parameters["skycube"].SetValue(this.skycube);

            this.CreateVertices();
            this.CreateIndices();
        }

        private void CreateVertices()
        {
            Vector3[] Vertices =
            {
                new Vector3(-1, -1, -1),
                new Vector3(-1, -1, 1),
                new Vector3(1, -1, 1),
                new Vector3(1, -1, -1),
                new Vector3(-1, 1, -1),
                new Vector3(-1, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, -1)
            };

            VertexDeclaration VertexPositionDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0));
            this.vertices = new VertexBuffer(this.game.GraphicsDevice, VertexPositionDeclaration, Vertices.Length, BufferUsage.WriteOnly);
            this.vertices.SetData<Vector3>(Vertices);
        }

        private void CreateIndices()
        {
            UInt16[] Indices =
            {
                0, 2, 3, 0, 1, 2, //bottom face
                4, 6, 5, 4, 7, 6, //top face
                5, 2, 1, 5, 6, 2, //front face
                0, 7, 4, 0, 3, 7, //back face
                0, 4, 1, 1, 4, 5, //left face
                2, 6, 3, 3, 6, 7, //right face
            };

            this.indices = new IndexBuffer(this.game.GraphicsDevice, IndexElementSize.SixteenBits, Indices.Length, BufferUsage.WriteOnly);
            this.indices.SetData<UInt16>(Indices);
        }

        public Matrix CreateWorldMatrix()
        {
            return Matrix.CreateScale(this.Scale)
                 * Matrix.CreateRotationX(MathHelper.ToRadians(this.Rotation_X))
                 * Matrix.CreateRotationY(MathHelper.ToRadians(this.Rotation_Y))
                 * Matrix.CreateRotationZ(MathHelper.ToRadians(this.Rotation_Z))
                 * Matrix.CreateTranslation(this.Position);
        }

        public void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition)
        {
            this.game.GraphicsDevice.SetVertexBuffer(this.vertices);
            this.game.GraphicsDevice.Indices = indices;

            this.skyeffect.Parameters["World"].SetValue(this.CreateWorldMatrix());
            this.skyeffect.Parameters["View"].SetValue(View);
            this.skyeffect.Parameters["Projection"].SetValue(Projection);
            this.skyeffect.Parameters["CameraPosition"].SetValue(CameraPosition);
            this.skyeffect.CurrentTechnique.Passes[0].Apply();

            this.game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 36 / 3);
        }
    }
}

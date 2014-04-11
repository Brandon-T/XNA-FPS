using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VPNT = Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture;

namespace Assignment_3D
{
    class TexturedCube
    {
        private Game game = null;
        private VertexBuffer vbo = null;
        private List<VPNT> vertices = new List<VPNT>();

        public Vector3 Position { get; set; }
        public float Rotation_X { get; set; }
        public float Rotation_Y { get; set; }
        public float Rotation_Z { get; set; }
        public float Scale { get; set; }

        public TexturedCube(Game game)
        {
            this.game = game;
            this.Scale = 1.0f;
            this.Initialize();
            this.vbo = new VertexBuffer(this.game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, this.vertices.Count, BufferUsage.WriteOnly);
            this.vbo.SetData(this.vertices.ToArray());
            this.vertices = null;
        }

        private void Initialize()
        {
            Vector3[] Vertices =
            {
                new Vector3(-1, -1, -1), //Bottom-Left Front.
                new Vector3(-1, -1, 1),  //Bottom-Left Back.
                new Vector3(1, -1, -1),  //Bottom-Right Front.
                new Vector3(1, -1, 1),   //Bottom-Right Back.

                new Vector3(-1, 1, -1),  //Top-Left Front.
                new Vector3(-1, 1, 1),   //Top-Left Back.
                new Vector3(1, 1, -1),   //Top-Right Front.
                new Vector3(1, 1, 1)     //Top-Right Back.
            };

            Vector3[] Normals = 
            {
                Vector3.UnitY,      //Top
                -Vector3.UnitY,     //Bottom
                -Vector3.Forward,   //Front
                Vector3.Forward,    //Back
                -Vector3.UnitX,     //Left
                Vector3.UnitX,      //Right
            };

            Vector2[] TextureCoordinates = 
            {
                new Vector2(1, 0),  //Top-Left
                new Vector2(0, 0),  //Top-Right
                new Vector2(1, 1),  //Bottom-Left
                new Vector2(0, 1)   //Bottom-Right
            };

            VPNT[] FrontFace =
            {
                new VPNT(Vertices[4], Normals[2], TextureCoordinates[0]),  //Top-Left Front
                new VPNT(Vertices[0], Normals[2], TextureCoordinates[2]),  //Bottom-Left Front
                new VPNT(Vertices[6], Normals[2], TextureCoordinates[1]),  //Top-Right Front
                new VPNT(Vertices[0], Normals[2], TextureCoordinates[2]),  //Bottom-Left Front
                new VPNT(Vertices[2], Normals[2], TextureCoordinates[3]),  //Bottom-Right Front
                new VPNT(Vertices[6], Normals[2], TextureCoordinates[1])   //Top-Right Front
            };

            VPNT[] BackFace = 
            {
                new VPNT(Vertices[5], Normals[3], TextureCoordinates[1]),  //Top-Left Back
                new VPNT(Vertices[7], Normals[3], TextureCoordinates[0]),  //Top-Right Back
                new VPNT(Vertices[1], Normals[3], TextureCoordinates[3]),  //Bottom-Left Back
                new VPNT(Vertices[1], Normals[3], TextureCoordinates[3]),  //Bottom-Left Back
                new VPNT(Vertices[7], Normals[3], TextureCoordinates[0]),  //Top-Right Back
                new VPNT(Vertices[3], Normals[3], TextureCoordinates[2]),  //Bottom-Right Back
            };

            VPNT[] TopFace = 
            {
                new VPNT(Vertices[4], Normals[0], TextureCoordinates[2]),  //Top-Left Front
                new VPNT(Vertices[7], Normals[0], TextureCoordinates[1]),  //Top-Right Back
                new VPNT(Vertices[5], Normals[0], TextureCoordinates[0]),  //Top-Left Back
                new VPNT(Vertices[4], Normals[0], TextureCoordinates[2]),  //Top-Left Front
                new VPNT(Vertices[6], Normals[0], TextureCoordinates[3]),  //Top-Right Front
                new VPNT(Vertices[7], Normals[0], TextureCoordinates[1]),  //Top-Right Back
            };

            VPNT[] BottomFace = 
            {
                new VPNT(Vertices[0], Normals[1], TextureCoordinates[0]),  //Bottom-Left Front
                new VPNT(Vertices[1], Normals[1], TextureCoordinates[2]),  //Bottom-Left Back
                new VPNT(Vertices[3], Normals[1], TextureCoordinates[3]),  //Bottom-Right Back
                new VPNT(Vertices[0], Normals[1], TextureCoordinates[0]),  //Bottom-Left Front
                new VPNT(Vertices[3], Normals[1], TextureCoordinates[3]),  //Bottom-Right Back
                new VPNT(Vertices[2], Normals[1], TextureCoordinates[1]),  //Bottom-Right Front
            };

            VPNT[] LeftFace = 
            {
                new VPNT(Vertices[4], Normals[4], TextureCoordinates[1]),  //Top-Left Front
                new VPNT(Vertices[1], Normals[4], TextureCoordinates[2]),  //Bottom-Left Back
                new VPNT(Vertices[0], Normals[4], TextureCoordinates[3]),  //Bottom-Left Front
                new VPNT(Vertices[5], Normals[4], TextureCoordinates[0]),  //Top-Left Back
                new VPNT(Vertices[1], Normals[4], TextureCoordinates[2]),  //Bottom-Left Back
                new VPNT(Vertices[4], Normals[4], TextureCoordinates[1]),  //Top-Left Front
            };

            VPNT[] RightFace = 
            {
                new VPNT(Vertices[6], Normals[5], TextureCoordinates[0]),  //Top-Right Front
                new VPNT(Vertices[2], Normals[5], TextureCoordinates[2]),  //Bottom-Right Front
                new VPNT(Vertices[3], Normals[5], TextureCoordinates[3]),  //Bottom-Right Back
                new VPNT(Vertices[7], Normals[5], TextureCoordinates[1]),  //Top-Right Back
                new VPNT(Vertices[6], Normals[5], TextureCoordinates[0]),  //Top-Right Front
                new VPNT(Vertices[3], Normals[5], TextureCoordinates[3]),  //Bottom-Right Back
            };

            this.vertices.AddRange(FrontFace);
            this.vertices.AddRange(BackFace);
            this.vertices.AddRange(TopFace);
            this.vertices.AddRange(BottomFace);
            this.vertices.AddRange(LeftFace);
            this.vertices.AddRange(RightFace);
        }

        public Matrix CreateWorldMatrix()
        {
            return Matrix.CreateScale(this.Scale)
                 * Matrix.CreateRotationX(MathHelper.ToRadians(this.Rotation_X))
                 * Matrix.CreateRotationY(MathHelper.ToRadians(this.Rotation_Y))
                 * Matrix.CreateRotationZ(MathHelper.ToRadians(this.Rotation_Z))
                 * Matrix.CreateTranslation(this.Position);
        }

        public void Draw()
        {
            this.game.GraphicsDevice.SetVertexBuffer(this.vbo);
            this.game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, this.vbo.VertexCount / 3);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VPNT = Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture;

namespace Assignment_3D
{
    class CityBuilder
    {
        private Game game = null;
        private VertexBuffer vbo = null;
        private BoundingBox cityBoundaries;

        private List<VPNT> vertices = new List<VPNT>();
        private List<BoundingBox> buildingBoundaries = new List<BoundingBox>();

        public Vector3 Position { get; set; }
        public float Rotation_X { get; set; }
        public float Rotation_Y { get; set; }
        public float Rotation_Z { get; set; }
        public Vector3 Scale { get; set; }

        public CityBuilder(Game game, int[,] layout, int[] heights, float textureCount)
        {
            this.game = game;
            this.Scale = new Vector3(1.0f, 1.0f, 1.0f);
            this.Initialize(layout, heights, textureCount);
            this.CreateBuildingBounds(layout, heights);
            this.CreateCityBounds(layout, heights, 0);
            this.vbo = new VertexBuffer(this.game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, this.vertices.Count, BufferUsage.WriteOnly);
            this.vbo.SetData<VertexPositionNormalTexture>(this.vertices.ToArray());
            this.vertices = null;
        }

        private void Initialize(int[,] layout, int[] Heights, float TextureCount)
        {
            int Width = layout.GetLength(0);
            int Height = layout.GetLength(1);

            Vector3[] Normals = 
            {
                Vector3.UnitY,      //Top
                -Vector3.UnitY,     //Bottom
                -Vector3.Forward,   //Front
                Vector3.Forward,    //Back
                -Vector3.UnitX,     //Left
                Vector3.UnitX,      //Right
            };

            for (int X = 0; X < Width; ++X)
            {
                for (int Z = 0; Z < Height; ++Z)
                {
                    int index = layout[X, Z];

                    Vector3[] Vertices =
                    {
                        new Vector3(X,     Heights[index], -Z    ),
                        new Vector3(X,     Heights[index], -Z - 1),
                        new Vector3(X + 1, Heights[index], -Z    ),
                        new Vector3(X + 1, Heights[index], -Z - 1),

                        new Vector3(X,           0,        -Z    ),
                        new Vector3(X,           0,        -Z - 1),
                        new Vector3(X + 1,       0,        -Z    ),
                        new Vector3(X + 1,       0,        -Z - 1)
                    };

                    Vector2[] TextureCoordinates = 
                    {
                        new Vector2((index * 2 + 1) / TextureCount, 0),
                        new Vector2((index * 2 + 0) / TextureCount, 0),
                        new Vector2((index * 2 + 1) / TextureCount, 1),
                        new Vector2((index * 2 + 0) / TextureCount, 1),

                        new Vector2((index * 2 - 1) / TextureCount, 0),
                        new Vector2((index * 2 - 1) / TextureCount, 1)
                    };

                    VPNT[] TopFace =
                    {
                        new VPNT(Vertices[0], Normals[0], TextureCoordinates[3]),
                        new VPNT(Vertices[1], Normals[0], TextureCoordinates[1]),
                        new VPNT(Vertices[2], Normals[0], TextureCoordinates[2]),

                        new VPNT(Vertices[1], Normals[0], TextureCoordinates[1]),
                        new VPNT(Vertices[3], Normals[0], TextureCoordinates[0]),
                        new VPNT(Vertices[2], Normals[0], TextureCoordinates[2]),
                    };

                    this.vertices.AddRange(TopFace);

                    if (index != 0)
                    {
                        VPNT[] FrontFace = 
                        {
                            new VPNT(Vertices[7], Normals[3], TextureCoordinates[3]),
                            new VPNT(Vertices[1], Normals[3], TextureCoordinates[4]),
                            new VPNT(Vertices[5], Normals[3], TextureCoordinates[5]),

                            new VPNT(Vertices[1], Normals[3], TextureCoordinates[4]),
                            new VPNT(Vertices[7], Normals[3], TextureCoordinates[3]),
                            new VPNT(Vertices[3], Normals[3], TextureCoordinates[1])
                        };

                        VPNT[] BackFace = 
                        {
                            new VPNT(Vertices[6], Normals[2], TextureCoordinates[3]),
                            new VPNT(Vertices[4], Normals[2], TextureCoordinates[5]),
                            new VPNT(Vertices[0], Normals[2], TextureCoordinates[4]),

                            new VPNT(Vertices[0], Normals[2], TextureCoordinates[4]),
                            new VPNT(Vertices[2], Normals[2], TextureCoordinates[1]),
                            new VPNT(Vertices[6], Normals[2], TextureCoordinates[3])
                        };

                        VPNT[] LeftFace = 
                        {
                            new VPNT(Vertices[4], Normals[4], TextureCoordinates[3]),
                            new VPNT(Vertices[5], Normals[4], TextureCoordinates[5]),
                            new VPNT(Vertices[1], Normals[4], TextureCoordinates[4]),

                            new VPNT(Vertices[1], Normals[4], TextureCoordinates[4]),
                            new VPNT(Vertices[0], Normals[4], TextureCoordinates[1]),
                            new VPNT(Vertices[4], Normals[4], TextureCoordinates[3])
                        };

                        VPNT[] RightFace = 
                        {
                            new VPNT(Vertices[6], Normals[5], TextureCoordinates[3]),
                            new VPNT(Vertices[3], Normals[5], TextureCoordinates[4]),
                            new VPNT(Vertices[7], Normals[5], TextureCoordinates[5]),

                            new VPNT(Vertices[3], Normals[5], TextureCoordinates[4]),
                            new VPNT(Vertices[6], Normals[5], TextureCoordinates[3]),
                            new VPNT(Vertices[2], Normals[5], TextureCoordinates[1])
                        };

                        this.vertices.AddRange(FrontFace);
                        this.vertices.AddRange(BackFace);
                        this.vertices.AddRange(LeftFace);
                        this.vertices.AddRange(RightFace);
                    }
                }
            }            
        }

        private void CreateBuildingBounds(int[,] layout, int[] Heights)
        {
            int Width = layout.GetLength(0);
            int Height = layout.GetLength(1);

            for (int X = 0; X < Width; ++X)
            {
                for (int Z = 0; Z < Height; ++Z)
                {
                    int index = layout[X, Z];

                    if (index != 0)
                    {
                        int BuildingHeight = Heights[index];
                        Vector3[] buildingPoints =
                        {
                            new Vector3(X, 0, -Z), //Lower-Back-Left
                            new Vector3(X + 1, BuildingHeight, -Z - 1) //Upper-Front-Right
                        };

                        this.buildingBoundaries.Add(BoundingBox.CreateFromPoints(buildingPoints));
                    }
                }
            }
        }

        private void CreateCityBounds(int[,] layout, int[] Heights, int HeightOffset = 0)
        {
            int Width = layout.GetLength(0);
            int Height = layout.GetLength(1);
            int TallestBuilding = Heights.Max();

            Vector3[] cityPoints =
            {
                new Vector3(0, 0, 0),
                new Vector3(Width, TallestBuilding + HeightOffset, -Height)
            };

            this.cityBoundaries = BoundingBox.CreateFromPoints(cityPoints);
        }

        public BoundingBox CityBounds
        {
            get { return this.cityBoundaries; }
        }

        public List<BoundingBox> BuildingBounds
        {
            get { return this.buildingBoundaries; }
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

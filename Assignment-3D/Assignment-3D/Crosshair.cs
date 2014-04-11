using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Assignment_3D
{
    class Crosshair
    {
        private bool manualDraw = false;
        private Texture2D texture = null;
        private GraphicsDevice graphics = null;
        private int Width = 0, Height = 0;
        private static RasterizerState state = new RasterizerState();

        public Crosshair(GraphicsDevice graphics, int Width, int Height)
        {
            this.graphics = graphics;
            this.texture = new Texture2D(graphics, 1, 1);
            this.texture.SetData(new Color[]{Color.White});
            this.Width = Width;
            this.Height = Height;
            state.CullMode = CullMode.None;
            manualDraw = true;
        }

        public Crosshair(GraphicsDevice graphics, Texture2D texture)
        {
            this.graphics = graphics;
            this.texture = texture;
            this.Width = texture.Width;
            this.Height = texture.Height;
            state.CullMode = CullMode.None;
        }

        public void Draw(SpriteBatch spriteBatch, Color Tint)
        {
            spriteBatch.Draw(this.texture, new Vector2(this.graphics.Viewport.Width / 2, this.graphics.Viewport.Height / 2) + new Vector2(Width / 2, Height / 2), Tint);
            if (manualDraw)
                this.DrawCross(spriteBatch, Color.Red, 10, 10);
        }

        private void DrawCross(SpriteBatch spriteBatch, Color Tint, int size, int centerDist)
        {
            int X = this.graphics.Viewport.Width / 2;
            int Y = this.graphics.Viewport.Height / 2;
            spriteBatch.Draw(this.texture, new Rectangle(X, Y - size - centerDist, 1, size), Tint);
            spriteBatch.Draw(this.texture, new Rectangle(X, Y + centerDist + 1, 1, size), Tint);

            spriteBatch.Draw(this.texture, new Rectangle(X - size - centerDist, Y, size, 1), Tint);
            spriteBatch.Draw(this.texture, new Rectangle(X + centerDist + 1, Y, size, 1), Tint);
        }

        public void ResetGraphicsState()
        {
            this.graphics.RasterizerState = state;
            this.graphics.DepthStencilState = DepthStencilState.Default;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Assignment_3D
{
    class Character : ModelObject
    {
        private float HP = 100;

        public Character(ContentManager Content)
            : base(Content)
        {
        }

        public void Shot(float Damage)
        {
            this.HP -= Damage;
        }

        public bool isDead()
        {
            return this.HP <= 0;
        }

        public void resetHP()
        {
            this.HP = 100;
        }

        public override void Draw(Matrix View, Matrix Projection)
        {
            if (this.HP > 0)
            {
                base.Draw(View, Projection);
            }
        }
    }
}

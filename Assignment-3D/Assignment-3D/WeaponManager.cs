using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Assignment_3D
{
    class WeaponManager
    {
        private String[] modelNames = null;
        private GunModel[] models = null;

        public WeaponManager(ContentManager Content, String[] ModelNames)
        {
            this.modelNames = ModelNames;
            this.models = new GunModel[ModelNames.Length];

            for (int I = 0; I < ModelNames.Length; ++I)
            {
                this.models[I] = new GunModel(Content);
                this.models[I].Load(ModelNames[I]);
            }
        }

        public WeaponManager(ContentManager Content, String[] ModelNames, String[] BulletNames, String[] ShotNames, int[] BulletCounts)
        {
            this.modelNames = ModelNames;
            this.models = new GunModel[ModelNames.Length];

            for (int I = 0; I < ModelNames.Length; ++I)
            {
                this.models[I] = new GunModel(Content);
                this.models[I].Load(ModelNames[I], BulletNames[I], ShotNames[I], BulletCounts[I]);
            }
        }

        public GunModel GetModel(int Index)
        {
            return models[Index];
        }

        public GunModel GetModel(String name)
        {
            for (int I = 0; I < models.Length; ++I)
            {
                if (modelNames[I] == name)
                {
                    return models[I];
                }
            }
            return null;
        }

        public GunModel[] GetModels
        {
            get { return this.models; }
        }
    }
}

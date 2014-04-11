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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        City city = null;
        Camera cam = null;
        Skybox skybox = null;
        Character soldier = null;
        ModelObject officer = null;

        WeaponManager weaponManager = null;
        BasicEffect effect = null;
        int gunIndex = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1300;
            graphics.PreferredBackBufferHeight = 660;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            city = new City(this);
            effect = new BasicEffect(this.GraphicsDevice);

            cam = new Camera(this);
            cam.LookAt(MathHelper.ToDegrees(MathHelper.PiOver4), 0.05f, 1000.0f, new Vector3(11f, 0f, -7.5f), Vector3.Forward, Vector3.Up);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            weaponManager = new WeaponManager(Content, new String[] { "Weapons/pistol", "Weapons/mp5" }, new String[] { "Weapons/bullet", "Weapons/bullet" }, new String[] {"Sounds/9mm", "Sounds/mp5"}, new int[] {30, 16});
            GunModel gun = weaponManager.GetModel(0);
            gun.SetUpInfo(0.03f, 0.03f, -0.03f, 0.4f, 0.4f, 0.3f);
            gun.crosshair = new Crosshair(this.GraphicsDevice, 1, 1);

            gun = weaponManager.GetModel(1);
            gun.SetUpInfo(0.02f, 0.02f, 0.02f, 0.5f, 0.5f, 0.2f);
            gun.crosshair = new Crosshair(this.GraphicsDevice, 1, 1);
            skybox = new Skybox(this);

            soldier = new Character(Content);
            soldier.Load("Models/swat", "Models/swatdiffuse");
            soldier.Rotation_X = 180.0f;
            soldier.Rotation_Z = -90.0f;

            officer = new ModelObject(Content);
            officer.Load("Models/officer");
            officer.Scale = new Vector3(0.01f, 0.01f, 0.01f);
            officer.Rotation_X = 180.0f;

            cam.onMouseClick += new EventHandler(this.onMouseClick);
            cam.onKeyPress += new EventHandler(this.onKeyPress);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
        }

        private void onMouseClick(object sender, EventArgs e)
        {
            weaponManager.GetModel(gunIndex).Fire(cam);
        }

        private void onKeyPress(object sender, EventArgs e)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Q))
            {
                ++gunIndex;
                if (gunIndex >= weaponManager.GetModels.Length)
                {
                    gunIndex = 0;
                }
            }

            if (state.IsKeyDown(Keys.R))
            {
                soldier.resetHP();
                weaponManager.GetModel(gunIndex).ResetAmmo();
                cam.LookAt(MathHelper.ToDegrees(MathHelper.PiOver4), 0.05f, 1000.0f, new Vector3(11f, 0f, -7.5f), Vector3.Forward, Vector3.Up);
            }

            if (state.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            Vector3 velocity = cam.ProcessKeyboardInput(gameTime);
            BoundingBox soldierbox = new BoundingBox(new Vector3(soldier.Position.X - 0.3f, soldier.Position.Y, soldier.Position.Z - 0.2f), new Vector3(soldier.Position.X + 0.3f, soldier.Position.Y + 1.8f, soldier.Position.Z + 0.2f));

            Collider c = new Collider();

            if (c.isColliding(city.BuildingBounds, cam, new Vector3(3, 1, 3)) || c.isColliding(soldierbox, cam, new Vector3(3, 1, 3)))
            {
                velocity *= -3.0f;
            }

            foreach (Projectile bullet in weaponManager.GetModel(gunIndex).bullets)
            {
                if (!bullet.destroy)
                {
                    BoundingBox bulletbox = new BoundingBox(new Vector3(bullet.Position.X + 0.015f, bullet.Position.Y + 0.015f, bullet.Position.Z - 0.055f), new Vector3(bullet.Position.X - 0.015f, bullet.Position.Y - 0.015f, bullet.Position.Z + 0.07f));

                    if (soldierbox.Intersects(bulletbox))
                    {
                        bullet.destroy = true;
                        soldier.Shot(20.0f);
                    }
                    else
                    {
                        foreach (BoundingBox building in city.BuildingBounds)
                        {
                            if (building.Contains(bulletbox) == ContainmentType.Contains)
                            {
                                bullet.destroy = true;
                            }
                        }
                    }
                }
            }

            cam.Update(gameTime, velocity);
            weaponManager.GetModel(gunIndex).Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            const float skyboxfactor = 2.0f;
            Vector3 CityCenter = city.Center;
            skybox.Scale = new Vector3((city.GetWidth / 2) * skyboxfactor, (city.GetBuildingHeight) * skyboxfactor, (city.GetHeight / 2) * skyboxfactor);
            skybox.Position = new Vector3((city.GetWidth - CityCenter.X), 0, -(city.GetHeight - CityCenter.Z));
            
            skybox.Draw(this.cam.View, this.cam.Projection, this.cam.GetPosition);
            city.Draw(this.cam);

            weaponManager.GetModel(this.gunIndex).Position = this.cam.GetTarget;
            weaponManager.GetModel(this.gunIndex).Draw(this.cam.View, this.cam.Projection, this.cam.GetTarget, this.cam.Angle_X, this.cam.Angle_Y);

            soldier.Position = new Vector3(CityCenter.X, 0, -CityCenter.Z);
            soldier.Draw(this.cam.View, this.cam.Projection);

            //officer.Position = new Vector3(CityCenter.X - 1, 0, -CityCenter.Z);
            //officer.Draw(this.cam.View, this.cam.Projection);

            spriteBatch.Begin();
                weaponManager.GetModel(this.gunIndex).crosshair.Draw(spriteBatch, Color.White);
            spriteBatch.End();
            weaponManager.GetModel(this.gunIndex).crosshair.ResetGraphicsState();

            base.Draw(gameTime);
        }
    }
}

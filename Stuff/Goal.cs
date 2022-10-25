using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;


namespace PG.Stuff
{
    internal class Goal
    {
        private Vector3 position;
        private bool enabled;

        private Matrix view = Matrix.CreateLookAt(new Vector3(0, -15, 20), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
        private Matrix world;

        public Goal(float x,float y,float z)
        {
            position = new Vector3(x,y,z);
            enabled = false;
            world = Matrix.CreateTranslation(position);
        }

        public Boolean update(Vector3 playerposition, SoundEffect sound,  List<Coin> coins)
        {
            //visible only if all coins collected
            enabled = true;
            foreach (Coin coin in coins)
            {
                if (!coin.getState()){ enabled = false; }
            }

            //collision
            if (IsBetween(playerposition.X, position.X - 2, position.X + 2)
                && IsBetween(playerposition.Y, position.Y - 2, position.Y + 2)
                && enabled)
            {
                sound.Play();
                return true;
            }
            return false;
        }

        public void draw(Model model)
        {   
            if (enabled)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    mesh.Draw();
                }
            }
        }

        private bool IsBetween(float item, float start, float end)
        {
            return Comparer<float>.Default.Compare(item, start) >= 0
                && Comparer<float>.Default.Compare(item, end) <= 0;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace PG.Stuff
{
    internal class Enemy
    {
        private Vector3 position;
        private Boolean moveleft;
        private Boolean movehorizontal;

        private int i = 0;
        private int distance = 100;

        private Matrix view = Matrix.CreateLookAt(new Vector3(0, -15, 20), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
        private Matrix world;

        public Enemy(float x,float y,float z,String direction,int _distance)
        {   
            position = new Vector3(x,y,z);
            world = Matrix.CreateTranslation(position);
            distance = _distance;

            //what direction to move
            switch (direction)
            {
                case "down":
                    moveleft = true;
                    movehorizontal = false;
                    break;
                case "up":
                    moveleft = false;
                    movehorizontal = false;
                    break;
                case "right":
                    moveleft = false;
                    movehorizontal = true;
                    break;
                default:
                    moveleft = true;
                    movehorizontal = true;
                    break;
            }

        }

        public Boolean update(Vector3 playerposition, SoundEffect sound)
        {
            i++;

            //move routine
            if (i >= distance) 
            { 
                moveleft = !moveleft;
                i = 0;
            }

            //direction
            if (movehorizontal)
            {
                if (moveleft) { position -= new Vector3(0.1f, 0f, 0); }

                if (!moveleft) { position += new Vector3(0.1f, 0f, 0); }
            }

            else
            {
                if (moveleft) { position -= new Vector3(0f, 0.1f, 0); }

                if (!moveleft) { position += new Vector3(0f, 0.1f, 0); }
            }

            //collision
            if (IsBetween(playerposition.X, position.X - 2, position.X + 2) && IsBetween(playerposition.Y, position.Y - 2, position.Y + 2))
            {
                sound.Play();
                return true;
            }
            return false;
        }

        public void draw(Model model)
        {   
            world = Matrix.CreateTranslation(position);
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


        private bool IsBetween(float item, float start, float end)
        {
            return Comparer<float>.Default.Compare(item, start) >= 0
                && Comparer<float>.Default.Compare(item, end) <= 0;
        }
    }
}

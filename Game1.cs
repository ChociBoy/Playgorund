using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using PG.Stuff;
using static System.Formats.Asn1.AsnWriter;


namespace PG
{
    public class Game1 : Game
    {   
        //globals
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        int i = 0;
        float movespeed =0.15f;
        
        private Model cube;
        private Model sphere;
        private Model plane;
        private Model _coin;
        private Model goal;
        private Model spieler;
        private Model gegner;

        private List<Coin> coins = new List<Coin>();
        private List<Enemy> enemys = new List<Enemy>();
        private Goal g1;  

        List<SoundEffect> songsoundeffects= new List<SoundEffect>();

            //cam
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, -15, 20), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
        
            //player
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Vector3 position;

            //floor
        private Matrix worldGR = Matrix.CreateTranslation(new Vector3(0, 7, -10));

        #region 
        enum BState {HOVER,UP,JUST_RELEASED,DOWN}
        const int   BUTTON_HEIGHT = 40,
                    BUTTON_WIDTH = 88;
        Color button_color;
        Rectangle button_rectangle;
        BState button_state;
        Texture2D button_texture;
        double button_timer;

        //mouse pressed and mouse just pressed
        bool mpressed, prev_mpressed = false;

        //mouse location in window
        int mx, my;
        double frame_time;
        #endregion


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            #region
            //button
            int x = 5;
            int y = 5;
            button_state = BState.UP;
            button_color = Color.White;
            button_timer = 0.0;
            button_rectangle = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            y += BUTTON_HEIGHT;
            #endregion

            IsMouseVisible = true;

            //player
            position = new Vector3(0, -5, 0);

            //coins
            coins.Add(new Coin(10, 0, 0));
            coins.Add(new Coin(-10, 0, 0));
            coins.Add(new Coin(-10, -7, 0));
            coins.Add(new Coin(10, -7, 0));

            //enemys
            enemys.Add(new Enemy(5, 5, 0,"left",100));
            enemys.Add(new Enemy(6, 2, 0, "down",100));
            enemys.Add(new Enemy(-6, 2, 0, "down",100));

            g1 = new Goal(0, 10, 0);

            //sounds
            songsoundeffects.Add(Content.Load<SoundEffect>(@"sfx/collect"));
            songsoundeffects.Add(Content.Load<SoundEffect>(@"sfx/damage"));
            songsoundeffects.Add(Content.Load<SoundEffect>(@"sfx/goal"));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            #region
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            button_texture =
            Content.Load<Texture2D>(@"images/exit");
            #endregion

            //load models in variables
            cube = Content.Load<Model>(@"cube/cube");
            sphere = Content.Load<Model>(@"cube/cube");
            plane = Content.Load<Model>(@"plane/plane");
            _coin = Content.Load<Model>(@"coin/coin");
            goal = Content.Load<Model>(@"goal/_goal");
            spieler = Content.Load<Model>(@"spieler/spieler");
            gegner = Content.Load<Model>(@"gegner/gegner");

        }


        protected override void Update(GameTime gameTime)
        {
            #region
            // get elapsed frame time in seconds
            frame_time = gameTime.ElapsedGameTime.Milliseconds / 1000.0;

            // update mouse variables
            MouseState mouse_state = Mouse.GetState();
            mx = mouse_state.X;
            my = mouse_state.Y;
            prev_mpressed = mpressed;
            mpressed = mouse_state.LeftButton == ButtonState.Pressed;

            update_buttons();
            #endregion
            
            //coins
            foreach (Coin coin in coins)
            {
                coin.update(position, songsoundeffects[0]);
            }

            //enemys
            foreach (Enemy enemy in enemys)
            {
                if (enemy.update(position, songsoundeffects[1])) 
                { 
                    position = new Vector3(0, -5, 0);
                    foreach (Coin coin in coins)
                    {
                        coin.setState(false);
                    }
                }

            }

            //goal
            if (g1.update(position, songsoundeffects[2], coins)) 
            {
                position = new Vector3(0, -5, 0);
                foreach (Coin coin in coins)
                {
                    coin.setState(false);
                }
            }


            //controlls
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                position -= new Vector3(0f, movespeed, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                position += new Vector3(0f, movespeed, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                position += new Vector3(movespeed, 0f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                position -= new Vector3(movespeed, 0f, 0);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //floor
            DrawModel(plane, worldGR, view, projection);

            //goal
            g1.draw(goal);

            //coins
            foreach (Coin coin in coins)
            {
                coin.draw(_coin);
            }

            //enemys
            foreach (Enemy enemy in enemys)
            {
                enemy.draw(gegner);
            } 

            //player
            world = Matrix.CreateTranslation(position);
            DrawModel(spieler, world, view, projection);

            #region
            _spriteBatch.Begin();
            _spriteBatch.Draw(button_texture, button_rectangle, button_color);
            _spriteBatch.End();
            #endregion

            base.Draw(gameTime);
        }

        //other methods
        private bool IsBetween(float item, float start, float end)
        {
            return Comparer<float>.Default.Compare(item, start) >= 0
                && Comparer<float>.Default.Compare(item, end) <= 0;
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
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

        #region

        //button stuff (copy paste)

        // wrapper for hit_image_alpha taking Rectangle and Texture
        Boolean hit_image_alpha(Rectangle rect, Texture2D tex, int x, int y)
        {
            return hit_image_alpha(0, 0, tex, tex.Width * (x - rect.X) /
                rect.Width, tex.Height * (y - rect.Y) / rect.Height);
        }

        // wraps hit_image then determines if hit a transparent part of image 
        Boolean hit_image_alpha(float tx, float ty, Texture2D tex, int x, int y)
        {
            if (hit_image(tx, ty, tex, x, y))
            {
                uint[] data = new uint[tex.Width * tex.Height];
                tex.GetData<uint>(data);
                if ((x - (int)tx) + (y - (int)ty) *
                    tex.Width < tex.Width * tex.Height)
                {
                    return ((data[
                        (x - (int)tx) + (y - (int)ty) * tex.Width
                        ] &
                                0xFF000000) >> 24) > 20;
                }
            }
            return false;
        }

        // determine if x,y is within rectangle formed by texture located at tx,ty
        Boolean hit_image(float tx, float ty, Texture2D tex, int x, int y)
        {
            return (x >= tx &&
                x <= tx + tex.Width &&
                y >= ty &&
                y <= ty + tex.Height);
        }

        // determine state and color of button
        void update_buttons()
        {
                if (hit_image_alpha(
                    button_rectangle, button_texture, mx, my))
                {
                    button_timer = 0.0;
                    if (mpressed)
                    {
                        // mouse is currently down
                        button_state = BState.DOWN;
                        button_color = Color.LightBlue;
                    }
                    else if (!mpressed && prev_mpressed)
                    {
                        // mouse was just released
                        if (button_state == BState.DOWN)
                        {
                            // button i was just down
                            button_state = BState.JUST_RELEASED;
                        }
                    }
                    else
                    {
                        button_state = BState.HOVER;
                        button_color = Color.Gray;
                    }
                }
                else
                {
                    button_state = BState.UP;
                    if (button_timer > 0)
                    {
                        button_timer = button_timer - frame_time;
                    }
                    else
                    {
                        button_color = Color.White;
                    }
                }

                if (button_state == BState.JUST_RELEASED)
                {
                    take_action_on_button(i);
                }
            
        }


        // Logic for each button click goes here
        void take_action_on_button(int i)
        {
            //take action corresponding to which button was clicked
            Exit();
        }
        #endregion
    }
}
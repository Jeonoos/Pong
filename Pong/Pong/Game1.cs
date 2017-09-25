using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System;

namespace Pong
{

    public class Game1 : Game
    {
        enum Gamestate
        {
            Menu,Game,GameOver,Pauze
        }
        Gamestate gamestate = Gamestate.Menu;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D speler1tex, speler2tex, balltex, harttex, startButtontex, restartTexttex, p1Wintex, p2Wintex;
        GameTime GameTime = new GameTime();
        Bar player1, player2;
        Ball ball;
        int screenWidth = 1280;
        int screenHeight = 720;
        Random randgen;
        float PowerUpTimer = 0;
        String winner;
        KeyboardState kstate;
        KeyboardState oldkstate;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            randgen = new Random();
        }

        void BuildNewBall() {
            Vector2 tempdirection = new Vector2(1, (float)randgen.NextDouble() * 2 - 1);
            //Vector2 tempdirection = new Vector2(1, 0);
            tempdirection.Normalize();
            if (randgen.Next(0, 2) == 0)
                tempdirection.X = -tempdirection.X;
            ball = new Ball(new Vector2(screenWidth / 2, screenHeight / 2), tempdirection, 0.5f, balltex, Vector2.One);
            ball.size = Vector2.One * 1.5f;
        }


        protected override void Initialize()
        {
            base.Initialize();

            player1 = new Bar(new Vector2(speler1tex.Width / 2 + 10, screenHeight / 2), Vector2.One, speler1tex);
            player2 = new Bar(new Vector2(screenWidth - speler2tex.Width / 2 - 10, screenHeight / 2), Vector2.One, speler2tex);
            BuildNewBall();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            speler1tex = Content.Load<Texture2D>("blauweSpeler");
            speler2tex = Content.Load<Texture2D>("rodeSpeler");
            balltex = Content.Load<Texture2D>("bal");
            harttex = Content.Load<Texture2D>("hart");
            startButtontex = Content.Load<Texture2D>("startButton");
            restartTexttex = Content.Load<Texture2D>("restartText");
            p1Wintex = Content.Load<Texture2D>("player1Win");
            p2Wintex = Content.Load<Texture2D>("player2Win");
            //PowerUp.textures[0] = Content.Load<Texture2D>("PowerUpSize");

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            oldkstate = kstate;
            kstate = Keyboard.GetState();
            switch (gamestate)
            {
                case Gamestate.Menu:
                    if (kstate.IsKeyDown(Keys.Space))
                        gamestate = Gamestate.Game;
                    break;
                case Gamestate.Pauze:
                    if (kstate.IsKeyDown(Keys.Space) && !oldkstate.IsKeyDown(Keys.Space))
                        gamestate = Gamestate.Game;
                    break;
                case Gamestate.Game:

                    if (kstate.IsKeyDown(Keys.Space) && !oldkstate.IsKeyDown(Keys.Space))
                        gamestate = Gamestate.Pauze;

                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Exit();
                    //p1 AI
                    if (ball.position.Y- 50 > player1.position.Y)
                        player1.position += new Vector2(0, gameTime.ElapsedGameTime.Milliseconds);
                    if (ball.position.Y + 50 < player1.position.Y)
                        player1.position += new Vector2(0, -gameTime.ElapsedGameTime.Milliseconds);

                    //p2 AI
                    if (ball.position.Y - 50 > player2.position.Y)
                        player2.position += new Vector2(0, gameTime.ElapsedGameTime.Milliseconds);
                    if (ball.position.Y + 50 < player2.position.Y)
                        player2.position += new Vector2(0, -gameTime.ElapsedGameTime.Milliseconds);

                    if (kstate.IsKeyDown(Keys.S) && player1.position.Y < screenHeight - player1.tex.Height / 2 * player1.size.Y)
                        player1.position += new Vector2(0, gameTime.ElapsedGameTime.Milliseconds);
                    if (kstate.IsKeyDown(Keys.W) && player1.position.Y > player1.tex.Height / 2 * player1.size.Y)
                        player1.position += new Vector2(0, -gameTime.ElapsedGameTime.Milliseconds);
                    if (kstate.IsKeyDown(Keys.Down) && player2.position.Y < screenHeight - player2.tex.Height / 2 * player2.size.Y)
                        player2.position += new Vector2(0, gameTime.ElapsedGameTime.Milliseconds);
                    if (kstate.IsKeyDown(Keys.Up) && player2.position.Y > player2.tex.Height / 2 * player2.size.Y)
                        player2.position += new Vector2(0, -gameTime.ElapsedGameTime.Milliseconds);

                    //ball physics
                    //linkerkant
                    if (ball.position.X + ball.direction.X * ball.speed - ball.tex.Width / 2 < player1.position.X + player1.tex.Width && ball.position.Y <= player1.position.Y + player1.tex.Height / 2 * player1.size.Y && ball.position.Y >= player1.position.Y - player1.tex.Height / 2 * player1.size.Y && ball.direction.X < 0)
                    {
                        ball.direction.X = -ball.direction.X;
                        ball.direction.Y = (ball.position.Y - player1.position.Y) / player1.tex.Height * ball.speed;
                        ball.direction.Normalize();
                        ball.speed += 0.05f;
                    }
                    //rechterkant
                    if (ball.position.X + ball.direction.X * ball.speed + ball.tex.Width / 2 > player2.position.X - player1.tex.Width && ball.position.Y <= player2.position.Y + player2.tex.Height / 2 * player2.size.Y && ball.position.Y >= player2.position.Y - player2.tex.Height / 2 * player2.size.Y && ball.direction.X > 0)
                    {
                        ball.direction.X = -ball.direction.X;
                        ball.direction.Y = (ball.position.Y - player2.position.Y) / player2.tex.Height * ball.speed;
                        ball.direction.Normalize();
                        ball.speed += 0.05f;
                    }
                    //
                    if (ball.position.X >= screenWidth && ball.direction.X > 0)
                    {
                        BuildNewBall();
                        player2.lives -= 1;
                        if (player2.lives <= 0)
                            GameOver("Player 1");
                    }
                    if (ball.position.X <= 0 && ball.direction.X < 0)
                    {
                        BuildNewBall();
                        player1.lives -= 1;
                        if (player1.lives <= 0)
                            GameOver("Player 2");
                    }
                    if (ball.position.Y <= 0 + ball.tex.Height / 2 * ball.size.Y)
                        ball.direction.Y = -ball.direction.Y;
                    if (ball.position.Y >= screenHeight - ball.tex.Height / 2 * ball.size.Y)
                        ball.direction.Y = -ball.direction.Y;

                   // if (kstate.IsKeyDown(Keys.Space))
                    //{
                    //    player1.size += new Vector2(0, 0.001f * gameTime.ElapsedGameTime.Milliseconds);
                    //}
                    ball.position += ball.direction * ball.speed * gameTime.ElapsedGameTime.Milliseconds;

                    PowerUpTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (PowerUpTimer > 5000)
                    {
                        PowerUp powerup = new PowerUp(new Vector2(randgen.Next(screenWidth), randgen.Next(screenHeight)), PowerUp.PowerUpType.Size);
                    }
                    break;
                case Gamestate.GameOver:
                    if (kstate.IsKeyDown(Keys.Space))
                        gamestate = Gamestate.Game;
                    break;
                default:
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            switch (gamestate)
            {
                case Gamestate.Menu:
                    spriteBatch.Draw(startButtontex, new Vector2(screenWidth / 2 - startButtontex.Width / 2, screenHeight / 2 - startButtontex.Height / 2), Color.White);
                    spriteBatch.Draw(restartTexttex, new Vector2(screenWidth / 2 - restartTexttex.Width / 2, screenHeight / 2 - restartTexttex.Height / 2), Color.White);
                    break;
                case Gamestate.Pauze:
                    spriteBatch.Draw(restartTexttex, new Vector2(screenWidth / 2 - restartTexttex.Width / 2, screenHeight / 2 - restartTexttex.Height / 2), Color.White);
                    break;
                case Gamestate.Game:
                    spriteBatch.Draw(player1.tex, player1.position, null, Color.White, 0, player1.origin, player1.size, SpriteEffects.None, 0);
                    spriteBatch.Draw(player2.tex, player2.position, null, Color.White, 0, player2.origin, player2.size, SpriteEffects.None, 0);
                    spriteBatch.Draw(ball.tex, ball.position, null, Color.White, 0, ball.origin, ball.size, SpriteEffects.None, 0);
                    for (int i = 0; i < player1.lives; i++)
                    {
                        spriteBatch.Draw(balltex, new Vector2(40 - balltex.Width/2 + 24 * i, 40), Color.White);
                    }
                    for (int i = 0; i < player2.lives; i++)
                    {
                        spriteBatch.Draw(balltex, new Vector2(screenWidth - 40 - balltex.Width/2 - 24 * i, 40), Color.White);
                    }
                    break;
                case Gamestate.GameOver:
                    spriteBatch.Draw(restartTexttex, new Vector2(screenWidth / 2 - restartTexttex.Width / 2, screenHeight / 2 - restartTexttex.Height / 2), Color.White);
                    if (winner == "Player 1")
                        spriteBatch.Draw(p1Wintex, new Vector2(screenWidth / 2 - p1Wintex.Width / 2, screenHeight / 2 - p1Wintex.Height / 2), Color.White);
                    else
                        spriteBatch.Draw(p2Wintex, new Vector2(screenWidth / 2 - p2Wintex.Width / 2, screenHeight / 2 - p2Wintex.Height / 2), Color.White);
                    break;
                default:
                    break;
            }
            spriteBatch.End();


            base.Draw(gameTime);
        }

        void GameOver(String winnaar) {
            Initialize();
            winner = winnaar;
            gamestate = Gamestate.GameOver;
        }
    }

    public class Bar {
        public int lives = 3;
        public Vector2 position;
        public Vector2 size;
        public Texture2D tex;
        public Vector2 origin;

        public Bar(Vector2 position_, Vector2 size_, Texture2D tex_) {
            tex = tex_;
            position = position_;
            size = size_;
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
        }
    }

    public class Ball {
        public Vector2 position;
        public Vector2 direction;
        public Texture2D tex;
        public float speed;
        public Vector2 size;
        public Vector2 origin;
        public Ball(Vector2 position_, Vector2 direction_, float speed_, Texture2D tex_, Vector2 size_)
        {
            position = position_;
            direction = direction_;
            speed = speed_;
            tex = tex_;
            size = size_;
            origin = new Vector2(tex.Width / 2, tex.Height / 2);
        }
    }

    public class PowerUp{
        public Vector2 position;
        public static Texture2D[] textures;
        public enum PowerUpType {Size};
        public PowerUpType type;

        public PowerUp(Vector2 position, PowerUpType type) {
            this.position = position;
            this.type = type;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong
{

    public class Game1 : Game
    {
        enum Gamestate
        {
            Menu,Game,GameOver,Pauze
        }
        enum InputMethod {
            Manual, EasyAI, AdvancedAI
        }
        InputMethod p1Input = InputMethod.Manual;
        InputMethod p2Input = InputMethod.Manual;
        Gamestate gamestate = Gamestate.Menu;
        GraphicsDeviceManager graphics;
        SpriteFont defaultFont;
        SpriteBatch spriteBatch;
        Texture2D spelerTex,balltex, harttex, startButtontex, restartTexttex, p1Wintex, p2Wintex, manualtex, easyAItex, advancedAItex, WStex, UDtex;
        GameTime GameTime = new GameTime();
        Bar player1, player2;
        Ball ball;
        public static int screenWidth = 1280;
        public static int screenHeight = 720;
        float gametimer = 0;
        Random randgen;
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
            tempdirection.Normalize();
            if (randgen.Next(0, 2) == 0)
                tempdirection.X = -tempdirection.X;
            ball = new Ball(new Vector2(screenWidth / 2, screenHeight / 2), tempdirection, 0.75f, balltex, Vector2.One);
            ball.size = Vector2.One * 1.5f;
        }

        void StartNewGame() {
            gametimer = 0;
            player1 = new Bar(new Vector2(spelerTex.Width / 2 + 10, screenHeight / 2), Vector2.One, spelerTex);
            player2 = new Bar(new Vector2(screenWidth - spelerTex.Width / 2 - 10, screenHeight / 2), Vector2.One, spelerTex);
            BuildNewBall();
        }

        protected override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spelerTex = Content.Load<Texture2D>("Speler");
            balltex = Content.Load<Texture2D>("bal");
            harttex = Content.Load<Texture2D>("hart");
            startButtontex = Content.Load<Texture2D>("startButton");
            restartTexttex = Content.Load<Texture2D>("restartText");
            p1Wintex = Content.Load<Texture2D>("player1Win");
            p2Wintex = Content.Load<Texture2D>("player2Win");
            manualtex = Content.Load<Texture2D>("ManualInput");
            easyAItex = Content.Load<Texture2D>("EasyAI");
            advancedAItex = Content.Load<Texture2D>("AdvancedAI");
            WStex = Content.Load<Texture2D>("SelectWS");
            UDtex = Content.Load<Texture2D>("SelectUD");

            defaultFont = Content.Load<SpriteFont>("Font");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        float predictBallY(float x) {
            float xDist = x - ball.position.X;
            xDist += (xDist > 0) ? -spelerTex.Width / 2 : spelerTex.Width / 2;
            float dY = ball.direction.Y / ball.direction.X * xDist;
            float endY = dY + ball.position.Y;
            while (endY >= screenHeight - ball.tex.Height / 2 * ball.size.Y || endY <= ball.tex.Height / 2 * ball.size.Y) {
                if (endY >= screenHeight - ball.tex.Height / 2 * ball.size.Y) {
                    endY = (screenHeight - ball.tex.Height / 2 * ball.size.Y) - (endY - (screenHeight - ball.tex.Height / 2 * ball.size.Y));
                }
                if (endY <= ball.tex.Height / 2 * ball.size.Y) {
                    endY = -endY + ball.tex.Height * ball.size.Y;
                }
            }
            return endY;
        }

        Vector2 ManualInput(Bar player, GameTime gameTime) {
            Vector2 movement = Vector2.Zero;
            oldkstate = kstate;
            kstate = Keyboard.GetState();
            if (player == player1) { 
                if (kstate.IsKeyDown(Keys.S))
                    movement = new Vector2(0, gameTime.ElapsedGameTime.Milliseconds);
                if (kstate.IsKeyDown(Keys.W))
                    movement = new Vector2(0, -gameTime.ElapsedGameTime.Milliseconds);
            }
            if (player == player2)
            {
                if (kstate.IsKeyDown(Keys.Down))
                    movement = new Vector2(0, gameTime.ElapsedGameTime.Milliseconds);
                if (kstate.IsKeyDown(Keys.Up))
                    movement = new Vector2(0, -gameTime.ElapsedGameTime.Milliseconds);
            }
            return movement;
        }

        Vector2 BasicAI(Bar player, GameTime gameTime) {
            Vector2 movement = Vector2.Zero;
            if (ball.position.Y - 50 > player.position.Y + 20)
                movement= new Vector2(0, gameTime.ElapsedGameTime.Milliseconds);
            if (ball.position.Y + 50 < player.position.Y - 20)
                movement= new Vector2(0, -gameTime.ElapsedGameTime.Milliseconds);
            if (Math.Abs(ball.position.Y - player.position.Y) < gameTime.ElapsedGameTime.Milliseconds)
            {
                movement.Y = ball.position.Y - player.position.Y;
            }
            if (ball.position.Y == ball.position.X) {
                movement.Y = gameTime.ElapsedGameTime.Milliseconds;
            }
            return movement;
        }

        Vector2 AdvancedAI(Bar player, GameTime gameTime) {
            Vector2 movement = Vector2.Zero;
            float offset = (predictBallY(player.position.X) / screenHeight * 2 - 1) * player.tex.Height /2;
            float GoToY = 0f;
            if (ball.position.X > player.position.X && ball.direction.X < 0 || ball.position.X < player.position.X && ball.direction.X > 0){

                GoToY = predictBallY(player.position.X) + offset;
            }else{
                if (player.position.X < ball.position.X)
                    GoToY = predictBallY(screenWidth * 2);
                else
                    GoToY = predictBallY(-screenWidth);
            }
            if (GoToY > player.position.Y + 30 && player.position.Y < screenHeight - player.tex.Height / 2 * player.size.Y)
                movement = new Vector2(0, gameTime.ElapsedGameTime.Milliseconds);
            if (GoToY < player.position.Y - 30 && player.position.Y > player.tex.Height / 2 * player.size.Y)
                movement = new Vector2(0, -gameTime.ElapsedGameTime.Milliseconds);
            return movement;
        }

        InputMethod nextMethod(InputMethod thismethod) {
            switch (thismethod)
            {
                case InputMethod.Manual: return InputMethod.EasyAI;
                case InputMethod.EasyAI: return InputMethod.AdvancedAI;
                case InputMethod.AdvancedAI: return InputMethod.Manual;
                default: return thismethod;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            oldkstate = kstate;
            kstate = Keyboard.GetState();
            switch (gamestate)
            {
                case Gamestate.Menu:

                    if (kstate.IsKeyDown(Keys.Escape) && !oldkstate.IsKeyDown(Keys.Escape))
                        Exit();

                    if (kstate.IsKeyDown(Keys.Space)) {
                    gamestate = Gamestate.Game;
                    StartNewGame();
                    }

                    if (kstate.IsKeyDown(Keys.W) && !oldkstate.IsKeyDown(Keys.W))
                        p1Input = nextMethod(p1Input);
                    if (kstate.IsKeyDown(Keys.S) && !oldkstate.IsKeyDown(Keys.S))
                        p1Input = nextMethod(nextMethod(p1Input));
                    if (kstate.IsKeyDown(Keys.Up) && !oldkstate.IsKeyDown(Keys.Up))
                        p2Input = nextMethod(p2Input);
                    if (kstate.IsKeyDown(Keys.Down) && !oldkstate.IsKeyDown(Keys.Down))
                        p2Input = nextMethod(nextMethod(p2Input));
                    break;
                case Gamestate.Pauze:
                    if (kstate.IsKeyDown(Keys.Space) && !oldkstate.IsKeyDown(Keys.Space))
                        gamestate = Gamestate.Game;
                    if (kstate.IsKeyDown(Keys.Escape))
                    {
                        gamestate = Gamestate.Menu;
                    }
                    break;
                case Gamestate.Game:
                    gametimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (kstate.IsKeyDown(Keys.Space) && !oldkstate.IsKeyDown(Keys.Space))
                        gamestate = Gamestate.Pauze;

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        gamestate = Gamestate.Menu;
                    switch (p1Input)
                    {
                        case InputMethod.Manual: player1.position += ManualInput(player1, gameTime) * player1.speed; break;
                        case InputMethod.EasyAI: player1.position += BasicAI(player1, gameTime) * player1.speed; break;
                        case InputMethod.AdvancedAI: player1.position += AdvancedAI(player1, gameTime) * player1.speed; break;
                    }
                    switch (p2Input)
                    {
                        case InputMethod.Manual: player2.position += ManualInput(player2, gameTime) * player2.speed; break;
                        case InputMethod.EasyAI: player2.position += BasicAI(player2, gameTime) * player2.speed; break;
                        case InputMethod.AdvancedAI: player2.position += AdvancedAI(player2, gameTime) * player2.speed; break;
                    }

                //ball physics

                    //boven- en onderkant
                    ball.position += ball.direction * ball.speed * gameTime.ElapsedGameTime.Milliseconds;

                    if (ball.position.Y <= 0 + ball.tex.Height / 2 * ball.size.Y)
                        ball.direction.Y = Math.Abs(ball.direction.Y);
                    if (ball.position.Y >= screenHeight - ball.tex.Height / 2 * ball.size.Y)
                        ball.direction.Y = -Math.Abs(ball.direction.Y);

                    //linkerkant
                    if (ball.position.X + ball.direction.X * ball.speed - ball.tex.Width / 2 < player1.position.X + player1.tex.Width / 2 && ball.position.Y - ball.tex.Height / 2 <= player1.position.Y + player1.tex.Height / 2 * player1.size.Y && ball.position.Y + ball.tex.Height / 2 >= player1.position.Y - player1.tex.Height / 2 * player1.size.Y && ball.direction.X < 0)
                    {
                        ball.position.X = player1.position.X + player1.tex.Width / 2;
                        ball.direction.X = -ball.direction.X;
                        ball.direction.Y = (ball.position.Y - player1.position.Y) / player1.tex.Height;
                        ball.direction.Normalize();
                        ball.speed += 0.05f;
                    }
                    //rechterkant
                    if (ball.position.X + ball.direction.X * ball.speed + ball.tex.Width / 2 > player2.position.X - player1.tex.Width / 2 && ball.position.Y - ball.tex.Height / 2 <= player2.position.Y + player2.tex.Height / 2 * player2.size.Y && ball.position.Y + ball.tex.Height / 2 >= player2.position.Y - player2.tex.Height / 2 * player2.size.Y && ball.direction.X > 0)
                    {
                        ball.position.X = player2.position.X - player2.tex.Width / 2;
                        ball.direction.X = -ball.direction.X;
                        ball.direction.Y = (ball.position.Y - player2.position.Y) / player2.tex.Height;
                        ball.direction.Normalize();
                        ball.speed += 0.05f;
                    }
                    //gescoord
                    if (ball.position.X >= screenWidth && ball.direction.X > 0)
                    {
                        BuildNewBall();
                        player2.lives -= 1;
                        if (player2.lives <= 0)
                            gamestate = Gamestate.GameOver;
                    }
                    if (ball.position.X <= 0 && ball.direction.X < 0)
                    {
                        BuildNewBall();
                        player1.lives -= 1;
                        if (player1.lives <= 0)
                            gamestate = Gamestate.GameOver;
                    }
                    break;
                case Gamestate.GameOver:
                    if (kstate.IsKeyDown(Keys.Space)) {
                        gamestate = Gamestate.Game;
                        StartNewGame();
                    }
                    if (kstate.IsKeyDown(Keys.Escape))
                        gamestate = Gamestate.Menu;
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
                    string modeText1 = "";
                    string modeText2 = "";
                    spriteBatch.Draw(startButtontex, new Vector2(screenWidth / 2 - startButtontex.Width / 2, screenHeight / 2 - startButtontex.Height / 2), Color.White);
                    switch (p1Input) {
                        case InputMethod.Manual: spriteBatch.Draw(manualtex, new Vector2(50, screenHeight / 2 - manualtex.Height / 2), Color.White);modeText1 = "Manual"; break;
                        case InputMethod.EasyAI: spriteBatch.Draw(easyAItex, new Vector2(50, screenHeight / 2 - easyAItex.Height / 2), Color.White);modeText1 = "Easy AI";break;
                        case InputMethod.AdvancedAI: spriteBatch.Draw(advancedAItex, new Vector2(50, screenHeight / 2 - advancedAItex.Height / 2), Color.White);modeText1 = "Advanced AI";break;
                    }
                    spriteBatch.Draw(WStex, new Vector2(50, screenHeight / 2 - WStex.Height / 2), Color.White);
                    switch (p2Input)
                    {
                        case InputMethod.Manual: spriteBatch.Draw(manualtex, new Vector2(screenWidth / 5*4 - manualtex.Width / 2 - 40, screenHeight / 2 - manualtex.Height / 2), Color.White); modeText2 = "Manual"; break;
                        case InputMethod.EasyAI: spriteBatch.Draw(easyAItex, new Vector2(screenWidth / 5*4 - easyAItex.Width / 2 - 40, screenHeight / 2 - easyAItex.Height / 2), Color.White); modeText2 = "Easy AI"; break;
                        case InputMethod.AdvancedAI: spriteBatch.Draw(advancedAItex, new Vector2(screenWidth / 5*4 - advancedAItex.Width / 2 - 40, screenHeight / 2 - advancedAItex.Height / 2), Color.White); modeText2 = "Advanced AI"; break;
                    }
                    spriteBatch.Draw(UDtex, new Vector2(screenWidth / 5 * 4 - UDtex.Width / 2 - 40, screenHeight / 2 - UDtex.Height / 2), Color.White);
                    spriteBatch.DrawString(defaultFont, modeText1, new Vector2(170, screenHeight / 2 - 20), Color.White);
                    spriteBatch.DrawString(defaultFont, modeText2, new Vector2(screenWidth / 5 * 4 + 25, screenHeight / 2 - 20), Color.White);
                    break;
                case Gamestate.Pauze:
                    DrawGame(Color.DarkGray);
                    spriteBatch.Draw(restartTexttex, new Vector2(screenWidth / 2 - restartTexttex.Width / 2, screenHeight / 2 - restartTexttex.Height / 2), Color.White);

                    break;
                case Gamestate.Game:
                    DrawGame(Color.White);
                    if (gametimer < 1500)
                        spriteBatch.DrawString(defaultFont, "Press Space to pause", new Vector2(screenWidth/2 - defaultFont.MeasureString("Press Space to pause").X/2, screenHeight / 2 + 200), Color.White * (1-(gametimer/1500)));

                    break;
                case Gamestate.GameOver:
                    spriteBatch.DrawString(defaultFont, string.Format("That game took {0} seconds!",gametimer/1000), new Vector2(screenWidth / 2 - defaultFont.MeasureString(string.Format("That game took {0} seconds!", gametimer / 1000)).X / 2, screenHeight / 2), Color.White);

                    spriteBatch.Draw(restartTexttex, new Vector2(screenWidth / 2 - restartTexttex.Width / 2, screenHeight / 2 - restartTexttex.Height / 2), Color.White);
                    if (player1.lives > 0)
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

        void DrawGame(Color color) {
            spriteBatch.Draw(player1.tex, player1.position, null, color, 0, player1.origin, player1.size, SpriteEffects.None, 0);
            spriteBatch.Draw(player2.tex, player2.position, null, color, 0, player2.origin, player2.size, SpriteEffects.None, 0);
            spriteBatch.Draw(ball.tex, ball.position, null, color, 0, ball.origin, ball.size, SpriteEffects.None, 0);
            for (int i = 0; i < player1.lives; i++)
            {
                spriteBatch.Draw(balltex, new Vector2(40 - balltex.Width / 2 + 24 * i, 40), color);
            }
            for (int i = 0; i < player2.lives; i++)
            {
                spriteBatch.Draw(balltex, new Vector2(screenWidth - 40 - balltex.Width / 2 - 24 * i, 40), color);
            }
        }
    }
}

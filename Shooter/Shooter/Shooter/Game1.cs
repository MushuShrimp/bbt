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
using Microsoft.Xna.Framework.Input.Touch;

namespace Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        PickUp pickUp;

        // Represents the player 
        Player player;
        Player player2;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Gamepad states used to determine button presses
        GamePadState currentGamePadState;
        GamePadState previousGamePadState;

        // A movement speed for the player
        

        // Image used to display the static background
        Texture2D mainBackground;

        // Parallaxing Layers
        ParallaxingBackground bgLayer1;
        ParallaxingBackground bgLayer2;

        // Enemies
        Texture2D enemyTexture;
        List<Enemy> enemies;

        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;

        List<PickUp> pickUps;
        Texture2D pickUp1;
        Texture2D pickUp2;
        Texture2D pickUp3;
        Texture2D pickUp4;

        // A random number generator
        Random random;

        Texture2D projectileTexture;
        Texture2D projectileTexture2;
        //List<Projectile> projectiles;
        Projectile projectile;
        Projectile projectile2;

        // The rate of fire of the player laser
        TimeSpan fireTime;
        TimeSpan previousPickupSpawnTime;
        TimeSpan pickupsSpawnTime;

        // Explosion graphics list
        Texture2D explosionTexture;
        List<Animation> explosions;

        // The sound that is played when a laser is fired
        SoundEffect laserSound;


        // The sound used when the player or an enemy dies
        SoundEffect explosionSound;


        // The music played during gameplay
        Song gameplayMusic;

        //Number that holds the player score
        // The font used to display UI elements
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 900;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {


            //Initialize the player class
            player = new Player();
            player2 = new Player();

            // Set a constant player move speed
            
            pickUps = new List<PickUp> ();

            //Enable the FreeDrag gesture.
            TouchPanel.EnabledGestures = GestureType.FreeDrag;

            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();

            // Initialize the enemies list
            enemies = new List<Enemy>();

            // Set the time keepers to zero
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            pickupsSpawnTime = TimeSpan.FromSeconds(1.0f);

            // Initialize our random number generator
            random = new Random();

           // projectiles = new List<Projectile>();
            projectile = new Projectile();
            projectile2 = new Projectile();

            // Set the laser to fire every quarter second
            fireTime = TimeSpan.FromSeconds(.15f);

            // Initialize the explosion list
            explosions = new List<Animation>();

            //Set player's score to zero

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the player resources
            Animation playerAnimation = new Animation();
            Texture2D playerTexture = Content.Load<Texture2D>("shipAnimation");
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.Red, 1f, true);

            Animation playerAnimation2 = new Animation();
            playerAnimation2.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.Blue, 1f, true);


            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 50, GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(playerAnimation, playerPosition);

            Vector2 playerPosition2 = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 1000, GraphicsDevice.Viewport.TitleSafeArea.Y
            + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player2.Initialize(playerAnimation2, playerPosition2);

            pickUp1 = Content.Load<Texture2D>("pickUp1");
            pickUp2 = Content.Load<Texture2D>("pickUp2");
            pickUp3 = Content.Load<Texture2D>("pickUp3");
            pickUp4 = Content.Load<Texture2D>("pickUp4");

            // Load the parallaxing background
            bgLayer1.Initialize(Content, "bgLayer1", GraphicsDevice.Viewport.Width, -1);
            bgLayer2.Initialize(Content, "bgLayer2", GraphicsDevice.Viewport.Width, -2);

            mainBackground = Content.Load<Texture2D>("mainbackground");

            enemyTexture = Content.Load<Texture2D>("mineAnimation");

            projectileTexture = Content.Load<Texture2D>("laser");
            projectileTexture2 = Content.Load<Texture2D>("laser2");

            projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, playerPosition);
            projectile2.Initialize(GraphicsDevice.Viewport, projectileTexture2, playerPosition2);

            explosionTexture = Content.Load<Texture2D>("explosion");

            // Load the music
            gameplayMusic = Content.Load<Song>("sound/gameMusic");


            // Load the laser and explosion sound effect
            laserSound = Content.Load<SoundEffect>("sound/laserFire");
            explosionSound = Content.Load<SoundEffect>("sound/explosion");

            // Load the score font
            font = Content.Load<SpriteFont>("gameFont");


            // Start the music right away
            PlayMusic(gameplayMusic);

        }

        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
               // MediaPlayer.Play(song);


                // Loop the currently playing song
               // MediaPlayer.IsRepeating = true;
            }
            catch { }
        }

        private void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, 134, 134, 12, 45, Color.White, 1f, false);
            explosions.Add(explosion);
        }

        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();

            // Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30, Color.White, 1f, true);

            // Randomly generate the position of the enemy
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height - 100));

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }

        private void AddPickUps()
        {

            Vector2 position = new Vector2(random.Next( GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Width - 100), random.Next( GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Height - 100));

            Random rand = new Random();
            int rantPickUp = rand.Next(0, 4);
            PickUp pickup;
            pickup = new PickUp();
            switch (rantPickUp)
            {
                case 0:
                    pickup.Initialize(GraphicsDevice.Viewport, pickUp1, position, PickUp.pickUpType.TYPE1);
                    pickUps.Add(pickup);
                    break;
                case 1:
                    pickup.Initialize(GraphicsDevice.Viewport, pickUp2, position, PickUp.pickUpType.TYPE2);
                    pickUps.Add(pickup);
                    break;
                case 2:
                    pickup.Initialize(GraphicsDevice.Viewport, pickUp3, position, PickUp.pickUpType.TYPE3);
                    pickUps.Add(pickup);
                    break;
                case 3:
                    pickup.Initialize(GraphicsDevice.Viewport, pickUp4, position, PickUp.pickUpType.TYPE4);
                    pickUps.Add(pickup);
                    break;
            }
        }

        private void UpdatePickup(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousPickupSpawnTime > pickupsSpawnTime)
            {
                previousPickupSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddPickUps();
            }

            // Update the Enemies
            for (int i = pickUps.Count - 1; i >= 0; i--)
            {
                pickUps[i].Update(gameTime);

                if (pickUps[i].Active == false)
                {


                    pickUps.RemoveAt(i);
                }
            }
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].Active == false)
                {
                    // If not active and health <= 0
                    if (enemies[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(enemies[i].Position);

                        // Play the explosion sound
                        explosionSound.Play();

                        //Add to the player's score
                    }

                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        //private void AddProjectile(Vector2 position)
        //{
        //    Projectile projectile = new Projectile();
        //    projectile.Initialize(GraphicsDevice.Viewport, projectileTexture, position);
        //    projectiles.Add(projectile);
        //}

        private void UpdateProjectiles()
        {
            // Update the Projectiles
            //for (int i = projectiles.Count - 1; i >= 0; i--)
            //{
            //    projectiles[i].Update();

            //    if (projectiles[i].Active == false)
            //    {
            //        projectiles.RemoveAt(i);
            //    }
            //}
            if (currentKeyboardState.IsKeyDown(Keys.H) )//||
            //currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                projectile.Position.X -= projectile.projectileMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.K) )//||
            //currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                projectile.Position.X += projectile.projectileMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.U) )//||
           // currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                projectile.Position.Y -= projectile.projectileMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.J) )//||
            //currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                projectile.Position.Y += projectile.projectileMoveSpeed;
            }




            if (currentKeyboardState.IsKeyDown(Keys.NumPad1) )//||
            //currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                projectile2.Position.X -= projectile.projectileMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.NumPad3) )//||
           // currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                projectile2.Position.X += projectile.projectileMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.NumPad5) )//||
            //currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                projectile2.Position.Y -= projectile.projectileMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.NumPad2) )//||
            //currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                projectile2.Position.Y += projectile.projectileMoveSpeed;
            }



            projectile.Position.X = MathHelper.Clamp(projectile.Position.X, 0, GraphicsDevice.Viewport.Width - projectile.Width + 50);
            projectile.Position.Y = MathHelper.Clamp(projectile.Position.Y, 0, GraphicsDevice.Viewport.Height - projectile.Height + 10);
            projectile2.Position.X = MathHelper.Clamp(projectile2.Position.X, 0, GraphicsDevice.Viewport.Width - projectile.Width + 50);
            projectile2.Position.Y = MathHelper.Clamp(projectile2.Position.Y, 0, GraphicsDevice.Viewport.Height - projectile.Height + 10);

        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
            previousGamePadState = currentGamePadState;
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            //Update the player
            UpdatePlayer(gameTime);

            UpdatePickup(gameTime);

            // Update the parallaxing background

            //////////////////////////////////////
            //NEED TO CHANGE BACKGROUND
            //////////////////////////////////////
           // bgLayer1.Update();
           // bgLayer2.Update();
            //////////////////////////////////////


            // Update the enemies
            //UpdateEnemies(gameTime);

            // Update the collision
            UpdateCollision();

            // Update the projectiles
            UpdateProjectiles();

            // Update the explosions
            UpdateExplosions(gameTime);


            base.Update(gameTime);
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);
            player2.Update(gameTime);

            // Windows Phone Controls
            //while (TouchPanel.IsGestureAvailable)
            //{
            //    GestureSample gesture = TouchPanel.ReadGesture();
            //    if (gesture.GestureType == GestureType.FreeDrag)
            //    {
            //        player.Position += gesture.Delta;
            //        player2.Position += gesture.Delta;
            //    }
            //}

            //// Get Thumbstick Controls
            //player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            //player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;
            //player2.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            //player2.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            // Use the Keyboard / Dpad
            if (currentKeyboardState.IsKeyDown(Keys.A) )//||
            //currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= player.MoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) )//||
           // currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += player.MoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.W) )//||
            //currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= player.MoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.S) )//||
            //currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += player.MoveSpeed;

            }


            if (currentKeyboardState.IsKeyDown(Keys.Left) )//||
            //currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player2.Position.X -= player2.MoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) )//||
            //currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player2.Position.X += player2.MoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) )//||
            //currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player2.Position.Y -= player2.MoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) )//||
            //currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player2.Position.Y += player2.MoveSpeed;
            }


            // Make sure that the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width + 100);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height + 50);
            player2.Position.X = MathHelper.Clamp(player2.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width + 100);
            player2.Position.Y = MathHelper.Clamp(player2.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height + 50);

            // Fire only every interval we set as the fireTime
            //if (gameTime.TotalGameTime - previousFireTime > fireTime)
            //{
            //    // Reset our current time
            //    previousFireTime = gameTime.TotalGameTime;

            //    // Add the projectile, but add it to the front and center of the player
            //    AddProjectile(player.Position + new Vector2(player.Width / 2, 0));

            //    // Play the laser sound
            //    laserSound.Play();
            //}

            //// reset score if player health goes to zero
            //if (player.Health <= 0)
            //{
            //    player.Health = 100;
            //}
            //if (player2.Health < -0)
            //{
            //    player2.Health = 100;
            //}

        }

        private void UpdateCollision()
        {
            // Use the Rectangle's built-in intersect functionto 
            // determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            // Only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X - player.Width / 2,
            (int)player.Position.Y - player.Height / 2,
            player.Width,
            player.Height);

            //create rectangle for the 2nd player's projectile
            rectangle2 = new Rectangle((int)projectile2.Position.X -
                    projectile2.Width / 2, (int)projectile2.Position.Y -
                    projectile2.Height / 2, projectile2.Width, projectile2.Height);

            //check for collision between the first player and the 2nd players projectile
            if (rectangle1.Intersects(rectangle2))
            {
                player2.Score += projectile2.Damage;
            }

            //check for collision between the first player and any pick ups available
            for (int i = 0; i < pickUps.Count; i++)
            {
                rectangle2 = new Rectangle((int)pickUps[i].Position.X, (int)pickUps[i].Position.Y, pickUps[i].Width / 20, pickUps[i].Height / 20);
                if (rectangle1.Intersects(rectangle2))
                {
                    if (pickUps[i].PickUpType == PickUp.pickUpType.TYPE1)
                    {
                        //increase the size of the projectile by 5%
                        projectile.projectileScale = projectile.projectileScale * 1.05f;
                    }
                    else if (pickUps[i].PickUpType == PickUp.pickUpType.TYPE2)
                    {
                        //decrease the enemy speed by 10%
                        player2.MoveSpeed = player2.MoveSpeed * 0.9f;
                    }
                    else if (pickUps[i].PickUpType == PickUp.pickUpType.TYPE3)
                    {
                        //increase player speed by 10%
                        player.MoveSpeed = player.MoveSpeed * 1.1f;
                    }
                    else if (pickUps[i].PickUpType == PickUp.pickUpType.TYPE4)
                    {
                        //decrease the size of enemy projectile by 5%
                        projectile2.projectileScale = projectile.projectileScale * 0.95f;
                    }
                    pickUps.RemoveAt(i);
                }
            }






            //create a rectangle for the 2nd player
            rectangle1 = new Rectangle((int)player2.Position.X - player.Width / 2,
            (int)player2.Position.Y - player.Height / 2,
            player2.Width,
            player2.Height);

            //create rectangle for the first players projectile
            rectangle2 = new Rectangle((int)projectile.Position.X - (projectile.Width * (1 + (int)projectile.projectileScale)) / 2,
                (int)projectile.Position.Y - (projectile.Height * (1 + (int)projectile.projectileScale)) / 2,
                projectile.Width * (1 + (int)projectile.projectileScale),
                projectile.Height * (1 + (int)projectile.projectileScale));

            //check for collision between the 2nd player and the first players projectile
            if (rectangle1.Intersects(rectangle2))
            {
                player.Score += projectile.Damage;
            }

            for (int i = 0; i < pickUps.Count; i++)
            {
                rectangle2 = new Rectangle((int)pickUps[i].Position.X, (int)pickUps[i].Position.Y, pickUps[i].Width / 20, pickUps[i].Height / 20);
                if (rectangle1.Intersects(rectangle2))
                {
                    if (pickUps[i].PickUpType == PickUp.pickUpType.TYPE1)
                    {
                        //increase the size of the projectile by 5%
                        projectile2.projectileScale = projectile2.projectileScale * 1.05f;
                    }
                    else if (pickUps[i].PickUpType == PickUp.pickUpType.TYPE2)
                    {
                        //decrease the enemy speed by 10%
                        player.MoveSpeed = player.MoveSpeed * 0.9f;
                    }
                    else if (pickUps[i].PickUpType == PickUp.pickUpType.TYPE3)
                    {
                        //increase player speed by 10%
                        player2.MoveSpeed = player.MoveSpeed * 1.1f;
                    }
                    else if (pickUps[i].PickUpType == PickUp.pickUpType.TYPE4)
                    {
                        //decrease decrease size of enemy projectile
                        projectile.projectileScale = projectile.projectileScale * 0.95f;
                    }
                    pickUps.RemoveAt(i);
                }
            }




            // Do the collision between the player and the enemies
            //for (int i = 0; i < enemies.Count; i++)
            //{
            //    rectangle2 = new Rectangle((int)enemies[i].Position.X,
            //    (int)enemies[i].Position.Y,
            //    enemies[i].Width,
            //    enemies[i].Height);

            //    // Determine if the two objects collided with each
            //    // other
            //    if (rectangle1.Intersects(rectangle2))
            //    {
            //        // Subtract the health from the player based on
            //        // the enemy damage
            //        player.Health -= enemies[i].Damage;

            //        // Since the enemy collided with the player
            //        // destroy it
            //        enemies[i].Health = 0;

            //        // If the player health is less than zero we died
            //        if (player.Health <= 0)
            //            player.Active = false;
            //    }

            //}

            // Projectile vs Enemy Collision
                //for (int j = 0; j < enemies.Count; j++)
                //{
                //    // Create the rectangles we need to determine if we collided with each other
                //    rectangle1 = new Rectangle((int)projectile.Position.X -
                //    projectile.Width / 2, (int)projectile.Position.Y -
                //    projectile.Height / 2, projectile.Width, projectile.Height);

                //    rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
                //    (int)enemies[j].Position.Y - enemies[j].Height / 2,
                //    enemies[j].Width, enemies[j].Height);

                //    // Determine if the two objects collided with each other
                //    if (rectangle1.Intersects(rectangle2))
                //    {
                //        enemies[j].Health -= projectile.Damage;
                //        //projectile.Active = false;
                //    }
                //}
            
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing
            spriteBatch.Begin();

           // spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

            // Draw the moving background


            ////////////////////////////////////////////////////////
            //NEED TO CHANGE BACKGROUND
            ////////////////////////////////////////////////////////
           // bgLayer1.Draw(spriteBatch);
           // bgLayer2.Draw(spriteBatch);
            ////////////////////////////////////////////////////////

            // Draw the Player
            player.Draw(spriteBatch);
            player2.Draw(spriteBatch);

            // Draw the Enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            // Draw the Projectiles
           // for (int i = 0; i < projectiles.Count; i++)
            // {
            projectile.Draw(spriteBatch);
            projectile2.Draw(spriteBatch);
            //}

            for (int i = 0; i < pickUps.Count; i++)
            {
                pickUps[i].Draw(spriteBatch);
            }
            // Draw the explosions
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }

            // Draw the score
            spriteBatch.DrawString(font, "Player 1 Score: " + player.Score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            spriteBatch.DrawString(font, "Player 2 Score: " + player2.Score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 500, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

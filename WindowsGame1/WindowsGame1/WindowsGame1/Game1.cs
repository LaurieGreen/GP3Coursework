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

namespace CourseworkLGreen
{
    /// <summary>
    /// Laurie Green 11/12
    /// Main Class for Games Programming 3 Coursework
    /// List of all tutorials and sources used:
    /// http://msdn.microsoft.com/en-us/library/bb197293(v=xnagamestudio.31).aspx
    /// http://rbwhitaker.wikidot.com/skyboxes-1
    /// http://soundbible.com/1949-Pew-Pew.html
    /// http://soundbible.com/1461-Big-Bomb.html
    /// http://en.wikibooks.org/wiki/Game_Creation_with_XNA/3D_Development/Landscape_Modelling
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static GameStates gamestate;
        // Instance of menu
        private Menu menu;
        // Instance of player
        private Player player;
        // Instance of skybox
        private Skybox skybox;
        // Instance of cam
        private Camera cam;
        // Instance of terrain
        private Terrain landscape2;

        // Keyboard states for input
        KeyboardState keyboardState;
        KeyboardState lastState;

        // Graphics Device
        GraphicsDeviceManager graphics;
        // Spritebatch for drawing 
        SpriteBatch spriteBatch;

        // Font
        SpriteFont fontToUse;

        // Creates an array of enemies
        private Model mdlDalek;
        private Matrix[] mdDalekTransforms;
        private Daleks[] dalekList = new Daleks[GameConstants.NumDaleks];
        public int Daleknum = 0;

        // Creates an array of enemies 
        private Model mdlLaser;
        private Matrix[] mdlLaserTransforms;
        private Laser[] laserList = new Laser[GameConstants.NumLasers];

        // Random number
        private Random random = new Random();

        // Shooting sound
        // http://soundbible.com/1461-Big-Bomb.html
        // Accessed: 01/12/12        
        private SoundEffect explosionFX;
        // Menu sound
        // http://www.soundjay.com/button/sounds/button-27.mp3
        // Accessed: 01/12/12        
        private SoundEffect menuFX;
        private Song music;

        // Texture for menu
        Texture2D backgroundTex;

        // Width and Height ints
        int screenWidth;
        int screenHeight;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        // Initialises basic effect for drawing model
        BasicEffect basicEffect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        // Game States for menus
        public enum GameStates
        {
            Menu,// Main Menu
            Instructions,// Instructions
            Running,// Game mode
            End// Game over screen
        }

        // Method to setup scene
        protected override void Initialize()
        {
            // Gives window a title
            Window.Title = "Space Conquerors";
            // Gets aspectRatio using the Graphics device
            aspectRatio = GraphicsDevice.Viewport.AspectRatio;
            // Creates instances of classes
            menu = new Menu();
            gamestate = GameStates.Menu;
            cam = new Camera();

            // Sets default values for the camera
            cam.InitializeCamera(aspectRatio);
            // Sets dafault values for effect
            InitializeEffect();
            // Creates instance of terrain
            landscape2 = new Terrain(GraphicsDevice);
            // Spawns baddies 
            ResetDaleks();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch to draw textures
            spriteBatch = new SpriteBatch(GraphicsDevice);
            screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            // Loads font
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\DrWho");
            // Creates instance of player class, passing thruogh file paths as strings
            player = new Player("Models/p1_wedge", "Audio/pew", Content);

            // Loads image for menu
            backgroundTex = Content.Load<Texture2D>("Graphics/newsplash copy");

            // Loads music, explosion and menu sound
            music = Content.Load<Song>(".\\Audio\\gamesound");
            explosionFX = Content.Load<SoundEffect>("Audio\\boom");
            menuFX = Content.Load<SoundEffect>("Audio\\menu");

            // Loads all assets for daleks and lasers
            mdlDalek = Content.Load<Model>(".\\Models\\dalek");
            mdDalekTransforms = SetupEffectTransformDefaults(mdlDalek);
            mdlLaser = Content.Load<Model>(".\\Models\\laser");
            mdlLaserTransforms = SetupEffectTransformDefaults(mdlLaser);

            // Starts music
            MediaPlayer.Play(music);

            // Creates terrain instance, loading a height map and a texture
            landscape2.SetHeightMapData(Content.Load<Texture2D>("Graphics/heightMap2"), Content.Load<Texture2D>("Graphics/newlandtex"));

            // Creates skybox instance, passing file path as a string
            skybox = new Skybox("Graphics/SkyBox", Content);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
        }

        // Sets up effect defaults by passing in the mesh
        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    // Takes matrices from the camera 
                    effect.Projection = cam.projectionMatrix;
                    effect.View = cam.viewMatrix;
                }
            }
            return absoluteTransforms;
        }

        // Method to be used when drawing models from an array, passes in model and transform details
        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {

            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.View = cam.viewMatrix;
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        protected override void UnloadContent()
        {
        }

        // Method to spawn daleks, called at beginning of game
        private void ResetDaleks()
        {
            float xStart;
            float zStart;
            // Loops through dalek list
            for (int i = 0; i < GameConstants.NumDaleks; i++)
            {
                // Starting position using random numbers
                xStart = -((float)-GameConstants.PlayfieldSizeX + 0);
                zStart = -(float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                // Gives daleks position using randomised xStart and zStart
                dalekList[i].position = new Vector3(xStart, 2.0f, zStart);
                // Creates a random angle
                double angle = random.NextDouble() * 2 * Math.PI;
                dalekList[i].direction.X = -(float)Math.Sin(angle);
                dalekList[i].direction.Z = (float)Math.Cos(angle);
                // Gives speed using random factor 
                dalekList[i].speed = GameConstants.DalekMinSpeed +
                   (float)random.NextDouble() * GameConstants.DalekMaxSpeed;
                // Makes dalek active by default
                dalekList[i].isActive = true;
                // Console write for testing
                Console.WriteLine("dalekpop");
                Daleknum++;
            }
        }

        // Method for checking input concerning the game 
        public void CheckGameInput()
        {
            lastState = keyboardState;
            player.ShipControls(laserList);
            keyboardState = Keyboard.GetState();
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back ==
    ButtonState.Pressed) || (keyboardState.IsKeyDown(Keys.Escape)))
                this.Exit();
        }

        // Method for checking input concerning the menu
        public void CheckMenuInput()
        {
            lastState = keyboardState;
            keyboardState = Keyboard.GetState();
            if (gamestate == GameStates.Menu)
            {
                if (keyboardState.IsKeyDown(Keys.Down) && (lastState.IsKeyUp(Keys.Down)))
                {
                    // Move selection down
                    menu.Iterator++;
                    menuFX.Play();
                }
                else if (keyboardState.IsKeyDown(Keys.Up) && (lastState.IsKeyUp(Keys.Up)))
                {
                    // Move selection up
                    menu.Iterator--;
                    menuFX.Play();
                }

                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    if (menu.Iterator == 0)
                    {
                        gamestate = GameStates.Running;
                    }
                    else if (menu.Iterator == 1)
                    {
                        gamestate = GameStates.Instructions;
                    }
                    else if (menu.Iterator == 2)
                    {
                        this.Exit();
                    }
                    menu.Iterator = 0;
                }
            }
            else if (gamestate == GameStates.Instructions)
            {
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    gamestate = GameStates.Menu;
                }
            }
            else if (gamestate == GameStates.End)
            {
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    this.Exit(); 
                }
            }
        }

        // Update method, updates all active instances, removes "dead" lasers and daleks, checks for input
        protected override void Update(GameTime gameTime)
        {    
            // Do only if game is in "running state"
            if (gamestate == GameStates.Running)
            {
                CheckGameInput();
                CheckCollisions();
                cam.camUpdate(player.position);

                float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                // Update dalek list
                for (int i = 0; i < GameConstants.NumDaleks; i++)
                {
                    dalekList[i].Update(timeDelta);
                }
                // Update laser list
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (laserList[i].isActive)
                    {
                        laserList[i].Update(timeDelta);
                    }
                }
            }
            // Do only if in a menu
            else if ((gamestate == GameStates.Menu) || (gamestate == GameStates.Instructions) || (gamestate == GameStates.End))
            {
                CheckMenuInput();
            }
            // Music controls
            if (keyboardState.IsKeyDown(Keys.C))
                MediaPlayer.Pause();
            else if (keyboardState.IsKeyDown(Keys.V))
                MediaPlayer.Resume();

            base.Update(gameTime);
        }

        // Check collisions between the different models
        private void CheckCollisions()
        {
            // Give bounding body to player
            BoundingSphere PlayerSphere =
              new BoundingSphere(player.position,
                       player.myModel.Meshes[0].BoundingSphere.Radius *
                             GameConstants.ShipBoundingSphereScale);
            // Loop through dalek list
            for (int i = 0; i < dalekList.Length; i++)
            {
                if (dalekList[i].isActive)
                {
                    // Give bounding body to dalek
                    BoundingSphere dalekSphereA =
                      new BoundingSphere(dalekList[i].position, mdlDalek.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.DalekBoundingSphereScale);
                    // Dalek vs Laser
                    // Loop through lasers
                    for (int k = 0; k < laserList.Length; k++)
                    {
                        if (laserList[k].isActive)
                        {
                            // Give bounding body to laser
                            BoundingSphere laserSphere = new BoundingSphere(
                              laserList[k].position, mdlLaser.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.LaserBoundingSphereScale);
                            // If dalek and laser intersect
                            if (dalekSphereA.Intersects(laserSphere))
                            {
                                // play explosion FX
                                explosionFX.Play();
                                // Set them to false if they collide so they are removed
                                dalekList[i].isActive = false;
                                laserList[k].isActive = false;
                                // Increase score
                                player.score++;
                                // Decrease enemy number
                                Daleknum--;
                                break;
                            }
                        }
                    }
                    // Dalek vs player
                    // If dalek and player intersect
                    if (dalekSphereA.Intersects(PlayerSphere))
                    {
                        // play explosion FX
                        explosionFX.Play();
                        // Put player back to starting position
                        ResetPlayer();
                        break;
                    }                    
                }
            }
        }

        // Method to reset the player when it is killed
        public void ResetPlayer()
        {
            // Decrease lives
            player.lives--;
            // Reset player position to playerspawn position
            player.position = player.playerspawn;
            // Write line for testing
            Console.WriteLine(player.lives);
            // If player is out of lives then trigger game over screen
            if ((player.lives == 0) || (Daleknum == 0))
            {
                gamestate = GameStates.End;
            }
        }

        // Method to setup effect
        private void InitializeEffect()
        {
            // Creates new effect, passes in graphics device
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            // Takes view, projection and world matrices from camera 
            basicEffect.View = cam.viewMatrix;
            basicEffect.Projection = cam.projectionMatrix;
            basicEffect.World = cam.worldMatrix;
            // Enable texture on effect
            basicEffect.TextureEnabled = true;
        }

        // Draw method of terrain
        public void DrawLand2()
        {
            landscape2.basicEffect.CurrentTechnique.Passes[0].Apply();
            SetEffects(landscape2.basicEffect);
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            foreach (EffectPass pass in landscape2.basicEffect.CurrentTechnique.Passes)
            {
                // Call draw method from terrain class
                landscape2.Draw();
            }
        }

        public void SetEffects(BasicEffect basicEffect)
        {
            basicEffect.View = Matrix.CreateLookAt(cam.camPosition, cam.camLookat, Vector3.Up);
            basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 50000.0f);
            basicEffect.World = cam.worldMatrix;
        }

        // Method to draw text on screen
        private void writeText(string msg, Vector2 msgPos, Color msgColour)
        {
            spriteBatch.Begin();
            string output = msg;
            // Find the center of the string
            Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
            Vector2 FontPos = msgPos;
            // Draw the string
            spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
            spriteBatch.End();
        }

        // Main draw method for game, calls draw method on all instances of objects
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            // All drawing code for game if it's running
            if (gamestate == GameStates.Running)
            {
                
                Matrix[] transforms = new Matrix[player.myModel.Bones.Count];
                // Draw player
                player.Draw(cam, aspectRatio, transforms);
                // Loop list of daleks
                for (int i = 0; i < GameConstants.NumDaleks; i++)
                {
                    // if instance is active, draw it
                    if (dalekList[i].isActive)
                    {
                        Matrix dalekTransform = Matrix.CreateScale(GameConstants.DalekScalar) * Matrix.CreateTranslation(dalekList[i].position);
                        DrawModel(mdlDalek, dalekTransform, mdDalekTransforms);
                    }
                }
                // Loop list of lasers
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    // if instance is active, draw it
                    if (laserList[i].isActive)
                    {
                        Matrix laserTransform = Matrix.CreateScale(GameConstants.LaserScalar) * Matrix.CreateTranslation(laserList[i].position);
                        DrawModel(mdlLaser, laserTransform, mdlLaserTransforms);
                    }
                }
                // Draw terrain
                DrawLand2();
                graphics.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                // Draw skybox
                skybox.Draw(cam);
                graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                // Draw HUD
                writeText("Score: "+player.score,new Vector2 (10,10),Color.Red);
                writeText("Lives: " + player.lives, new Vector2(10, 25), Color.Red);
                writeText("Camera: " + cam.camtext, new Vector2(10, 40), Color.Red);
                writeText("Baddies left: " + Daleknum, new Vector2(10, screenHeight - 25), Color.Red); 

                // Reset drawing states after spritebatch from writetext changes them  
                GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            // Drawing code for Main Menu
            else if (gamestate == GameStates.Menu)
            {
                spriteBatch.Begin();
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(backgroundTex, screenRectangle, Color.White);
                menu.DrawMenu(spriteBatch, screenWidth, fontToUse);
                spriteBatch.End();
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            // Drawing code for Game Over Screen
            else if (gamestate == GameStates.End)
            {
                spriteBatch.Begin();
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(backgroundTex, screenRectangle, Color.White);
                menu.DrawEnd(spriteBatch, screenWidth, fontToUse, player.score);
                spriteBatch.End();
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            // Drawing code for Instructions sc
            else if (gamestate == GameStates.Instructions)
            {
                spriteBatch.Begin();
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(backgroundTex, screenRectangle, Color.White);
                menu.DrawInstructions(spriteBatch, screenWidth, fontToUse);
                spriteBatch.End();
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            base.Draw(gameTime);
        }
    }
}

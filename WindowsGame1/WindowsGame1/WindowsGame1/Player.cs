using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Laurie Green
    /// Created: 10/11/12
    /// </summary>
    class Player : Microsoft.Xna.Framework.Game
    {
        // Vector 3 to hold the players spawnpoint
        public Vector3 playerspawn = new Vector3(Vector3.Zero.X + 90, Vector3.Zero.Y + 0.7f, Vector3.Zero.Z - 10);
        // int for lives and score
        public int score = 0;
        public int lives = 5;
        // Players position
        public Vector3 position;
        // Players rotation
        public float rotation = 0.0f;
        // Players 3D Model
        // http://msdn.microsoft.com/en-us/library/bb197293(v=xnagamestudio.31).aspx
        // Accessed: 10/11/12
        public Model myModel;
        // Shooting sound
        // http://soundbible.com/1949-Pew-Pew.html
        // Accessed: 01/12/12
        private SoundEffect pewFX;
        // Graphics device 
        GraphicsDeviceManager graphics;

        // Vector3s for applying velocity to the player
        public Vector3 myVelocityZ = new Vector3(0, 0, -1);
        public Vector3 myVelocityX = new Vector3(-0.5f, 0, 0);

        private KeyboardState lastState;

        // Constructor for player, takes in file path for model and shooting sound effect. Also takes in Content from main class. 
        public Player(string modeltext ,string soundtext , ContentManager Content)
        {
            position = playerspawn;
            graphics = new GraphicsDeviceManager(this);
            // Model and sound loaded using strings from main class
            myModel = Content.Load<Model>(modeltext);
            pewFX = Content.Load<SoundEffect>(soundtext);
        }

        public void Update()
        {

        }

        // Method to take input from keyboard and applies movement to player
        public void ShipControls(Laser [] laserList)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
            {
                // Forward
                position += myVelocityZ;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                // Backwards
                position -= myVelocityZ;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                // Left
                position -= myVelocityX;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                // Right
                position += myVelocityX;
            }

            if (keyboardState.IsKeyDown(Keys.Space) && lastState.IsKeyUp(Keys.Space))
            {
                // Adds new bullet if there is an available slot
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (!laserList[i].isActive)
                    {
                        Matrix playerTransform = Matrix.CreateRotationY(rotation);
                        laserList[i].direction = playerTransform.Forward;
                        laserList[i].speed = GameConstants.LaserSpeedAdjustment;
                        laserList[i].position = position + laserList[i].direction;
                        laserList[i].isActive = true;
                        // Console write line for testing
                        Console.WriteLine("laserpop");
                        pewFX.Play();
                        break; //exit the loop     
                    }
                }
            }
            lastState = keyboardState;
        }

        // Method to draw model
        public void Draw(Camera cam, float aspectRatio, Matrix [] transforms)
        {
            // Copy any parent transforms.
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // Mesh orientation is set, and camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(position);
                    effect.View = Matrix.CreateLookAt(cam.camPosition, cam.camLookat, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 50000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}

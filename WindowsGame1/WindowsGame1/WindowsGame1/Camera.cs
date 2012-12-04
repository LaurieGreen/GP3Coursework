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
    /// Adapted from:
    /// http://blackboard.gcal.ac.uk/bbcswebdav/pid-1163578-dt-content-rid-776603_2/courses/MHG605291-12-A/GP3%20Lab%204%20Exercises%20%26%20Code%20Snippet.pdf
    /// Tutorial for creating camera 
    /// Accessed: 05/11/12
    /// </summary>
    public class Camera
    {
        // Rotation Matrix for camera to reflect movement around Y & X axis
        public Matrix camRotationMatrixY; 
        public Matrix camRotationMatrixX;
        // Position of camera 
        public Vector3 camPosition; 
        // Direction that the camera is currently looking or pointing at
        public Vector3 camLookat; 
        // Used for repositioning the camera after it has been rotated
        public Vector3 camTransformX;
        public Vector3 camTransformY;
        // Amount of rotation
        public float camRotationSpeed; 
        // Rotation on Y axis
        public float camYaw; 
        // Rotation on Z axis
        public float camPitch;

        // Text to display which cam is active, default third person
        public string camtext = "Third Person";

        // Booleans for checking which camera is active 
        public bool firstperson = false;
        public bool thirdperson = true;
        public bool freecam=false;

        // Matrices for projection, world and view
        public Matrix projectionMatrix;
        public Matrix worldMatrix;
        public Matrix viewMatrix; 

        // Method to set default values for a camera instance
        public void InitializeCamera(float aspectRatio)
        {
            camPosition = new Vector3(0.0f, 5.0f, 10.0f);
            camRotationSpeed = 100.0f;
            viewMatrix = Matrix.CreateLookAt(camPosition, Vector3.Zero, Vector3.Up);            
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45),
                aspectRatio,
                1.0f, 10000.0f);
            worldMatrix = Matrix.Identity;
        }

        // Keeps camera updated with changes in the world
        public void camUpdate(Vector3 Position)
        {
            camRotationMatrixY = Matrix.CreateRotationY(camYaw);
            camRotationMatrixX = Matrix.CreateRotationX(camPitch);
            camTransformX = Vector3.Transform(Vector3.Forward, camRotationMatrixX);
            camTransformY = Vector3.Transform(Vector3.Forward, camRotationMatrixY);
            // Updates where camera is looking 
            camLookat = camPosition + camTransformX + camTransformY;
            // Detects if a change in camera has been requested 
            ChangeCamera();
            // Passes out different values depending on cam type detected
            CamBehaviour(Position);
            // Updates viewMatrix using new values 
            viewMatrix = Matrix.CreateLookAt(camPosition, camLookat, Vector3.Up);
        }

        // Method to switch camera depending on user input
        public void ChangeCamera()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.D1))
            {
                // First person
                firstperson = true;
                camtext = "First Person";
                thirdperson = false;
                freecam = false;
            }
            if (keyboardState.IsKeyDown(Keys.D2))
            {
                // Third person
                thirdperson = true;
                camtext = "Third Person";
                freecam = false;
                firstperson = false;
            }
            if (keyboardState.IsKeyDown(Keys.D3))
            {
                // Top down
                freecam = true;
                camtext = "Top Down";
                thirdperson = false;
                firstperson = false;
            }
        }

        // Method to make the camera behave differently depending on type selected
        public void CamBehaviour(Vector3 Position)
        {
            if (thirdperson == true)
            {
                // Position set to behind and above player
                camPosition = new Vector3(Position.X, Position.Y + 3, Position.Z + 10);
                // Looking at player
                camLookat = Position;
            }
            else if (freecam == true)
            {
                // Position set to directly above player
                camPosition = new Vector3(Position.X, Position.Y + 60, Position.Z + 1);
                // Looking slightly infront of player
                camLookat = new Vector3(Position.X, Position.Y, Position.Z - 20);
            }
            else if (firstperson == true)
            {
                // Position set to players position
                camPosition = new Vector3(Position.X, Position.Y, Position.Z-1);
                // Looking slightly infront of player
                camLookat = new Vector3(Position.X, Position.Y, Position.Z - 3);
            }
        }
    }
}

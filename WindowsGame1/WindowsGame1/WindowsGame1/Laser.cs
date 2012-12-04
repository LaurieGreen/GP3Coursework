using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CourseworkLGreen
{
    /// <summary>
    /// Taken from Robert Law's lab 5
    /// Simple class to creat instance of laser
    /// </summary>
    struct Laser
    {
        // Laser position
        public Vector3 position;
        // Laser direction
        public Vector3 direction;
        // Laser speed
        public float speed;
        // Bool to hold whether instance is active or not
        public bool isActive;

        public void Update(float delta)
        {
            // Move laser using direction, speed and randomly generated number
            position += direction * speed *
                        GameConstants.LaserSpeedAdjustment * delta;
            // Deactivate laser if it goes out of play field 
            if (position.X > GameConstants.PlayfieldSizeX +80||
                position.X < -GameConstants.PlayfieldSizeX + 90 ||
                position.Z > GameConstants.PlayfieldSizeZ ||
                position.Z < -GameConstants.PlayfieldSizeZ)
                isActive = false;
        }
    }
}

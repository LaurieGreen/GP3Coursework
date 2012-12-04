using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace CourseworkLGreen
{
    /// <summary>
    /// Taken from Robert Law's lab 5
    /// Simple class to creat instance of dalek
    /// </summary>
    struct Daleks
    {
        // Enemies position
        public Vector3 position;
        // Enemies direction
        public Vector3 direction;
        // Enemies speed
        public float speed;
        // Bool to hold whether instance is active or not
        public bool isActive;


        public void Update(float delta)
        {
            // Move enemy using direction, speed and randomly generated number
            position += direction * speed *
                        GameConstants.DalekSpeedAdjustment * delta;
            // If player leaves play field invert their direction so they stay within the area
            if (position.X > GameConstants.PlayfieldSizeX + 80)
                direction = -direction;
            if (position.X < -GameConstants.PlayfieldSizeX + 90)
                direction = -direction;
            if (position.Z > GameConstants.PlayfieldSizeZ)
                direction = -direction;
            if (position.Z < -GameConstants.PlayfieldSizeZ)
                direction = -direction;
        }
    }
}

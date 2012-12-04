using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkLGreen
{
    static class GameConstants
    {
        //Class to hold constant values to be used through-out the project

        //Camera constants
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 45f;
        public const float PlayfieldSizeZ = 500f;
        //Dalek constants
        public const int NumDaleks = 10;
        public const float DalekMinSpeed = 5f;
        public const float DalekMaxSpeed = 25.0f;
        public const float DalekSpeedAdjustment = 2.5f;
        public const float DalekScalar = 0.01f;
        //Collision constants
        public const float DalekBoundingSphereScale = 0.025f;  //50% size
        public const float ShipBoundingSphereScale = 0.5f;  //50% size
        public const float LaserBoundingSphereScale = 0.85f;  //50% size
        //Bullet constants
        public const int NumLasers = 40;
        public const float LaserSpeedAdjustment = 15.0f;
        public const float LaserScalar = 3.0f;
        //Text constants
        public const string text = "Kill all the enemies.";
        public const string controls = "WASD    move.\r\nSpace    fire.\r\nC    pause music. \r\nV    resume music. \r\n1    first person camera. \r\n2    third Person.\r\n3    top Down.";
        public const string endtext = "Gameover, you scored: ";
    }
}

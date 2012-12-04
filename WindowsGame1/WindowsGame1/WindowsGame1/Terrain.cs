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
    /// http://en.wikibooks.org/wiki/Game_Creation_with_XNA/3D_Development/Landscape_Modelling
    /// Tutorial for creating landscape using a heightmap
    /// Accessed: 23/11/12
    /// </summary>
    class Terrain
    {
        // Class for creating terrain from heightMap

        GraphicsDevice graphicsDevice;
        // HeightMap
        Texture2D heightMap;
        // Texture for ground
        Texture2D heightMapTexture;
        // Terrain vertices
        VertexPositionTexture[] vertices;
        // Width and height for area, taken from heightMap
        int width;
        int height;
        // Effect
        public BasicEffect basicEffect;
        // Terrain vertices 
        int[] indices;

        // Array to read heightMap data
        float[,] heightMapData;

        public Terrain(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        // Sets values to prepare, then builds terrain
        public void SetHeightMapData(Texture2D heightMap, Texture2D heightMapTexture)
        {
            this.heightMap = heightMap;
            this.heightMapTexture = heightMapTexture;
            width = heightMap.Width;
            height = heightMap.Height;

            // Values now ready, build terrain
            SetHeights();
            SetVertices();
            SetIndices();
            SetEffects();
        }

        // Takes height from heightMap depending on pixel brightness 
        public void SetHeights()
        {
            Color[] greyValues = new Color[width * height];
            heightMap.GetData(greyValues);
            heightMapData = new float[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heightMapData[x, y] = greyValues[x + y * width].G / 3.1f;
                }
            }
        }

        // Creates coordinates for building terrain
        public void SetIndices()
        {
            // Amount of triangles
            indices = new int[6 * (width - 1) * (height - 1)];
            int number = 0;
            // Collect data for corners
            for (int y = 0; y < height - 1; y++)
                for (int x = 0; x < width - 1; x++)
                {
                    // Create double triangles
                    // Two triangles make a square
                    indices[number] = x + (y + 1) * width;      // up left
                    indices[number + 1] = x + y * width + 1;        // down right
                    indices[number + 2] = x + y * width;            // down left
                    indices[number + 3] = x + (y + 1) * width;      // up left
                    indices[number + 4] = x + (y + 1) * width + 1;  // up right
                    indices[number + 5] = x + y * width + 1;        // down right
                    number += 6;
                }
        }

        // Creates shapes by linking points 
        public void SetVertices()
        {
            vertices = new VertexPositionTexture[width * height];
            Vector2 texturePosition;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    texturePosition = new Vector2((float)x / 25.5f, (float)y / 25.5f);
                    vertices[x + y * width] = new VertexPositionTexture(new Vector3(x, heightMapData[x, y], -y), texturePosition);
                }            
            }
        }

        // Applies the texture to the shapes 
        public void SetEffects()
        {
            // Takes in Graphics Device from main class
            basicEffect = new BasicEffect(graphicsDevice);
            // Applies texture from main class
            basicEffect.Texture = heightMapTexture;
            basicEffect.TextureEnabled = true;
        }

        // Draw method to draw terrain 
        public void Draw()
        {
            graphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
        }
    }
}

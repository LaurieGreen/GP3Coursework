using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CourseworkLGreen
{
    /// <summary>
    /// Adapted from:
    /// http://rbwhitaker.wikidot.com/skyboxes-1
    /// Tutorial for creating skybox
    /// Accessed: 19/11/12
    /// </summary>
    public class Skybox
    {
        // The skybox model, which is a cube
        private Model skyBox;
        // The skybox texture        
        private TextureCube skyBoxTexture;
        // The effect file that the skybox will use to render
        private Effect skyBoxEffect;
        // The size of the cube, used so that we can resize the box for different sized environments.
        private float size = 50f;


        // Creates a new skybox instance using the texture provided 
        public Skybox(string skyboxTexture, ContentManager Content)
        {
            // Loads cube model
            skyBox = Content.Load<Model>("Models/cube");
            // Loads texture
            skyBoxTexture = Content.Load<TextureCube>(skyboxTexture);
            // Loads fx file
            skyBoxEffect = Content.Load<Effect>("Effects/Skybox");
        }

        // Draws Skybox, no world matrix since it won't move
        // Passes in camera instance so to use current view and projection matrices
        public void Draw(Camera cam)
        {
            // Go through each pass in the effect, but we know there is only one...
            foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
            {
                // Draw all of the components of the mesh, even though the cube only has one mesh
                foreach (ModelMesh mesh in skyBox.Meshes)
                {
                    // Assign the appropriate values to each of the parameters
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = skyBoxEffect;
                        part.Effect.Parameters["World"].SetValue(
                            Matrix.CreateScale(size) * Matrix.CreateTranslation(cam.camPosition));
                        part.Effect.Parameters["View"].SetValue(cam.viewMatrix);
                        part.Effect.Parameters["Projection"].SetValue(cam.projectionMatrix);
                        part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                        part.Effect.Parameters["CameraPosition"].SetValue(cam.camPosition);
                    }

                    // Draw the mesh with the skybox effect
                    mesh.Draw();
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkLGreen
{
    /// <summary>
    /// Adapted from:
    /// http://ross-warren.co.uk/pong-clone-in-xna-4-0-for-windows/4/
    /// Tutorial for creating menu
    /// Accessed: 15/11/12
    /// </summary>
    class Menu
    {
        // Class for creating a menu

        // A list for the menu items
        private List<string> MenuItems;
        // Int to keep track of selected item
        private int iterator;
        // Strings for title of menu and info text
        public string InfoText { get; set; }
        public string Title { get; set; }

        // get and set for the iterator
        public int Iterator
        {
            get
            {
                return iterator;
            }
            set
            {
                iterator = value;
                if (iterator > MenuItems.Count - 1) iterator = MenuItems.Count - 1;
                if (iterator < 0) iterator = 0;
            }
        }

        // Constructor for menu
        public Menu()
        {
            Title = "";
            MenuItems = new List<string>();
            MenuItems.Add("Play");
            MenuItems.Add("Instructions");
            MenuItems.Add("Exit");
            Iterator = 0;
            InfoText = string.Empty;
        }

        public int GetNumberOfOptions()
        {
            return MenuItems.Count;
        }

        public string GetItem(int index)
        {
            return MenuItems[index];
        }

        // Draw method for Main menu screen
        public void DrawMenu(SpriteBatch batch, int screenWidth, SpriteFont arial)
        {
            batch.DrawString(arial, Title, new Vector2(screenWidth / 2 - arial.MeasureString(Title).X / 2, 20), Color.White);
            int yPos = 120;
            for (int i = 0; i < GetNumberOfOptions(); i++)
            {
                Color colour = Color.White;
                if (i == Iterator)
                {
                    colour = Color.Cyan;
                }
                batch.DrawString(arial, GetItem(i), new Vector2(screenWidth / 2 - arial.MeasureString(GetItem(i)).X / 2, yPos), colour);
                yPos += 30;
            }
        }

        // Draw method for the Gameover screen 
        public void DrawEnd(SpriteBatch batch, int screenWidth, SpriteFont arial, int Score)
        {
            int yPos = 120;
            int xpos = 30;
            batch.DrawString(arial, GameConstants.endtext+Score, new Vector2(xpos , yPos), Color.Red);
            string prompt = "Press Escape to End";
            batch.DrawString(arial, prompt, new Vector2(screenWidth / 2 - arial.MeasureString(prompt).X / 2, 400), Color.White);
        }

        // Draw method for the Instructions screen
        public void DrawInstructions(SpriteBatch batch, int screenWidth, SpriteFont arial)
        {
            batch.DrawString(arial, Title, new Vector2(screenWidth / 2 - arial.MeasureString(Title).X / 2, 20), Color.White);
            int yPos = 120;
            int xpos = 30;
            Color colour = Color.White;
            batch.DrawString(arial, "Instructions", new Vector2(xpos, (yPos + 20)), Color.Red);
            batch.DrawString(arial, GameConstants.text, new Vector2(xpos, yPos + 50), colour);
            batch.DrawString(arial, "Controls", new Vector2(xpos, (yPos + 90)), Color.Red);
            batch.DrawString(arial, GameConstants.controls, new Vector2(xpos, yPos+120), colour);
            yPos += 30;
            
        }
    }
}

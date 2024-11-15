using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Project.Sprites.Enemies;

public class AlertedEffect
{
    private const int alert_count = 3;
    private const int total_width = 20;
    private const int bar_width = 4;
    private const int bar_final_height = 10;

    // Provide post of the bottom-center where the alerted symbols will be drawn
    public static void Draw(SpriteBatch spriteBatch, Vector2 pos,int elapsedTime, int completionTime)
    {
        // Create a blank pixel to add color to later
        var whitePixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        whitePixel.SetData(new[] { Color.White });


        // Solve for positons of rectangles and Draw them
        spriteBatch.Begin();

        var xOffset = (float)(total_width - (alert_count * bar_width)) / (alert_count - 1) + bar_width;
        var startPos = pos.X - total_width / 2;
        for (int i = 0; i < alert_count; i++) {
            int yOffset = -(int)(bar_final_height * ((float)elapsedTime / completionTime));

            // Draw main bar
            spriteBatch.Draw(whitePixel, new Rectangle((int) (startPos + xOffset * i), (int)pos.Y + yOffset, bar_width, -yOffset), Color.DarkRed);
            // Draw Tips
            spriteBatch.Draw(whitePixel, new Rectangle((int)(startPos + xOffset * i), (int)pos.Y + yOffset, bar_width, bar_width), Color.Red);
        }

        spriteBatch.End();
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Sprites;

public class HealthBar
{
    private const int HealthBarWidth = 48;
    private const int HealthBarHeight = 5;
    private const float HealthBarVerticalOffset = -10;

    public static void DrawHealthBar(SpriteBatch spriteBatch, float x, float y, float maxHealth, float currentHealth, float currentShield = 0)
    {
        // Create a blank pixel to add color to later
        var whitePixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        whitePixel.SetData(new[] { Color.White });

        // Find the new location for the health bar;ds
        Rectangle backgroundRectangle  = new((int)(x - HealthBarWidth / 2f), (int)(y + HealthBarVerticalOffset), HealthBarWidth, HealthBarHeight);
        Rectangle healthRectangle      = new((int)(x - HealthBarWidth / 2f), (int)(y + HealthBarVerticalOffset), (int)(HealthBarWidth * (1 - (maxHealth - currentHealth) / (float)maxHealth)), HealthBarHeight);
        Rectangle background2Rectangle = new((int)(x - HealthBarWidth / 2f), (int)(y + HealthBarVerticalOffset - HealthBarHeight), HealthBarWidth, HealthBarHeight);
        Rectangle shieldRectangle      = new((int)(x - HealthBarWidth / 2f), (int)(y + HealthBarVerticalOffset - HealthBarHeight), (int)(HealthBarWidth * (1 - (maxHealth - currentShield) / (float)maxHealth)), HealthBarHeight);

        // Draw Background and Foreground for Health and Shield
        spriteBatch.Begin();
        spriteBatch.Draw(whitePixel, backgroundRectangle, Color.DarkRed);
        spriteBatch.Draw(whitePixel, healthRectangle, Color.Green);

        if (currentShield > 0)
        {
            spriteBatch.Draw(whitePixel, background2Rectangle, Color.DimGray);
            spriteBatch.Draw(whitePixel, shieldRectangle, Color.LightSkyBlue);
        }

        spriteBatch.End();
    }
}
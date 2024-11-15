using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Sprites.Players
{
    // Utility
    public struct Animation
    {
        public Animation(string name, Texture2D tex, int frameCount, Vector2 startPos, Vector2 size, float period, Vector2 offset = new())
        {
            Name = name;
            Tex = tex;
            FrameCount = frameCount;
            StartPos = startPos;
            Size = size;
            Offset = offset;
            Period = period;
        }

        public string Name { get; set; }

        public Texture2D Tex { get; set; }

        public int FrameCount { get; set; }

        public Vector2 StartPos { get; set; }

        public Vector2 Size { get; set; }
        public Vector2 Offset { get; set; }

        public float Period { get; set; } // milliseconds per cycle
    }
}

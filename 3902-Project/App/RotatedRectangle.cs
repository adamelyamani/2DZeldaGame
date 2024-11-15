using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Project.App
{
    public class RotatedRectangle
    {
        public RotatedRectangle(Rectangle rectangle, Vector2 rotationalPoint, double angleInRadians)
        {
            Rectangle = rectangle;
            RotationalPoint = rotationalPoint;
            AngleInRadians = angleInRadians;
        }

        public Rectangle Rectangle { get; set; }
        public Vector2 RotationalPoint { get; set; }
        public double AngleInRadians { get; set; }

        public bool CollidesWith(Rectangle axisAlignedRectangle)
        {
            // Find points of rotated rectangle
            var vertices = new List<Point>
            {
                new Point(Rectangle.X, Rectangle.Y),
                new Point(Rectangle.X + Rectangle.Width, Rectangle.Y),
                new Point(Rectangle.X, Rectangle.Y + Rectangle.Height),
                new Point(Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height),
            };

            //Center points about origin, rotate, then add back to original location
            for (var i = 0; i < vertices.Count; i++)
            {
                vertices[i] -= new Point((int)RotationalPoint.X + Rectangle.X, (int)RotationalPoint.Y + Rectangle.Y);
                vertices[i] = new Point(
                    (int)(vertices[i].X * Math.Cos(AngleInRadians) - vertices[i].Y * Math.Sin(AngleInRadians)),
                    (int)(vertices[i].X * Math.Sin(AngleInRadians) + vertices[i].Y * Math.Cos(AngleInRadians)));
                vertices[i] += new Point((int)RotationalPoint.X + Rectangle.X, (int)RotationalPoint.Y + Rectangle.Y);
            }

            // Calculate the 4 corners of the axis-aligned rectangle
            var axisAlignedVertices = new List<Point>
            {
                new Point(axisAlignedRectangle.X, axisAlignedRectangle.Y),
                new Point(axisAlignedRectangle.X + axisAlignedRectangle.Width, axisAlignedRectangle.Y),
                new Point(axisAlignedRectangle.X, axisAlignedRectangle.Y + axisAlignedRectangle.Height),
                new Point(axisAlignedRectangle.X + axisAlignedRectangle.Width,
                    axisAlignedRectangle.Y + axisAlignedRectangle.Height),
            };

            //Use the SAT algorithm to see if there is a separating axis between the 2 rectangles
            return !IsSeparatingAxis(axisAlignedVertices, vertices) && !IsSeparatingAxis(vertices, axisAlignedVertices);
        }

        private static bool IsSeparatingAxis(List<Point> rec1, List<Point> rec2)
        {
            //Loop over every edge, as these are all the possible separating axis
            for (var i = 0; i < 4; i++)
            {
                // Initial values to be used later
                var minRect1 = float.MaxValue;
                var maxRect1 = float.MinValue;
                var minRect2 = float.MaxValue;
                var maxRect2 = float.MinValue;

                //Get the next edge
                var j = (i + 1) % 4;
                var edge = new Point(rec1[j].X - rec1[i].X, rec1[j].Y - rec1[i].Y);
                var perpendicular = new Point(-edge.Y, edge.X);

                // Project vertices of rec1 to perpendicular axis
                foreach (var point in rec1)
                {
                    var projection = (point.X * perpendicular.X + point.Y * perpendicular.Y);
                    minRect1 = Math.Min(minRect1, projection);
                    maxRect1 = Math.Max(maxRect1, projection);
                }

                // Project vertices of rec2 to perpendicular axis
                foreach (var point in rec2)
                {
                    var projection = (point.X * perpendicular.X + point.Y * perpendicular.Y);
                    minRect2 = Math.Min(minRect2, projection);
                    maxRect2 = Math.Max(maxRect2, projection);
                }

                // Check for gap between rectangles
                if (maxRect1 < minRect2 || minRect1 > maxRect2)
                {
                    return true; // Separating axis found
                }
            }

            return false;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Players;
using System;

namespace Project.Sprites.Enemies.PathFinding
{
    public abstract class ActionPatternEnemy : Enemy
    {
        // Action Pattern Callback Functions
        // These functions are meant for when the same Action is used for multiple Enemies

        // ACTIONS

        // settings[0] = TargetX
        // settings[1] = TargetY
        protected virtual void SetPathToPosAction(GameTime time, int time_since_action, int[] settings)
        {
            if (settings == null || settings.Length < 2)
                return;

            Vector2 t = new Vector2(settings[0], settings[1]);
            if (!PathTo(t))
                return;
        }

        // Uses Pathfinder to move to a random valid point within a radius (square)
        // settings[0] = TargetX
        // settings[1] = TargetY
        // settings[2] = Radius
        // settings[3] = max attempts
        protected virtual void SetPathToPosInRadiusAction(GameTime time, int time_since_action, int[] settings)
        {
            if (settings == null || settings.Length < 4)
                return;

            Vector2 pos = new Vector2(settings[0], settings[1]);

            // try random position until one is found in range and valid
            Random rand = new Random();
            int attempts = 0;

            Vector2 tPos;
            do
            {
                tPos = new Vector2((float)rand.NextDouble(), (float)rand.NextDouble()) * settings[2];
                attempts++;
            } while (attempts < settings[3] && !PathTo(pos + tPos));
        }

        // Follows a preset path
        protected virtual void FollowPathAction(GameTime time, int time_since_action, int[] settings)
        {
            if (pathFinder == null || !pathFinder.PathFound())
                return;

            UpdatePathing();

            Move(time);
        }

        protected virtual void MoveAction(GameTime time, int time_since_action, int[] settings)
        {
            if (settings == null || settings.Length < 2)
                return;

            Vector2 dir = new Vector2(settings[0], settings[1]);
            if (Math.Abs(settings[0]) * Math.Abs(settings[1]) != 0)
                dir.Normalize();

            target = GetPosition() + dir;
            Move(time);
        }

        // Runs away from player directly
        protected virtual void FleeAction(GameTime time, int time_since_action, int[] settings)
        {
            Vector2 dir = GetPlayerPosition() - GetPosition();
            target = GetPosition() - dir;
            Move(time);
        }

        // Does nothing
        protected virtual void IdleAction(GameTime time, int time_since_action, int[] settings)
        {
            // do nothing
        }

        // CONDITIONS

        // Returns true until the time_since_action is greater than the time setting
        // settings[0] = Time
        protected virtual bool TimeCondition(int time_since_action, int[] settings)
        {
            if (settings == null || settings.Length < 1)
                return false;

            return (time_since_action <= settings[0]);
        }

        // Returns true if the enemy is not at the target position yet
        // settings[0] = StopX
        // settings[1] = StopY
        protected virtual bool PositionCondition(int time_since_action, int[] settings)
        {
            const int distance_error = 2;

            if (settings == null || settings.Length < 2)
                return false;

            Vector2 stopPos = new Vector2(settings[0], settings[1]);
            Vector2 dir = stopPos - GetPosition();

            if (dir.Length() <= distance_error)
            {
                Position += dir; // correct position for distance error
                return false;
            }

            return true;
        }

        // Returns true if Enemy is not at it's path destination yet
        protected virtual bool FinishedPathCondition(int time_since_action, int[] settings)
        {
            if (pathFinder == null || !pathFinder.PathFound())
                return false;

            Vector2 pos = pathFinder.target;
            int[] target = new int[2] { (int)pos.X, (int)pos.Y };

            return PositionCondition(time_since_action, target);
        }

        // Single Use Action Pattern
        // Returns true once, and then false for all other calls
        protected virtual bool SingleUseCondition(int time_since_action, int[] settings)
        {
            return time_since_action == 0;
        }
    }
}

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using System.Runtime.CompilerServices;

namespace Project.Sprites.Enemies.PathFinding
{
    // A data class which stores callbacks for enemy actions
    public class ActionPattern
    {
        public delegate void ActionCallback(GameTime time, int time_since_action, int[] settings);
        public delegate bool ConditionCallback(int time_since_action, int[] settings);
        public delegate void FinishedPatternCallback(int iterations);

        // lists operate in pairs
        private List<ActionCallback> actions;
        private List<ConditionCallback> conditions;
        private List<int[]> actionSettings;
        private List<int[]> conditionSettings;

        private FinishedPatternCallback finishedCallback;

        private int totalIterations;
        private int currentAction;
        private int timeSinceAction;

        // Optional callback for when each pattern iteration is finished
        public ActionPattern(FinishedPatternCallback finishedCallback = null)
        {
            actions = new List<ActionCallback>();
            conditions = new List<ConditionCallback>();
            actionSettings = new List<int[]>();
            conditionSettings = new List<int[]>();

            this.finishedCallback = finishedCallback;

            Reset();
        }

        public void AddAction(ActionCallback action, ConditionCallback condition, int[] actionSetting = null, int[] conditionSetting = null)
        {
            actions.Add(action);
            conditions.Add(condition);

            if (actionSetting != null)
                actionSettings.Add(actionSetting);
            else
                actionSettings.Add(null);

            if (conditionSetting != null)
                conditionSettings.Add(conditionSetting);
            else
                conditionSettings.Add(null);
        }

        public void Update(GameTime time)
        {
            // Increment iteration counter, activate callback and move back to first
            if (currentAction >= actions.Count)
            {
                SetAction(0);
                finishedCallback?.Invoke(++totalIterations);
            }

            if (conditions[currentAction](timeSinceAction, conditionSettings[currentAction]))
                actions[currentAction](time, timeSinceAction, actionSettings[currentAction]);
            else
                NextAction();

            timeSinceAction += time.ElapsedGameTime.Milliseconds;
        }

        // Resets all counters and sets the current action to the first
        public void Reset()
        {
            SetAction(0);
            totalIterations = 0;
        }

        public void NextAction()
        {
            SetAction(currentAction + 1);
        }

        public void SetAction(int action)
        {
            currentAction = action;
            timeSinceAction = 0;
        }
    }
}

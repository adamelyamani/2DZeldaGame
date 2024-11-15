using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Microsoft.Xna.Framework.Graphics;
using Project.App;

namespace Project.Sprites.Enemies.PathFinding
{
    // Uses A* algorithm to find best path
    public class PathFinder : IPathFinder
    {
        // constants
        const int grid_size = 15; // Checks every 30 pixels in
        const int search_limit = 150; // Max number of closed nodes to search before giving up
        const int found_target_proximity = grid_size * 3; // How close the pos must be to the target before stopping search
        const int final_path_node_proximity = 2; // How close the pos must be to each sub-target in finalPath before switching to next target
        const int bounding_size_offset = -2; // How much to offset the bounding box size by for initial collision check
        const int wide_bounding_size_offset = 1; // How much to offset the bounding box size by for computational purposes

        public Vector2 start { get; }
        public Vector2 target { get; }

        // convert start and target to int for efficiency with large runs
        Tuple<int, int> startI;
        Tuple<int, int> targetI;
        
        // reference to level for Path Collisions
        private Level level;

        private Rectangle boundingBox;
        private Rectangle wideBoundingBox;

        // map integer x,y to PathNode
        private Dictionary<int, PathNode> nodes = new Dictionary<int, PathNode>(); // Complete list of all nodes added to all lists
        private Dictionary<Tuple<int, int>, PathNode> open = new Dictionary<Tuple<int, int>, PathNode>();
        private Dictionary<Tuple<int, int>, PathNode> closed = new Dictionary<Tuple<int, int>, PathNode>();

        private int node_count;

        // Final PathNode closest to target
        private List<Vector2> finalPath;

        // Vars for Accessing Path
        private int lastPathIndex;

        // An instance class to find a path from start to target through the level using A* algorithm
        // When Creating, Specify Start and End, level for collision and a bounding box
        public PathFinder(Vector2 start, Vector2 target, Level level, Rectangle boundingBox)
        {
            this.start = start;
            this.target = target;
            this.level = level;

            this.startI = new Tuple<int, int>((int)start.X, (int)start.Y);
            this.targetI = new Tuple<int, int>((int)target.X, (int)target.Y);

            // Correct bounding box so it's the correct size
            boundingBox.Offset(-boundingBox.Center.X, -boundingBox.Center.Y);

            this.boundingBox = boundingBox;
            this.boundingBox.Inflate(bounding_size_offset, bounding_size_offset);

            this.wideBoundingBox = boundingBox;
            this.wideBoundingBox.Inflate(wide_bounding_size_offset, wide_bounding_size_offset);

            node_count = 0;
            lastPathIndex = 1; // starts at first non-start node

            GeneratePathGraph();
        }

        public Vector2 GetTargetAlongPath(Vector2 pos)
        {
            // If There is no path generated then return the target
            if (finalPath == null) { return target; }

            Vector2 currentSub = finalPath[lastPathIndex];

            float currentPosDist = Vector2.Distance(currentSub, pos);

            // First check if pos is ready to switch to next sub-target
            if (currentPosDist <= final_path_node_proximity && lastPathIndex + 1 < finalPath.Count) { lastPathIndex++; }

            return finalPath[lastPathIndex];
        }

        // Implement A* Algorithm
        public void GeneratePathGraph()
        {
            // Create Start Node
            PathNode start_node = InitNode(-1, startI.Item1, startI.Item2, 0, 0);
            closed.Add(startI, start_node);

            // Create 2d array of all vertical, horizontal, and diagonal possible directions
            int[,] directions = new int[4, 2] {
                { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 }
            };

            // Loop through all nodes in the closed list until the target is found
            PathNode base_node = start_node;
            PathNode node = base_node; // default set

            bool found_target = false;

            while (closed.Count < search_limit)
            {

                // Loop through all directions
                for (int i = 0; i < directions.GetLength(0); i++)
                {
                    // get direction
                    int x = base_node.x + directions[i, 0] * grid_size;
                    int y = base_node.y + directions[i, 1] * grid_size;

                    Tuple<int, int> pos = new Tuple<int, int>(x, y);

                    // check if pos is valid
                    if (!closed.ContainsKey(pos) && CheckLevelPos(x, y, wideBoundingBox))
                    {
                        node = CreateNode(base_node, x, y, GetF(x, y), GetH(x, y));

                        // See if there is already an identical node in the open list
                        PathNode openNode;

                        // If pos is the same
                        if (open.TryGetValue(pos, out openNode))
                        {
                            // Compare G Values
                            if (node.g < openNode.g)
                            {
                                // Replace the old node with the current node in the list
                                open.Remove(pos);
                                open.Add(pos, node);
                            }

                        }
                        else
                        {
                            // add to open list
                            open.Add(pos, node);
                        }
                    }
                }

                // Exit if there are no valid nodes around base node
                if (node == start_node)
                    return;

                // Choose the Lowest G value for next path
                KeyValuePair<Tuple<int, int>, PathNode> lowest = new KeyValuePair<Tuple<int, int>, PathNode>(new Tuple<int, int>(node.x, node.y), node);

                // iterate through all entries in open list and find the lowest g value node
                foreach (KeyValuePair<Tuple<int, int>, PathNode> entry in open)
                {
                    if (entry.Value.g < lowest.Value.g)
                    {
                        lowest = entry;
                    }
                }

                // check if any nodes were valid
                // if not, then back track to previous node's parent
                if (closed.ContainsKey(lowest.Key))
                {
                    // This means we have backtracked as far as possible
                    if (base_node.parent == -1)
                        return;

                    base_node = nodes[base_node.parent];
                }
                // Normal Case
                else
                {
                    // remove lowest from open list
                    open.Remove(lowest.Key);
                    closed.Add(lowest.Key, lowest.Value);

                    // Set next search node to lowest
                    base_node = lowest.Value;

                    // Break if target is within grid range of base node
                    int distX = (base_node.x - targetI.Item1);
                    int distY = (base_node.y - targetI.Item2);
                    distX *= distX;
                    distY *= distY;

                    // Sqrt is too expensive, so we can just compare squares
                    if (distX + distY < found_target_proximity * found_target_proximity)
                    {
                        found_target = true;
                        break;
                    }
                }
            }

            // Populate final path list
            if (closed.Count < search_limit && found_target) { GenerateFinalPath(base_node); }
            else { Debug.Print("No Path was found"); }
        }

        // Generate Final Path
        // This is done by backtracking from the target node to the start node
        // The final path is a list of Vector2 positions from start to finish
        private void GenerateFinalPath(PathNode node)
        {
            finalPath = new List<Vector2> {
                new Vector2(node.x, node.y),
                target
            };

            // report final path
            while (node.parent != -1)
            {
                // Set node to parent
                node = nodes[node.parent];

                finalPath.Insert(0, new Vector2(node.x, node.y));
            }
        }

        // Create A* H Heuristic
        // This is simply a baby distance formula
        private int GetH(int x, int y)
        {
            return DistanceFormula(x, y, (int)target.X, (int)target.Y);
        }

        // Create A* F Heuristic
        // This is simply a baby distance formula
        private int GetF(int x, int y)
        {
            return DistanceFormula(x, y, (int)start.X, (int)start.Y);
        }

        // Simplistic Low Cost Distance Formula
        private int BabyDistanceFormula(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        // Proper Distance Formula
        private int DistanceFormula(int x1, int y1, int x2, int y2)
        {
            return (int)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)); ;
        }

        // returns true if there are no obstacles at position
        // takes bounding box into account
        private bool CheckLevelPos(int x, int y, Rectangle boundingBox)
        {
            boundingBox.Offset(x, y);
            bool result = !level.CheckRectToEnviornment(boundingBox);
            boundingBox.Offset(-x, -y);

            return result;
        }

        public PathNode CreateNode(PathNode parent, int x, int y, int f, int h)
        {
            return InitNode(parent.id, x, y, f, h);
        }

        private PathNode InitNode(int parent, int x, int y, int f, int h)
        {
            PathNode node = new PathNode(node_count++, parent, x, y, f, h);
            nodes.Add(node.id, node);

            return node;
        }

        public bool PathFound()
        {
            return finalPath != null;
        }

        public void Draw(Game1 game, SpriteBatch obj)
        {
            //DrawFinalPath(game, obj);
            DrawNodes(game, obj);
        }

        public void DrawNodes(Game1 game, SpriteBatch obj)
        {
            // Check if there is a path
            if (nodes.Count == 0) { Debug.Print("No Path was found"); return; }

            // Draw final path
            const int path_width = 5;

            Texture2D tex = new Texture2D(game.GraphicsDevice, 1, 1);
            tex.SetData(new[] { Color.Red });

            Rectangle destinationRectangle;

            obj.Begin();
            foreach (var entry in nodes.Values)
            {
                // create vector2's of entry and parent
                Vector2 node = new Vector2(entry.x, entry.y);

                obj.Draw(tex, new Rectangle((int)node.X, (int)node.Y, path_width, path_width), Color.Red);
            }
            obj.End();
        }

        public void DrawClosedNodes(Game1 game, SpriteBatch obj)
        {
            // Check if there is a path
            if (closed.Count == 0) { Debug.Print("No Path was found"); return; }

            // Draw final path
            const int path_width = 5;

            Texture2D tex = new Texture2D(game.GraphicsDevice, 1, 1);
            tex.SetData(new[] { Color.White });

            Rectangle destinationRectangle;

            obj.Begin();
            foreach (var entry in closed.Values)
            {
                // Create keypair
                if (entry.parent == -1) { continue; }
                PathNode parent = nodes[entry.parent];

                // find double angle between entry and parent
                float angle = (float)(Math.Atan2(entry.y - parent.y, entry.x - parent.x) + Math.PI * 2);

                // create vector2's of entry and parent
                Vector2 prevNode = new Vector2(parent.x, parent.y);
                Vector2 node = new Vector2(entry.x, entry.y);

                destinationRectangle = new((int)prevNode.X, (int)prevNode.Y, (int)Vector2.Distance(prevNode, node), path_width);
                obj.Draw(tex, destinationRectangle, null, Color.Red, angle, Vector2.Zero, SpriteEffects.None, 0);

                //obj.Draw(tex, new Rectangle((int)prevNode.X, (int)prevNode.Y, path_width, path_width), Color.Red);
            }
            obj.End();
        }

        public void DrawFinalPath(Game1 game, SpriteBatch obj)
        {
            // Check if there is a path
            if (finalPath == null) { Debug.Print("No Path was found"); return; }

            // Draw final path
            const int path_width = 5;

            Texture2D tex = new Texture2D(game.GraphicsDevice, 1, 1);
            tex.SetData(new[] { Color.White });

            Rectangle sourceRectangle = new(0, 0, tex.Width, tex.Height);
            Rectangle destinationRectangle;

            obj.Begin();
            for (int i = 1; i < finalPath.Count; i++)
            {
                Vector2 prevNode = finalPath[i - 1];
                Vector2 node = finalPath[i];

                // find double angle between prevNode and node
                float angle = (float)(Math.Atan2(node.Y - prevNode.Y, node.X - prevNode.X) + Math.PI * 2);

                destinationRectangle = new((int)prevNode.X, (int)prevNode.Y, (int)Vector2.Distance(prevNode, node), path_width);
                obj.Draw(tex, destinationRectangle, null, Color.Red, angle, Vector2.Zero, SpriteEffects.None, 0);
            }
            obj.End();
        }

        public void DebugPathNode(PathNode node)
        {
            // report final path
            int count = 0;
            while (node.parent != -1)
            {
                // Add node to path
                int dist = BabyDistanceFormula(node.x, node.y, nodes[node.parent].x, nodes[node.parent].y);

                Debug.Print("Node " + ++count + "- Dist: " + dist + " X: " + node.x + " Y: " + node.y);

                // Set node to parent
                node = nodes[node.parent];
            }
        }
    }
}

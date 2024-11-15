using Microsoft.Xna.Framework;
using Project.Interfaces;
using Project.Sprites.Players;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Project.Sprites.Enemies.PathFinding
{
    public struct PathNode
    {
        public int id;
        
        // ints reference an outer list which contain all PathNodes
        public List<int> children;
        public int parent;

        // position of node
        public int x;
        public int y;

        // Following A* Algorithm
        public int f;
        public int h;
        public int g;

        public PathNode(int id, int parent, int x, int y, int f, int h)
        {
            this.id = id;
            this.parent = parent;
            this.children = new List<int>();

            this.x = x;
            this.y = y;

            this.f = f;
            this.h = h;

            this.g = this.f + this.h;
        }

        // Returns true if the position of the node is the same as the given position
        public static bool ComparePos(PathNode a, PathNode b)
        {
            return (a.x == b.x) && (a.y == b.y);
        }

        // Compares id
        public static bool operator ==(PathNode a, PathNode b)
        {
            return a.id == b.id;
        }

        // Compares id
        public static bool operator !=(PathNode a, PathNode b)
        {
            return a.id != b.id;
        }

        // Compares id
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) { return false; }

            PathNode other = (PathNode)obj;
            return this.id == other.id;
        }
    }
}

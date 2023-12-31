﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giny.World.Managers.Maps.Paths
{
    public class Node : INode
    {
        private MapPoint m_point;
        private Node m_parent;

        public Node(MapPoint point)
        {
            this.m_point = point;
        }

        public short CellId
        {
            get
            {
                return this.m_point.CellId;
            }
        }
        public MapPoint[] Neighbors
        {
            get
            {
                return m_point.GetNearPoints();
            }
        }

        public int X
        {
            get
            {
                return this.m_point.X;
            }
        }
        public int Y
        {
            get { return this.m_point.Y; }
        }
        public Node Parent
        {
            get
            {
                return this.m_parent;
            }
            set
            {
                this.SetParent(value);
            }
        }

        public int G
        {
            get;
            private set;
        }
        public int H
        {
            get;
            private set;
        }

        public int F => this.G + this.H;

        public bool Walkable
        {
            get;
            set;
        }

        private void SetParent(Node parent)
        {
            this.m_parent = parent;
            if (parent != null)
                this.G = this.m_parent.G + 10;
        }

        public void SetHeuristic(Node endPoint)
        {
            this.H = Math.Abs(this.X - endPoint.X) + Math.Abs(this.Y - endPoint.Y);
        }

        public int CostWillBe()
        {
            return (this.m_parent != null ? this.m_parent.G + 10 : 0);
        }
    }

    public class NodeList<T> : List<T> where T : INode
    {
        public T RemoveFirst()
        {
            T first = this[0];
            this.RemoveAt(0);
            return first;
        }

        public new bool Contains(T node)
        {
            return this[node] != null;
        }

        public T this[T node]
        {
            get
            {
                foreach (T n in this)
                {
                    if (n.CellId == node.CellId) return n;
                }
                return default(T);
            }
        }
    }

    public class SortedNodeList<T> : NodeList<T> where T : INode
    {
        public void AddDichotomic(T node)
        {
            int left = 0;
            int right = this.Count - 1;
            int center = 0;

            while (left <= right)
            {
                center = (left + right) / 2;
                if (node.F < this[center].F)
                    right = center - 1;
                else if (node.F > this[center].F)
                    left = center + 1;
                else
                {
                    left = center;
                    break;
                }
            }
            this.Insert(left, node);
        }
    }

    public interface INode
    {
        int F
        {
            get;
        }
        short CellId
        {
            get;
        }
    }
}

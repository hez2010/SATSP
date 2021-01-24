using System;

namespace SATSP
{
    public record Node(string Name, double X, double Y)
    {
        /// <summary>
        /// Calculate distance between two nodes
        /// </summary>
        /// <param name="a">The first node</param>
        /// <param name="b">The second node</param>
        /// <returns>Distance between two nodes</returns>
        public static double operator -(Node a, Node b) => 
            Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }
}

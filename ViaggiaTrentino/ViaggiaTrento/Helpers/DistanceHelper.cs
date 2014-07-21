using System;
using System.Windows;

namespace ViaggiaTrentino.Helpers
{
    public static class DistanceHelper
    {
        public static double GetDistanceTo(this Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
    }
}

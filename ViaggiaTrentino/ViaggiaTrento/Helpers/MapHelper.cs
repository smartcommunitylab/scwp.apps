using Microsoft.Phone.Maps.Controls;
using System.Windows;

namespace ViaggiaTrentino.Helpers
{
    public static class MapHelper
    {
        public static bool IsVisiblePoint(this Map map, Point point)
        {
            return point.X > 0 && point.X < map.ActualWidth && point.Y > 0 && point.Y < map.ActualHeight;
        }
    }
}

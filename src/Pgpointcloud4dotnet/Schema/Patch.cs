using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pgpointcloud4dotnet
{
    public class Patch
    {
        public int NumberOfPoints { get; }

        private List<Point> _points;

        internal Patch(uint numberOfPoints)
        {
            NumberOfPoints = (int)numberOfPoints;
            _points = new List<Point>((int)numberOfPoints);
        }

        public IReadOnlyList<Point> Points => _points;

        internal void AddPoint(Point point)
        {
            _points.Add(point);
        }
    }
}

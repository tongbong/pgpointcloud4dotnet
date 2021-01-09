using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pgpointcloud4dotnet
{
    public class Patch
    {

        private List<Point> _points;

        internal Patch(uint numberOfPoints)
        {
            _points = new List<Point>((int)numberOfPoints);
        }

        public IReadOnlyList<Point> Points => _points;

        internal void AddPoint(Point point)
        {
            _points.Add(point);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billknye.GameLib
{
    // http://blogs.msdn.com/b/ericlippert/archive/2011/12/29/shadowcasting-in-c-part-six.aspx
    public class GridFieldOfView
    {
        private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

        /// <summary>
        /// The plot function delegate
        /// </summary>
        /// <param name="x">The x co-ord being plotted</param>
        /// <param name="y">The y co-ord being plotted</param>
        /// <returns>True to continue, false to stop the algorithm</returns>
        public delegate bool PlotFunction(int x, int y);

        /// <summary>
        /// Plot the line from (x0, y0) to (x1, y10
        /// </summary>
        /// <param name="x0">The start x</param>
        /// <param name="y0">The start y</param>
        /// <param name="x1">The end x</param>
        /// <param name="y1">The end y</param>
        /// <param name="plot">The plotting function (if this returns false, the algorithm stops early)</param>
        public static void Line(int x0, int y0, int x1, int y1, PlotFunction plot)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) { Swap<int>(ref x0, ref y0); Swap<int>(ref x1, ref y1); }
            if (x0 > x1) { Swap<int>(ref x0, ref x1); Swap<int>(ref y0, ref y1); }
            int dX = (x1 - x0), dY = Math.Abs(y1 - y0), err = (dX / 2), ystep = (y0 < y1 ? 1 : -1), y = y0;

            for (int x = x0; x <= x1; ++x)
            {
                if (!(steep ? plot(y, x) : plot(x, y))) return;
                err = err - dY;
                if (err < 0) { y += ystep; err += dX; }
            }
        }


        public static void ComputeFieldOfViewWithShadowCasting(int x, int y, int radius, Func<int, int, bool> isOpaque, Action<int, int> setFoV)
        {
            Func<int, int, bool> opaque = TranslateOrigin(isOpaque, x, y);
            Action<int, int> fov = TranslateOrigin(setFoV, x, y);

            for (int octant = 0; octant < 8; ++octant)
            {
                ComputeFieldOfViewInOctantZero(TranslateOctant(opaque, octant), TranslateOctant(fov, octant), radius);
            }
        }

        private static void ComputeFieldOfViewInOctantZero(Func<int, int, bool> isOpaque, Action<int, int> setFieldOfView, int radius)
        {
            var queue = new Queue<ColumnPortion>(20);
            queue.Enqueue(new ColumnPortion(0, new DirectionVector(1, 0), new DirectionVector(1, 1)));
            while (queue.Count != 0)
            {
                var current = queue.Dequeue();
                if (current.X > radius)
                    continue;

                ComputeFoVForColumnPortion(current.X, current.TopVector, current.BottomVector, isOpaque, setFieldOfView, radius, queue);
            }
        }

        // This method has two main purposes: (1) it marks points inside the
        // portion that are within the radius as in the field of view, and 
        // (2) it computes which portions of the following column are in the 
        // field of view, and puts them on a work queue for later processing. 
        private static void ComputeFoVForColumnPortion(int x, DirectionVector topVector, DirectionVector bottomVector, Func<int, int, bool> isOpaque, Action<int, int> setFieldOfView, int radius, Queue<ColumnPortion> queue)
        {
            // Search for transitions from opaque to transparent or
            // transparent to opaque and use those to determine what
            // portions of the *next* column are visible from the origin.

            // Start at the top of the column portion and work down.

            int topY;
            if (x == 0)
                topY = 0;
            else
            {
                int quotient = (2 * x + 1) * topVector.Y / (2 * topVector.X);
                int remainder = (2 * x + 1) * topVector.Y % (2 * topVector.X);

                if (remainder > topVector.X)
                    topY = quotient + 1;
                else
                    topY = quotient;
            }

            // Note that this can find a top cell that is actually entirely blocked by
            // the cell below it; consider detecting and eliminating that.


            int bottomY;
            if (x == 0)
                bottomY = 0;
            else
            {
                int quotient = (2 * x - 1) * bottomVector.Y / (2 * bottomVector.X);
                int remainder = (2 * x - 1) * bottomVector.Y % (2 * bottomVector.X);

                if (remainder >= bottomVector.X)
                    bottomY = quotient + 1;
                else
                    bottomY = quotient;
            }

            // A more sophisticated algorithm would say that a cell is visible if there is 
            // *any* straight line segment that passes through *any* portion of the origin cell
            // and any portion of the target cell, passing through only transparent cells
            // along the way. This is the "Permissive Field Of View" algorithm, and it
            // is much harder to implement.

            bool? wasLastCellOpaque = null;
            for (int y = topY; y >= bottomY; --y)
            {
                bool inRadius = IsInRadius(x, y, radius);
                if (inRadius)
                {
                    // The current cell is in the field of view.
                    setFieldOfView(x, y);
                }

                // A cell that was too far away to be seen is effectively
                // an opaque cell; nothing "above" it is going to be visible
                // in the next column, so we might as well treat it as 
                // an opaque cell and not scan the cells that are also too
                // far away in the next column.

                bool currentIsOpaque = !inRadius || isOpaque(x, y);
                if (wasLastCellOpaque != null)
                {
                    if (currentIsOpaque)
                    {
                        // We've found a boundary from transparent to opaque. Make a note
                        // of it and revisit it later.
                        if (!wasLastCellOpaque.Value)
                        {
                            // The new bottom vector touches the upper left corner of 
                            // opaque cell that is below the transparent cell. 
                            queue.Enqueue(new ColumnPortion(
                                x + 1,
                                new DirectionVector(x * 2 - 1, y * 2 + 1),
                                topVector));
                        }
                    }
                    else if (wasLastCellOpaque.Value)
                    {
                        // We've found a boundary from opaque to transparent. Adjust the
                        // top vector so that when we find the next boundary or do
                        // the bottom cell, we have the right top vector.
                        //
                        // The new top vector touches the lower right corner of the 
                        // opaque cell that is above the transparent cell, which is
                        // the upper right corner of the current transparent cell.
                        topVector = new DirectionVector(x * 2 + 1, y * 2 + 1);
                    }
                }
                wasLastCellOpaque = currentIsOpaque;
            }

            // Make a note of the lowest opaque-->transparent transition, if there is one. 
            if (wasLastCellOpaque != null && !wasLastCellOpaque.Value)
                queue.Enqueue(new ColumnPortion(x + 1, bottomVector, topVector));
        }

        private struct ColumnPortion
        {
            public int X { get; private set; }
            public DirectionVector BottomVector { get; private set; }
            public DirectionVector TopVector { get; private set; }

            public ColumnPortion(int x, DirectionVector bottom, DirectionVector top)
                : this()
            {
                this.X = x;
                this.BottomVector = bottom;
                this.TopVector = top;
            }
        }

        // Is the lower-left corner of cell (x,y) within the radius?
        private static bool IsInRadius(int x, int y, int length)
        {
            return (2 * x - 1) * (2 * x - 1) + (2 * y - 1) * (2 * y - 1) <= 4 * length * length;
        }

        private struct DirectionVector
        {
            public int X { get; private set; }
            public int Y { get; private set; }

            public DirectionVector(int x, int y)
                : this()
            {
                this.X = x;
                this.Y = y;
            }
        }

        // Octant helpers
        //
        //
        //                 \2|1/
        //                 3\|/0
        //               ----+----
        //                 4/|\7
        //                 /5|6\
        //
        // 

        private static Func<int, int, T> TranslateOrigin<T>(Func<int, int, T> f, int x, int y)
        {
            return (a, b) => f(a + x, b + y);
        }

        private static Action<int, int> TranslateOrigin(Action<int, int> f, int x, int y)
        {
            return (a, b) => f(a + x, b + y);
        }

        private static Func<int, int, T> TranslateOctant<T>(Func<int, int, T> f, int octant)
        {
            switch (octant)
            {
                default: return f;
                case 1: return (x, y) => f(y, x);
                case 2: return (x, y) => f(-y, x);
                case 3: return (x, y) => f(-x, y);
                case 4: return (x, y) => f(-x, -y);
                case 5: return (x, y) => f(-y, -x);
                case 6: return (x, y) => f(y, -x);
                case 7: return (x, y) => f(x, -y);
            }
        }

        private static Action<int, int> TranslateOctant(Action<int, int> f, int octant)
        {
            switch (octant)
            {
                default: return f;
                case 1: return (x, y) => f(y, x);
                case 2: return (x, y) => f(-y, x);
                case 3: return (x, y) => f(-x, y);
                case 4: return (x, y) => f(-x, -y);
                case 5: return (x, y) => f(-y, -x);
                case 6: return (x, y) => f(y, -x);
                case 7: return (x, y) => f(x, -y);
            }
        }
    }
}

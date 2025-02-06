using Spirit604.Extensions;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Car
{
    public struct ObstacleLayout
    {
        public Vector3 LeftBottomPoint { get; private set; }
        public Vector3 LeftTopPoint { get; private set; }
        public Vector3 RightBottomPoint { get; private set; }
        public Vector3 RightTopPoint { get; private set; }

        public ObstacleLayout(Vector3 position, Quaternion rotation, Vector3 extents)
        {
            LeftBottomPoint = position + rotation * new Vector3(-extents.x, 0, -extents.z);
            LeftTopPoint = position + rotation * new Vector3(-extents.x, 0, extents.z);
            RightBottomPoint = position + rotation * new Vector3(extents.x, 0, -extents.z);
            RightTopPoint = position + rotation * new Vector3(extents.x, 0, extents.z);
        }

        public Vector3 GetCurrentSize()
        {
            var leftBottomPoint = LeftBottomPoint;
            var rightBottomPoint = RightBottomPoint;
            var rightTopPoint = RightTopPoint;
            var leftTopPoint = LeftTopPoint;

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minZ = float.MaxValue;
            float maxZ = float.MinValue;

            if (minX > leftBottomPoint.x)
            {
                minX = leftBottomPoint.x;
            }
            if (minX > rightBottomPoint.x)
            {
                minX = rightBottomPoint.x;
            }
            if (minX > rightTopPoint.x)
            {
                minX = rightTopPoint.x;
            }
            if (minX > leftTopPoint.x)
            {
                minX = leftTopPoint.x;
            }

            if (minZ > leftBottomPoint.z)
            {
                minZ = leftBottomPoint.z;
            }
            if (minZ > rightBottomPoint.z)
            {
                minZ = rightBottomPoint.z;
            }
            if (minZ > rightTopPoint.z)
            {
                minZ = rightTopPoint.z;
            }
            if (minZ > leftTopPoint.z)
            {
                minZ = leftTopPoint.z;
            }

            if (maxX < leftBottomPoint.x)
            {
                maxX = leftBottomPoint.x;
            }
            if (maxX < rightBottomPoint.x)
            {
                maxX = rightBottomPoint.x;
            }
            if (maxX < rightTopPoint.x)
            {
                maxX = rightTopPoint.x;
            }
            if (maxX < leftTopPoint.x)
            {
                maxX = leftTopPoint.x;
            }

            if (maxZ < leftBottomPoint.z)
            {
                maxZ = leftBottomPoint.z;
            }
            if (maxZ < rightBottomPoint.z)
            {
                maxZ = rightBottomPoint.z;
            }
            if (maxZ < rightTopPoint.z)
            {
                maxZ = rightTopPoint.z;
            }
            if (maxZ < leftTopPoint.z)
            {
                maxZ = leftTopPoint.z;
            }

            var size = new Vector3(maxX - minX, 0, maxZ - minZ) / 2;

            return size;
        }
    }

    public struct ObstacleSquare
    {
        public VectorExtensions.Square Square { get; set; }

        public ObstacleSquare(Vector3 position, Quaternion rotation, Vector3 extents)
        {
            ObstacleLayout layout = new ObstacleLayout(position, rotation, extents);

            VectorExtensions.Line line1 = new VectorExtensions.Line(layout.LeftBottomPoint, layout.LeftTopPoint);
            VectorExtensions.Line line2 = new VectorExtensions.Line(layout.RightBottomPoint, layout.RightTopPoint);

            Square = new VectorExtensions.Square(line1, line2);
        }

        public ObstacleSquare(ObstacleLayout layout)
        {
            VectorExtensions.Line line1 = new VectorExtensions.Line(layout.LeftBottomPoint, layout.LeftTopPoint);
            VectorExtensions.Line line2 = new VectorExtensions.Line(layout.RightBottomPoint, layout.RightTopPoint);

            Square = new VectorExtensions.Square(line1, line2);
        }
    }
}
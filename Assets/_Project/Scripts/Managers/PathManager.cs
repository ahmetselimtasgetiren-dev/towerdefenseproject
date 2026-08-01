using System;
using UnityEngine;

namespace TowerDefenseIncremental
{
    public sealed class PathManager : MonoBehaviour
    {
        private LevelData level;

        public Vector2[] Waypoints => level != null
            ? level.Waypoints
            : Array.Empty<Vector2>();

        public void Initialize(LevelData levelData)
        {
            level = levelData;
            if (level == null || Waypoints.Length < 2)
                Debug.LogError("A LevelData asset with at least two waypoints is required.");
        }

        public bool IsBuildable(Vector2 point)
        {
            if (level == null ||
                point.x < level.BuildMinimum.x ||
                point.y < level.BuildMinimum.y ||
                point.x > level.BuildMaximum.x ||
                point.y > level.BuildMaximum.y)
                return false;

            for (var index = 0; index < Waypoints.Length - 1; index++)
            {
                if (Distance(point, Waypoints[index], Waypoints[index + 1]) < level.PathClearance)
                    return false;
            }

            return true;
        }

        private static float Distance(Vector2 point, Vector2 start, Vector2 end)
        {
            var segment = end - start;
            if (segment.sqrMagnitude <= Mathf.Epsilon)
                return Vector2.Distance(point, start);

            var nearest = start + segment *
                Mathf.Clamp01(Vector2.Dot(point - start, segment) / segment.sqrMagnitude);
            return Vector2.Distance(point, nearest);
        }
    }
}

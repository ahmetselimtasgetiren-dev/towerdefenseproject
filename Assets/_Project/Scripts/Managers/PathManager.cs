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

        public void BuildBoard()
        {
            if (Waypoints.Length < 2)
                return;

            var boardSize = level.BuildMaximum - level.BuildMinimum;
            var boardCenter = (level.BuildMinimum + level.BuildMaximum) * 0.5f;
            SpriteFactory.Box(
                "Ground",
                boardCenter,
                boardSize + Vector2.one,
                new Color(0.10f, 0.16f, 0.23f),
                -10);

            for (var index = 0; index < Waypoints.Length - 1; index++)
            {
                var delta = Waypoints[index + 1] - Waypoints[index];
                var road = SpriteFactory.Box(
                    "Path",
                    (Waypoints[index] + Waypoints[index + 1]) * 0.5f,
                    new Vector2(delta.magnitude + 0.7f, 0.78f),
                    new Color(0.27f, 0.31f, 0.36f),
                    -5);
                road.transform.right = delta.normalized;
            }

            SpriteFactory.Box(
                "Start",
                Waypoints[0],
                new Vector2(0.6f, 0.6f),
                new Color(0.25f, 0.78f, 0.62f),
                -3);
            SpriteFactory.Box(
                "Exit",
                Waypoints[^1],
                new Vector2(0.6f, 0.6f),
                new Color(0.96f, 0.42f, 0.36f),
                -3);
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

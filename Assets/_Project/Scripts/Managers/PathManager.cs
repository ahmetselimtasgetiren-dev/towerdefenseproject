using UnityEngine;
namespace TowerDefenseIncremental
{
    public sealed class PathManager : MonoBehaviour
    {
        public Vector2[] Waypoints { get; } = { new(-8.2f, -2.5f), new(-4.6f, -2.5f), new(-4.6f, 1.8f), new(1.2f, 1.8f), new(1.2f, -1.2f), new(8.2f, -1.2f) };
        public void BuildBoard() { SpriteFactory.Box("Ground", Vector2.zero, new(18.5f, 10.5f), new(.10f, .16f, .23f), -10); for (var i = 0; i < Waypoints.Length - 1; i++) { var delta = Waypoints[i + 1] - Waypoints[i]; var road = SpriteFactory.Box("Path", (Waypoints[i] + Waypoints[i + 1]) * .5f, new(delta.magnitude + .7f, .78f), new(.27f, .31f, .36f), -5); road.transform.right = delta.normalized; } SpriteFactory.Box("Start", Waypoints[0], new(.6f, .6f), new(.25f, .78f, .62f), -3); SpriteFactory.Box("Exit", Waypoints[^1], new(.6f, .6f), new(.96f, .42f, .36f), -3); }
        public bool IsBuildable(Vector2 point) { if (Mathf.Abs(point.x) > 8.5f || Mathf.Abs(point.y) > 4.7f) return false; for (var i = 0; i < Waypoints.Length - 1; i++) if (Distance(point, Waypoints[i], Waypoints[i + 1]) < .72f) return false; return true; }
        private static float Distance(Vector2 p, Vector2 a, Vector2 b) { var ab = b - a; return Vector2.Distance(p, a + ab * Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude)); }
    }
}

using UnityEngine;

namespace TowerDefenseIncremental.Rendering
{
    /// <summary>Shared, renderer-agnostic operations for the offset-duplicate visual style.</summary>
    public static class FlatShadowUtility
    {
        public static Color DarkenValue(Color source, float darkenAmount)
        {
            Color.RGBToHSV(source, out var hue, out var saturation, out var value);
            var darkened = Color.HSVToRGB(hue, saturation, value * (1f - Mathf.Clamp01(darkenAmount)));
            darkened.a = source.a;
            return darkened;
        }

        public static void DestroyObject(Object target)
        {
            if (target == null) return;
            if (Application.isPlaying) Object.Destroy(target);
            else Object.DestroyImmediate(target);
        }
    }
}

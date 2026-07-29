using System.Collections.Generic;
using TowerDefenseIncremental.Rendering;
using UnityEngine;

namespace TowerDefenseIncremental
{
    /// <summary>Creates the small, deliberately hard-edged sprites used by the runtime prototype.</summary>
    public static class SpriteFactory
    {
        public enum Shape { Square, Circle, Triangle, Hexagon }

        private static readonly Dictionary<Shape, Sprite> sprites = new();

        public static GameObject Box(string name, Vector2 position, Vector2 scale, Color color, int order, bool withShadow = false)
            => CreateShape(name, Shape.Square, position, scale, color, order, withShadow);

        public static GameObject CreateTower(string name, Shape shape, Vector2 position, Color color, int order)
        {
            var tower = CreateShape(name, shape, position, new Vector2(.72f, .72f), color, order, true);
            AddFace(tower.transform, order + 1, false);
            return tower;
        }

        public static GameObject CreateEnemy(string name, Vector2 position, Color color, int order, bool tough)
        {
            var enemy = CreateShape(name, Shape.Circle, position, new Vector2(.48f, .48f), color, order, true);
            AddFace(enemy.transform, order + 1, tough);
            return enemy;
        }

        public static GameObject CreateShape(string name, Shape shape, Vector2 position, Vector2 scale, Color color, int order, bool withShadow = false)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            go.transform.localScale = scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = GetSprite(shape);
            renderer.color = color;
            renderer.sortingOrder = order;
            if (withShadow) go.AddComponent<FlatShadowDuplicator>();
            return go;
        }

        private static void AddFace(Transform parent, int order, bool tough)
        {
            var eyeColor = new Color(.05f, .08f, .13f);
            AddFacePixel(parent, "Eye Left", new Vector2(-.18f, .08f), new Vector2(.13f, .13f), eyeColor, order);
            AddFacePixel(parent, "Eye Right", new Vector2(.18f, .08f), new Vector2(.13f, .13f), eyeColor, order);
            if (tough) AddFacePixel(parent, "Mouth", new Vector2(0f, -.16f), new Vector2(.23f, .08f), eyeColor, order);
        }

        private static void AddFacePixel(Transform parent, string name, Vector2 localPosition, Vector2 size, Color color, int order)
        {
            var pixel = CreateShape(name, Shape.Square, Vector2.zero, size, color, order);
            pixel.transform.SetParent(parent, false);
            pixel.transform.localPosition = localPosition;
        }

        private static Sprite GetSprite(Shape shape)
        {
            if (sprites.TryGetValue(shape, out var sprite)) return sprite;

            const int size = 16;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
                texture.SetPixel(x, y, IsInside(shape, x, y, size) ? Color.white : Color.clear);
            texture.Apply(false, true);

            sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(.5f, .5f), size);
            sprites.Add(shape, sprite);
            return sprite;
        }

        private static bool IsInside(Shape shape, int x, int y, int size)
        {
            var px = x + .5f - size * .5f;
            var py = y + .5f - size * .5f;
            return shape switch
            {
                Shape.Square => true,
                Shape.Circle => px * px + py * py <= 55f,
                Shape.Triangle => py >= -6.5f && py <= 7.5f && Mathf.Abs(px) <= (7.5f - py) * .55f,
                Shape.Hexagon => Mathf.Abs(px) <= 7f && Mathf.Abs(py) <= 7f && Mathf.Abs(px) + Mathf.Abs(py) * .58f <= 9.8f,
                _ => false
            };
        }
    }
}

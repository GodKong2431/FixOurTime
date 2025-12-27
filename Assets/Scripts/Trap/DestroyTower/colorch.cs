using System.Collections.Generic;
using UnityEngine;

public static class SpriteAverageColorCache
{
    private static Dictionary<Sprite, Color> cache = new Dictionary<Sprite, Color>();

    public static Color GetAverageColor(Sprite sprite)
    {
        if (sprite == null)
            return Color.white;

        if (cache.TryGetValue(sprite, out Color cached))
            return cached;

        Texture2D tex = sprite.texture;
        Rect rect = sprite.rect;

        Color[] pixels = tex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

        Color sum = Color.black;
        int count = 0;

        foreach (var c in pixels)
        {
            if (c.a < 0.1f) continue;
            sum += c;
            count++;
        }

        Color avg = count > 0 ? sum / count : Color.white;
        cache.Add(sprite, avg);
        return avg;
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace UrbanFox
{
    public static class ColorExtensions
    {
        public static Color SetRed(this Color color, float r)
        {
            return new Color(r, color.g, color.b, color.a);
        }

        public static Color SetGreen(this Color color, float g)
        {
            return new Color(color.r, g, color.b, color.a);
        }

        public static Color SetBlue(this Color color, float b)
        {
            return new Color(color.r, color.g, b, color.a);
        }

        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Color SetHue(this Color color, float hue)
        {
            Color.RGBToHSV(color, out _, out float s, out float v);
            return Color.HSVToRGB(hue, s, v);
        }

        public static Color SetSaturation(this Color color, float saturation)
        {
            Color.RGBToHSV(color, out float h, out _, out float v);
            return Color.HSVToRGB(h, saturation, v);
        }

        public static Color SetValue(this Color color, float value)
        {
            Color.RGBToHSV(color, out float h, out float s, out _);
            return Color.HSVToRGB(h, s, value);
        }

        public static Graphic SetColor(this Graphic graphic, Color color)
        {
            if (graphic == null)
            {
                return null;
            }
            graphic.color = color;
            return graphic;
        }

        public static Graphic SetRed(this Graphic graphic, float r)
        {
            if (graphic == null)
            {
                return null;
            }
            return graphic.SetColor(graphic.color.SetRed(r));
        }

        public static Graphic SetGreen(this Graphic graphic, float g)
        {
            if (graphic == null)
            {
                return null;
            }
            return graphic.SetColor(graphic.color.SetGreen(g));
        }

        public static Graphic SetBlue(this Graphic graphic, float b)
        {
            if (graphic == null)
            {
                return null;
            }
            return graphic.SetColor(graphic.color.SetBlue(b));
        }

        public static Graphic SetAlpha(this Graphic graphic, float alpha)
        {
            if (graphic == null)
            {
                return null;
            }
            return graphic.SetColor(graphic.color.SetAlpha(alpha));
        }

        public static Graphic SetHue(this Graphic graphic, float hue)
        {
            if (graphic == null)
            {
                return null;
            }
            return graphic.SetColor(graphic.color.SetHue(hue));
        }

        public static Graphic SetSaturation(this Graphic graphic, float saturation)
        {
            if (graphic == null)
            {
                return null;
            }
            return graphic.SetColor(graphic.color.SetSaturation(saturation));
        }

        public static Graphic SetValue(this Graphic graphic, float value)
        {
            if (graphic == null)
            {
                return null;
            }
            return graphic.SetColor(graphic.color.SetValue(value));
        }
    }
}

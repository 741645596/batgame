using System;
using UnityEngine;

internal static class ExtensionMethods
{
    public static int G(this Color col)
    {
        return (int)(col.g * 255.0f);
    }
    public static int B(this Color col)
    {
        return (int)(col.b * 255.0f);
    }
    public static int R(this Color col)
    {
        return (int)(col.r * 255.0f);
    }
    public static int A(this Color col)
    {
        return (int)(col.a * 255.0f);
    }

    public static float GetHue(this Color c)
    {
        int r = c.R();
        int g = c.G();
        int b = c.B();

        byte minval = (byte)Math.Min(r, Math.Min(g, b));
        byte maxval = (byte)Math.Max(r, Math.Max(g, b));

        if (maxval == minval)
            return 0.0f;

        float diff = (float)(maxval - minval);
        float rnorm = (maxval - r) / diff;
        float gnorm = (maxval - g) / diff;
        float bnorm = (maxval - b) / diff;

        float hue = 0.0f;
        if (r == maxval)
            hue = 60.0f * (6.0f + bnorm - gnorm);
        if (g == maxval)
            hue = 60.0f * (2.0f + rnorm - bnorm);
        if (b == maxval)
            hue = 60.0f * (4.0f + gnorm - rnorm);
        if (hue > 360.0f)
            hue = hue - 360.0f;

        return hue;
    }
}
/// <summary>
/// 颜色转换辅助类
/// </summary>
/// <author>QFord</author>
public class ColorUtils
{

    public static Color FromArgb(int alpha, int red, int green, int blue)
    {
        float fa = ((float)alpha) / 255.0f;
        float fr = ((float)red) / 255.0f;
        float fg = ((float)green) / 255.0f;
        float fb = ((float)blue) / 255.0f;
        return new Color(fr, fg, fb, fa);
    }


    public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
    {
        int max = Math.Max(color.R(), Math.Max(color.G(), color.B()));
        int min = Math.Min(color.R(), Math.Min(color.G(), color.B()));

        hue = color.GetHue();
        saturation = (max == 0) ? 0 : 1d - (1d * min / max);
        value = max / 255d;
    }

    public static Color ColorFromHSV(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        int v = Convert.ToInt32(value);
        int p = Convert.ToInt32(value * (1 - saturation));
        int q = Convert.ToInt32(value * (1 - f * saturation));
        int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

        if (hi == 0)
            return FromArgb(255, v, t, p);
        else if (hi == 1)
            return FromArgb(255, q, v, p);
        else if (hi == 2)
            return FromArgb(255, p, v, t);
        else if (hi == 3)
            return FromArgb(255, p, q, v);
        else if (hi == 4)
            return FromArgb(255, t, p, v);
        else
            return FromArgb(255, v, p, q);
    }
}

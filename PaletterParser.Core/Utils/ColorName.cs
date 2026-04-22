public static class NextColorNaming
{
    static readonly string[] Intensity =
    {
        "Very Dark", "Dark", "Dim", "Muted",
        "Soft", "Bright", "Vivid", "Intense"
    };

    public static (int Index, string Name) GetName(int r, int g, int b)
    {
        int index = (r << 6) | (g << 3) | b;

        // --- Special cases ---
        if (r == 0 && g == 0 && b == 0)
            return (index, "Black");

        if (r == 7 && g == 7 && b == 7)
            return (index, "White");

        if (r == g && g == b)
            return (index, $"{Intensity[r]} Gray");

        // --- Determine dominant channels ---
        int max = Math.Max(r, Math.Max(g, b));

        bool red = r == max;
        bool green = g == max;
        bool blue = b == max;

        string baseColor;

        if (red && green && !blue)
            baseColor = "Yellow";
        else if (red && blue && !green)
            baseColor = "Magenta";
        else if (green && blue && !red)
            baseColor = "Cyan";
        else if (red)
            baseColor = "Red";
        else if (green)
            baseColor = "Green";
        else
            baseColor = "Blue";

        // --- Intensity from dominant channel ---
        string intensity = Intensity[max];

        return (index, $"{intensity} {baseColor}");
    }
}
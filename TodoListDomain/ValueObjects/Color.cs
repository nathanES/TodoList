using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace TodoList.Domain.ValueObjects;

public class Color : ValueObject
{

    private int red;

    public int Red
    {
        get { return red; }
        private set
        {
            if (!IsValidColor(value))
                throw new ArgumentException($"{nameof(Red)} must be a valid color value");
            red = value;
        }
    }

    private int green;

    public int Green
    {
        get { return green; }
        private set
        {
            if (!IsValidColor(value))
                throw new ArgumentException($"{nameof(Green)} must be a valid color value");
            green = value;
        }
    }

    private int blue;

    public int Blue
    {
        get { return blue; }
        private set
        {
            if (!IsValidColor(value))
                throw new ArgumentException($"{nameof(Blue)} must be a valid color value");
            blue = value;
        }
    }
    private static readonly Regex HexColorRegex = new("^[0-9a-fA-F]{2}$", RegexOptions.Compiled);

    [JsonConstructor]
    private Color()
    {
    }
    public Color(int red, int green, int blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }
    public Color(string redHexValue, string greenHexValue, string blueHexValue)
    {
        Red = ConvertHexComponentToDecimal(redHexValue);
        Green = ConvertHexComponentToDecimal(greenHexValue);
        Blue = ConvertHexComponentToDecimal(blueHexValue);
    }
    public Color(string colorHexValue)
    {
        if (colorHexValue.StartsWith("#"))
            colorHexValue = colorHexValue[1..]; // Retire le caractère '#'

        if (colorHexValue.Length != 6)
            throw new ArgumentException($"{nameof(colorHexValue)} must be exactly 6 characters long.");

        Red = ConvertHexComponentToDecimal(colorHexValue[..2]);
        Green = ConvertHexComponentToDecimal(colorHexValue.Substring(2, 2));
        Blue = ConvertHexComponentToDecimal(colorHexValue.Substring(4, 2));
    }
    private int ConvertHexComponentToDecimal(string hexValue)
    {
        if (!IsValidHexValue(hexValue))
            throw new ArgumentException($"{hexValue} is not a valid Hex Value");
        return Convert.ToInt32(hexValue, 16);
    }

    private bool IsValidColor(int color)
    {
        return color is >= 0 and <= 255;
    }
    private static bool IsValidHexValue(string value)
    {
        return HexColorRegex.IsMatch(value);
        //return value.All(c => c is (>= '0' and <= '9') or (>= 'A' and <= 'F') or (>= 'a' and <= 'f'));
    }

    public static Color Default
    {
        get
        {
            return new Color(255, 255, 255);
        }
    }

    public override string ToString()
    {
        return $"#{Red:X2}{Green:X2}{Blue:X2}";
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Red;
        yield return Green;
        yield return Blue;
    }
}

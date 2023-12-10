namespace TodoList.Domain.ValueObjects
{
  public class Color : ValueObject
  {
    //Todo : peut être mettre un enum pour les couleurs, laisser le format #RRGGBB pour le moment
    public string ColorValue { get; private set; }

    public Color(string color)
    {
      if (IsValidColor(color))
        ColorValue = color;
    }

    private bool IsValidColor(string color)
    {
      if (color.Length != 7)
        throw new ArgumentException("Color must be in the format #RRGGBB");
      return true;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return ColorValue;
    }
  }
}

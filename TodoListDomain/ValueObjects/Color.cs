using Newtonsoft.Json;

namespace TodoList.Domain.ValueObjects;

public class Color : ValueObject
{
  //Todo : peut être mettre un enum pour les couleurs, laisser le format #RRGGBB pour le moment
  [JsonProperty("ColorValue")]
  public string ColorValue { get; private set; }

  [JsonConstructor]
  private Color()
  {
  }

  public Color(string color) => ColorValue = IsValidColor(color) ? color : "#000000";
  private bool IsValidColor(string color)
  {
    if (color?.Length != 7)
      return false;
    return true;
  }
  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return ColorValue;
  }
}

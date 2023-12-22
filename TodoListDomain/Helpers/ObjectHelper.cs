using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Domain.Helpers
{
//TODO surement changer de classe, je ne suis pas trop fan
  public static class ObjectHelper
  {
  public static bool AreObjectsEqual<T>(T obj1, T obj2)
    {
      if (obj1 == null && obj2 == null)
        return true;

      if (obj1 == null || obj2 == null)
        return false;

      Type type = typeof(T);
      PropertyInfo[] properties = type.GetProperties();

      foreach (PropertyInfo property in properties)
      {
        object value1 = property.GetValue(obj1);
        object value2 = property.GetValue(obj2);

        if (!object.Equals(value1, value2))
        {
          return false;
        }
      }

      return true;
    }
  }
}

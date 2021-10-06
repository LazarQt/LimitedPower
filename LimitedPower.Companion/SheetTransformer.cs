using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LimitedPower.Companion.Model;

namespace LimitedPower.Companion
{
    public static class SheetTransformer<T> where T : new()
    {
        public static List<T> GetSheet(List<List<object>> sets)
        {
            var setsHeaders = sets.First().Select(o => o.ToString()).ToList();
            sets.RemoveAt(0);
            var allSets = new List<T>();
            foreach (var row in sets)
            {
                var newSet = new T();
                var t = newSet.GetType();

                for (var i = 0; i < setsHeaders.Count; i++)
                {
                    var header = setsHeaders[i];
                    if (string.IsNullOrEmpty(header)) continue;
                    var prop = t.GetProperty(header);
                    if (prop == null) continue;
                    object target;

                    if (i >= row.Count) continue;

                    var rowValue = row[i];
                    if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?))
                    {
                        target = Convert.ToInt32(rowValue);
                    }
                    else if (prop.PropertyType == typeof(string))
                    {
                        target = Convert.ToString(rowValue);
                    }
                    else if (prop.PropertyType == typeof(DateTime))
                    {
                        target = DateTime.Parse(Convert.ToString(rowValue), CultureInfo.CurrentCulture);
                    }
                    else if (prop.PropertyType == typeof(Outcome))
                    {
                        if (Enum.TryParse(Convert.ToString(rowValue), out Outcome outcome))
                        {
                            target = outcome;
                        }
                        else
                        {
                            throw new Exception("invalid enum");
                        }
                    }
                    else
                    {
                        throw new Exception("unexpected type");
                    }

                    prop.SetValue(newSet, target);
                }

                allSets.Add(newSet);
            }

            return allSets;
        }
    }
}

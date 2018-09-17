using System;
using System.Collections.Generic;
using System.Text;

namespace Random.Generator
{
    public static class IListExtensions
    {
        public static IList<double> Scale(this IList<double> input)
        {
            for (var i = 0; i < input.Count; i++)
            {
                input[i] *= input.Count;
            }
            return input;
        }
    }
}

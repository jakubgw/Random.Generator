using System;
using System.Collections.Generic;
using System.Text;

namespace Random.Generator
{
    public interface IRandom
    {
        int Generate(int maxValue);
    }
}

using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public interface LambdaFunc<T>
    {
        T Apply();
    }
}
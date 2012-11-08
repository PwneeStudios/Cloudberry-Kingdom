using System;
using System.Collections.Generic;

using CloudberryKingdom.Levels;

namespace CloudberryKingdom
{
    public interface LambdaFunc_1<T, OutputType>
    {
        OutputType Apply(T t);
    }
}
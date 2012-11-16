using System;
using System.Collections.Generic;

namespace CloudberryKingdom
{
    public interface LambdaFunc_1<T, OutputType>
    {
        OutputType Apply(T t);
    }
}
using System;
using System.Collections.Generic;



namespace CloudberryKingdom
{
    public interface LambdaFunc_2<T1, T2, OutputType>
    {
        OutputType Apply(T1 t1, T2 t2);
    }
}
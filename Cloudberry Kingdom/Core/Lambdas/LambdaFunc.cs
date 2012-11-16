using System;
using System.Collections.Generic;



namespace CloudberryKingdom
{
    public interface LambdaFunc<T>
    {
        T Apply();
    }
}
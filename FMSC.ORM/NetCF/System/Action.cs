using System;

using System.Collections.Generic;
using System.Text;

namespace System
{
    public delegate void Action();

    //public delegate void Action<T>(T obj);

    public delegate TResult Func<T, TResult>(T arg);
}

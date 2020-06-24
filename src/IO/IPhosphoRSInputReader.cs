using System;
using PhosphoRS.Wrapper;

namespace PhosphoRS.IO
{
    public interface IPhosphoRSInputReader : IDisposable
    {
         PhosphoRSInput Read();
    }
}
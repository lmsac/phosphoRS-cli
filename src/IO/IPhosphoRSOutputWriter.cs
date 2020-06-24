using System;
using PhosphoRS.Wrapper;

namespace PhosphoRS.IO
{
    public interface IPhosphoRSOutputWriter : IDisposable
    {
         void Write(PhosphoRSOutput output);
    }
}
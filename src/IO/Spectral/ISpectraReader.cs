using System;

namespace PhosphoRS.IO.Spectral
{
    public interface ISpectraReader : IDisposable
    {
         Spectrum Read();
    }
}
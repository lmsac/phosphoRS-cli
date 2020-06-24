using System;

namespace PhosphoRS.IO.PeptideResult
{
    public interface IPeptideResultReader : IDisposable
    {
         SpectrumQuery Read();
    }
}
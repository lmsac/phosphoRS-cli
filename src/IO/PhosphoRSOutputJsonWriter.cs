using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using IMP.PhosphoRS;
using PhosphoRS.Wrapper;
using PhosphoRS.Util;

namespace PhosphoRS.IO
{
    public class PhosphoRSOutputJsonWriter : IPhosphoRSOutputWriter
    {
        private readonly TextWriter _writer;

        public PhosphoRSOutputJsonWriter(Stream stream)
            : this(new StreamWriter(stream))
        {
        }

        public PhosphoRSOutputJsonWriter(TextWriter writer)            
        {
            _writer = writer;
        }

        public PhosphoRSOutputJsonWriter(string path)
            : this(File.CreateText(path))
        {
        }

        public virtual void Close() => _writer.Close();

        public void Dispose() => _writer.Close();

        public void Write(PhosphoRSOutput output)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var s = JsonConvert.SerializeObject(output, Formatting.Indented);
            _writer.Write(s);
        }
    }
}

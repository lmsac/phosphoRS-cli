using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PhosphoRS.Util;

namespace PhosphoRS.IO.Util
{
    public class TableWriter : IDisposable
    {
        public TableWriter(TextWriter writer, char delimiter = ' ')
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            _writer = writer;
            Delimiter = delimiter;
        }

        public char Delimiter { get; }

        public TableWriter WriteRow(IEnumerable<string> row)
        {
            if (row == null)
                return this;
            _writer.WriteLine(string.Join(
                Delimiter.ToString(),
                row.Select(f =>
                {
                    if (f?.Any(c => c == Delimiter || c == '\"' || c == '\r' || c == '\n' || char.IsWhiteSpace(c)) ?? false)
                        return "\"" + f.Replace("\"", "\"\"") + "\"";
                    else
                        return f;
                })
            ));
            return this;
        }

        public TableWriter WriteRow(params string[] row)
        {
            return WriteRow(row.AsEnumerable());
        }

        public TableWriter WriteRows(IEnumerable<IEnumerable<string>> rows)
        {
            rows?.ForEach(r => WriteRow(r));
            return this;
        }

        public virtual void Close()
        {
            _writer.Close();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _writer.Dispose();
                }
                _isDisposed = true;
            }
        }

        private bool _isDisposed = false;

        private readonly TextWriter _writer;
    }
}
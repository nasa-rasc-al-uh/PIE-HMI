using PIE_HMI.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIE_HMI.Util
{
    class CSVWriter
    {
        private StreamWriter _writer;
        private string _filename;

        public CSVWriter(string filename)
        {
            _filename = filename;
            try
            {
                _writer = new StreamWriter(_filename, true);
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
            }
        }

        public void writeDataPoint(string x, string y)
        {
            if (_writer == null) return;

            //_writer.WriteLine()
        }

    }
}

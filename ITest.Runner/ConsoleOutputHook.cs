using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ITest.Runner
{
    public class ConsoleOutputHook : IDisposable
    {
        readonly StringWriter _w;
        readonly Action<string> _collector;
        TextWriter _current;

        public ConsoleOutputHook( Action<string> c )
        {
            _collector = c;
            _w = new StringWriter();
            _current = Console.Out;
            Console.SetOut( _w );
        }

        public void Dispose()
        {
            if( _current != null )
            {
                Console.SetOut( _current );
                _current = null;
                _collector( _w.ToString() );
            }
        }

        public override string ToString() => _w.ToString();
    }
}

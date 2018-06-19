using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ITest.Runner
{
    public abstract class TestNode
    {
        public static readonly XName xName = XNamespace.None + "Name";

        readonly XAttribute _hasError;
        XAttribute _initializationError;

        private protected TestNode( TestNode parent, XName elementName, string nodeName )
        {
            Result = new XElement( elementName, nodeName != null ? new XAttribute( xName, nodeName ) : null );
            _hasError = new XAttribute( "ErrorCount", 0 );
            Result.Add( _hasError );
            parent?.Result.Add( Result );
        }

        public abstract TestNode Parent { get; }

        private protected abstract IReadOnlyList<TestNode> TestNodeChildren { get; }

        public XElement Result { get; }

        internal int Execute( ExecutionContext ctx )
        {
            int errorCount = DoExecute( ctx );
            _hasError.SetValue( errorCount );
            return errorCount;
        }

        private protected virtual int DoExecute( ExecutionContext ctx )
        {
            int errorCount = 0;
            foreach( var c in TestNodeChildren )
            {
                errorCount += c.Execute( ctx );
            }
            _hasError.SetValue( errorCount );
            return errorCount;
        }

        public bool HasInitializationError => _initializationError != null;

        internal string Initialize()
        {
            string error = DoInitialize();
            if( error != null )
            {
                _initializationError = new XAttribute( "InitializationError", error );
                Result.Add( _initializationError );
            }
            return error;
        }

        private protected virtual string DoInitialize()
        {
            int errorCount = 0;
            foreach( var c in TestNodeChildren )
            {
                if( c.Initialize() != null ) ++errorCount;
            }
            return errorCount > 0 ? $"{errorCount} initialization error(s)." : null;
        }
    }
}

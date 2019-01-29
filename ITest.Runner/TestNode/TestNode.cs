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
        public static readonly XName xLastRunErrorCount = XNamespace.None + "LastRunErrorCount";

        readonly XAttribute _lastRunErrorCount;
        XAttribute _initializationError;

        private protected TestNode( TestNode parent, XName elementName, string nodeName )
        {
            NodeName = nodeName ?? "";
            Result = new XElement( elementName, nodeName != null ? new XAttribute( xName, NodeName ) : null );
            _lastRunErrorCount = new XAttribute( xLastRunErrorCount, 0 );
            Result.Add( _lastRunErrorCount );
            parent?.Result.Add( Result );
        }

        public abstract TestNode Parent { get; }

        private protected abstract IReadOnlyList<TestNode> TestNodeChildren { get; }

        /// <summary>
        /// Gets all the childre nodes recursively.
        /// </summary>
        public IEnumerable<TestNode> AllChildrenNodes
        {
            get
            {
                foreach( var n in TestNodeChildren )
                {
                    yield return n;
                    foreach( var c in n.AllChildrenNodes )
                    {
                        yield return c;
                    }
                }
            }
        }

        public XElement Result { get; }

        public string NodeName { get; }

        public int LastRunErrorCount => (int)_lastRunErrorCount;

        /// <summary>
        /// Execution. Typically overridden but here calls DoExecute to execute the children
        /// and sets the .
        /// When overriding, this base method must be called.
        /// </summary>
        /// <param name="ctx">The internal execution context.</param>
        /// <returns>The number of errors.</returns>
        internal int Execute( ExecutionContext ctx )
        {
            int errorCount = DoExecute( ctx );
            _lastRunErrorCount.SetValue( errorCount );
            return errorCount;
        }

        /// <summary>
        /// Recursive execution on children.
        /// </summary>
        /// <param name="ctx">The internal execution context.</param>
        /// <returns>The number of errors.</returns>
        private protected virtual int DoExecute( ExecutionContext ctx )
        {
            int errorCount = 0;
            foreach( var c in TestNodeChildren )
            {
                errorCount += c.Execute( ctx );
            }
            _lastRunErrorCount.SetValue( errorCount );
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

        public override string ToString()
        {
            return (Parent != null && Parent.ToString() != "")
                ? Parent.ToString() + "/" + NodeName
                : NodeName;
        }
    }
}

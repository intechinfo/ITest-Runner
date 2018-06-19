using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ITest.Runner
{
    public class ExecutionResult
    {
        public static readonly XName xRunCount = XNamespace.None + "RunCount";
        public static readonly XName xLastError = XNamespace.None + "LastError";

        public struct OneRun
        {
            public readonly bool Skip;
            public readonly Exception Exception;

            internal OneRun( bool skip )
            {
                Skip = skip;
                Exception = null;
            }

            internal OneRun( Exception ex )
            {
                Skip = false;
                Exception = ex;
            }
        }

        readonly List<OneRun> _runs;
        readonly XAttribute _runCount;
        readonly XElement _lastError;

        internal ExecutionResult( TestNode holder, XName subElementName )
            : this( CreateSubElement( holder, subElementName ) )
        {
        }

        static XElement CreateSubElement( TestNode holder, XName subElementName )
        {
            var e = new XElement( subElementName );
            holder.Result.Add( e );
            return e; ;
        }

        internal ExecutionResult( XElement parent )
        {
            _runs = new List<OneRun>();
            _runCount = new XAttribute( xRunCount, 0 );
            _lastError = new XElement( xLastError );
            parent.Add( _runCount, _lastError );
        }

        public int ExecutionCount => _runs.Count;

        public bool Success => LastError == null;

        public Exception LastError { get; private set; }

        public IReadOnlyList<OneRun> Runs => _runs;

        void AddRun( OneRun r )
        {
            _runs.Add( r );
            _runCount.SetValue( _runs.Count );
        }

        internal void Skip()
        {
            AddRun( new OneRun( true ) );
        }

        internal bool Run( Action a )
        {
            try
            {
                a();
            }
            catch( Exception ex )
            {
                if( ex is TargetInvocationException tI ) ex = tI.InnerException;
                LastError = ex;
                AddRun( new OneRun( ex ) );
                _lastError.RemoveAll();
                _lastError.Add( ex.ToXml() );
                return false;
            }
            AddRun( new OneRun( false ) );
            return true;
        }

        internal bool Run( bool isAsync, object o, MethodInfo m, object[] parameters )
        {
            if( !isAsync ) return Run( () => m.Invoke( o, parameters ) );
            return Run( () => ((Task)m.Invoke( o, parameters )).GetAwaiter().GetResult() );
        }


    }
}

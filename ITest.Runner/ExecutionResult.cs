using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static readonly XName xRuns = XNamespace.None + "Runs";
        public static readonly XName xRun = XNamespace.None + "Run";

        public struct OneRun
        {
            public readonly RunSkipReason _skipReason;
            public readonly Exception Exception;
            public readonly string ConsoleOutput;
            public readonly int RunNumber; 

            internal OneRun( int runNumber, RunSkipReason skip )
            {
                _skipReason = skip;
                Exception = null;
                ConsoleOutput = null;
                RunNumber = runNumber;
            }

            internal OneRun( int runNumber, string consoleOuput, Exception ex )
            {
                _skipReason = RunSkipReason.None;
                ConsoleOutput = consoleOuput;
                Exception = ex;
                RunNumber = runNumber;
            }

            public XElement ToXml()
            {
                var e = new XElement( xRun, new XAttribute( "RunNumber", RunNumber ), new XElement("Console", ConsoleOutput ) );
                if( _skipReason != RunSkipReason.None )
                {
                    e.Add( new XAttribute( "Skip", _skipReason.ToString() ) );
                }
                else if( Exception != null )
                {
                    e.Add( Exception.ToXml() );
                }
                return e;
            }
        }

        readonly List<OneRun> _runs;
        readonly XElement _xmlRuns;

        internal ExecutionResult( TestNode holder, XName subElementName )
            : this( CreateSubElement( holder, subElementName ) )
        {
        }

        static XElement CreateSubElement( TestNode holder, XName subElementName )
        {
            var e = new XElement( subElementName );
            holder.Result.Add( e );
            return e;
        }

        internal ExecutionResult( XElement parent )
        {
            _runs = new List<OneRun>();
            _xmlRuns = new XElement( xRuns );
            parent.Add( _xmlRuns );
        }

        public IReadOnlyList<OneRun> Runs => _runs;

        void AddRun( OneRun r )
        {
            _runs.Add( r );
            _xmlRuns.Add( r.ToXml() );
        }

        internal void Skip( int runNumber, RunSkipReason reason )
        {
            AddRun( new OneRun( runNumber, reason ) );
        }

        internal bool Run( int runNumber, Action a )
        {
            string consoleOutput = null;
            try
            {
                a();
            }
            catch( Exception ex )
            {
                if( ex is TargetInvocationException tI ) ex = tI.InnerException;
                AddRun( new OneRun( runNumber, consoleOutput, ex ) );
                return false;
            }
            AddRun( new OneRun( runNumber, consoleOutput, null ) );
            return true;
        }

        internal bool Run( int runNumber, bool isAsync, object o, MethodInfo m, object[] parameters )
        {
            if( !isAsync ) return Run( runNumber, () => m.Invoke( o, parameters ) );
            return Run( runNumber, () => ((Task)m.Invoke( o, parameters )).GetAwaiter().GetResult() );
        }


    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ITest.Runner
{
    class NUnitBindings
    {
        public static FixtureDescriptor GetFixtureDescriptor( Type t )
        {
            var attributes = t.GetCustomAttributes();
            if( !attributes.Select( a => a.GetType().Name ).Any( n => n == "TestFixtureAttribute" ) ) return null;
            return new FixtureDescriptor { IsExplicit = attributes.Any( o => o.GetType().Name == "ExplicitAttribute" ) };
        }

        public static MethodDescriptor GetMethodDescriptor( MethodInfo m )
        {
            if( m.IsSpecialName ) return null;
            var attributes = m.GetCustomAttributes();
            bool isStatic = m.IsStatic;
            bool hasExplicit = attributes.Any( o => o.GetType().Name == "ExplicitAttribute" );
            bool hasTest = attributes.Any( o => o.GetType().Name == "TestAttribute" );
            bool hasSetUp = attributes.Any( o => o.GetType().Name == "SetUpAttribute" );
            bool hasTearDown = attributes.Any( o => o.GetType().Name == "TearDownAttribute" );
            bool hasTestCase = attributes.Any( o => o.GetType().Name == "TestCaseAttribute" );
            // Allow Test and TesCase to be both defined. Here we privilegiate TestCase but below,
            // if there is no parameters we'll switch to Test.
            if( hasTestCase && hasTest ) hasTest = false;

            if( hasSetUp && hasTearDown ) return Error( m, "Cannot be both a SetUp and a TearDown method." );
            if( (hasSetUp || hasTearDown) && hasExplicit ) return Error( m, "A Setup or TearDown method cannot be marked Explicit." );
            if( (hasSetUp || hasTearDown) && (hasTest || hasTestCase) ) return Error( m, "A SetUp or TearDown method cannot be a Test or TestCase method." );
            if( (hasTest || hasTestCase) && isStatic ) return Error( m, "A Test or TestCase methods can not be static." );
            if( !hasTest && !hasTestCase && !hasSetUp && !hasTearDown ) return null;

            var returnType = m.ReturnType;
            if( returnType != typeof( void ) && returnType != typeof( Task ) )
            {
                return Error( m, "Invalid return type. It can only be void or Task." );
            }
            MethodKind kind = returnType == typeof( Task ) ? MethodKind.Async : MethodKind.None;

            IReadOnlyList<ParameterInfo> parameters = m.GetParameters();
            // Auto fix: allow TestCase to have no parameters.
            if( hasTestCase && parameters.Count == 0 )
            {
                hasTest = true;
                hasTestCase = false;
            }
            if( hasExplicit ) kind |= MethodKind.Explicit;

            if( hasSetUp ) return new MethodDescriptor( m ) { MethodKind = kind | MethodKind.SetUp };
            if( hasTearDown ) return new MethodDescriptor( m ) { MethodKind = kind | MethodKind.TearDown };
            if( hasTest ) return new MethodDescriptor( m ) { MethodKind = kind | MethodKind.Test };

            Debug.Assert( hasTestCase && parameters.Count > 0 );
            kind |= MethodKind.TestCase;
            var infos = new List<TestCaseDetail>();
            StringBuilder signatureBuilder = new StringBuilder();
            foreach( var a in attributes.Where( a => a.GetType().Name == "TestCaseAttribute" ) )
            {
                var pArgs = a.GetType().GetProperty( "Arguments" );
                if( pArgs == null || !pArgs.CanRead || pArgs.PropertyType != typeof( object[] ) )
                {
                    return Error( m, "Attribute TestCase has an invalid type. A TestCase must expose a property Arguments that is an array of objects." );
                }
                if( pArgs.GetIndexParameters().Length > 0 )
                {
                    return Error( m, "Attribute TestCase has an invalid type. The Arguments property must be a simple property, without index parameters." );
                }
                signatureBuilder.Clear();
                object[] parameterValues = (object[])pArgs.GetValue( a );
                bool hasBefore = false;
                foreach( var v in parameterValues )
                {
                    if( hasBefore ) signatureBuilder.Append( ", " );
                    hasBefore = true;
                    if( v == null ) signatureBuilder.Append( "null" );
                    else if( v is string s )
                    {
                        s = s.Replace( "\"", "\\\"" );
                        signatureBuilder.Append( "\"" ).Append( s ).Append( "\"" );
                    }
                    else signatureBuilder.Append( v );
                }
                string error = null;
                if( parameterValues.Length != parameters.Count )
                {
                    error = $"Invalid TestCase attribute. There must be exactly {parameters.Count} values defined.";
                }
                bool isExplicit = false;
                var pExplicit = a.GetType().GetProperty( "Explicit" );
                if( pExplicit != null && pExplicit.CanRead && pExplicit.PropertyType == typeof(bool) )
                {
                    isExplicit = (bool)pExplicit.GetValue( a );
                }
                infos.Add( new TestCaseDetail( signatureBuilder.ToString(), parameterValues, error, isExplicit ) );
            }
            return new MethodDescriptor( m )
            {
                MethodKind = kind | MethodKind.TestCase,
                Parameters = parameters,
                TestCaseDetails = infos
            };
        }

        static MethodDescriptor Error( MethodInfo m, string msg )
        {
            msg = $"Method {m.Name} of type {m.DeclaringType.Name} (in namespace {m.DeclaringType.Namespace}): {msg}";
            return new MethodDescriptor( m ) { InitializationError = msg };
        }
    }
}

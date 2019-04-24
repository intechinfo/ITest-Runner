using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    /// <summary>
    /// Describes a test case: 
    /// </summary>
    public struct TestCaseDetail
    {
        /// <summary>
        /// Internal access allowed here to avoid a copy of the array
        /// when calling the method by reflexion (from <see cref="TestCaseMethod.DoExecute"/>).
        /// </summary>
        internal readonly object[] ArrayValues;

        /// <summary>
        /// The method and its parameters names. 
        /// </summary>
        public readonly string CaseSignature;

        /// <summary>
        /// The inititialzation error if any.
        /// </summary>
        public readonly string InitializationError;

        /// <summary>
        /// Whether this test case is explicit.
        /// </summary>
        public readonly bool IsExplicit;

        /// <summary>
        /// Gets the values of the parameter.
        /// </summary>
        public IReadOnlyList<object> Values => ArrayValues;

        internal TestCaseDetail( string s, object[] v, string e, bool isExplicit )
        {
            CaseSignature = s;
            ArrayValues = v;
            InitializationError = e;
            IsExplicit = isExplicit;
        }
    }

}

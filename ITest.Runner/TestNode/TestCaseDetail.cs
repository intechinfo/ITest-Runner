using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    public struct TestCaseDetail
    {
        internal readonly object[] ArrayValues;

        public readonly string CaseSignature;

        public readonly string InitializationError;

        public readonly bool IsExplicit;

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

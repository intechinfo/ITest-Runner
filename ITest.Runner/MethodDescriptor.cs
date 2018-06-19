using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ITest.Runner
{ 
    class MethodDescriptor : IMethodDescriptor
    {
        public MethodDescriptor( MethodInfo m )
        {
            Method = m;
            Parameters = Array.Empty<ParameterInfo>();
            TestCaseDetails = Array.Empty<TestCaseDetail>();
        }

        public MethodInfo Method { get; }

        public IReadOnlyList<ParameterInfo> Parameters { get; set; }

        public string InitializationError { get; set; }

        public MethodKind MethodKind { get; set; }

        public IReadOnlyList<TestCaseDetail> TestCaseDetails { get; set; }
    }
}

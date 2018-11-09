using System.Collections.Generic;
using System.Reflection;

namespace ITest.Runner
{
    public interface IMethodDescriptor
    {
        /// <summary>
        /// Gets the method info.
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// Gets the method kind.
        /// </summary>
        MethodKind MethodKind { get; }

        /// <summary>
        /// Gets the parameters. Never null.
        /// </summary>
        IReadOnlyList<ParameterInfo> Parameters { get; }

        /// <summary>
        /// Gets the initialization error.
        /// Null when there is no error.
        /// </summary>
        string InitializationError { get; }

        /// <summary>
        /// Gets the details. Never null.
        /// </summary>
        IReadOnlyList<TestCaseDetail> TestCaseDetails { get; }
    }
}

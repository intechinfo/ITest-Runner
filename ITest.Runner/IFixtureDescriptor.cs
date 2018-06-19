using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    public interface IFixtureDescriptor
    {
        bool IsExplicit { get; }
    }
}

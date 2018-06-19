#if !NET461
using NUnitLite;
using System.Reflection;

namespace ITests.Runner.Tests
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return new AutoRun(Assembly.GetEntryAssembly()).Execute(args);
        }
    }
}
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace ITest.Runner
{
    public class ProjectManager
    {
        public string DllPath { get; }
        public string XMLResultPath { get; }
        Assembly _assembly;


        public ProjectManager(string dllPath, string xmlResultPath)
        {
            DllPath = dllPath;
            XMLResultPath = xmlResultPath.EndsWith(".xml") ? xmlResultPath : (xmlResultPath + ".xml");
            _assembly = Assembly.LoadFrom( dllPath );
        }


        public void Discover()
        {
            var testRoot = new TestRoot( new[] { _assembly } );
            testRoot.Execute( new DiscoverExecuteStrategy() );

            testRoot.ResultDocument.Save( XMLResultPath );
        }

        public void Launch(string target)
        {
            var testRoot = new TestRoot( new[] { _assembly } );
            testRoot.Execute(
                new ExplicitExecuteStrategy( testRoot.AllChildrenNodes.Where( n => n.ToString() == target ) )
            );

            testRoot.ResultDocument.Save( XMLResultPath );
        }
    }
}

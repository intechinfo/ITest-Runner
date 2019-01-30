using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ITest.Runner
{
    public class ProjectManager
    {
        public string DllPath { get; }
        public string XmlFilename { get; }
        Assembly _assembly;


        public ProjectManager(string dllPath, string xmlFilename)
        {
            DllPath = dllPath;
            XmlFilename = xmlFilename.EndsWith(".xml") ? xmlFilename : xmlFilename + ".xml";
            _assembly = Assembly.LoadFrom( dllPath );
        }

        public void LoadDll()
        {
            var testRoot = new TestRoot( new[] { _assembly } );
            testRoot.Execute( new DiscoverExecuteStrategy() );

            testRoot.ResultDocument.Save( XmlFilename );
        }
    }
}

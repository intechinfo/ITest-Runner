using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ITest.Runner
{
    static class ExceptionExtension
    {
        public static XElement ToXml( this Exception @this )
        {
            return new XElement( "Exception",
                        new XAttribute( "Message", @this.Message ),
                        new XAttribute( "Type", @this.GetType().AssemblyQualifiedName ),
                        new XElement( "Stack", @this.StackTrace ),
                        @this.InnerException != null
                            ? new XElement( "Inner", ToXml( @this.InnerException ) )
                            : null );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace ITest.Runner
{
    public enum Identifier { Class, Method };

    public class SelectionParser
    {
        private readonly string _filter;
        private Dictionary<string, Identifier> _identifiers;

        public Func<string, bool> MatchClass;
        public Func<string, bool> MatchMethod;

        public SelectionParser(string filter = "")
        {
            _filter = filter;
            if( _filter != "" )
            {
                _identifiers.Add( "class", Identifier.Class );
                _identifiers.Add( "method", Identifier.Method );

                Init();
            }
        }

        public bool IsTarget(TestNode node)
        {
            string nodeName = GetNodeName(node);
            if (MatchClass != null)
            {
                if (MatchMethod != null)
                {
                    if( MatchMethod( nodeName )
                        && node.Parent != null
                        && MatchClass( GetNodeName(node.Parent) ) )
                        return true;
                }
                else
                {
                    return MatchClass( nodeName );
                }
            }
            else if (MatchMethod != null)
            {
                return MatchMethod( nodeName );
            }

            return false;
        }

        private string GetNodeName( TestNode node ) => node.Result.Name.LocalName;

        private void Init()
        {
            var payload = _filter.Split( ' ' ).Select(f => f.Trim()).ToArray();
            if( payload.Length < 3 ) return;

            string tId = payload[0];
            string op = payload[1];
            string target = payload[2];

            Identifier identifier = GetIdentifier( tId );
            switch( identifier )
            {
                case Identifier.Class:
                    MatchClass = name => ApplyOperatorTo(op, name, target);
                    break;
                case Identifier.Method:
                    MatchMethod = name => ApplyOperatorTo( op, name, target );
                    break;
                default:
                    break;
            }
        }
        
        private bool ApplyOperatorTo(string op, string value, string target)
        {
            switch (op)
            {
                case "==":
                    return value.Equals( target );
                case "=~":
                    return new Regex( target ).IsMatch( value );
            }
            return false;
        }

        private Identifier GetIdentifier(string key)
        {
            _identifiers.TryGetValue( key, out Identifier id );
            return id;
        }
    }
}

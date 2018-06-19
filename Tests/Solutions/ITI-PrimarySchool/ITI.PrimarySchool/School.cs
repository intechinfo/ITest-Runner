using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ITI.PrimarySchool
{
    [Serializable]
    public class School
    {
        string _name;
        readonly Dictionary<string,Classroom> _classes;
        readonly Dictionary<string,Teacher> _teachers;

        public School( string name )
        {
            if( String.IsNullOrEmpty( name ) ) throw new ArgumentException();
            _classes = new Dictionary<string, Classroom>();
            _teachers = new Dictionary<string, Teacher>();
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public Teacher AddTeacher( string name )
        {
            if( String.IsNullOrEmpty( name ) ) throw new ArgumentException();
            if( _teachers.ContainsKey( name ) ) throw new ArgumentException();
            var t = new Teacher( this, name );
            _teachers.Add( name, t );
            return t;
        }
        
        public Teacher FindTeacher( string name )
        {
            Teacher t;
            _teachers.TryGetValue( name, out t );
            return t;
        }

        public Classroom AddClassRoom( string name )
        {
            if( String.IsNullOrEmpty( name ) ) throw new ArgumentException();
            if( _classes.ContainsKey( name ) ) throw new ArgumentException();
            var c = new Classroom( this, name );
            _classes.Add( name, c );
            return c;
        }

        public Classroom FindClassRoom( string name )
        {
            Classroom c;
            _classes.TryGetValue( name, out c );
            return c;
        }

        internal void ChangeClassroomName( Classroom classroom, string newName )
        {
            _classes.Remove( classroom.Name );
            _classes.Add( newName, classroom );
        }
    }
}

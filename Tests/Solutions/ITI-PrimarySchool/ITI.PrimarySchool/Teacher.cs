using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.PrimarySchool
{
    [Serializable]
    public class Teacher
    {
        readonly School _school;
        readonly string _name;
        Classroom _classroom;

        internal Teacher( School s, string name )
        {
            _school = s;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public School School { get { return _school; } }
        
        public Classroom Assignment
        {
            get { return _classroom; }
        }
        
        public void AssignTo( Classroom c )
        {
            if( _classroom != c )
            {
                if( c != null && c.School != _school ) throw new ArgumentException();
                if( _classroom != null ) _classroom.Teacher = null;
                _classroom = c;
                if( _classroom != null ) _classroom.Teacher = this;
            }
        }
    }
}

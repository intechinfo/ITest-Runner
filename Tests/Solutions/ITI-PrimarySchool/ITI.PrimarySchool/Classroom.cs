using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.PrimarySchool
{
    [Serializable]
    public class Classroom
    {
        readonly School _school;
        readonly List<Pupil> _pupils;
        string _name;
        Teacher _currentTeacher;

        internal Classroom( School s, string name )
        {
            _school = s;
            _name = name;
            _pupils = new List<Pupil>();
        }

        public School School { get { return _school; } }

        public string Name
        {
            get { return _name; }
            set 
            { 
                if( _name != value )
                {
                    if( String.IsNullOrEmpty( value ) ) throw new ArgumentException();
                    if( _school.FindClassRoom( value ) != null ) throw new ArgumentException();
                    _school.ChangeClassroomName( this, value );
                    _name = value;
                }
            }
        }

        public Teacher Teacher
        {
            get { return _currentTeacher; }
            internal set { _currentTeacher = value; }
        }

        public Pupil AddPupil( string firstName, string lastName )
        {
            if( firstName == null
                || lastName == null
                || firstName.Length < 2
                || lastName.Length < 2 )
            {
                throw new ArgumentException();
            }
            if( _pupils.FirstOrDefault( p => p.FirstName == firstName && p.LastName == lastName ) != null ) throw new ArgumentException();
            var newOne = new Pupil( this, firstName, lastName );
            _pupils.Add( newOne );
            return newOne;
        }

        public Pupil FindPupil( string firstName, string lastName )
        {
            return _pupils.FirstOrDefault( p => p.FirstName == firstName && p.LastName == lastName );
        }
    }
}

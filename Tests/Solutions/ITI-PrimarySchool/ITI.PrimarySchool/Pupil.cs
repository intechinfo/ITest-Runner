using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.PrimarySchool
{
    [Serializable]
    public class Pupil
    {
        readonly Classroom _classroom;
        readonly string _firstName;
        readonly string _lastName;

        internal Pupil( Classroom c, string firstName, string lastName )
        {
            _classroom = c;
            _firstName = firstName;
            _lastName = lastName;
        }

        public string FirstName
        {
            get { return _firstName; }
        }

        public string LastName
        {
            get { return _lastName; }
        }

        public Classroom Classroom { get { return _classroom; } }


    }
}

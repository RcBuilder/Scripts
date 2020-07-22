using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTipalti
{    
    public class Person
    {
        public Name FullName { get; set; }
        public Address Address { get; set; }
    }

    public class Name
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{this.FirstName}_{this.LastName}";
        }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }

        public override string ToString()
        {
            return $"{this.Street}_{this.City}";
        }
    }

    public class Utility {
        protected List<Person> people { set; get; } = new List<Person>();
        
        public Utility() : this(null) {}
        public Utility(IEnumerable<Person> people) {
            if(people != null)
                this.Init(people.ToArray());
        }

        public void Init(Person[] people){
            /*
            this.people = people.Select(p => new PersonExtended{
                person = p,
                direct = people.Where(x => new PersonComparerLevel1().Equals(p, x))
            }).ToList();         
            */

            this.people = people.ToList();
        }

        public int FindMinRelationLevel(Person personA, Person personB){  
            // one of the people doesn't exists in the provided list
            if(!this.people.Contains(personA) || !this.people.Contains(personB))
                return -1;

            // level 1 comparison
            if(new PersonComparerLevel1().Equals(personA, personB))
                return 1;
            
            // level-n comparison

            return  _GetLevel(this.people.Where(p => p != personA).ToList(), personA, personB);
        }

        public int _GetLevel(List<Person> db, Person current, Person personB, int step = 1) {
            if(current == null)
                return -1; // no match!

            var matches = db.Where(x => x != current && new PersonComparerLevel1().Equals(current, x));
            if(matches.Contains(personB)) return step;

            if(matches.Count() == 0)
                return -1;

            var level = int.MaxValue;
            foreach(var match in matches){
                var res = this._GetLevel(db.Where(p => p != current).ToList(), match, personB, step + 1);
                if(res != -1 && res < level) level = res;
            }

            if(level == int.MaxValue) level = -1;
            return level;
        }
    }

    public class PersonComparerLevel1 : IEqualityComparer<Person> {
        public bool Equals(Person x, Person y) {
            return (x.FullName.ToString() == y.FullName.ToString()) || (x.Address.ToString() == y.Address.ToString());
        }

        public int GetHashCode(Person obj) {
            return obj.FullName.ToString().GetHashCode() ^ obj.Address.ToString().GetHashCode();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<Person>{
                new Person{ FullName = new Name { FirstName = "Alan", LastName = "Turing" }, Address = new Address{ City = "Bletchley", Street = "Park"}},
                new Person{ FullName = new Name { FirstName = "Alan", LastName = "Turing" }, Address = new Address{ City = "Cambridge", Street = ""}},
                new Person{ FullName = new Name { FirstName = "John", LastName = "Clarke" }, Address = new Address{ City = "Bletchley", Street = "Park"}},
                new Person{ FullName = new Name { FirstName = "John", LastName = "Clarke" }, Address = new Address{ City = "London", Street = ""}},
                new Person{ FullName = new Name { FirstName = "Roby", LastName = "Cohen" }, Address = new Address{ City = "Kfar-Saba", Street = ""}}
            };

            var utility = new Utility(list);
            int? level = null;

            level = utility.FindMinRelationLevel(list[0], list[3]);
            Console.WriteLine(level);   // 2

            level = utility.FindMinRelationLevel(list[2], list[1]);
            Console.WriteLine(level);   // 2

            level = utility.FindMinRelationLevel(list[0], list[1]);
            Console.WriteLine(level);   // 1

            level = utility.FindMinRelationLevel(list[2], list[3]);
            Console.WriteLine(level);   // 1

            level = utility.FindMinRelationLevel(list[1], list[4]);
            Console.WriteLine(level);   // -1

            level = utility.FindMinRelationLevel(list[1], new Person());
            Console.WriteLine(level);   // -1

            level = utility.FindMinRelationLevel(new Person(), list[2]);
            Console.WriteLine(level);   // -1
    
            Console.ReadKey();
        }
    }
}

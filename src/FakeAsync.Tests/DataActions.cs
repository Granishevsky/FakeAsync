
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace FakeAsync.Tests
{
    [TestClass]
    public class DataActions
    {
        [TestMethod]
        public void Add()
        {
            var set = new FakeDbSet<Person>()
                .SetupAddAndRemove();

            var person = new Person();
            var result = set.Object.Add(person);

            Assert.AreSame(person, result);
            Assert.AreEqual(1, set.Data.Count());
            Assert.IsTrue(set.Data.Contains(person));
        }

        [TestMethod]
        public void Remove()
        {
            var person1 = new Person();
            var person2 = new Person();
            var data = new List<Person> { person1, person2 };
            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupAddAndRemove();

            var result = set.Object.Remove(person1);

            Assert.AreSame(person1, result);
            Assert.AreEqual(1, set.Data.Count());
            Assert.IsFalse(set.Data.Contains(person1));
            Assert.IsTrue(set.Data.Contains(person2));
        }

        [TestMethod]
        public void Add_remove_with_enumeration()
        {
            var person1 = new Person();
            var person2 = new Person();
            var person3 = new Person();
            var data = new List<Person> { person1, person2 };
            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupLinq()
                .SetupAddAndRemove();

            set.Object.Remove(person2);
            set.Object.Add(person3);

            var result = set.Object.ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(person3));
            Assert.IsTrue(result.Contains(person1));
        }

        [TestMethod]
        public void Find()
        {
            var person = new Person { Id = 1 };
            var data = new List<Person> { person, new Person { Id = 2 } };
            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupFind((keyValues, entity) => entity.Id == (int)keyValues.Single());

            var result = set.Object.Find(1);

            Assert.AreSame(person, result);
        }

        [TestMethod]
        public void Find_for_no_match()
        {
            var data = new List<Person> { new Person { Id = 1 }, new Person { Id = 2 } };
            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupFind((keyValues, entity) => entity.Id == (int)keyValues.Single());

            var result = set.Object.Find(99);

            Assert.IsNull(result);
        }

        public class Person
        {
            public int Id { get; set; }
            public string Url { get; set; }
        }
    }
}

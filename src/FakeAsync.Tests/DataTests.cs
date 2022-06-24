using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace FakeAsync.Tests
{
    [TestClass]
    public class DataTests
    {
        [TestMethod]
        public void Data_is_addded()
        {
            var data = new List<Person> { new Person(), new Person() };

            var set = new FakeDbSet<Person>()
                .SetupSeedData(data);

            var result = set.Data.ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreSame(data[0], result[0]);
            Assert.AreSame(data[1], result[1]);
        }
        [TestMethod]
        public void Can_enumerate_set()
        {
            var data = new List<Person> { new Person { }, new Person { } };

            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupLinq();

            var count = 0;
            foreach (var item in set.Object)
            {
                count++;
            }

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public async Task Can_enumerate_set_async()
        {
            var data = new List<Person> { new Person(), new Person() };

            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupLinq();

            var count = 0;
            await set.Object.ForEachAsync(b => count++);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void Can_use_linq_directly_on_set()
        {
            var data = new List<Person> { new Person(), new Person() };

            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupLinq();

            var result = set.Object.ToList();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task Can_use_linq_directly_on_set_async()
        {
            var data = new List<Person> { new Person(), new Person() };

            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupLinq();

            var result = await set.Object.ToListAsync();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void Can_use_linq_opeartors()
        {
            var data = new List<Person>
            {
                new Person { Id = 1 },
                new Person { Id = 2 },
                new Person { Id = 3}
            };

            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupLinq();

            var result = set.Object
                .Where(b => b.Id > 1)
                .OrderByDescending(b => b.Id)
                .ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(3, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
        }

        [TestMethod]
        public async Task Can_use_linq_opeartors_async()
        {
            var data = new List<Person>
            {
                new Person { Id = 1 },
                new Person { Id = 2 },
                new Person { Id = 3}
            };

            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupLinq();

            var result = await set.Object
                .Where(b => b.Id > 1)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(3, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
        }

        [TestMethod]
        public void Can_use_include_directly_on_set()
        {
            var data = new List<Person> { new Person(), new Person() };

            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupLinq();

            var result = set.Object
                .Include(b => b.Posts)
                .ToList();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void Can_use_include_after_linq_operator()
        {
            var data = new List<Person> { new Person(), new Person() };

            var set = new FakeDbSet<Person>()
                .SetupSeedData(data)
                .SetupLinq();

            var result = set.Object
                .OrderBy(b => b.Id)
                .Include(b => b.Posts)
                .ToList();

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void Can_add_data_after_setting_up_linq()
        {
            var data = new List<Person> { new Person(), new Person() };

            var set = new FakeDbSet<Person>()
                .SetupLinq()
                .SetupSeedData(data);

            var result = set.Object.ToList();

            Assert.AreEqual(2, result.Count);
        }

        public class Person
        {
            public int Id { get; set; }
            public string Url { get; set; }

            public List<Post> Posts { get; set; }
        }

        public class Post
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public int PersonId { get; set; }
            public Person Person { get; set; }
        }
    }
}

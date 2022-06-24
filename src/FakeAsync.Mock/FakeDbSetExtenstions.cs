using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace FakeAsync
{
    public static  class FakeDbSetExtenstions
    {

        public static FakeDbSet<TEntity> SetupSeedData<TEntity>(
               this FakeDbSet<TEntity> set,
               IEnumerable<TEntity> data)
               where TEntity : class
        {
            set.AddData(data);

            return set;
        }

        public static FakeDbSet<TEntity> SetupLinq<TEntity>(this FakeDbSet<TEntity> set)
            where TEntity : class
        {
            // Record so that we can re-setup linq if the data is changed
            set.IsLinqSetup = true;

            // Enable direct async enumeration of set
            set.As<IDbAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(() => new DbEnumeratorAsync<TEntity>(set.Queryable.GetEnumerator()));

            // Enable LINQ queries with async enumeration
            set.As<IQueryable<TEntity>>()
                .Setup(m => m.Provider)
                .Returns(() => new DbQueryProviderAsync<TEntity>(set.Queryable.Provider));

            // Wire up LINQ provider to fall back to in memory LINQ provider of the data
            set.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(() => set.Queryable.Expression);
            set.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(() => set.Queryable.ElementType);
            set.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => set.Queryable.GetEnumerator());

            // Enable Include directly on the DbSet (Include extension method on IQueryable is a no-op when it's not a DbSet/DbQuery)
            // Include(string) and Include(Func<TEntity, TProperty) both fall back to string
            set.Setup(s => s.Include(It.IsAny<string>())).Returns(set.Object);
            return set;
        }

        public static FakeDbSet<TEntity> SetupAddAndRemove<TEntity>(this FakeDbSet<TEntity> set)
            where TEntity : class
        {
            set.Setup(s => s.Add(It.IsAny<TEntity>()))
                .Returns((TEntity t) => t)
                .Callback((TEntity t) => set.AddData(t));

            set.Setup(s => s.Remove(It.IsAny<TEntity>()))
                .Returns((TEntity t) => t)
                .Callback((TEntity t) => set.RemoveData(t));

            return set;
        }

        public static FakeDbSet<TEntity> SetupFind<TEntity>(this FakeDbSet<TEntity> set, Func<object[], TEntity, bool> finder)
            where TEntity : class
        {
            set.Setup(s => s.Find(It.IsAny<object[]>()))
                .Returns((object[] keyValues) => set.Data.SingleOrDefault(e => finder(keyValues, e)));

            return set;
        }
    }
}

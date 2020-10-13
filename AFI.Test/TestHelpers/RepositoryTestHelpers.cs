using AFI.Persistance.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AFI.Test.TestHelpers
{
    // See https://docs.microsoft.com/sv-se/ef/ef6/fundamentals/testing/mocking
    // https://www.endpoint.com/blog/2019/07/16/mocking-asynchronous-database-calls-net-core
    // https://stackoverflow.com/questions/57314896/iasyncqueryprovider-mock-issue-when-migrated-to-net-core-3-adding-tresult-iasyn

    internal static class DbMockSetup
    {
        public static Mock<DbSet<T>> SetupMockDbSet<T>(IList<T> dataToBeReturnedOnGet) where T : class
        {
            var mocks = dataToBeReturnedOnGet.AsQueryable();

            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(mocks.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(mocks.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(mocks.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(mocks.GetEnumerator());

            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(mocks.GetEnumerator()));

            mockSet.Setup(x => x.Add(It.IsAny<T>()))
                .Callback<T>((T x) => dataToBeReturnedOnGet.Add(x))
                .Returns<T>(x => (EntityEntry<T>)dataToBeReturnedOnGet);

            mockSet.Setup(x => x.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .Returns<T, CancellationToken>((T x, CancellationToken c) => {
                    dataToBeReturnedOnGet.Add(x);
                    return new ValueTask<EntityEntry<T>>(Task.FromResult((EntityEntry<T>)null));
                });

            mockSet.Setup(x => x.Remove(It.IsAny<T>()))
                .Returns<T>(r => {
                    dataToBeReturnedOnGet.Remove(r);
                    return null;
                });

            return mockSet;
        }

        public static Mock<AFIContext> SetupStarterContext<T, U>(
                IList<T> dataToBeReturnedOnGet, IList<U> userData)
            where T : class
            where U : class
        {
            var mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            var mockContext = new Mock<AFIContext>(mockConfiguration.Object);
            var mockSS = DbMockSetup.SetupMockDbSet<T>(dataToBeReturnedOnGet);
            var mockU = DbMockSetup.SetupMockDbSet<U>(userData);

            mockContext.Setup(c => c.Set<T>()).Returns(mockSS.Object);
            mockContext.Setup(c => c.Set<U>()).Returns(mockU.Object);

            return mockContext;
        }
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }

        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                                 .GetMethod(
                                      name: nameof(IQueryProvider.Execute),
                                      genericParameterCount: 1,
                                      types: new[] { typeof(Expression) })
                                 .MakeGenericMethod(expectedResultType)
                                 .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                                        ?.MakeGenericMethod(expectedResultType)
                                         .Invoke(null, new[] { executionResult });
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestAsyncQueryProvider<T>(this); }
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public T Current
        {
            get { return _inner.Current; }
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(Task.FromResult(_inner.MoveNext()));
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }
}

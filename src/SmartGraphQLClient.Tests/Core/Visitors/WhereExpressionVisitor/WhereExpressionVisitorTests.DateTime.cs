using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.WhereExpressionVisitor
{
    public partial class WhereExpressionVisitorTests
    {
        [TestMethod]
        public void DateTime_EqExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.CreateDate == new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) == x.CreateDate,
                (x) => x.CreateDate == date,
                (x) => date == x.CreateDate,
            };

            var expected = "createDate: { eq: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTime_NotEqExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.CreateDate != new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) != x.CreateDate,
                (x) => x.CreateDate != date,
                (x) => date != x.CreateDate,
            };

            var expected = "createDate: { neq: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTime_GtExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.CreateDate > new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) < x.CreateDate,
                (x) => x.CreateDate > date,
                (x) => date < x.CreateDate,
            };

            var expected = "createDate: { gt: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTime_GteExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.CreateDate >= new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) <= x.CreateDate,
                (x) => x.CreateDate >= date,
                (x) => date <= x.CreateDate,
            };

            var expected = "createDate: { gte: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTime_LtExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.CreateDate < new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) > x.CreateDate,
                (x) => x.CreateDate < date,
                (x) => date > x.CreateDate,
            };

            var expected = "createDate: { lt: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTime_LteExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.CreateDate <= new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) >= x.CreateDate,
                (x) => x.CreateDate <= date,
                (x) => date >= x.CreateDate,
            };

            var expected = "createDate: { lte: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTime_InExpression()
        {
            var dates = new[]
            {
                new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                new DateTime(2002, 01, 02, 03, 04, 05, 006, DateTimeKind.Utc)
            };

            Expression<Func<TestEntity, bool>> expression = (x) => dates.Contains(x.CreateDate);

            var expected = "createDate: { in: [ 2001-12-31T23:59:59Z, 2002-01-02T03:04:05Z ] }".Tokenize();
            var visitor = CreateVisitor(expression);

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void DateTime_NInExpression()
        {
            var dates = new[]
            {
                new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                new DateTime(2002, 01, 02, 03, 04, 05, 006, DateTimeKind.Utc)
            };

            Expression<Func<TestEntity, bool>> expression = (x) => !dates.Contains(x.CreateDate);

            var expected = "createDate: { nin: [ 2001-12-31T23:59:59Z, 2002-01-02T03:04:05Z ] }".Tokenize();
            var visitor = CreateVisitor(expression);

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod, Ignore("TODO: Пока игнорируем, нужно переписывать определение методов, чтобы работали с Nullable")]
        public void DateTime_InExpression_NullableSource()
        {
            var dates = new DateTime?[]
            {
                new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                new DateTime(2002, 01, 02, 03, 04, 05, 006, DateTimeKind.Utc)
            };

            Expression<Func<TestEntity, bool>> expression = (x) => dates.Contains(x.CreateDate);

            var expected = "createDate: { in: [ 2001-12-31T23:59:59Z, 2002-01-02T03:04:05Z ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void DateTimeNullable_EqExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.UpdateDate == new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) == x.UpdateDate,
                (x) => x.UpdateDate == date,
                (x) => date == x.UpdateDate,
            };

            var expected = "updateDate: { eq: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTimeNullable_EqNullExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.UpdateDate == null;

            var expected = "updateDate: { eq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void DateTimeNullable_NotHasValueExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.UpdateDate.HasValue;

            var expected = "updateDate: { eq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void DateTimeNullable_NotEqExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.UpdateDate != new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) != x.UpdateDate,
                (x) => x.UpdateDate != date,
                (x) => date != x.UpdateDate,
            };

            var expected = "updateDate: { neq: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTimeNullable_NotEqNullExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.UpdateDate != null;

            var expected = "updateDate: { neq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void DateTimeNullable_HasValueExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.UpdateDate.HasValue;

            var expected = "updateDate: { neq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void DateTimeNullable_GtExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.UpdateDate > new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) < x.UpdateDate,
                (x) => x.UpdateDate > date,
                (x) => date < x.UpdateDate,
            };

            var expected = "updateDate: { gt: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTimeNullable_GteExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.UpdateDate >= new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) <= x.UpdateDate,
                (x) => x.UpdateDate >= date,
                (x) => date <= x.UpdateDate,
            };

            var expected = "updateDate: { gte: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTimeNullable_LtExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.UpdateDate < new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) > x.UpdateDate,
                (x) => x.UpdateDate < date,
                (x) => date > x.UpdateDate,
            };

            var expected = "updateDate: { lt: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTimeNullable_LteExpression()
        {
            var date = new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.UpdateDate <= new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                (x) => new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc) >= x.UpdateDate,
                (x) => x.UpdateDate <= date,
                (x) => date >= x.UpdateDate,
            };

            var expected = "updateDate: { lte: 2001-12-31T23:59:59Z }".Tokenize();

            foreach (var expression in expressions)
            {
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void DateTimeNullable_InExpression()
        {
            var dates = new DateTime?[]
            {
                new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                new DateTime(2002, 01, 02, 03, 04, 05, 006, DateTimeKind.Utc),
                null
            };

            Expression<Func<TestEntity, bool>> expression = (x) => dates.Contains(x.UpdateDate);

            var expected = "updateDate: { in: [ 2001-12-31T23:59:59Z, 2002-01-02T03:04:05Z, null ] }".Tokenize();
            var visitor = CreateVisitor(expression);

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void DateTimeNullable_NInExpression()
        {
            var dates = new DateTime?[]
            {
                new DateTime(2001, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc),
                new DateTime(2002, 01, 02, 03, 04, 05, 006, DateTimeKind.Utc),
                null
            };

            Expression<Func<TestEntity, bool>> expression = (x) => !dates.Contains(x.UpdateDate);

            var expected = "updateDate: { nin: [ 2001-12-31T23:59:59Z, 2002-01-02T03:04:05Z, null ] }".Tokenize();
            var visitor = CreateVisitor(expression);

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}

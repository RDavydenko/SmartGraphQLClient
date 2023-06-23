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
        public void Int_Eq_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.Id == 1,
                (x) => 1 == x.Id,
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { eq: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Neq_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.Id != 1,
                (x) => 1 != x.Id,
                (x) => !(x.Id == 1),
                (x) => !(1 == x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { neq: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_In_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => new[] { 1, 2, 3 }.Contains(x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { in: [ 1, 2, 3 ] }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Nin_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => !new[] { 1, 2, 3 }.Contains(x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { nin: [ 1, 2, 3 ] }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Gt_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.Id > 1,
                (x) => 1 < x.Id,
                (x) => !(x.Id <= 1),
                (x) => !(1 >= x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { gt: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Ngt_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => !(x.Id > 1),
                (x) => !(1 < x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { lte: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Gte_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.Id >= 1,
                (x) => 1 <= x.Id,
                (x) => !(x.Id < 1),
                (x) => !(1 > x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { gte: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Ngte_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => !(x.Id >= 1),
                (x) => !(1 <= x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { lt: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Lt_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.Id < 1,
                (x) => 1 > x.Id,
                (x) => !(x.Id >= 1),
                (x) => !(1 <= x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { lt: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Nlt_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => !(x.Id < 1),
                (x) => !(1 > x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { gte: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Lte_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.Id <= 1,
                (x) => 1 >= x.Id,
                (x) => !(x.Id > 1),
                (x) => !(1 < x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { lte: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void Int_Nlte_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => !(x.Id <= 1),
                (x) => !(1 >= x.Id),
            };

            foreach (var expression in expressions)
            {
                var expected = "id: { gt: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void IntNullable_Eq_Expression()
        {
            var expressions = new Expression<Func<TestEntity, bool>>[]
            {
                (x) => x.MaybeId == 1,
            };

            foreach (var expression in expressions)
            {
                var expected = "maybeId: { eq: 1 }".Tokenize();

                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }
    }
}

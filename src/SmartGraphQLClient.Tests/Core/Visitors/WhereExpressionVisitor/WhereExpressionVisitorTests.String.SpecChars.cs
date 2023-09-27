using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.WhereExpressionVisitor
{
    public partial class WhereExpressionVisitorTests
    {
        [TestMethod]
        public void String_EscapingSpecialCharacters_DoubleQuote()
        {
            Expression<Func<TestEntity, bool>> expression = 
                (x) => x.Name == "\"abc\"";

            var expected = "name: { eq: \"\\\"abc\\\"\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
        
        [TestMethod]
        public void String_EscapingSpecialCharacters_Backslash()
        {
            Expression<Func<TestEntity, bool>> expression = 
                (x) => x.Name == "\\abc";

            var expected = "name: { eq: \"\\\\abc\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
        
        [TestMethod]
        public void String_EscapingSpecialCharacters_NewLine()
        {
            Expression<Func<TestEntity, bool>> expression = 
                (x) => x.Name == "\nabc";

            var expected = "name: { eq: \"\\nabc\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
        
        [TestMethod]
        public void String_EscapingSpecialCharacters_CarriageReturn()
        {
            Expression<Func<TestEntity, bool>> expression = 
                (x) => x.Name == "\rabc";

            var expected = "name: { eq: \"\\rabc\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
        
        [TestMethod]
        public void String_EscapingSpecialCharacters_Tab()
        {
            Expression<Func<TestEntity, bool>> expression = 
                (x) => x.Name == "\tabc";

            var expected = "name: { eq: \"\\tabc\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
        
        [TestMethod]
        public void String_EscapingSpecialCharacters_NullCharacter()
        {
            Expression<Func<TestEntity, bool>> expression = 
                (x) => x.Name == "\0abc";

            var expected = "name: { eq: \"\\0abc\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
        
        [TestMethod]
        public void String_EscapingSpecialCharacters_Backspace()
        {
            Expression<Func<TestEntity, bool>> expression = 
                (x) => x.Name == "\babc";

            var expected = "name: { eq: \"\\babc\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}

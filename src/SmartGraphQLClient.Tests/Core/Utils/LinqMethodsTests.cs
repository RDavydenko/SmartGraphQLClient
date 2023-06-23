using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;

namespace SmartGraphQLClient.Tests.Core.Utils
{
    [TestClass]
    public class LinqMethodsTests
    {
        [TestMethod]
        public void LinqMethods_GetSelectMethod_ShouldReturnNotNullMethodInfo()
        {
            var methodInfo = LinqMethods.GetSelectMethod(typeof(TestEntity), typeof(TestEntity));
            Assert.IsNotNull(methodInfo);
        }

        [TestMethod]
        public void LinqMethods_GetWhereMethod_ShouldReturnNotNullMethodInfo()
        {
            var methodInfo = LinqMethods.GetWhereMethod(typeof(TestEntity));
            Assert.IsNotNull(methodInfo);
        }

        [TestMethod]
        public void LinqMethods_GetFirstMethod_ShouldReturnNotNullMethodInfo()
        {
            var methodInfo = LinqMethods.GetFirstMethod(typeof(TestEntity));
            Assert.IsNotNull(methodInfo);
        }

        [TestMethod]
        public void LinqMethods_GetToArrayMethod_ShouldReturnNotNullMethodInfo()
        {
            var methodInfo = LinqMethods.GetToArrayMethod(typeof(TestEntity));
            Assert.IsNotNull(methodInfo);
        }

        [TestMethod]
        public void LinqMethods_GetTakeMethod_ShouldReturnNotNullMethodInfo()
        {
            var methodInfo = LinqMethods.GetTakeMethod(typeof(TestEntity));
            Assert.IsNotNull(methodInfo);
        }

        [TestMethod]
        public void LinqMethods_GetSkipMethod_ShouldReturnNotNullMethodInfo()
        {
            var methodInfo = LinqMethods.GetSkipMethod(typeof(TestEntity));
            Assert.IsNotNull(methodInfo);
        }

        [TestMethod]
        public void LinqMethods_GetOrderByMethod_ShouldReturnNotNullMethodInfo()
        {
            var methodInfo = LinqMethods.GetOrderByMethod(typeof(TestEntity), typeof(Child));
            Assert.IsNotNull(methodInfo);
        }

        [TestMethod]
        public void LinqMethods_GetOrderByDescendingMethod_ShouldReturnNotNullMethodInfo()
        {
            var methodInfo = LinqMethods.GetOrderByDescendingMethod(typeof(TestEntity), typeof(Child));
            Assert.IsNotNull(methodInfo);
        }
    }
}

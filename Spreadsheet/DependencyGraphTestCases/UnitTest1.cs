using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;

namespace DependencyGraphTestCases
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DependencyGraph test = new DependencyGraph();
            for(int i = 1; i <= 100;i++)
            {
                for (int j = 1; j <= 1000; j++)
                {
                    test.AddDependency("dpa"+j, "dpe"+i);
                    test.AddDependency("dpe"+((j*11 % 1000)+1), "dpa"+i);
                }
            }
            
            for (int j = 1; j <= 1000; j++)
            {
                test.GetDependents("dpa" + j);
                test.GetDependees("dpa" + j);
            }
            
            test.AddDependency("dpa0", "dpe0");
            Assert.IsTrue(test.HasDependees("dpa0"));
            Assert.IsTrue(test.HasDependents("dpe0"));
            test.RemoveDependency("dpa0", "dpe0");
            Assert.IsFalse(test.HasDependees("dpa0"));
            Assert.IsFalse(test.HasDependents("dpe0"));
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

namespace DependencyGraphTestCases
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TimeTest()
        {
            DependencyGraph test = new DependencyGraph();
            for (int i = 1; i <= 100; i++)
            {
                for (int j = 1; j <= 1000; j++)
                {
                    test.AddDependency("dpa" + j, "dpe" + i);
                    //test.AddDependency("dpe"+((j*11 % 1000)+1), "dpa"+i);
                }
            }

            for (int j = 1; j <= 1000; j++)
            {
                test.GetDependents("dpa" + j);
                test.GetDependees("dpa" + j);
            }
        }

        [TestMethod]
        public void AddAndRemoveDependecyTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("dpa0", "dpe0");
            Assert.IsTrue(test.HasDependees("dpe0"));
            Assert.IsTrue(test.HasDependents("dpa0"));
            test.RemoveDependency("dpa0", "dpe0");
            Assert.IsFalse(test.HasDependees("dpe0"));
            Assert.IsFalse(test.HasDependents("dpa0"));
        }

        [TestMethod]
        public void SizeTestWithSameElements()
        {
            DependencyGraph test = new DependencyGraph();

            for (int i = 0; i < 100; i++)
            {
                test.AddDependency("dpa", "dpe");
            }
            Assert.AreEqual(1, test.Size);
        }

        [TestMethod]
        public void SizeTestWithUniqueElements()
        {
            DependencyGraph test = new DependencyGraph();

            for (int i = 0; i < 100; i++)
            {
                test.AddDependency("dpa" + i, "dpe" + i);
            }

            DependencyGraph t = new DependencyGraph(test);
            Assert.AreEqual(test.Size, t.Size);
            Assert.AreEqual(100, test.Size);
        }

        [TestMethod]
        public void HasDpendentsTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps", "maths");
            Assert.IsTrue(test.HasDependents("comps"));
        }

        [TestMethod]
        public void HasDpendeesTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps", "maths");
            Assert.IsTrue(test.HasDependees("maths"));
        }

        [TestMethod]
        public void DpendentsTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps", "maths");

            foreach (string s in test.GetDependents("comps"))
            {
                Assert.AreEqual("maths", s);
            }
        }

        [TestMethod]
        public void DpendeesTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps", "maths");

            foreach (string s in test.GetDependees("maths"))
            {
                Assert.AreEqual("comps", s);
            }
        }

        [TestMethod]
        public void ReplaceDependentsTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps", "maths");
            List<string> replacment = new List<string>();
            replacment.Add("logic");
            replacment.Add("stuff");

            test.ReplaceDependents("comps", replacment);
            int i = 0;
            foreach (string s in test.GetDependents("comps"))
            {
                Assert.IsTrue(replacment[i].Equals(s));
                i++;
            }
        }

        [TestMethod]
        public void ReplaceDependeesTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps", "maths");
            List<string> replacment = new List<string>();
            replacment.Add("logic");
            replacment.Add("stuff");

            test.ReplaceDependees("maths", replacment);
            int i = 0;
            foreach (string s in test.GetDependees("maths"))
            {
                Assert.IsTrue(replacment[i].Equals(s));
                i++;
            }
        }

        [TestMethod][ExpectedException(typeof(ArgumentNullException))]
        public void NullTest1()
        {
            DependencyGraph test = new DependencyGraph(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTest2()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency(null, null);
        }
    }
}


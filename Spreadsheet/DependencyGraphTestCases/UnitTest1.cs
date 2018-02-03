using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

namespace DependencyGraphTestCases
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// adds 100000 elemnts
        /// </summary>
        [TestMethod]
        public void TimeTest()
        {
            DependencyGraph test = new DependencyGraph();
            for(int i = 1; i <= 100;i++)
            {
                for (int j = 1; j <= 1000; j++)
                {
                    test.AddDependency("dpa"+j, "dpe"+i);
                }
            }
        }

        /// <summary>
        /// adds a bunch of items then removes them
        /// </summary>
        [TestMethod]
        public void AddRemoveTest()
        {
            DependencyGraph test = new DependencyGraph();
            for (int i = 1; i <= 100; i++)
            {
                for (int j = 1; j <= 100; j++)
                {
                    test.AddDependency("dpa" + j, "dpe" + i);
                }
            }

            for (int i = 1; i <= 100; i++)
            {
                for (int j = 1; j <= 100; j++)
                {
                    test.RemoveDependency("dpa" + j, "dpe" + i);
                }
            }
        }

        /// <summary>
        /// adds and removes things
        /// </summary>
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

        /// <summary>
        /// adds the same element over and over and makes sure it was only added once
        /// </summary>
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

        /// <summary>
        /// adds 100 elemetns and makes sure that the size is 100
        /// </summary>
        [TestMethod]
        public void SizeTestWithUniqueElements()
        {
            DependencyGraph test = new DependencyGraph();

            for (int i = 0; i < 100; i++)
            {
                test.AddDependency("dpa"+i, "dpe"+i);
            }
            Assert.AreEqual(100, test.Size);
        }

        /// <summary>
        /// test the has dependents funtion
        /// </summary>
        [TestMethod]
        public void HasDpendentsTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps","maths");
            Assert.IsTrue(test.HasDependents("comps"));
        }

        /// <summary>
        /// tests the has dependees function
        /// </summary>
        [TestMethod]
        public void HasDpendeesTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps", "maths");
            Assert.IsTrue(test.HasDependees("maths"));
        }

        /// <summary>
        /// tests the get depedents function
        /// </summary>
        [TestMethod]
        public void DpendentsTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps", "maths");
           
            foreach(string s in test.GetDependents("comps"))
            {
                Assert.AreEqual("maths",s);
            }
        }

        /// <summary>
        /// tests the get dependees function
        /// </summary>
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

        /// <summary>
        /// tests the Replace dependents function 
        /// </summary>
        [TestMethod]
        public void ReplaceDependentsTest()
        {
            DependencyGraph test = new DependencyGraph();
            test.AddDependency("comps", "maths");
            List<string> replacment = new List<string>();
            replacment.Add("logic");
            replacment.Add("stuff");

            test.ReplaceDependents("comps",replacment);
            int i = 0;
            foreach(string s in test.GetDependents("comps"))
            {
                Assert.IsTrue(replacment[i].Equals(s));
                i++;
            }    
        }

        /// <summary>
        ///tests the replace dependees function
        /// </summary>
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
    }
}


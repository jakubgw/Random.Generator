using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Moq;
using System;

namespace Random.Generator.Test
{
    [TestClass]
    public class UnitTest1
    {

        // Test which should check inputs are missing only one as example...

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
                 "Empty input collection !")]
        public void Should_Throw_Empty_Imput_Collection()
        {
            var generator = new AliasMethodGenerator(null, CreateRealRandom());
        }


        [TestMethod]
        public void Should_Return_Correct_Value_If_Only_Item()
        {
            var generator = new AliasMethodGenerator(new double[] { 1 }, CreateRealRandom());
            Assert.AreEqual(0, generator.GetValue());
        }

        [TestMethod]
        public void Should_Generate_1_Value()
        {
            var random = CreateRandomMock(new List<Tuple<int, int>>() { Tuple.Create(2, 1),  Tuple.Create(3, 1) });
            var generator = new AliasMethodGenerator(new double[] { 1, 2 }, random);
            Assert.AreEqual(1, generator.GetValue());
        }

        [TestMethod]
        public void Should_Generate_0_Value()
        {
            var random = CreateRandomMock(new List<Tuple<int, int>>() { Tuple.Create(2, 0), Tuple.Create(3, 0) });
            var generator = new AliasMethodGenerator(new double[] { 1, 2 }, random);
            Assert.AreEqual(0, generator.GetValue());
        }


        [TestMethod, Timeout(2000)]
        public void Should_Return_Corrent_Values_For_10K_Tries_And_1_And_2_Distribution()
        {
            Test_10K_Tries(new double[] { 1, 2 }, new double[] { 0.33, 0.67 });
        }



        [TestMethod, Timeout(2000)]
        public void Should_Return_Corrent_Values_For_10K_Tries_And_Equal_Distribution()
        {
            Test_10K_Tries(new double[] { 1,1,1,1 }, new double[] { 0.25, 0.25, 0.25, 0.25 });
        }


        [TestMethod, Timeout(2000)]
        public void Should_Return_Corrent_Values_For_10K_Tries_And_Contains_0_Distribution()
        {
            Test_10K_Tries(new double[] { 2, 0, 2 }, new double[] { 0.5, 0, 0.5 });
        }

        private void Test_10K_Tries(double[] probabilties, double[] resultsToCheck)
        {
            var generator = new AliasMethodGenerator(probabilties, CreateRealRandom());
            const int attempts = 200000;
            var results = new List<int>();
            for (var i = 0; i < attempts; i++)
            {
                results.Add(generator.GetValue());
            }
            var grouped = results.GroupBy(i => i).ToDictionary(key => key.Key, value=> (double)value.Count());

            for (var i=0; i < resultsToCheck.Length; i++)
            {
                if (grouped.ContainsKey(i))
                {
                    var roundedProbability = System.Math.Round(grouped[i] / attempts, 2);
                    Assert.AreEqual(roundedProbability, resultsToCheck[i]);
                }
                else
                {
                    Assert.AreEqual(0, resultsToCheck[i]);
                }
            }

        }

        private IRandom CreateRandomMock(IEnumerable<Tuple<int,int>> probabilitesAndValue)
        {
            var randomMock = new Mock<IRandom>();
            foreach(var toSetup in probabilitesAndValue)
            {
                randomMock.Setup(item => item.Generate(toSetup.Item1)).Returns(toSetup.Item2);
            }
            return randomMock.Object;
        }

        private IRandom CreateRealRandom()
        {
            var random = new System.Random();
            var randomMock = new Mock<IRandom>();
            randomMock.Setup(item => item.Generate(It.IsAny<int>())).Returns((int number) => random.Next(number));
            return randomMock.Object;

        }

    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BetLive.Hubs;

namespace BetLive.Tests
{
    [TestClass]
    public class XmlLoaderTests
    {
        MyController _myController = new MyController();
        [TestMethod]
        public void CanLoadScoresFromXmlFile()
        {
           
            var scores = _myController.GetGamesScoresFromXml();
           
            Assert.IsNotNull(scores);
           // Assert.IsTrue(scores.Result != null);
        }

        [TestMethod]
        public void CanLoadOddsFromXmlFile()
        {
            MyController _myController = new MyController();
           
            var odds = _myController.GetGamesOddsFromXml();
           
            Assert.IsNotNull(odds);
        }
    }
}

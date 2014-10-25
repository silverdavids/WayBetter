using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BetLive.Hubs;

namespace BetLive.TestsBetLiveTestProject1
{
    [TestClass]
    public class XmlLoaderTests
    {
        MyController _myController = new MyController();
        [TestMethod]
        public void CanLoadScoresFromXmlFile()
        {
           
            var scores = _myController.GetGamesScoresFromXml(1);
           
            Assert.IsNotNull(scores);
           // Assert.IsTrue(scores.Result != null);
        }

        [TestMethod]
        public void CanLoadOddsFromXmlFile()
        {
            MyController _myController = new MyController();
           
            var odds = _myController.GetGamesOddsFromXml(1);
           
            Assert.IsNotNull(odds);
        }
    }
}

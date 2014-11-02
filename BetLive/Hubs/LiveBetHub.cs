#region Assemblies
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Concurrent;

using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Text;
using System.IO;
using System.Threading;
using WebUI.Helpers;
using BetLive.Controllers.Api;
using System.Web.Http;
using Domain.Models.ViewModels;
using Domain.Models.Concrete;
using WebUI.DataAccessLayer;
using System.Data.Entity.Migrations;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
#endregion
namespace BetLive.Hubs
{
    [HubName("liveBetHubAng")]
    public class LiveGameHub : Hub
    {
        private LiveBetsBase _liveBetsBase;
        public LiveGameHub() : this(LiveBetsBase.Instance) { }
      
        #region LiveGameHubConstructor
        public LiveGameHub(LiveBetsBase liveBetsBase)
        {
            _liveBetsBase = liveBetsBase;
        }

        #endregion

        public Task<IEnumerable<Game>> GetAllGames() {

            return   _liveBetsBase.GetAllGames();
        }

        public  Task<List<GameViewModel>> getAllNormalGames()
        {

            return _liveBetsBase.getAllNormalGames();
        }
        public string TestString()
        {
            return "Connected to the hub.....";
        }

       
    }

  



}
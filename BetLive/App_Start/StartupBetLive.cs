using BetLive.App_Start;
using BetLive.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
[assembly: OwinStartup(typeof(StartupBetLive))]
namespace BetLive.App_Start
{
    public class StartupBetLive
    {
       
        public void Configuration(IAppBuilder app) 
        {
          
            HttpConfiguration config=new HttpConfiguration();
            ConfigureAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            WebApiConfig.Register(config);
            app.UseWebApi(config);
           
             app.MapSignalR();
        }
        public  void ConfigureAuth(IAppBuilder app)
        {
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(2),
                Provider =new SimpleAuthorizationServerProvider()
            };
            //TokenGeneration
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        
        }
    }
}
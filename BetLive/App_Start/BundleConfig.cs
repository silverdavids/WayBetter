using System.Web;
using System.Web.Optimization;

namespace BetLive
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                       "~/Content/bootstrap.*"));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       "~/Scripts/bootstrap.js",
                       "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
        
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));


            bundles.Add(new ScriptBundle("~/bundles/liveApp").Include( "~/Scripts/jquery-1.8.2.js",
    "~/Scripts/jquery.signalR-2.1.2.js",
    "~/Scripts/Angular/angularjs/angular.min.js",
    "~/Scripts/Angular/vender/json3.js",
    "~/Scripts/Bootstrap/bootstrap.min.js",
    "~/Scripts/Angular/angularjs/angular-resource.js",
    "~/Scripts/Angular/angularjs/angular-cookies.js",
    "~/Scripts/Angular/angularjs/angular-sanitize.js",
    "~/Scripts/Angular/angularjs/angular-animate.js",
    "~/Scripts/Angular/angularjs/angular-touch.js",
    "~/Scripts/Angular/angularjs/angular-route.js",
    "~/Scripts/Angular/vender/angular-local-storage.min.js",
    "~/Scripts/Angular/vender/loading-bar.js",
    "~/Scripts/Bootstrap/ui-bootstrap-tpls-0.11.2.min.js",

   
    "~/Scripts/app/autoNumeric.js",
    "~/Scripts/app/bet.js",
    "~/Scripts/app/receipt.js",
    "~/Scripts/app/receiptSender.js",
    "~/Scripts/app/printExt.js",
    "~/Scripts/app/receiptGen.js",
   
    "~/Scripts/Angular/scripts/services/dataservice.js",

    "~/Scripts/Angular/scripts/app.js",

    "~/Scripts/Angular/scripts/services/authservice.js",
    "~/Scripts/Angular/scripts/services/authinterceptorservice.js",
    "~/Scripts/Angular/scripts/services/matchservice.js",
    "~/Scripts/Angular/scripts/controllers/main.js",
    "~/Scripts/Angular/scripts/controllers/about.js",
    "~/Scripts/Angular/scripts/controllers/signup.js",
    "~/Scripts/Angular/scripts/controllers/login.js",
    "~/Scripts/Angular/scripts/controllers/index.js",
    "~/Scripts/Angular/scripts/services/liveBetsSrvc.js",
    "~/Scripts/Angular/scripts/controllers/newMatchCtrl.js",
    "~/Scripts/Angular/scripts/controllers/home.js",
    "~/Scripts/Angular/scripts/controllers/matches.js",
    "~/Scripts/Angular/scripts/controllers/company.js",
    "~/Scripts/Angular/scripts/controllers/branch.js",
    "~/Scripts/Angular/scripts/filters/datatime.js",
    "~/Scripts/Angular/scripts/controllers/terminal.js",
    "~/Scripts/Angular/scripts/controllers/shift.js",
    "~/Scripts/Angular/directives/tablesDirectives.js",
    "~/Scripts/Bootstrap/jasny-bootstrap.js"

          ));
        }
    }
}
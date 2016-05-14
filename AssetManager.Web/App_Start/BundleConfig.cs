using System.Web.Optimization;

namespace AssetManager.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            //VENDOR RESOURCES

            //~/Bundles/App/vendor/css
            //FIXED 2015-11-18. See http://forum.aspnetboilerplate.com/viewtopic.php?p=1962
            bundles.Add(
                new StyleBundle("~/Bundles/App/vendor/css")
                    .Include("~/Content/themes/base/all.css", new CssRewriteUrlTransform())
                    .Include("~/Content/bootstrap-cerulean.min.css", new CssRewriteUrlTransform())
                    //.Include("~/Content/bootstrap-cosmo.min.css", new CssRewriteUrlTransform())
                    //.Include("~/Content/bootstrap-paper.min.css", new CssRewriteUrlTransform())
                    //.Include("~/Content/bootstrap-simplex.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/toastr.min.css")
                    .Include("~/Scripts/sweetalert/sweet-alert.css")
                    .Include("~/Content/flags/famfamfam-flags.css", new CssRewriteUrlTransform())
                    .Include("~/Content/font-awesome.min.css", new CssRewriteUrlTransform())
                    .Include("~/Content/ui-grid.min.css", new CssRewriteUrlTransform())     // Angular UI Grid
                    .Include("~/Content/showErrors.css", new CssRewriteUrlTransform())     // Bootstrap form validation -- see http://blog.yodersolutions.com/bootstrap-form-validation-done-right-in-angularjs/
                );

            //~/Bundles/App/vendor/js
            bundles.Add(
                new ScriptBundle("~/Bundles/App/vendor/js")
                    .Include(
                        "~/Abp/Framework/scripts/utils/ie10fix.js",
                        "~/Scripts/json2.min.js",

                        "~/Scripts/modernizr-2.8.3.js",
                        
                        "~/Scripts/jquery-2.2.3.min.js",
                        "~/Scripts/jquery-ui-1.11.4.min.js",

                        "~/Scripts/bootstrap.min.js",

                        "~/Scripts/moment-with-locales.min.js",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Scripts/toastr.min.js",
                        "~/Scripts/sweetalert/sweet-alert.min.js",
                        "~/Scripts/others/spinjs/spin.js",
                        "~/Scripts/others/spinjs/jquery.spin.js",

                        "~/Scripts/angular.min.js",
                        "~/Scripts/angular-animate.min.js",
                        "~/Scripts/angular-sanitize.min.js",
                        "~/Scripts/angular-ui-router.min.js",
                        "~/Scripts/angular-ui/ui-bootstrap.min.js",
                        "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                        "~/Scripts/angular-ui/ui-utils.min.js",
                        "~/Scripts/ui-grid.js",                 // Angular UI Grid
                        "~/Scripts/ui-bootstrap-tpls-1.3.2.min.js", // UI Bootstrap -- see https://angular-ui.github.io/bootstrap/

                        "~/Scripts/flot/jquery.flot.min.js",    // Flot -- see http://www.flotcharts.org/
                        "~/Scripts/angular-flot.js",            // Flot helper library -- see https://docs.omniref.com/js/npm/angular-flot/0.0.6

                        //"~/Scripts/jquery.canvasjs.min.js",   // CanvasJS -- see http://canvasjs.com/
                        "~/Scripts/canvasjs.min.js",            // CanvasJS -- see http://canvasjs.com/

                        //"~/Scripts/validator.min.js",           // Bootstrap form validation -- see http://1000hz.github.io/bootstrap-validator/
                        "~/Scripts/showErrors.min.js",          // Bootstrap form validation -- see http://blog.yodersolutions.com/bootstrap-form-validation-done-right-in-angularjs/

                        "~/Abp/Framework/scripts/abp.js",
                        "~/Abp/Framework/scripts/libs/abp.jquery.js",
                        "~/Abp/Framework/scripts/libs/abp.toastr.js",
                        "~/Abp/Framework/scripts/libs/abp.blockUI.js",
                        "~/Abp/Framework/scripts/libs/abp.spin.js",
                        "~/Abp/Framework/scripts/libs/abp.sweet-alert.js",
                        "~/Abp/Framework/scripts/libs/angularjs/abp.ng.js"
                    )
                );

            //APPLICATION RESOURCES

            //~/Bundles/App/Main/css
            bundles.Add(
                new StyleBundle("~/Bundles/App/Main/css")
                    .IncludeDirectory("~/App/Main", "*.css", true)
                );

            //~/Bundles/App/Main/js
            bundles.Add(
                new ScriptBundle("~/Bundles/App/Main/js")
                    .IncludeDirectory("~/App/Main", "*.js", true)
                );
        }
    }
}
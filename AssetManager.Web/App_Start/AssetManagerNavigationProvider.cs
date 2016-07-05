using Abp.Application.Navigation;
using Abp.Localization;

namespace AssetManager.Web
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// See .cshtml and .js files under App/Main/views/layout/header to know how to render menu.
    /// </summary>
    public class AssetManagerNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        "AssetHealthTtlAssetHealth",
                        new LocalizableString("AssetHealthTtlAssetHealth", AssetManagerConsts.LocalizationSourceName),
                        url: "#/",
                        icon: "fa fa-heartbeat"
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "AssetHealthDashboard",
                            new LocalizableString("AssetHealthTtlDashboard", AssetManagerConsts.LocalizationSourceName),
                            url: "#/assethealthdashboard",
                            icon: "fa fa-heartbeat"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "AssetHealthDashboardForAssets",
                            new LocalizableString("AssetHealthTtlDashboardForAssets", AssetManagerConsts.LocalizationSourceName),
                            url: "#/assethealthdashboardforassets",
                            icon: "fa fa-bullseye"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "AssetHealthDashboardForTypes",
                            new LocalizableString("AssetHealthTtlDashboardForTypes", AssetManagerConsts.LocalizationSourceName),
                            url: "#/assethealthdashboardfortypes",
                            icon: "fa fa-dot-circle-o"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "HealthMetricList",
                            new LocalizableString("AssetHealthMnuHealthMetricList", AssetManagerConsts.LocalizationSourceName),
                            url: "#/healthmetriclist",
                            icon: "fa fa-heartbeat"
                            )
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "IOW",
                        new LocalizableString("IOWs", AssetManagerConsts.LocalizationSourceName),
                        url: "#/",
                        icon: "fa fa-bar-chart"
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "IOWDashboard",
                            new LocalizableString("IowMnuDashboard", AssetManagerConsts.LocalizationSourceName),
                            url: "#/iowdashboard",
                            icon: "fa fa-bar-chart"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "IOWMain",
                            new LocalizableString("IOWs", AssetManagerConsts.LocalizationSourceName),
                            url: "#/iowmain",
                            icon: "fa fa-bar-chart"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "IOWLimitSummary",
                            new LocalizableString("IowTtlLimitSummary", AssetManagerConsts.LocalizationSourceName),
                            url: "#/iowlimitsummary",
                            icon: "fa fa-dashboard"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "IOWLimitStatus",
                            new LocalizableString("IowTtlLimitStatus", AssetManagerConsts.LocalizationSourceName),
                            url: "#/iowlimitstatus",
                            icon: "fa fa-exclamation-circle"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "IOWVariables",
                            new LocalizableString("IOWVariables", AssetManagerConsts.LocalizationSourceName),
                            url: "#/iowvariablelist",
                            icon: "fa fa-crosshairs"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "IOWLevelList",
                            new LocalizableString("IOWLevels", AssetManagerConsts.LocalizationSourceName),
                            url: "#/iowlevellist",
                            icon: "fa fa-tasks"
                            )
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "TagList",
                        new LocalizableString("Tags", AssetManagerConsts.LocalizationSourceName),
                        url: "#/taglist",
                        icon: "fa fa-database"
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "AssetList",
                        new LocalizableString("Assets", AssetManagerConsts.LocalizationSourceName),
                        url: "#/assetlist",
                        icon: "fa fa-tags"
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "AssetList",
                            new LocalizableString("Assets", AssetManagerConsts.LocalizationSourceName),
                            url: "#/assetlist",
                            icon: "fa fa-tags"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "AssetHierarchy",
                            new LocalizableString("AssetMnuAssetHierarchy", AssetManagerConsts.LocalizationSourceName),
                            url: "#/assethierarchy",
                            icon: "fa fa-sitemap"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "AssetLevelBarChart",
                            new LocalizableString("AssetMnuAssetLevelBarChart", AssetManagerConsts.LocalizationSourceName),
                            url: "#/assetlevelbarchart",
                            icon: "fa fa-area-chart"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "AssetVariableList",
                            new LocalizableString("AssetMnuAssetVariableList", AssetManagerConsts.LocalizationSourceName),
                            url: "#/assetvariablelist",
                            icon: "fa fa-exchange"
                            )
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "AssetVariableAssignment",
                            new LocalizableString("AssetMnuAssetVariableAssignment", AssetManagerConsts.LocalizationSourceName),
                            url: "#/assetvariableassignment",
                            icon: "fa fa-exchange"
                            )
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "Admin",
                        new LocalizableString("AdminMenu", AssetManagerConsts.LocalizationSourceName),
                        url: "#/userlist",
                        icon: "fa fa-gear"
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "UserList",
                            new LocalizableString("UserMenu", AssetManagerConsts.LocalizationSourceName),
                            url: "#/userlist",
                            icon: "fa fa-gear"
                            )
                    )
                   .AddItem(
                        new MenuItemDefinition(
                            "Swagger",
                            new LocalizableString("SwaggerMenu", AssetManagerConsts.LocalizationSourceName),
                            url: "swagger/ui/index#/",
                            icon: "fa fa-key"
                            )
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        "Home",
                        new LocalizableString("Help", AssetManagerConsts.LocalizationSourceName),
                        url: "#/",
                        icon: "fa fa-question"
                    )
                    .AddItem(
                        new MenuItemDefinition(
                            "About",
                            new LocalizableString("About", AssetManagerConsts.LocalizationSourceName),
                            url: "#/about",
                            icon: "fa fa-info"
                            )
                    )
                );
        }
    }
}

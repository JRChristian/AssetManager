﻿using Abp.Application.Navigation;
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
                        "Home",
                        new LocalizableString("HomePage", AssetManagerConsts.LocalizationSourceName),
                        url: "#/",
                        icon: "fa fa-home"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "About",
                        new LocalizableString("About", AssetManagerConsts.LocalizationSourceName),
                        url: "#/about",
                        icon: "fa fa-info"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "AssetList",
                        new LocalizableString("Assets", AssetManagerConsts.LocalizationSourceName),
                        url: "#/assetlist",
                        icon: "fa fa-tasks"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "AssetNew",
                        new LocalizableString("NewAsset", AssetManagerConsts.LocalizationSourceName),
                        url: "#/assetnew",
                        icon: "fa fa-asterisk"
                        )
                );
        }
    }
}

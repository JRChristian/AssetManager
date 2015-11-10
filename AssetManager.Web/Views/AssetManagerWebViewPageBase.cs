using Abp.Web.Mvc.Views;

namespace AssetManager.Web.Views
{
    public abstract class AssetManagerWebViewPageBase : AssetManagerWebViewPageBase<dynamic>
    {

    }

    public abstract class AssetManagerWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected AssetManagerWebViewPageBase()
        {
            LocalizationSourceName = AssetManagerConsts.LocalizationSourceName;
        }
    }
}
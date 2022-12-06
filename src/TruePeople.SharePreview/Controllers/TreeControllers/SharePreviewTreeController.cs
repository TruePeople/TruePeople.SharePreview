using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.ModelBinders;
using UmbConstants = Umbraco.Cms.Core.Constants;

namespace TruePeople.SharePreview.Controllers.TreeControllers
{
    [PluginController("TruePeopleSharePreview")]
    [Tree(
        UmbConstants.Applications.Settings,
        "shareablepreview",
        IsSingleNodeTree = true,
        TreeGroup = UmbConstants.Trees.Groups.ThirdParty,
        TreeUse = TreeUse.Main)]
    public class ShareaPreviewTreeController : TreeController
    {
        public ShareaPreviewTreeController(
            ILocalizedTextService localizedTextService,
            UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
            IEventAggregator eventAggregator)
            : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        {
        }

        protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.Value.Icon = "icon-link";
            root.Value.HasChildren = false;
            root.Value.MenuUrl = null;
            root.Value.Name = "Shareable Preview Settings";
            root.Value.RoutePath = "settings/shareablepreview/settings";

            return root;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings) => default;

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings) => default;
    }
}
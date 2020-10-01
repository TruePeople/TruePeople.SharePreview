using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.ModelBinding;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.WebApi.Filters;
using UmbConstants = Umbraco.Core.Constants;

namespace TruePeople.SharePreview.Controllers.TreeControllers
{
    [UmbracoTreeAuthorize("shareablepreview", Roles = UmbConstants.Security.AdminGroupAlias)]
    [Tree(UmbConstants.Applications.Settings, "shareablepreview", IsSingleNodeTree = true, SortOrder = 30, TreeGroup = UmbConstants.Trees.Groups.Settings)]
    [PluginController("TruePeopleSharePreview")]
    public class ShareaPreviewTreeController : TreeController
    {
        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.Icon = "icon-link";
            root.HasChildren = false;
            root.MenuUrl = null;
            root.Name = "Shareable Preview Settings";
            root.RoutePath = "settings/shareablepreview/settings";

            return root;
        }

        protected override MenuItemCollection GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }

        protected override TreeNodeCollection GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            return new TreeNodeCollection();
        }
    }
}

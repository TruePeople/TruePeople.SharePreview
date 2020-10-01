using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using TruePeople.SharePreview.Composers.Handlers;
using TruePeople.SharePreview.Helpers;
using TruePeople.SharePreview.Models;
using TruePeople.SharePreview.Services;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace TruePeople.SharePreview.Composers
{
    internal class TPSharePreviewComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
          RouteTable.Routes.MapUmbracoRoute(
          name: "TPSharePreview",
          url: "umbraco/sharepreview/{action}/{pageId}",
          defaults: new { controller = "Default" },
          new TPPreviewShareRouteHandler()
          );

            composition.Register(typeof(SharePreviewSettingsService));
        }
    }
}

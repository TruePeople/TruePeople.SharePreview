angular.module("umbraco").controller("TP.SharePreview.Button.Controller",
    function ($scope, overlayService) {
        console.log("init controller");

        $scope.clickLog = function (e) {
            console.log("click test");
            console.log(e.href);
        };

        $scope.openShareLinksOverlay = function () {
            var overlay = {
                view: "/App_Plugins/TPSharePreview/views/multiple-variants-sharepreview-overlay.html",
                size: "medium",
                content: "<h1 class='umb-overlay__title'>Variants</h1><div class='mb3'><p>Choose what variant you would like to share.</p></div>",
                close: function () {
                    overlayService.close();
                }
            }
            overlayService.open(overlay);
        }
    });
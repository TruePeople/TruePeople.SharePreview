(function () {
    'use strict';

    function multiShareLinkDirective(overlayService, sharePreviewResource, $routeParams, eventsService) {

        function link($scope) {
            $scope.shouldShow = true;

            $scope.openShareLinksOverlay = function () {
                sharePreviewResource.getShareableLinks($routeParams.id).then(function (res) {
                    var overlay = {
                        view: "/App_Plugins/TruePeopleSharePreview/overlays/multiple-variants-sharepreview-overlay.html",
                        size: "medium",
                        links: res,
                        close: function () {
                            overlayService.close();
                        }
                    }
                    overlayService.open(overlay);
                });
            };

            eventsService.on("app.tabChange", function (name, args) {
                if (args.alias == "umbContent" || args.alias == "umbInfo") {
                    $scope.shouldShow = true;
                } else {
                    $scope.shouldShow = false;
                }
            });
        }

        var directive = {
            scope: {
                buttonlabel: "@",
                enabled: "@"
            },
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/TruePeopleSharePreview/components/multi-share-link.html?umb_rnd=' + Umbraco.Sys.ServerVariables.application.cacheBuster,
            link: link,
        };

        return directive;

    }

    angular.module('umbraco.directives').directive('multiShareLink', multiShareLinkDirective);

})();
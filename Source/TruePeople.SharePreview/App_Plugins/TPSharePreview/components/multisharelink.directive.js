(function () {
    'use strict';

    function multiShareLinkDirective(overlayService, sharePreviewResource, $routeParams) {

        function link($scope, $element) {
            $scope.openShareLinksOverlay = function () {
                sharePreviewResource.getShareableLinks($routeParams.id).then(function (res) {
                    var overlay = {
                        view: "/App_Plugins/TPSharePreview/overlays/multiple-variants-sharepreview-overlay.html",
                        size: "medium",
                        links: res,
                        close: function () {
                            overlayService.close();
                        }
                    }
                    overlayService.open(overlay);
                });

            }
        }

        var directive = {
            scope: {
                buttonlabel: "@",
                enabled: "@"
            },
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/TPSharePreview/components/multi-share-link.html?umb_rnd=' + Umbraco.Sys.ServerVariables.application.cacheBuster,
            link: link,
        };

        return directive;

    }

    angular.module('umbraco.directives').directive('multiShareLink', multiShareLinkDirective);

})();
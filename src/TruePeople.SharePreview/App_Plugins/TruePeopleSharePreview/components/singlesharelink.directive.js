(function () {
    'use strict';

    function singleShareLinkDirective(sharePreviewCopyService, eventsService) {

        function link($scope) {
            $scope.shouldShow = true;

            $scope.copyShareLink = function (link) {
                sharePreviewCopyService.copyShareLink(link);
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
                shareurl: "@",
                enabled: "@"
            },
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/TruePeopleSharePreview/components/single-share-link.html?umb_rnd=' + Umbraco.Sys.ServerVariables.application.cacheBuster,
            link: link
        };

        return directive;

    }

    angular.module('umbraco.directives').directive('singleShareLink', singleShareLinkDirective);

})();
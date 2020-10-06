angular.module("umbraco").controller("TPSharePreview.MultiVariants.Overlay.controller",
    function ($scope, sharePreviewCopyService) {

        $scope.copyShareLink = function (link) {
            sharePreviewCopyService.copyShareLink(link);
        }
    });
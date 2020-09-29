(function () {
    "use strict";

    function sharePreviewResource($http, umbRequestHelper) {

        var sharePreviewBaseUrl = "/umbraco/backoffice/api/SharePreviewApi/";

        var resource = {
            getShareableLinks: getShareableLinks,
            getShareableLink: getShareableLink,
            hasShareableLink: hasShareableLink
        };

        return resource;

        function getShareableLinks(id) {
            return umbRequestHelper.resourcePromise(
                $http.get(sharePreviewBaseUrl + "GetShareableLinks?nodeId=" + id),
                "Failed getting share links"
            );
        };

        function getShareableLink(id, culture) {
            return umbRequestHelper.resourcePromise(
                $http.get(sharePreviewBaseUrl + "GetShareableLink?nodeId=" + id + "&culture=" + culture),
                "Failed getting share link"
            );
        };

        function hasShareableLink(id) {
            return umbRequestHelper.resourcePromise(
                $http.get(sharePreviewBaseUrl + "HasShareableLink?nodeId=" + id),
                "Failed getting status"
            );
        }
    }

    angular.module("umbraco.resources").factory("sharePreviewResource", sharePreviewResource);

})();
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
                $http.get(sharePreviewBaseUrl + "GetShareableLinks", {
                    params: {
                        nodeId: id
                    }
                }));
        };

        function getShareableLink(id, culture) {
            return umbRequestHelper.resourcePromise(
                $http.get(sharePreviewBaseUrl + "GetShareableLink", {
                    params: {
                        nodeId: id,
                        culture: culture
                    }
                }));
        };

        function hasShareableLink(id) {
            return umbRequestHelper.resourcePromise(
                $http.get(sharePreviewBaseUrl + "HasShareableLink", {
                    params: {
                        nodeId: id
                    }
                }));
        }
    }

    angular.module("umbraco.resources").factory("sharePreviewResource", sharePreviewResource);

})();
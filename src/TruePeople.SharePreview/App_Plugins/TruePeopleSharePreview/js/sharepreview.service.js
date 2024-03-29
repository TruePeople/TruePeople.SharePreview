﻿angular.module("umbraco").run(function ($rootScope, $routeParams, eventsService, localizationService, sharePreviewResource, $compile) {
    var buttonLabel = "Share preview";
    localizationService.localize("shareablepreview_buttonlabel").then(function (value) {
        buttonLabel = value;
    });

    $rootScope.$on('$routeUpdate', function (event, next) {
        if ($routeParams.id === undefined || $routeParams.id === -1 || $routeParams.section !== "content") {
            return;
        }
        var culture = next.params.cculture ? next.params.cculture : $routeParams.mculture;
        if (culture !== undefined && culture !== null) {
            setSharePreviewButton($routeParams.id, culture);
        } else {
            setSharePreviewButton($routeParams.id);
        }
    });

    eventsService.on("content.loaded", function (name, args) {
        if (args.content.id === parseInt($routeParams.id)) {
            contentReload(args.content.id, args.content.variants.length);
        }
    });

    eventsService.on("content.saved", function (name, args) {
        if (args.content.id === parseInt($routeParams.id)) {
            contentReload(args.content.id, args.content.variants.length);
        }
    });

    function contentReload(id, variantsLength) {
        if (id === undefined) {
            return;
        }
        if (variantsLength === 1) {
            initializeButtonLoader(id);

        } else {
            var culture = $routeParams.cculture ? $routeParams.cculture : $routeParams.mculture;
            initializeButtonLoader(id, culture);
        }
    }

    function setSharePreviewButton(nodeId, culture = "") {
        sharePreviewResource.hasShareableLink(nodeId).then(function (res) {
            sharePreviewResource.getShareableLink(nodeId, culture).then(function (data) {
                if (data == null || data == undefined) {
                    return;
                }
                var buttonElement = "";
                if (culture === "") {
                    buttonElement = angular.element("<single-share-link enabled='" + res +"' shareUrl='" + data + "' buttonLabel='" + buttonLabel + "' />");
                } else {
                    buttonElement = angular.element("<multi-share-link enabled='" + res +"' buttonLabel='" + buttonLabel + "' />");
                }
                var linkFN = $compile(buttonElement);
                var el = linkFN($rootScope);

                if (document.querySelector(".umb-editor-footer-content__right-side") === null) {
                    return;
                }

                if (document.querySelector("#shareLink") === null) {
                    document.querySelector(".umb-editor-footer-content__right-side").prepend(el[0]);
                } else {
                    document.querySelector("#shareLink").replaceWith(el[0]);
                }
            });
        });
    }

    function initializeButtonLoader(contentId, culture = "") {
        setTimeout(function () {
            setSharePreviewButton(contentId, culture);
        }, 10);
    }
});

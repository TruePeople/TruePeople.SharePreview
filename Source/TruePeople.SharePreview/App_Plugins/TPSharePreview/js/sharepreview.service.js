angular.module("umbraco").run(function ($rootScope, $routeParams, eventsService, localizationService, $sce, $parse, $compile) {
    var buttonLabel = "Share preview";
    localizationService.localize("shareablepreview_buttonlabel").then(function (value) {
        buttonLabel = value;
    });

    $rootScope.$on('$routeUpdate', function (event, next) {
        var culture = next.params.cculture ? next.params.cculture : $routeParams.mculture;
        if (culture !== undefined && culture !== null) {
            setSharePreviewButton($routeParams.id, culture);
        } else {
            setSharePreviewButton($routeParams.id, null);
        }
    });

    eventsService.on("content.loaded", function (name, args) {
        console.log(args.content);
        initializeButtonLoader(args.content.id);

    });

    eventsService.on("content.saved", function (name, args) {
        initializeButtonLoader(args.content.id);
    });


    function setSharePreviewButton(nodeId, culture = "") {
        fetch('/umbraco/backoffice/api/SharePreviewApi/GetShareableLink?nodeId=' + nodeId + '&culture=' + culture).then(function (res) {
            res.json().then(function (data) {
                var buttonElement = angular.element("<a ng-controller='TP.SharePreview.Button.Controller' ng-click='openShareLinksOverlay()' id='shareLink' class='btn umb-button__button btn-info' >" + buttonLabel + "</a>");
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

    function initializeButtonLoader(contentId) {
        setTimeout(function () {
            setSharePreviewButton(contentId);
        }, 10);
    }
});

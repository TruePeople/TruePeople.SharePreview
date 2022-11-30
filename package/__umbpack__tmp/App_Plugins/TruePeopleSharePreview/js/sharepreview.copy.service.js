(function () {
    "use strict";

    function sharePreviewCopyService(notificationsService, localizationService) {
        var service = {
            copyShareLink: function (link) {
                localizationService.localize("shareablepreview_copytext").then(function (res) {
                    textToClipboard(window.location.origin + link);

                    notificationsService.info(res);
                });

            }
        };

        function textToClipboard(text) {
            var dummy = document.createElement("textarea");
            document.body.appendChild(dummy);
            dummy.value = text;
            dummy.select();
            document.execCommand("copy");
            document.body.removeChild(dummy);
        }

        return service;
    }

    angular.module("umbraco.services").factory("sharePreviewCopyService", sharePreviewCopyService);

})();
define(["services/config"], function (config) {
    var test,
        activate = function () {
            console.log(config.application_name);
        },
        attached = function () {

        };

    return {
        config: config,
        activate: activate,
        attached: attached
    };
});
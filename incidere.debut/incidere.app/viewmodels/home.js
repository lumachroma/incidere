define(["services/config"], function (config) {

    return {
        activate: function () {
            console.log(config.application_name);
        },
        attached: function () {

        }
    };
});
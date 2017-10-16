define(['knockout', 'schemas', 'durandal/system', 'durandal/app', 'plugins/router', 'services/config', 'services/datacontext'],
    function (ko, schemas, system, app, router, config, context) {
        var isBusy = ko.observable(false),
            id = ko.observable(),
            showEditForm = ko.observable(false),
            entity = ko.observable(new schemas.LocalUser(system.guid())),
            userNewPassword = ko.observable(),
            userConfirmPassword = ko.observable(),
            activate = function () {
                id(config.sub);
                isBusy(true);
                context.get(`/api/local-users/${id()}`, true, {})
                    .done(function (result) {
                        console.log(result);
                        entity(new schemas.LocalUser(result._result));
                        console.log(ko.toJSON(entity));
                        isBusy(false);
                    }).fail(function (e) {
                        console.log(e.status);
                        console.log(e.statusText);
                        isBusy(false);
                    });
            },
            defultOperationEndpoint = function (json, endpoint, verb, successMessage) {
                context.send(json, `/${endpoint}`, verb)
                    .done(function (result) {
                        console.log(result);
                        isBusy(false);
                        app.showMessage(successMessage, config.application_name, ["OK"])
                            .done(function (result) {
                                if (result == "OK") {
                                    backToEntityList();
                                }
                            });
                    }).fail(function (e) {
                        console.log(e.status);
                        console.log(e.statusText);
                        isBusy(false);
                    });
            },
            editEntity = function () {
                var data = ko.toJSON(entity);
                var endpoint = `api/local-users/${id()}`;
                var msg = "Successfully edited.";
                console.log(data);
                defultOperationEndpoint(data, endpoint, "PUT", msg);
            },
            editPassword = function () {
                if (!$("#profile-password-form").valid()) {
                    console.log("#profile-password-form invalid");
                    return;
                }
                // TODO: move logic to mvc controller
                if (!(userNewPassword() === userConfirmPassword() && userConfirmPassword().length >= 8)) {
                    console.log("password invalid");
                    return;
                }
                entity().Password(userConfirmPassword());
                var data = ko.toJSON(entity);
                var endpoint = `api/local-users/${id()}`;
                var msg = "Successfully edited.";
                console.log(data);
                defultOperationEndpoint(data, endpoint, "PUT", msg);
            },
            toggleShowEditForm = function () {
                showEditForm(!showEditForm());
            },
            backToEntityList = function () {
                router.navigate("");
            },
            attached = function () {
                $("#profile-password-form").validate({
                    rules: {
                        NewPassword: {
                            required: true,
                            minlength: 8
                        },
                        ConfirmPassword: {
                            required: true,
                            minlength: 8,
                            equalTo: "#NewPassword"
                        }
                    },
                    messages: {
                        NewPassword: {
                            required: "Please provide a password.",
                            minlength: "Your password must be at least 8 characters long."
                        },
                        confirm_password: {
                            required: "Please provide a password.",
                            minlength: "Your password must be at least 8 characters long.",
                            equalTo: "Please enter the same password as above."
                        }
                    }
                });
            },
            compositionComplete = function () {

            },
            deactivate = function () {
                id(null);
                showEditForm(false);
            };

        return {
            id: id,
            entity: entity,
            userNewPassword: userNewPassword,
            userConfirmPassword: userConfirmPassword,
            editEntity: editEntity,
            editPassword: editPassword,
            showEditForm: showEditForm,
            toggleShowEditForm: toggleShowEditForm,
            backToEntityList: backToEntityList,
            isBusy: isBusy,
            activate: activate,
            attached: attached,
            compositionComplete: compositionComplete,
            deactivate: deactivate
        };
    });

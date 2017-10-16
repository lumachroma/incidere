define(['knockout', 'schemas', 'durandal/system', 'durandal/app', 'plugins/router', 'services/config', 'services/datacontext'],
    function (ko, schemas, system, app, router, config, context) {
        var isBusy = ko.observable(false),
            id = ko.observable(),
            showEditForm = ko.observable(false),
            entity = ko.observable(new schemas.LocalUser(system.guid())),
            userOldPassword = ko.observable(),
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
                isBusy(true);
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
                if (!(userNewPassword() === userConfirmPassword() && userConfirmPassword().length >= 8)) {
                    console.log("password invalid");
                    return;
                }
                var json = JSON.stringify({
                    "OldPassword": userOldPassword(),
                    "NewPassword": userNewPassword(),
                    "ConfirmPassword": userConfirmPassword()
                });
                isBusy(true);
                context.post(json, `/incidere-account/change-password/${id()}`, {})
                    .done(function (result) {
                        console.log(result);
                        isBusy(false);
                        app.showMessage("Successfully edited.", config.application_name, ["OK"])
                            .done(function (result) {
                                if (result == "OK") {
                                    backToEntityList();
                                }
                            });
                    }).fail(function (e) {
                        console.log(e.status);
                        console.log(e.statusText);
                        isBusy(false);
                        app.showMessage(e.statusText, config.application_name, ["OK"])
                            .done(function (result) {
                                if (result == "OK") {
                                    userOldPassword(null);
                                    userNewPassword(null);
                                    userConfirmPassword(null);
                                }
                            });
                    });
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
                        OldPassword: {
                            required: true
                        },
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
                        OldPassword: {
                            required: "Please provide old password."
                        },
                        NewPassword: {
                            required: "Please provide new password.",
                            minlength: "Your new password must be at least 8 characters long."
                        },
                        ConfirmPassword: {
                            required: "Please confirm new password.",
                            minlength: "Your new password must be at least 8 characters long.",
                            equalTo: "Your new passwords must be the same."
                        }
                    }
                });
            },
            compositionComplete = function () {

            },
            deactivate = function () {
                id(null);
                showEditForm(false);
                userOldPassword(null);
                userNewPassword(null);
                userConfirmPassword(null);
            };

        return {
            id: id,
            entity: entity,
            userOldPassword: userOldPassword,
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

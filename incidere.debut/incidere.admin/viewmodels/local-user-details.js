define(['knockout', 'schemas', 'durandal/system', 'durandal/app',
    'plugins/router', 'services/config', 'services/datacontext'],
    function (ko, schemas, system, app, router, config, context) {
        var isBusy = ko.observable(false),
            id = ko.observable(),
            showEditForm = ko.observable(false),
            entity = ko.observable(new schemas.LocalUser(system.guid())),
            usernameExist = ko.observable(false),
            emailExist = ko.observable(false),
            activate = function (entityId) {
                id(entityId);
                if (!entityId || entityId === "0") {
                    entity(schemas.LocalUser(system.guid()));
                    showEditForm(true);
                } else {
                    isBusy(true);
                    return context.get(`/api/local-users/${id()}`, true, {})
                        .done(function (result) {
                            //console.log(result);
                            entity(schemas.LocalUser(result._result));
                            console.log(ko.toJSON(entity));
                            isBusy(false);
                        }).fail(function (e) {
                            console.log(`${e.status} ${e.statusText} ${e.responseJSON.status}`);
                            isBusy(false);
                            app.showMessage(`${e.status}: ${e.statusText}. Invalid Entity Id: ${id()}`,
                                config.application_name, ["OK"])
                                .done(function (result) {
                                    if (result == "OK") {
                                        router.navigate("local-users-all");
                                    }
                                });
                        });
                }
            },
            defultOperationEndpoint = function (json, endpoint, verb, successMessage) {
                if (!$('#local-user-details-form').valid()) {
                    console.log("#local-user-details-form invalid");
                    return;
                }
                if (usernameExist() || emailExist()) {
                    console.log("username/email invalid");
                    return;
                }
                isBusy(true);
                context.send(json, `/${endpoint}`, verb)
                    .done(function (result) {
                        console.log(result);
                        isBusy(false);
                        app.showMessage(successMessage, config.application_name, ["OK"])
                            .done(function (result) {
                                if (result == "OK") {
                                    router.navigate("local-users-all");
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
                //console.log(data);
                defultOperationEndpoint(data, endpoint, "PUT", msg);
            },
            addEntity = function () {
                var data = ko.toJSON(entity);
                var endpoint = "api/local-users";
                var msg = "Successfully added.";
                //console.log(data);
                defultOperationEndpoint(data, endpoint, "POST", msg);
            },
            deleteEntity = function () {
                var data = ko.toJSON(entity);
                var endpoint = `api/local-users/${id()}`;
                var msg = "Successfully deleted.";
                //console.log(data);
                app.showMessage("Are you sure you want to delete?", config.application_name, ["Yes", "No"])
                    .done(function (result) {
                        if (result === "No") {
                            return
                        }
                        defultOperationEndpoint(data, endpoint, "DELETE", msg);
                    });
            },
            checkPropertyExist = function (endpoint, property) {
                isBusy(true);
                context.get(`/${endpoint}`, false, {})
                    .done(function (result) {
                        //console.log(result._result);
                        if (result._status.success) {
                            property(true);
                        }
                        isBusy(false);
                    }).fail(function (e) {
                        //console.log(`${e.status} ${e.statusText} ${e.responseJSON.status}`);
                        property(false);
                        isBusy(false);
                    });
            },
            checkUsernameExist = function (newUsername) {
                var endpoint = `api/local-users/Username/${newUsername}/`;
                checkPropertyExist(endpoint, usernameExist);
            },
            checkEmailExist = function (newEmail) {
                var endpoint = `api/local-users/Email/${newEmail}/`;
                checkPropertyExist(endpoint, emailExist);
            },
            toggleShowEditForm = function () {
                showEditForm(!showEditForm());
            },
            backToEntityList = function () {
                router.navigate("local-users-all");
            },
            attached = function () {
                $('#local-user-details-form').validate({
                    errorClass: "is-invalid",
                    rules: {
                        Username: { required: true },
                        Email: { required: true, email: true },
                        FirstName: { required: true },
                        LastName: { required: true },
                        DateOfBirth: { required: true },
                        Location: { required: true }
                    },
                    messages: {
                        Username: { required: "Username is required" },
                        Email: { required: "Email is required" },
                        FirstName: { required: "FirstName is required" },
                        LastName: { required: "LastName is required" },
                        DateOfBirth: { required: "DateOfBirt is required" },
                        Location: { required: "Location is required" }
                    }
                });
            },
            compositionComplete = function () {
                entity().Username.subscribe(checkUsernameExist);
                entity().Email.subscribe(checkEmailExist);
            },
            deactivate = function () {
                id(null);
                usernameExist(false);
                emailExist(false);
                showEditForm(false);
            };

        return {
            id: id,
            entity: entity,
            usernameExist: usernameExist,
            emailExist: emailExist,
            editEntity: editEntity,
            addEntity: addEntity,
            deleteEntity: deleteEntity,
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

define(['knockout', 'schemas', 'durandal/system', 'durandal/app', 'plugins/router', 'services/datacontext'],
    function (ko, schemas, system, app, router, context) {
        var isBusy = ko.observable(false),
            id = ko.observable(),
            showEditForm = ko.observable(false),
            entity = ko.observable(new schemas.LocalUser(system.guid())),
            activate = function (entityId) {
                id(entityId);
                if (!entityId || entityId === "0") {
                    entity(new schemas.LocalUser(system.guid()));
                    showEditForm(true);
                } else {
                    isBusy(true);
                    context.get(`/api/local-users/${id()}`, true, {})
                        .done(function (result) {
                            //console.log(result);
                            entity(new schemas.LocalUser(result._result));
                            console.log(ko.toJSON(entity));
                            isBusy(false);
                        }).fail(function (e) {
                            console.log(e.status);
                            console.log(e.statusText);
                            isBusy(false);
                        });
                }
            },
            defultOperationEndpoint = function (json, endpoint, verb, successMessage) {
                isBusy(true);
                context.send(json, `/${endpoint}`, verb)
                    .done(function (result) {
                        console.log(result);
                        isBusy(false);
                        app.showMessage(successMessage, "MVC Durandal", ["OK"])
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
                app.showMessage("Are you sure you want to delete?", "MVC Durandal", ["Yes", "No"])
                    .done(function (result) {
                        if (result === "No") {
                            return
                        }
                        defultOperationEndpoint(data, endpoint, "DELETE", msg);
                    });
            },
            toggleShowEditForm = function () {
                showEditForm(!showEditForm());
            },
            backToEntityList = function () {
                router.navigate("local-users-all");
            },
            attached = function () {

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

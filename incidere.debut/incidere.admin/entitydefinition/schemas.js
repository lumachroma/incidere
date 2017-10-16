define(['knockout'], function (ko) {

    var incidere = incidere || {};
    incidere.domain = incidere.domain || {};
    incidere.domain.LocalUser = function (optionOrWebid) {

        var model = {
            Id: ko.observable("0"),
            ReferenceNo: ko.observable(),
            Username: ko.observable(),
            Password: ko.observable(),
            Email: ko.observable(),
            FirstName: ko.observable(),
            LastName: ko.observable(),
            DateOfBirth: ko.observable(),
            Location: ko.observable(),
            ExternalUsers: ko.observableArray([]),
            Roles: ko.observableArray([]),
            WebId: ko.observable(),
            FirebaseKey: ko.observable(),

            addChildItem: function (list, type) {
                if (typeof type === "object") {
                    return function () {
                        list.push(new type(system.guid()));
                    }
                }
                return function () {
                    list.push(type);
                }
            },

            removeChildItem: function (list, obj) {
                return function () {
                    list.remove(obj);
                }
            },
        };

        if (typeof optionOrWebid === "object") {
            if (optionOrWebid.Id) {
                model.Id(optionOrWebid.Id);
            }
            if (optionOrWebid.WebId) {
                model.WebId(optionOrWebid.WebId);
            }
            if (optionOrWebid.FirebaseKey) {
                model.FirebaseKey(optionOrWebid.FirebaseKey);
            }
            if (optionOrWebid.ReferenceNo) {
                model.ReferenceNo(optionOrWebid.ReferenceNo);
            }
            if (optionOrWebid.Username) {
                model.Username(optionOrWebid.Username);
            }
            if (optionOrWebid.Password) {
                model.Password(optionOrWebid.Password);
            }
            if (optionOrWebid.Email) {
                model.Email(optionOrWebid.Email);
            }
            if (optionOrWebid.FirstName) {
                model.FirstName(optionOrWebid.FirstName);
            }
            if (optionOrWebid.LastName) {
                model.LastName(optionOrWebid.LastName);
            }
            if (optionOrWebid.DateOfBirth) {
                model.DateOfBirth(optionOrWebid.DateOfBirth);
            }
            if (optionOrWebid.Location) {
                model.Location(optionOrWebid.Location);
            }
            if (optionOrWebid.ExternalUsers) {
                var externalUsersList = optionOrWebid.ExternalUsers.map(function (v) {
                    return new incidere.domain.ExternalUser(v);
                });
                model.ExternalUsers(externalUsersList);
            }
            if (optionOrWebid.Roles) {
                model.Roles(optionOrWebid.Roles);
            }
        }

        if (optionOrWebid && typeof optionOrWebid === "string") {
            model.WebId(optionOrWebid);
        }

        return model;
    };

    incidere.domain.ExternalUser = function (optionOrWebid) {
        var model = {
            Provider: ko.observable(),
            ProviderId: ko.observable(),
            Roles: ko.observableArray([]),
            WebId: ko.observable()
        };

        if (typeof optionOrWebid === "object") {
            if (optionOrWebid.WebId) {
                model.WebId(optionOrWebid.WebId);
            }
            if (optionOrWebid.Provider) {
                model.Lat(optionOrWebid.Provider);
            }
            if (optionOrWebid.ProviderId) {
                model.Long(optionOrWebid.ProviderId);
            }
            if (optionOrWebid.Roles) {
                model.Roles(optionOrWebid.Roles);
            }
        }

        if (optionOrWebid && typeof optionOrWebid === "string") {
            model.WebId(optionOrWebid);
        }

        return model;
    };

    return {
        LocalUser: incidere.domain.LocalUser // TODO: namespace
    };
});
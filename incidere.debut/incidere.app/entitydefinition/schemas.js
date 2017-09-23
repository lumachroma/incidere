define(['knockout'], function (ko) {

    var incidere = incidere || {};
    incidere.domain = incidere.domain || {};
    incidere.domain.AddressBook = function (optionOrWebid) {

        var model = {
            Id: ko.observable("0"),
            ReferenceNo: ko.observable(),
            CompanyName: ko.observable(),
            ContactPerson: ko.observable(),
            Groups: ko.observableArray([]),
            ContactInformation: ko.observable(new incidere.domain.ContactInformation()),
            Address: ko.observable(new incidere.domain.Address()),
            ProfilePictureUrl: ko.observable(),
            UserId: ko.observable(),
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
            if (optionOrWebid.CompanyName) {
                model.CompanyName(optionOrWebid.CompanyName);
            }
            if (optionOrWebid.ContactPerson) {
                model.ContactPerson(optionOrWebid.ContactPerson);
            }
            if (optionOrWebid.Groups) {
                model.Groups(optionOrWebid.Groups);
            }
            if (optionOrWebid.ContactInformation) {
                model.ContactInformation(new incidere.domain.ContactInformation(optionOrWebid.ContactInformation));
            }
            if (optionOrWebid.Address) {
                model.Address(new incidere.domain.Address(optionOrWebid.Address));
            }
            if (optionOrWebid.ProfilePictureUrl) {
                model.ProfilePictureUrl(optionOrWebid.ProfilePictureUrl);
            }
            if (optionOrWebid.UserId) {
                model.UserId(optionOrWebid.UserId);
            }
        }

        if (optionOrWebid && typeof optionOrWebid === "string") {
            model.WebId(optionOrWebid);
        }

        return model;
    };

    incidere.domain.ContactInformation = function (optionOrWebid) {
        var model = {
            Email: ko.observable(),
            ContactNumber: ko.observable(),
            AlternativeContactNumber: ko.observable(),
            WebId: ko.observable()
        };

        if (typeof optionOrWebid === "object") {
            if (optionOrWebid.WebId) {
                model.WebId(optionOrWebid.WebId);
            }
            if (optionOrWebid.Email) {
                model.Email(optionOrWebid.Email);
            }
            if (optionOrWebid.ContactNumber) {
                model.ContactNumber(optionOrWebid.ContactNumber);
            }
            if (optionOrWebid.AlternativeContactNumber) {
                model.AlternativeContactNumber(optionOrWebid.AlternativeContactNumber);
            }
        }

        if (optionOrWebid && typeof optionOrWebid === "string") {
            model.WebId(optionOrWebid);
        }

        return model;
    };

    incidere.domain.Address = function (optionOrWebid) {
        var model = {
            Address1: ko.observable(),
            Address2: ko.observable(),
            Address3: ko.observable(),
            Address4: ko.observable(),
            City: ko.observable(),
            State: ko.observable(),
            Country: ko.observable(),
            Postcode: ko.observable(),
            GeoLocation: ko.observable(new incidere.domain.GeoLocation()),
            WebId: ko.observable()
        };

        if (typeof optionOrWebid === "object") {
            if (optionOrWebid.WebId) {
                model.WebId(optionOrWebid.WebId);
            }
            if (optionOrWebid.Address1) {
                model.Address1(optionOrWebid.Address1);
            }
            if (optionOrWebid.Address2) {
                model.Address2(optionOrWebid.Address2);
            }
            if (optionOrWebid.Address3) {
                model.Address3(optionOrWebid.Address3);
            }
            if (optionOrWebid.Address4) {
                model.Address4(optionOrWebid.Address4);
            }
            if (optionOrWebid.City) {
                model.City(optionOrWebid.City);
            }
            if (optionOrWebid.State) {
                model.State(optionOrWebid.State);
            }
            if (optionOrWebid.Country) {
                model.Country(optionOrWebid.Country);
            }
            if (optionOrWebid.Postcode) {
                model.Postcode(optionOrWebid.Postcode);
            }
            if (optionOrWebid.GeoLocation) {
                model.GeoLocation(new incidere.domain.GeoLocation(optionOrWebid.GeoLocation));
            }
        }

        if (optionOrWebid && typeof optionOrWebid === "string") {
            model.WebId(optionOrWebid);
        }

        return model;
    };

    incidere.domain.GeoLocation = function (optionOrWebid) {
        var model = {
            Lat: ko.observable(),
            Long: ko.observable(),
            WebId: ko.observable()
        };

        if (typeof optionOrWebid === "object") {
            if (optionOrWebid.WebId) {
                model.WebId(optionOrWebid.WebId);
            }
            if (optionOrWebid.Lat) {
                model.Lat(optionOrWebid.Lat);
            }
            if (optionOrWebid.Long) {
                model.Long(optionOrWebid.Long);
            }
        }

        if (optionOrWebid && typeof optionOrWebid === "string") {
            model.WebId(optionOrWebid);
        }

        return model;
    };

    return {
        AddressBook: incidere.domain.AddressBook // TODO: namespace
    };
});
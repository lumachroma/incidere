define(['plugins/router'], function (router) {
    return {
        router: router,
        activate: function () {
            router.map([
                { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
                { route: 'playground', title: 'Playground', moduleId: 'viewmodels/playground', nav: true },
                { route: 'addressbooks-all', title: 'Address Books', moduleId: 'viewmodels/addressbooks-all', nav: true },
                { route: 'addressbook-details/:entityId', title: 'Address Books Details', moduleId: 'viewmodels/addressbook-details', nav: false }
            ]).buildNavigationModel();
            return router.activate();
        }
    };
});
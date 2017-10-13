define(['plugins/router'], function (router) {
    return {
        router: router,
        activate: function () {
            router.map([
                { route: '', title: 'Dashboard', moduleId: 'viewmodels/dashboard', nav: true },
                { route: 'playground', title: 'Playground', moduleId: 'viewmodels/playground', nav: true }
            ]).buildNavigationModel();
            return router.activate();
        }
    };
});
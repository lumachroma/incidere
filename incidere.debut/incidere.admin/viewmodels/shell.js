define(['plugins/router', 'services/config'], function (router, config) {
    return {
        router: router,
        config: config,
        activate: function () {
            router.map([
                { route: '', title: 'Dashboard', moduleId: 'viewmodels/dashboard', nav: true },
                { route: 'playground', title: 'Playground', moduleId: 'viewmodels/playground', nav: true }
            ]).buildNavigationModel();
            return router.activate();
        }
    };
});
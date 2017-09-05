define(['plugins/router'], function (router) {
    return {
        router: router,
        activate: function () {
            router.map([
                { route: '', title: 'Home', moduleId: 'viewmodels/home', nav: true },
                { route: 'playground', title: 'Playground', moduleId: 'viewmodels/playground', nav: true }
            ]).buildNavigationModel();
            return router.activate();
        }
    };
});
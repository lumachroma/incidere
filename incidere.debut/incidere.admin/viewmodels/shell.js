define(['plugins/router', 'services/config'], function (router, config) {
    return {
        router: router,
        username: config.preferred_username,
        activate: function () {
            router.map([
                { route: '', title: 'Dashboard', moduleId: 'viewmodels/dashboard', nav: true },
                { route: 'profile', title: 'Profile', moduleId: 'viewmodels/profile', nav: true },
                { route: 'setting', title: 'Setting', moduleId: 'viewmodels/setting', nav: true },
                { route: 'playground', title: 'Playground', moduleId: 'viewmodels/playground', nav: true }
            ]).buildNavigationModel();
            return router.activate();
        }
    };
});
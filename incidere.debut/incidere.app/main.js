requirejs.config({
    paths: {
        'text': '../Scripts/text',
        'durandal': '../Scripts/durandal',
        'plugins': '../Scripts/durandal/plugins',
        'transitions': '../Scripts/durandal/transitions',
        'knockout': '../Scripts/knockout-3.4.2',
        'bootstrap': '../Scripts/bootstrap.bundle',
        'jquery': '../Scripts/jquery-3.1.1',
        'moment': '../Scripts/moment',
        'schemas': 'entitydefinition/schemas'
    },
    shim: {
        'bootstrap': {
            deps: ['jquery'],
            exports: 'jQuery'
        }
    }
});

define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'bootstrap'], function (system, app, viewLocator) {
    //>>excludeStart("build", true);
    system.debug(true);
    //>>excludeEnd("build");

    app.title = 'Incidere';

    app.configurePlugins({
        router: true,
        dialog: true
    });

    app.start().then(function () {
        //Replace 'viewmodels' in the moduleId with 'views' to locate the view.
        //Look for partial views in a 'views' folder in the root.
        viewLocator.useConvention();

        //Show the app by setting the root view model for our application with a transition.
        app.setRoot('viewmodels/shell', 'entrance');
    });
});

define('services/datacontext', [], function () {

    function send(json, url, verb) {
        var tcs = new $.Deferred();
        $.ajax({
            type: verb,
            data: json,
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: tcs.reject,
            success: tcs.resolve
        });
        return tcs.promise();
    }

    function get(url, cache, headers) {
        var tcs = new $.Deferred();
        $.ajax({
            type: "GET",
            cache: cache,
            headers: headers,
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: tcs.reject,
            success: tcs.resolve
        });
        return tcs.promise();
    }

    function post(json, url, headers) {
        var tcs = new $.Deferred();
        $.ajax({
            type: "POST",
            data: json,
            headers: headers,
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: tcs.reject,
            success: tcs.resolve
        });
        return tcs.promise();
    }

    function put(json, url, headers) {
        var tcs = new $.Deferred();
        $.ajax({
            type: "PUT",
            data: json,
            headers: headers,
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: tcs.reject,
            success: tcs.resolve
        });
        return tcs.promise();
    }

    function patch(json, url, headers) {
        var tcs = new $.Deferred();
        $.ajax({
            type: "PATCH",
            data: json,
            headers: headers,
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: tcs.reject,
            success: tcs.resolve
        });
        return tcs.promise();
    }

    function remove(url) {
        var tcs = new $.Deferred();
        $.ajax({
            type: "DELETE",
            data: "{}",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            error: tcs.reject,
            success: tcs.resolve
        });
        return tcs.promise();
    }

    return {
        send: send,
        get: get,
        post: post,
        put: put,
        patch: patch,
        remove: remove
    };
});
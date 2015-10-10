/// <binding AfterBuild='build' ProjectOpened='watch' />
'use strict';

var browserify = require('browserify');
var fs = require('fs');
var gulp = require('gulp');
var rimraf = require('rimraf');
var watchify = require('watchify');

function definition(watch) {
    var def = browserify(watchify.args);
    if (watch) {
        def = watchify(def);
        def.on('update', function () {
            bundle(def, watch);
        }).on('log', function (log) {
            console.log(log.toString());
        });
    }

    def
        .add('app.tsx')
        .plugin('tsify', { noImplicitAny: true });

    return bundle(def, watch);
}

function bundle(def, watch) {
    return def.bundle()
        .on('error', function (error) {
            console.error(error.toString());
            if (!watch) {
                throw error;
            }
        })
        .pipe(fs.createWriteStream('./bundle.js'));
}

gulp.task('build', function () {
    return definition(false);
});

gulp.task('watch', function (cb) {
    definition(true);
});

gulp.task('clean', function (cb) {
    rimraf('./dist', cb);
});

gulp.task('deploy', ['build'], function () {
    return gulp.src([
        '*.html',
        'bundle.js',
        'css/*.css'
    ], {
        base: '.'
    }).pipe(gulp.dest('./dist'));
});

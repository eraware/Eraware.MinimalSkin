const { 
    dest,
    parallel,
    series,
    src
} = require('gulp');

/**
 * Cleans the generated files
 * @param cb {any} any
 * 
 */
function clean(cb){
    cb();
}

/**
 * Transpile, minify and combine script
 * @param cb 
 */
function styles(cb){
    cb();

}

/**
 * Transpile, minify and combine script
 * @param cb 
 */
function scripts(cb){
    cb();
}

/**
 * Optimizes images
 * @param cb 
 */
function images(cb){
    cb();
}

/**
 * Generates or updates the Dnn manifest
 */
function manifest(cb) {
    cb();
}

/**
 * Packages the theme for distribution
 * @param cb
 */
function package(cb){
    cb();
}

exports.default = series(
    clean,
    parallel(styles, scripts, images, manifest),
    package,
    clean
);

// Notes
// clean -> (js, css, manifest, images) -> package -> clean
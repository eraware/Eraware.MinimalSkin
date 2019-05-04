import { ThemeSettings } from './theme-settings';
import * as gulp from 'gulp';
const rename = require('gulp-rename');
const cheerio = require('gulp-cheerio');

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
function manifest() {
    const themeSettings = new ThemeSettings();
    return gulp.src('./manifest.xml')
    .pipe(
        cheerio( 
            {
                run: function ($, file, done) {
                    const pack = $('packages package');
                    pack.attr('name', themeSettings.packageName);
                    pack.attr('version', themeSettings.version);
                    $('friendlyName', pack).text(themeSettings.friendlyName);
                    $('description', pack).text(themeSettings.description);
                    const owner = $('owner', pack);
                    $('name', owner).text(themeSettings.ownerName);
                    $('organization', owner).text(themeSettings.ownerOrganization);
                    $('url', owner).text(themeSettings.ownerUrl);
                    $('email', owner).text(themeSettings.ownerEmail);
                    const components = $('components', pack);
                    const skinFiles = $('component[type="Skin"] skinFiles', components);
                    $('skinName', skinFiles).text(themeSettings.friendlyName);
                    $('basePath', skinFiles).text(themeSettings.skinpath);
                    const skinComponent = $('component[type="ResourceFile"]')[0];
                    $('resourceFiles basePath', skinComponent).text(themeSettings.skinpath);
                    const containersComponent = $('component[type="ResourceFile"]')[1];
                    $('resourceFiles basePath', containersComponent).text(themeSettings.containersPath);
                    done();
                },
                parserOptions: {
                    xmlMode: true,
                    lowerCaseTags: false,
                    decodeEntities: false
                }
            }
        )
    )        
    .pipe(rename('manifest.dnn'))
    .pipe(gulp.dest('./dist'));
}

/**
 * Packages the theme for distribution
 * @param cb
 */
function packageModule(cb){
    cb();
}

function watch() {
    gulp.watch('gulpfile.ts', manifest);
}

exports.watch = watch;
exports.default = gulp.series(
    clean,
    gulp.parallel(styles, scripts, images, manifest),
    packageModule,
    clean
);
import { ThemeSettings } from './theme-settings';
import * as gulp from 'gulp';
import del from 'del';
import * as debug from 'gulp-debug';
import * as sourcemaps from 'gulp-sourcemaps';
import * as cleanCSS from 'gulp-clean-css';
const rename = require('gulp-rename');
const cheerio = require('gulp-cheerio');
const sass = require('gulp-sass');
const browserSync = require('browser-sync').create();

const themeSettings = new ThemeSettings();

/**
 * Cleans the generated files
 * @param cb {any} any
 * 
 */
function clean(){
    return del([
        `../Skins/${themeSettings.packageName}/**/*`,
        `../Containers/${themeSettings.packageName}/**/*`
    ], {
        force: true
    });
}

/**
 * Transpile, minify and combine script
 * @param cb 
 */
function styles(){
    return gulp.src('./src/styles/**/*.scss')
    .pipe(sourcemaps.init())
    .pipe(sass({
            outputStyle: 'compressed'
        }).on('error', sass.logError))
    .pipe(cleanCSS({level: 2}))
    .pipe(rename('theme.min.css'))
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest(`../Skins/${themeSettings.packageName}/css`))
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
    .pipe(gulp.dest(`./../Skins/${themeSettings.packageName}`));
}

/**
 * Packages the theme for distribution
 * @param cb
 */
function packageModule(cb){
    cb();
}

/**
 * Copies the html templates (ascx)
 */
function html(){
    return gulp.src(['src/html/**/*.ascx', '!node_modules'])
    .pipe(gulp.dest(`../Skins/${themeSettings.packageName}`, {overwrite: true}));
}

function watch() {
    browserSync.init({
        proxy: "http://dnn932clean.localtest.me/"
    });
    gulp.watch('theme-settings.ts', manifest);
    gulp.watch('./src/html/**/*.ascx', html);
    gulp.watch('./src/styles/**/*.scss', styles);
    gulp.watch('./**/*').on("change", browserSync.reload);
}

exports.default = gulp.series(
    clean,
    gulp.parallel(html, styles, scripts, images, manifest),
    packageModule
    );
exports.watch = watch;
exports.cleanup = clean;
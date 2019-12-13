import { ThemeSettings } from './theme-settings';
import * as gulp from 'gulp';
import * as debug from 'gulp-debug';
import * as sourcemaps from 'gulp-sourcemaps';
import * as cleanCSS from 'gulp-clean-css';
import * as ts from 'gulp-typescript';
import * as concat from 'gulp-concat';
import * as uglify from 'gulp-uglify';
import * as prompt from 'inquirer';
import * as imagemin from 'gulp-imagemin';
import * as merge from 'merge2';
import * as zip from 'gulp-zip';
import * as replace from 'gulp-replace';
import * as color from 'gulp-color';
import * as gulpif from 'gulp-if';
const rename = require('gulp-rename');
const del = require('del');
const cheerio = require('gulp-cheerio');
const sass = require('gulp-sass');
const browserSync = require('browser-sync').create();
const gitVersion = require('git-tag-version');

const themeSettings = new ThemeSettings();
var semVer = {
    fullSemVer: "",
    major: 0,
    minor: 0,
    patch: 0,
    majorMinorPatch: ""
};

function clean(){
    return del([
        `../Skins/${themeSettings.packageName}/**/*`,
        `../Containers/${themeSettings.packageName}/**/*`
    ], {
        force: true
    });
}

function version(cb){
    semVer.fullSemVer = gitVersion({
        uniqueSnapshot: true
    });
    semVer.major = Number.parseInt(semVer.fullSemVer.split('.')[0]);
    semVer.minor = Number.parseInt(semVer.fullSemVer.split('.')[1]);
    semVer.patch = Number.parseInt(semVer.fullSemVer.split('.')[2]);
    semVer.majorMinorPatch = `${semVer.major}.${semVer.minor}.${semVer.patch}`;
    console.log(semVer);
    cb();
}

function styles(){
    
    // setup required files according to options
    let files = [];
    files.push('./src/styles/main.scss');
    if (themeSettings.useBootstrap){
        files.push('./src/styles/bootstrap/bootstrap.scss');
    }
    if (themeSettings.useFontAwesome){
        files.push('./src/styles/fontawesome/fontawesome.scss');
    }

    return gulp.src(files)
    .pipe(debug({title: 'styles:'}))
    .pipe(sourcemaps.init())
    .pipe(sass({
        outputStyle: 'compressed'
    }).on('error', sass.logError))
    .pipe(concat('theme.min.css'))
    .pipe(cleanCSS({level: 2}))
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest(`../Skins/${themeSettings.packageName}/css`))
}

/**
 * Transpile, minify and combine script
 * @param cb 
 */
function scripts(){

    let files = [];
    if (themeSettings.useBootstrap){
        files.push('./node_modules/bootstrap/dist/js/bootstrap.bundle.js');
        files.push('./src/scripts/bootstrap/bootstrap.ts');
    }
    files.push('./src/scripts/main.ts');

    return gulp.src(files)
    .pipe(sourcemaps.init())
    .pipe(ts({
        module: "commonjs", 
        target: "es5", 
        allowJs: true, 
        noImplicitAny: true, 
        moduleResolution: "node"
    }))
    .pipe(concat('skin.min.js'))
    .pipe(uglify())
    .pipe(sourcemaps.write('./'))
    .pipe(gulp.dest(`../Skins/${themeSettings.packageName}/js`));
}

/**
 * Optimizes images
 */
function images(){
    return gulp.src('./src/images/*')
    .pipe(imagemin())
    .pipe(gulp.dest(`../Skins/${themeSettings.packageName}/Images/`));
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
                    // Cedit package info
                    const pack = $('packages package');
                    pack.attr('name', themeSettings.packageName);
                    pack.attr('version', semVer.majorMinorPatch);
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
function packageTheme(){
    return merge(
        packageSkin(),
        packageContainers(),
        gulp.src(
            [
                `../Skins/${themeSettings.packageName}/manifest.dnn`,
                './releaseNotes.txt',
                'LICENSE'
            ]
        )
    )
    .pipe(zip(`${themeSettings.packageName}_${semVer.fullSemVer}_install.zip`))
    .pipe(gulp.dest('./install'));
}

function packageSkin() {
    return gulp.src(`../Skins/${themeSettings.packageName}/**/*`)
    .pipe(zip('./skinResources.zip'));
}

function packageContainers() {
    return gulp.src(`../Containers/${themeSettings.packageName}/**/*`)
    .pipe(zip('./containersResources.zip'));
}

/**
 * Copies the html templates (ascx)
 */
function html(){
    return gulp.src('src/html/**/*.ascx')
    .pipe(gulp.dest(`../Skins/${themeSettings.packageName}`, {overwrite: true}));
}

/**
 * Copies containers html templates (ascx)
 */
function containersHtml(){
    return gulp.src('src/containers/**/*.ascx')
    .pipe(gulp.dest(`../Containers/${themeSettings.packageName}`, {overwrite: true}));
}

function menu(){
    return gulp.src('src/html/menus/**/*')
    .pipe(gulp.dest(`../Skins/${themeSettings.packageName}/menus`, {overwrite: true}));
}

function fonts(){
    return gulp.src('node_modules/@fortawesome/fontawesome-free/webfonts/*')
    .pipe(gulpif(themeSettings.useFontAwesome, 
        gulp.dest(`../Skins/${themeSettings.packageName}/webfonts`)
    ));
}

function doctype(){
    return gulp.src('./skin.doctype.xml')
    .pipe(gulp.dest(`../Skins/${themeSettings.packageName}`));
}

function watch() {
    const questions = [
        {
            type: 'input',
            name: 'url',
            question: 'What is the url of your dev site?',
            default: 'http://dnndev.localtest.me'
        }
    ];

    return prompt.prompt(questions).then(answer => {
        browserSync.init({
            proxy: answer.url,
            reloadDelay: 1000
        });
        gulp.watch('./theme-settings.ts', manifest);
        gulp.watch('./src/html/**/*.ascx', html);
        gulp.watch('./src/container/**/*.ascx', containersHtml);
        gulp.watch('./src/html/menus/**/*', menu);
        gulp.watch('./src/styles/**/*.scss', styles);
        gulp.watch('./src/scripts/*.ts', scripts);
        gulp.watch('./src/images/**/*', images);
        gulp.watch('./**/*').on("change", browserSync.reload);
    });
}

function config() {
    const questions = [
        {
            type: 'input',
            name: 'packageName',
            message: 'What package name would you like to use (should be unique and no special characters)?',
            default: themeSettings.packageName
        },
        {
            type: 'input',
            name: 'friendlyName',
            message: 'What friendly name would you like (shown to users)?',
            default: themeSettings.friendlyName
        },
        {
            type: 'input',
            name: 'ownerName',
            message: 'What is your name?',
            default: themeSettings.ownerName
        },
        {
            type: 'input',
            name: 'ownerOrganization',
            message: 'What is your organization name',
            default: themeSettings.ownerOrganization
        },
        {
            type: 'input',
            name: 'ownerUrl',
            message: 'Which website can people visit for information about this theme?',
            default: themeSettings.ownerUrl
        },
        {
            type: 'input',
            name: 'ownerEmail',
            message: 'What is the email people can contact you about this theme?',
            default: themeSettings.ownerEmail
        },
        {
            type: 'checkbox',
            name: 'options',
            message: 'What would you like to include in this theme?',
            choices: [
                {
                    name: 'bootstrap 4',
                    value: 'bs4',
                    checked: themeSettings.useBootstrap
                },
                {
                    name: 'fontawesome 5',
                    value: 'fa5',
                    checked: themeSettings.useFontAwesome
                }
            ]
        }
    ];

    return prompt.prompt(questions).then(answers => {
        gulp.src('theme-settings.ts')
        .pipe(replace(/this\.packageName = "(.*)";/, `this.packageName = "${answers.packageName}";`))
        .pipe(replace(/this\.friendlyName = "(.*)";/, `this.friendlyName = "${answers.friendlyName}";`))
        .pipe(replace(/this\.ownerName = "(.*)";/, `this.ownerName = "${answers.ownerName}";`))
        .pipe(replace(/this\.ownerOrganization = "(.*)";/, `this.ownerOrganization = "${answers.ownerOrganization}";`))
        .pipe(replace(/this\.ownerUrl = "(.*)";/, `this.ownerUrl = "${answers.ownerUrl}";`))
        .pipe(replace(/this\.onwerEmail = "(.*)";/, `this.ownerEmail = "${answers.ownerEmail}";`))
        .pipe(gulpif(answers.options.includes("bs4"), 
            replace(/this\.useBootstrap = (.*);/, `this.useBootstrap = true;`),
            replace(/this\.useBootstrap = (.*);/, `this.useBootstrap = false;`)
        ))
        .pipe(gulpif(answers.options.includes("fa5"),
            replace(/this\.useFontAwesome = (.*);/, `this.useFontAwesome = true;`),
            replace(/this\.useFontAwesome = (.*);/, `this.useFontAwesome = false;`)
        ))
        .pipe(gulp.dest('./'))
        .on('end', () =>{
            console.log(color('You are all set !', 'GREEN'));
            console.log(color('Further customizations can be done in the theme-settings.ts file.', 'CYAN'));
        });
    });
}

exports.default = gulp.series(
    clean,
    version,
    gulp.parallel(html, containersHtml, menu, styles, scripts, images, manifest, fonts, doctype),
    packageTheme
    );
exports.watch = watch;
exports.cleanup = clean;
exports.config = config;
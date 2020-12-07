import { ThemeSettings } from './theme-settings';
import * as gulp from 'gulp';
const addSrc = require('gulp-add-src');
const browserify = require('browserify');
const browserSync = require('browser-sync').create();
const buffer = require('vinyl-buffer');
const cheerio = require('gulp-cheerio');
const cleanCSS = require('gulp-clean-css');
const color = require('gulp-color');
const concat = require('gulp-concat');
const debug = require('gulp-debug');
const del = require('del');
const gitVersion = require('git-tag-version');
const gulpif = require('gulp-if');
const gulpOrder = require('gulp-order');
const imagemin = require('gulp-imagemin');
const merge = require('merge2');
const prompt = require('inquirer');
const rename = require('gulp-rename');
const replace = require('gulp-replace');
const sass = require('gulp-sass');
const source = require('vinyl-source-stream');
const sourcemaps = require('gulp-sourcemaps');
const tsify = require('tsify');
const uglify = require('gulp-uglify');
const zip = require('gulp-zip');

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
        './install/**/*',
        `../Skins/${themeSettings.packageName}/**/*`,
        `../Containers/${themeSettings.packageName}/**/*`
    ], {
        force: true
    });
}

function version(cb: any){
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
    if (themeSettings.useBootstrap == 'grid'){
        files.push('./node_modules/bootstrap/scss/bootstrap-grid.scss');
        files.push('./src/styles/bootstrap/bootstrap.scss');
    }
    if (themeSettings.useBootstrap == 'all'){
        files.push('./node_modules/bootstrap/scss/bootstrap.scss');
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
    return browserify({
        basedir: '.',
        debug: true,
        entries: ['./src/scripts/main.ts'],
        cache: {},
        packageCache: {}
    })
    .plugin(tsify)
    .transform('babelify', {
        presets: ['env'],
        extensions: ['.ts']
    })
    .bundle()
    .pipe(source('skin.js'))
    .pipe(buffer())
    .pipe(gulpif(themeSettings.useBootstrap === 'all', addSrc.prepend('./src/scripts/bootstrap/bootstrap.js')))
    .pipe(gulpif(themeSettings.useBootstrap === 'all', addSrc.prepend('./node_modules/bootstrap/dist/js/bootstrap.bundle.js')))
    .pipe(gulpOrder([
        "bootstrap.bundle.js",
        "bootstrap.ts",
        "skin.js"
    ]))
    .pipe(concat('skin.min.js'))
    .pipe(sourcemaps.init({loadMaps: true}))
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
                run: function ($: any, _file: any, done: any) {
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
            default: themeSettings.testSiteUrl
        }
    ];

    return prompt.prompt(questions).then((answer: any) => {
        gulp.src('theme-settings.ts')
        .pipe(replace(/this\.testSiteUrl = "(.*)";/, `this.testSiteUrl = "${answer.url}";`))
        .pipe(gulp.dest('./'));

        browserSync.init({
            proxy: {
                target: answer.url,
                proxyRes: [
                    function(_proxyRes: any, _req: any, res: any){
                        res.setHeader('Access-Control-Allow-Origin', '*');
                    }
                ]
            },
            reloadDelay: 1000,
            cors: true
        });
        gulp.watch('./theme-settings.ts', manifest);
        gulp.watch('./src/html/**/*.ascx', html);
        gulp.watch('./src/container/**/*.ascx', containersHtml);
        gulp.watch('./src/html/menus/**/*', menu);
        gulp.watch('./src/styles/**/*.scss', styles);
        gulp.watch('./src/scripts/**/*.ts', scripts);
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
            type: 'list',
            name: 'bootstrap',
            message: 'Would you like to use bootstrap?',
            choices: [
                {
                    name: 'no',
                    value: 'no',
                    checked: themeSettings.useBootstrap === 'no'
                },
                {
                    name: 'grid and responsive utilities only (adds 44Kb)',
                    value: 'grid',
                    checked: themeSettings.useBootstrap === 'grid'
                },
                {
                    name: 'all of bootstrap (adds 221Kb)',
                    value: 'all',
                    checked: themeSettings.useBootstrap === 'all'
                }
            ]
        },
        {
            type: 'checkbox',
            name: 'icons',
            message: 'Would you like to include an icon font?',
            choices: [
                {
                    name: 'fontawesome 5 (adds 206Kb)',
                    value: 'fa5',
                    checked: themeSettings.useFontAwesome
                }
            ]
        }
    ];

    return prompt.prompt(questions).then((answers: any) => {
        gulp.src('theme-settings.ts')
        .pipe(replace(/this\.packageName = "(.*)";/, `this.packageName = "${answers.packageName}";`))
        .pipe(replace(/this\.friendlyName = "(.*)";/, `this.friendlyName = "${answers.friendlyName}";`))
        .pipe(replace(/this\.ownerName = "(.*)";/, `this.ownerName = "${answers.ownerName}";`))
        .pipe(replace(/this\.ownerOrganization = "(.*)";/, `this.ownerOrganization = "${answers.ownerOrganization}";`))
        .pipe(replace(/this\.ownerUrl = "(.*)";/, `this.ownerUrl = "${answers.ownerUrl}";`))
        .pipe(replace(/this\.onwerEmail = "(.*)";/, `this.ownerEmail = "${answers.ownerEmail}";`))
        .pipe(gulpif(answers.bootstrap.includes('no'),
            replace(/this.useBootstrap = (.*);/, `this.useBootstrap = 'no';`)
        ))
        .pipe(gulpif(answers.bootstrap.includes('grid'),
            replace(/this.useBootstrap = (.*);/, `this.useBootstrap = 'grid';`)
        ))
        .pipe(gulpif(answers.bootstrap.includes('all'),
            replace(/this.useBootstrap = (.*);/, `this.useBootstrap = 'all';`)
        ))
        .pipe(gulpif(answers.icons.includes("fa5"),
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
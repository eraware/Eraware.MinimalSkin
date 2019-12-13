## Minimal Dnn theme

### Getting started
1. **Important**: This is a template repository, if your intention is to contribute to it, please use the `Fork` button. If your intention is to build a theme using this as a template, please use the `Use this template` button.
2. Clone the repository to the `Portals\_default` folder of a working Dnn site.
3. Navigate to the created folder.
4. You will need `yarn` if you do not have it installed yet, please visit [the yarn website](https://yarnpkg.com/lang/en/).
5. In the command line, run `yarn install` to fetch the dependencies.

### Customizing your theme
To cusomize your theme to your needs, run `yarn settings` and answer a couple of questions.
Note: *The base theme is not yet updated to work without bootstrap, comming soon. But for now, please include bootstrap.*

### Building
In order for the theme to work in Dnn, it needs to be installed one first time.

1. Run `yarn build` (*You can skip this step if you just ran `yarn settings` since it does it automatically after each setting change*).
2. Navigate to the generated install folder and install the .zip file as any other Dnn extension.

### Development
The build scripts have a live-server proxy than will automatically rebuild upon any file change and reload your browser. This is really useful for fast development but has limitations such as not being able to login using the proxy.

1. Run `yarn start`.
2. When asked, enter the url of the site you would normally visit.
3. That site will be proxied as http://localhost:3333 and will live-reload unpon any file change.

If you run into cache issues and do not see your changes right away, disable Dnn cache, bundling and minification settings and keep your development panel open. In order to give Dnn time to realize changes, we need to inject a 1 second delay to the reload. If you need more or less delay, it can be adjusted in the `reloadDelay` option of the `browserSync` options. BrowserSync also allows you to view the site in your local network on multiple devices at once and will reload them all! All other BrowserSync options are supported, read [BrowerSync Documentation](https://www.browsersync.io/docs/options) for more details.

### Versioning
This project uses (GitFlow)[https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow] which means official releases live on the `master` branch and are tagged as in `v1.2.3`, it also uses semantic versioning, so the first digit represents a major rewrite that probably has breaking changes, the middle digit represents a minor change or new feature (may also include some bug fixes) and the last digit represents a bug fix only. The build script will automatically generate a version number based on some branches and tags. If a commit is tagged it will alsways use that tag, if not it will try to generate a meaningful tag for you depending on the branch you are on currently. If you need to explicitely set the version of your theme, you simple need to tag your commit by running `git tag v2.3.4` or any other version you want, then build.
# Minimal Dnn theme

## Getting started
1. **Important**: This is a template repository, if your intention is to contribute to it, please use the `Fork` button. If your intention is to build a theme using this as a template, please use the `Use this template` button.
2. Clone the repository to the `Portals\_default` folder of a working Dnn site.
3. Navigate to the created folder.
4. You will need `npm` if you do not have it installed yet, please visit [the node website](https://nodejs.org/en/download/).
5. In the command line, run `npm install` to fetch the dependencies.

## Customizing your theme
To cusomize your theme to your needs, run `npm settings` and answer a couple of questions.

## Development
The build scripts have a live-server proxy than will automatically rebuild upon any file change and reload your browser. This is really useful for fast development but has limitations such as not being able to use the persona bar using the proxy.

1. Run `npm watch`.
2. When asked, enter the url of the site you would normally visit.
3. That site will be proxied as http://localhost:3000 and will live-reload unpon any file change.

If you run into cache issues and do not see your changes right away, disable Dnn cache, bundling and minification settings and keep your development panel open. In order to give Dnn time to realize changes, we need to inject a 1 second delay to the reload. If you need more or less delay, it can be adjusted in the `reloadDelay` option of the `browserSync` options. BrowserSync also allows you to view the site in your local network on multiple devices at once and will reload them all! All other BrowserSync options are supported, read [BrowerSync Documentation](https://www.browsersync.io/docs/options) for more details.

## Versioning
This project uses GitVersion to automatically update versions using the [Gitflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) release management workflow. In summary, the `develop` branch is the one where development happens most of the time for the next minor release. Creating a `release/x.x.x` branch will trigger a beta build and create an unpublished pre-release with version `x.x.x`. Merging the `release/x/x/x` branch into the `main` branch will trigger another build that will create an official (non-beta) release for version `x.x.x`. For each of those releases, the release notes will be automatically generated from pull requests that were merged with a milestone of the same version. Those release notes are also groupped by labels used.

## Javascript utilities
In the `scr/scripts/utilities` you will find some useful utilities you can opt into, right now those utilities are:
* `replaceClasses` which takes a list of old>new classes, replace them in content and optionally log that replacement in the browser console.
* There is also a `migrateFa4ToFa5` function that replaces all deprecated FontAwesome4 classes to their best lookalike in FontAwesome5 using the previous `replaceClasses` function. To opt-in into this feature, add the following snipped in the `main.ts` file:
```ts
import { migrateFa4ToFa5 } from './utilities/utils';

document.addEventListener('DOMContentLoaded', () => {
    migrateFa4ToFa5();
});
```
import themeSettings from "./theme-settings.json";
import browsersync from "./rollup-plugin-browsersync"
import * as prodConfig from "./rollup.config.prod";

const devConfig = {
    ...prodConfig.default,
    plugins: [
        ...prodConfig.default.plugins,
        browsersync({
            proxy: themeSettings.TestSiteUrl,
            files: [
                `../../../${themeSettings.SkinPath}/**/*`,
                `../../../${themeSettings.ContainersPath}/**/*`,
            ],
        }),
    ]
};

export default devConfig;
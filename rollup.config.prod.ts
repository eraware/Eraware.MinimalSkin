import typescript from '@rollup/plugin-typescript';
import { terser } from "rollup-plugin-terser";
import sourcemaps from "rollup-plugin-sourcemaps";
import scss from 'rollup-plugin-scss';
import themeSettings from "./theme-settings.json";
import copy from 'rollup-plugin-copy';
import cleaner from 'rollup-plugin-cleaner';

export default {
    input: 'src/scripts/main.ts',
    output: {
        file: `../../../${themeSettings.SkinPath}/js/skin.min.js`,
        format: 'iife',
        sourcemap: true,
    },
    plugins: [
        typescript({
            sourceMap: true,
            inlineSources: true,
        }),
        sourcemaps(),
        terser(),
        scss({
            output: `../../../${themeSettings.SkinPath}/css/theme.min.css`,
            sourceMap: true,
            failOnError: true,
        }),
        cleaner({
            targets: [
                `../../../${themeSettings.SkinPath}`,
                `../../../${themeSettings.ContainersPath}`,
            ],
        }),
        copy({
            targets: [
                {
                    src: ["html/**/*.ascx", "html/**/*.xml", "html/**/*.cshtml"],
                    dest: `../../../${themeSettings.SkinPath}`,
                },
                {
                    src: ["containers/**/*.ascx"],
                    dest: `../../../${themeSettings.ContainersPath}`,
                },
                {
                    src: ["images/**/*"],
                    dest: `../../../${themeSettings.SkinPath}/images`,
                },
            ],
            flatten: false,
            verbose: true,
        }),
    ],
};
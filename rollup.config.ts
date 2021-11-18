import { defineConfig } from 'rollup';
import typescript from '@rollup/plugin-typescript';
import { terser } from "rollup-plugin-terser";
import sourcemaps from "rollup-plugin-sourcemaps";
import scss from 'rollup-plugin-scss';

export default defineConfig([{
    input: 'src/scripts/main.ts',
    output: {
        file: 'dist/skin/js/skin.min.js',
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
            output: 'dist/skin/css/theme.min.css',
            sourceMap: true,
            failOnError: true,
        }),
    ],
},]);
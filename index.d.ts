declare module 'rollup-plugin-cleaner' {
    interface CleanerOptions{
        targets?: string[];
        silent?: boolean;
    }

    function cleaner(options: CleanerOptions): any;
    
    export = cleaner;
}

declare module 'rollup-plugin-browsersync' {
    interface BrowserSyncOptions{
        proxy: string,
        files: string[],
        inject: boolean,
    }

    function browsersync(options: BrowserSyncOptions): any;

    export = browsersync;
}

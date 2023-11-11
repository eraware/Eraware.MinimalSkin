import bs from "browser-sync";
const browserSync = bs.create("rollup");

export function browsersync(browsersyncOptions: bs.Options){
    return {
        name: "browsersync",
        writeBundle: function(options: any){
            if (!browserSync.active){
                browserSync.init(browsersyncOptions || {server: "."});
            } else {
                browserSync.reload(options.file);
            }
        }
    }
}
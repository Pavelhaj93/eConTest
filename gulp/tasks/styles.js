import gulp from 'gulp';
import { argv } from 'yargs';
import gulpif from 'gulp-if';
import rename from 'gulp-rename';
import sourcemaps from 'gulp-sourcemaps';
import sass from 'gulp-sass';
import postcss from 'gulp-postcss';
import autoprefixer from 'autoprefixer';
import cssnano from 'cssnano';
import cssGlobbing from 'gulp-css-globbing';
import browserSync from 'browser-sync';
import config from '../config';
import inlineSVG from 'postcss-inline-svg';

const { src, dist } = config.paths;
const isDev = argv.dev || false;

gulp.task('styles', () => {
    const browsers = ['last 2 versions', 'iOS 7'];

    const postcssPlugins = {
        dev: [
            autoprefixer({ browsers }),
            inlineSVG()
        ],
        dist: [
            cssnano({
                add: true,
                browsers,
                safe: true
            })
        ]
    };

    return gulp.src(src.styles.entry)
        .pipe(cssGlobbing({ extensions: ['.css', '.scss'] }))
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(postcss(postcssPlugins.dev))
        .pipe(gulpif(isDev, sourcemaps.write()))
        .pipe(gulp.dest(isDev ? src.styles.dest : dist.css))
        .pipe(gulpif(isDev, browserSync.stream()))
        .pipe(gulpif(!isDev, postcss(postcssPlugins.dist)))
        .pipe(gulpif(!isDev, rename(path => path.basename += '.min')))
        .pipe(gulpif(!isDev, gulp.dest(dist.css)));
});

import gulp from 'gulp';
import config from '../config';

const { src, dist } = config.paths;

gulp.task('fonts', () => {
	gulp.src(src.base + '/gfx/favicon/**/*')
		.pipe(gulp.dest(dist.base + '/gfx/favicon'));

	return gulp.src(src.base + '/fonts/**/*')
		.pipe(gulp.dest(dist.base + '/fonts'));
});
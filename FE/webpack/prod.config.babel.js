import MiniCssExtractPlugin from 'mini-css-extract-plugin'
import WebpackShellPlugin from 'webpack-shell-plugin'
import CopyPlugin from 'copy-webpack-plugin'
import ReplaceInFilePlugin from 'replace-in-file-webpack-plugin'
import merge from 'webpack-merge'
import baseConfig from './base.config.babel'
import path from 'path'

const productionPolyfillsPath = '/Assets/eContracting2/js/polyfills.js'

// We need to modify path to polyfills and fonts in generated files.
// eslint-disable-next-line @typescript-eslint/explicit-function-return-type
const replaceProductionPaths = () => {
  return [0].map(() => {
    return new ReplaceInFilePlugin([
      {
        dir: './build/js/',
        test: /\.js$/,
        rules: [
          {
            search: /src="\/js\/polyfills\.js"/g,
            replace: `src="${productionPolyfillsPath}"`,
          },
        ],
      },
      {
        dir: './build/css/',
        test: /\.css$/,
        rules: [
          {
            search: /url\(\/fonts\//g,
            replace: 'url(../fonts/',
          },
        ],
      },
    ])
  })
}

export default merge(baseConfig, {
  mode: 'production',
  devtool: false,

  module: {
    rules: [
      {
        test: /\.scss$/,
        use: [
          {
            loader: MiniCssExtractPlugin.loader,
          },
          {
            loader: 'css-loader',
            options: {
              url: true,
            },
          },
          {
            loader: 'postcss-loader',
            options: {
              ident: 'postcss',
              plugins: () => [require('postcss-inline-svg')(), require('autoprefixer')()],
            },
          },
          {
            loader: 'sass-loader',
            options: {
              sassOptions: {
                outputStyle: 'compressed',
                precision: 9,
                includePaths: [path.resolve(__dirname, './node_modules/')],
              },
            },
          },
        ],
      },
    ],
  },

  plugins: [
    // copy favicon images to build directory
    new CopyPlugin({
      patterns: [
        {
          from: './favicon/**/*',
          to: './gfx/',
          context: './src/icons/',
        },
      ],
    }),
    // copy build files to assets directory for BE
    new WebpackShellPlugin({
      onBuildEnd: ['node ./scripts/copyBuildFiles.js'],
    }),
  ].concat(replaceProductionPaths()),
})

import MiniCssExtractPlugin from 'mini-css-extract-plugin'
import WebpackShellPlugin from 'webpack-shell-plugin'
import CopyPlugin from 'copy-webpack-plugin'
import ReplaceInFilePlugin from 'replace-in-file-webpack-plugin'
import merge from 'webpack-merge'
import baseConfig from './base.config.babel'
import path from 'path'
import { removeDataTestIdTransformer } from 'typescript-transformer-jsx-remove-data-test-id'

const productionPolyfillsPath = '/Assets/eContracting2/js/polyfills.js'

export default merge(baseConfig, {
  mode: 'production',
  devtool: false,

  module: {
    rules: [
      {
        test: /\.(j|t)sx?$/,
        loader: 'awesome-typescript-loader',
        options: {
          getCustomTransformers: () => ({
            before: [removeDataTestIdTransformer()],
          }),
        },
        exclude: [/node_modules/, /build/],
      },
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
        {
          from: './mocks/**/*',
          to: './',
          context: './src/',
        }
      ],
    }),

    // we need to modify path to polyfills and fonts in generated files
    new ReplaceInFilePlugin([
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
    ]),

    // copy build files to assets directory for BE
    new WebpackShellPlugin({
      onBuildExit: ['node ./scripts/copyBuildFiles.js'],
    }),
  ],
})

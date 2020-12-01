import HtmlWebpackPlugin from 'html-webpack-plugin'
import { CleanWebpackPlugin } from 'clean-webpack-plugin'
import MiniCssExtractPlugin from 'mini-css-extract-plugin'
import TimeFixPlugin from 'time-fix-plugin'
import PolyfillInjectorPlugin from 'webpack-polyfill-injector'
import SpriteLoaderPlugin from 'svg-sprite-loader/plugin'
import webpack from 'webpack'
import path from 'path'
import fs from 'fs'
import packageJson from '../package.json'

// eslint-disable-next-line @typescript-eslint/explicit-function-return-type
const generateHtmlFiles = dir => {
  const dirents = fs.readdirSync(path.resolve(__dirname, dir), { withFileTypes: true })

  // filter out directories
  return dirents
    .filter(dirent => dirent.isFile())
    .map(dirent => {
      return new HtmlWebpackPlugin({
        template: `./src/tpl/${dirent.name}`,
        filename: `./${dirent.name.replace('.handlebars', '.html')}`,
        hash: true,
      })
    })
}

export default {
  entry: [
    `webpack-polyfill-injector?${JSON.stringify({
      modules: './src/app/index.tsx',
    })}!`,
    './src/styles/main.scss',
  ],
  output: {
    path: path.resolve(__dirname, '../build'),
    filename: `js/${packageJson.name}.bundle.js`,
    publicPath: '/',
  },

  resolve: {
    extensions: ['.ts', '.tsx', '.js', '.jsx'],
    alias: {
      '@components': path.resolve(__dirname, '../src/app/components'),
      '@hooks': path.resolve(__dirname, '../src/app/hooks'),
      '@types': path.resolve(__dirname, '../src/app/types'),
      '@utils': path.resolve(__dirname, '../src/app/utils'),
      '@icons': path.resolve(__dirname, '../src/icons/svg'),
      '@views': path.resolve(__dirname, '../src/app/views'),
      '@theme': path.resolve(__dirname, '../src/app/theme'),
      '@stores': path.resolve(__dirname, '../src/app/stores'),
      '@context': path.resolve(__dirname, '../src/app/context'),
    },
  },

  module: {
    rules: [
      {
        test: /\.handlebars$/,
        loader: 'handlebars-loader',
        exclude: [/node_modules/, /build/],
        query: {
          partialDirs: [path.resolve(__dirname, '../src/tpl/partials')],
          helperDirs: [path.resolve(__dirname, '../src/tpl/helpers')],
        },
      },
      {
        test: /\.woff2?$/,
        use: [
          {
            loader: 'file-loader',
            options: {
              name: '[name].[ext]',
              outputPath: 'fonts/',
            },
          },
        ],
      },
      {
        test: /\.svg$/,
        loader: 'svg-sprite-loader',
        options: {
          extract: true,
          spriteFilename: 'sprite.svg',
          outputPath: 'gfx/icon/',
        },
      },
    ],
  },

  plugins: [
    new webpack.ProgressPlugin(),

    new CleanWebpackPlugin(),

    new TimeFixPlugin(),

    new MiniCssExtractPlugin({
      filename: `css/${packageJson.name}.min.css`,
    }),

    new PolyfillInjectorPlugin({
      polyfills: [
        'Object.assign',
        'Array.from',
        'Array.prototype.find',
        'Array.prototype.findIndex',
        'Array.prototype.fill',
        'fetch',
        'Promise',
        'AbortController',
        'Number.isFinite',
        'Math.log10',
      ],
      singleFile: true,
      filename: 'js/polyfills.js',
    }),

    new SpriteLoaderPlugin({
      plainSprite: true,
    }),
  ].concat(generateHtmlFiles('../src/tpl')),
}

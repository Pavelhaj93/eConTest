import HtmlWebpackPlugin from 'html-webpack-plugin'
import { CleanWebpackPlugin } from 'clean-webpack-plugin'
import MiniCssExtractPlugin from 'mini-css-extract-plugin'
import TimeFixPlugin from 'time-fix-plugin'
import PolyfillInjectorPlugin from 'webpack-polyfill-injector'
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
      '@components': path.resolve(__dirname, 'src/components'),
      '@hooks': path.resolve(__dirname, 'src/hooks'),
      '@types': path.resolve(__dirname, 'src/types'),
      '@utils': path.resolve(__dirname, 'src/utils'),
      '@icons': path.resolve(__dirname, 'src/icons/svg'),
    },
  },

  module: {
    rules: [
      {
        test: /\.(j|t)sx?$/,
        loader: 'awesome-typescript-loader',
        exclude: [/node_modules/, /public/],
      },
      {
        test: /\.handlebars$/,
        loader: 'handlebars-loader',
        exclude: [/node_modules/, /public/],
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
        'fetch',
        'Promise',
      ],
      singleFile: true,
      filename: 'js/polyfills.js',
    }),
  ].concat(generateHtmlFiles('../src/tpl')),
}

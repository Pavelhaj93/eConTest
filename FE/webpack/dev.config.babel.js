import MiniCssExtractPlugin from 'mini-css-extract-plugin'
import merge from 'webpack-merge'
import baseConfig from './base.config.babel'
import path from 'path'
import apiMocker from 'connect-api-mocker'

export default merge(baseConfig, {
  mode: 'development',
  devtool: 'source-map',

  devServer: {
    host: '0.0.0.0',
    before: app => {
      app.use(apiMocker('/api', '/src/mocks/api'))
    },
  },

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
              sourceMap: true,
              url: true,
            },
          },
          {
            loader: 'postcss-loader',
            options: {
              ident: 'postcss',
              // 1. encode svg references as base64
              // 2. add browser prefixes
              plugins: () => [require('postcss-inline-svg')(), require('autoprefixer')()],
            },
          },
          {
            loader: 'sass-loader',
            options: {
              sassOptions: {
                outputStyle: 'expanded',
                precision: 9,
                lineNumbers: true,
                sourceMap: true,
                includePaths: [path.resolve(__dirname, './node_modules/')],
              },
            },
          },
        ],
      },
    ],
  },
})

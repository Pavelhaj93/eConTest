import MiniCssExtractPlugin from 'mini-css-extract-plugin'
import merge from 'webpack-merge'
import baseConfig from './base.config.babel'
import path from 'path'

export default merge(baseConfig, {
  mode: 'development',
  devtool: 'source-map',

  devServer: {
    host: '0.0.0.0',
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
              // eslint-disable-next-line @typescript-eslint/no-var-requires
              plugins: loader => [require('postcss-inline-svg')()], // encode svg references as base64
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

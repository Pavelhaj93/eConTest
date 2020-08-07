/* eslint-disable @typescript-eslint/no-var-requires */
const fs = require('fs')
const path = require('path')
const ncp = require('ncp')

const BUILD_SRC_DIR = '../build'
const STYLES_SRC_DIR = `${BUILD_SRC_DIR}/css`
const JS_SRC_DIR = `${BUILD_SRC_DIR}/js`
const FONTS_SRC_DIR = `${BUILD_SRC_DIR}/fonts`
const GFX_SRC_DIR = `${BUILD_SRC_DIR}/gfx`

const DEST_DIR = '../eContracting.Website/Assets/eContracting'
const STYLES_DEST_DIR = `${DEST_DIR}/css`
const JS_DEST_DIR = `${DEST_DIR}/js`
const FONTS_DEST_DIR = `${DEST_DIR}/fonts`
const GFX_DEST_DIR = `${DEST_DIR}/gfx`

const styles = fs.readdirSync(path.resolve(__dirname, STYLES_SRC_DIR))
const scripts = fs.readdirSync(path.resolve(__dirname, JS_SRC_DIR))
const fonts = fs.readdirSync(path.resolve(__dirname, FONTS_SRC_DIR))

// copy all css files
styles.forEach(file => {
  fs.copyFileSync(
    path.resolve(`./FE/${STYLES_SRC_DIR}/${file}`),
    path.resolve(`${STYLES_DEST_DIR}/${file}`),
  )
})

// copy all js files
scripts.forEach(file => {
  fs.copyFileSync(
    path.resolve(`./FE/${JS_SRC_DIR}/${file}`),
    path.resolve(`${JS_DEST_DIR}/${file}`),
  )
})

// copy all font files
fonts.forEach(file => {
  fs.copyFileSync(
    path.resolve(`./FE/${FONTS_SRC_DIR}/${file}`),
    path.resolve(`${FONTS_DEST_DIR}/${file}`),
  )
})

// copy the entire gfx directory
ncp(path.resolve(`./FE/${GFX_SRC_DIR}`), path.resolve(`${GFX_DEST_DIR}`), err => {
  if (err) {
    return console.error(err)
  }
})

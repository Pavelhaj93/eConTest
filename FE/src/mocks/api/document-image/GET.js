/**
 * Set PNG image as a mock response.
 * https://github.com/muratcorlu/connect-api-mocker/issues/31#issuecomment-619395826
 */
import path from 'path'

// eslint-disable-next-line no-undef
const { type, file } = require('connect-api-mocker/helpers')

// eslint-disable-next-line no-undef
const filePath = path.join(__dirname, './document-to-be-signed.png')

// eslint-disable-next-line no-undef
module.exports = [type('image/png'), file(filePath)]
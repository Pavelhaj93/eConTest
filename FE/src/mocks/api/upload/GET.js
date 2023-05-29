// copy from TODO: add here link from swagger

/**
 * Mock documents response.
 * Also simulate a short delay.
 */
import path from 'path'

const { type, file, delay } = require('connect-api-mocker/helpers')

const filePath = path.join(__dirname, './upload.json')

module.exports = [delay(1000), type('application/json'), file(filePath)]

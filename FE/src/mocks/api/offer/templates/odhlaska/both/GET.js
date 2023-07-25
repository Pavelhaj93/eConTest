import path from 'path'

const { type, file, delay } = require('connect-api-mocker/helpers')

const filePath = path.join(__dirname, './odhlaskaBoth.json')

module.exports = [delay(1000), type('application/json'), file(filePath)]
// TODO: implement https://virtserver.swaggerhub.com/LukasDvorak/eContracting2/1.0.0/api/eCon/offer

import path from 'path'

const { type, file, delay } = require('connect-api-mocker/helpers')

const filePath = path.join(__dirname, './documents.json')

module.exports = [delay(1000), type('application/json'), file(filePath)]
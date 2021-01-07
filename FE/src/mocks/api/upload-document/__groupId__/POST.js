// const { generateId } = require('../../../strings')

/**
 * Mock upload document response by returning 200 or 400 HTTP status code.
 * Also simulate a short delay.
 */

module.exports = function (request, response) {
  const random = Math.random() * 100
  const delay = 1000
  let statusCode
  let jsonResponse = {
    id: request.params.groupId,
    size: 4320,
    files: [],
  }

  if (random > 10) {
    statusCode = 200
    jsonResponse = {
      ...jsonResponse,
      files: [
        {
          // the real API will return the same key as `request.params.key` but currently don't know,
          // how to parse FormData from request
          key: 'STATIC_MOCK_KEY',
          name: 'IMG_3422.jpg',
          size: 1234,
          mime: 'image/jpeg',
        },
      ],
    }
  } else {
    statusCode = 400
    jsonResponse = {
      Message: 'Uploaded files size limit exceeded.',
    }
  }

  setTimeout(() => {
    response.status(statusCode).send(JSON.stringify(jsonResponse))
  }, delay)
}

/**
 * Mock upload document response by returning 200 or 400 HTTP status code.
 * Also simulate a short delay.
 */

module.exports = function (request, response) {
  const random = Math.random() * 100
  const delay = 1000
  let statusCode
  let jsonResponse = {}

  if (random > 10) {
    statusCode = 200
    jsonResponse = {
      uploaded: random > 40,
      message: random < 40 && 'Document has invalid type'
    }
  } else {
    statusCode = 400
  }

  setTimeout(() => {
    response.status(statusCode).send(JSON.stringify(jsonResponse))
  }, delay)
}

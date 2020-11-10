/**
 * Mock signature response by returning 200 or 400 HTTP status code.
 * Also simulate a short delay.
 */

module.exports = function (request, response) {
  const random = Math.random() * 100
  const delay = 400
  let statusCode

  if (random > 40) {
    statusCode = 200
  } else {
    statusCode = 400
  }

  setTimeout(() => {
    response.status(statusCode).send(' ')
  }, delay)
}
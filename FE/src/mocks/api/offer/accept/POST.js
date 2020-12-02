module.exports = function (request, response) {
  const random = Math.random() * 100
  const delay = 200
  let statusCode

  if (random > 10) {
    statusCode = 200
  } else {
    statusCode = 400
  }

  setTimeout(() => {
    response.status(statusCode).send(JSON.stringify({}))
  }, delay)
}

module.exports = function (request, response) {
  const random = Math.random() * 100
  const delay = 1000
  let statusCode
  let jsonResponse = {
    labels: {
      TITLE: 'Děkujeme za kontakt',
      TEXT: 'Zavoláme Vám',
      CLOSE: 'Zavřít',
    },
  }

  if (random > 10) {
    statusCode = 200
    jsonResponse = {
      ...jsonResponse,
    }
  } else {
    statusCode = 400
    jsonResponse = {
      message: 'Something goes wrong. Refresh the page and try once more.',
    }
  }

  setTimeout(() => {
    response.status(statusCode).send(JSON.stringify(jsonResponse))
  }, delay)
}

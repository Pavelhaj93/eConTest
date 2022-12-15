// copy from https://virtserver.swaggerhub.com/LukasDvorak/eContracting2/1.0.0/api/eCon/acceptedOffer

/**
 * Mock documents response. For both cases (success 200 / failed 500) we receive 200 status and depends on "succeed" property we reflect with UI and following behaviour.
 * Also simulate a short delay.
 */

const succeedResponseJson = require('./response.json')

module.exports = function (request, response) {
  const statusCode = 200
  const delay = 1000
  const succeed = true
  let jsonResponse = succeedResponseJson

  if (succeed) {
    jsonResponse = {
      ...jsonResponse,
    }
  } else {
    jsonResponse = {
      succeed: false,
      error:
        'Selhalo odeslání požadavku na zpětné volání. Kontaktujte prosím zákaznickou linku 800 11 33 55.',
    }
  }

  setTimeout(() => {
    response.status(statusCode).send(JSON.stringify(jsonResponse))
  }, delay)
}

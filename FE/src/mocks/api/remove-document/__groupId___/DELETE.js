module.exports = function (request, response) {
  // if the response is just an empty string, Chrome will log "Fetch failed loading" into console
  response.status(200).send(JSON.stringify({}))
}

module.exports = function (request, response) {
  const json = {
    id: request.params.groupId,
    size: 0,
    files: [],
  }
  response.status(200).send(JSON.stringify(json))
}

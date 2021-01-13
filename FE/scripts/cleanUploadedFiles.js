const path = require('path')
const rimraf = require('rimraf')

const filesDir = '../src/mocks/api/upload-document/__groupId__/files'
const dir = path.resolve(__dirname, filesDir)

rimraf(dir, () => {
  // eslint-disable-next-line no-console
  console.log('Uploaded files dir succesfully deleted.')
})

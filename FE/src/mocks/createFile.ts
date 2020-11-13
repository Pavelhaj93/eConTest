type MockFile = {
  name: string
  body: string
  mimeType: string
}

export const createFileFromMockFile = (file: MockFile): File => {
  const blob = new Blob([file.body], { type: file.mimeType }) as any
  blob['lastModifiedDate'] = new Date()
  blob['name'] = file.name
  return blob as File
}

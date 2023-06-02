import { createFileFromMockFile } from '../../../mocks/createFile'
import { UploadStore } from '../UploadStore'
import fetch from 'jest-fetch-mock'

describe('General offer', () => {
  it('adds file to the specified category', () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const category = 'testCategory'
    const store = new UploadStore('', '')

    store.addUserFiles([{ file }], category)

    // first check if new a category was created
    expect(store.userDocuments[category]).not.toBeFalsy()

    const document = store.userDocuments[category][0]

    // then check if the document is there
    expect(document).not.toBeFalsy()
  })

  it('uploads the document successfully', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const category = 'testCategory'
    const store = new UploadStore('', '')

    store.addUserFiles([{ file }], category)

    const document = store.userDocuments[category][0]

    const mockResponse = {
      id: 'groupId',
      files: [
        {
          key: 'hf83hnufwenfiuw',
          name: document.file.name,
          size: 4320,
          mime: document.file.type,
        },
      ],
    }
    fetch.mockResponseOnce(JSON.stringify(mockResponse))
    await store.uploadDocument(document, category)

    expect(document.touched).toBe(true)
    expect(document.uploading).toBe(false)
    expect(document.error).toBeFalsy()
  })

  it('rejects the document during upload', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const category = 'testCategory'
    const store = new UploadStore('', '')

    store.addUserFiles([{ file }], category)

    const document = store.userDocuments[category][0]

    const apiMessage = 'API is unavailable'
    fetch.mockRejectOnce(() => Promise.reject(apiMessage))
    await store.uploadDocument(document, category)

    expect(document.touched).toBe(true)
    expect(document.error).toBe(apiMessage)
  })

  it('removes previously added document', () => {
    const file1 = createFileFromMockFile({
      name: 'document.txt',
      body: 'content 1',
      mimeType: 'text/plain',
    })
    const file2 = createFileFromMockFile({
      name: 'contract.pdf',
      body: 'content 2',
      mimeType: 'application/pdf',
    })
    const category = 'testCategory'
    const store = new UploadStore('', '')

    store.addUserFiles([{ file: file1 }, { file: file2 }], category)

    const document = store.userDocuments[category][0]

    fetch.mockResponseOnce(JSON.stringify({}))
    store.removeUserDocument(document.key, category)

    expect(store.getUserDocument(document.key, category)).toBeFalsy()
    expect(store.userDocuments[category].length).toBe(1)
  })
})

describe('Offer with documents for upload', () => {
  const uploadResponseOneDoc = {
    data: [
      {
        type: 'docsUpload',
        position: 1,
        header: {
          title: 'Vložení dokumentů',
        },
        body: {
          docs: {
            title: 'Povinné dokumenty k nahrání',
            text: null,
            note: '',
            files: [
              {
                id: 'testCategory',
                title: 'Dokument prokazující vztah k nemovitosti',
                info:
                  '<p><b>Doklad o vlastnictví nemovitosti</b><br />Výpis z katastru nemovitostí, případně nájemní smlouva.<br /></p>',
                mandatory: true,
                idx: 0,
              },
            ],
          },
        },
      },
    ],
  }

  const uploadResponseTwoDocs = {
    data: [
      {
        type: 'docsUpload',
        position: 1,
        header: {
          title: 'Vložení dokumentů',
        },
        body: {
          docs: {
            title: 'Povinné dokumenty k nahrání',
            text: null,
            note: '',
            files: [
              {
                id: 'testCategory',
                title: 'Dokument prokazující vztah k nemovitosti',
                info:
                  '<p><b>Doklad o vlastnictví nemovitosti</b><br />Výpis z katastru nemovitostí, případně nájemní smlouva.<br /></p>',
                mandatory: true,
                idx: 0,
              },
              {
                id: 'testCategory2',
                title: 'Dokument prokazující vztah k nemovitosti',
                info:
                  '<p><b>Doklad o vlastnictví nemovitosti</b><br />Výpis z katastru nemovitostí, případně nájemní smlouva.<br /></p>',
                mandatory: true,
                idx: 0,
              },
            ],
          },
        },
      },
    ],
  }
  it('allows to continue to the accept offer page', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const categoryId = 'testCategory'
    const store = new UploadStore('', '')

    store.maxUploadGroupSize = 2000

    fetchMock.mockResponseOnce(JSON.stringify(uploadResponseOneDoc))
    await store.fetchUploads()

    store.addUserFiles([{ file }], categoryId)

    const document = store.userDocuments[categoryId][0]

    const mockUploadResponse = {
      id: categoryId,
      files: [
        {
          key: 'testId123',
          name: document.file.name,
          size: 4320,
          mime: document.file.type,
        },
      ],
    }
    fetch.mockResponseOnce(JSON.stringify(mockUploadResponse))
    await store.uploadDocument(document, categoryId)

    expect(store.isUploadFinished).toBe(true)
  })

  it('doesnt allow to continue to the accept offer page when only 1 of 2 documents uploaded', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })

    const categoryId = 'testCategory'
    const store = new UploadStore('', '')

    store.maxUploadGroupSize = 2000

    fetchMock.mockResponseOnce(JSON.stringify(uploadResponseTwoDocs))
    await store.fetchUploads()

    store.addUserFiles([{ file }], categoryId)

    const document = store.userDocuments[categoryId][0]

    const mockUploadResponse = {
      id: categoryId,
      files: [
        {
          key: 'testId123',
          name: document.file.name,
          size: 4320,
          mime: document.file.type,
        },
      ],
    }
    fetch.mockResponseOnce(JSON.stringify(mockUploadResponse))
    await store.uploadDocument(document, categoryId)

    expect(store.isUploadFinished).toBe(false)
  })
})

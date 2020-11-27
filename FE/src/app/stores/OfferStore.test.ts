import { OfferType } from '@types'
import fetch from 'jest-fetch-mock'
import mockDocumentGroups from '../../mocks/api/offer/accepted/documents.json'
import { createFileFromMockFile } from '../../mocks/createFile'
import { OfferStore } from './'

describe('Accepted offer', () => {
  it('should have zero documents after init', () => {
    const store = new OfferStore(OfferType.ACCEPTED, '')

    expect(store.documentGroups.length).toBe(0)
  })

  it('should fetch documents', async () => {
    const store = new OfferStore(OfferType.ACCEPTED, '')

    fetch.mockResponseOnce(JSON.stringify(mockDocumentGroups))
    await store.fetchOffer()

    expect(store.documentGroups.length).toBe(mockDocumentGroups.groups.length)
  })

  it('should set error when fetch fails', async () => {
    const store = new OfferStore(OfferType.ACCEPTED, '')

    fetch.mockReject(() => Promise.reject('API is unavailable'))
    await store.fetchOffer()

    expect(store.error).toBe(true)
  })
})

describe('General offer', () => {
  it('adds file to the specified category', () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const category = 'testCategory'
    const store = new OfferStore(OfferType.NEW, '')

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
    const store = new OfferStore(OfferType.NEW, '')

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
    const store = new OfferStore(OfferType.NEW, '')

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
    const store = new OfferStore(OfferType.NEW, '')

    store.addUserFiles([{ file: file1 }, { file: file2 }], category)

    const document = store.userDocuments[category][0]

    fetch.mockResponseOnce(JSON.stringify({}))
    store.removeUserDocument(document.key, category)

    expect(store.getUserDocument(document.key, category)).toBeFalsy()
    expect(store.userDocuments[category].length).toBe(1)
  })
})

describe('Offer with documents for acceptance', () => {
  const offerResponse = {
    documents: {
      acceptance: {
        accept: {
          files: [
            {
              label: 'Informace pro zákazníka - spotřebitele',
              prefix: 'Souhlasím s',
              key: 'e1c9a5ce583743e6928ee4df91862a91',
              mandatory: true,
            },
            {
              label: 'Dodatek',
              prefix: 'Jsem poučen o',
              key: '8e0ed4754f99464eb2d0155c140a2541',
              mandatory: true,
            },
          ],
        },
        sign: null,
      },
    },
  }

  it('fetches documents', async () => {
    const store = new OfferStore(OfferType.NEW, '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    expect(store.documentsToBeAccepted.length).toBe(
      offerResponse.documents.acceptance.accept.files.length,
    )
  })

  it('accepts single document', async () => {
    const key = offerResponse.documents.acceptance.accept.files[0].key
    const store = new OfferStore(OfferType.NEW, '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.acceptDocument(key)
    const document = store.getDocument(key)

    expect(document?.accepted).toBe(true)
  })

  it('accepts all documents', async () => {
    const store = new OfferStore(OfferType.NEW, '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.acceptAllDocuments(store.documentsToBeAccepted)

    expect(store.allDocumentsAreAccepted).toBe(true)
  })

  it('allows to accept the offer', async () => {
    const store = new OfferStore(OfferType.NEW, '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.acceptAllDocuments(store.documentsToBeAccepted)

    expect(store.isOfferReadyToAccept).toBe(true)
  })
})

describe('Offer with documents for signing', () => {
  const offerResponse = {
    documents: {
      acceptance: {
        accept: null,
        sign: {
          files: [
            {
              label: 'Plná moc',
              prefix: 'Jsem poučen o',
              key: '5e40683abb1441f5a0ec99425c2c6908',
              mandatory: true,
            },
          ],
        },
      },
    },
  }

  it('fetches documents', async () => {
    const store = new OfferStore(OfferType.NEW, '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    expect(store.documentsToBeSigned.length).toBe(
      offerResponse.documents.acceptance.sign.files.length,
    )
  })

  it('signs the document', async () => {
    const key = offerResponse.documents.acceptance.sign.files[0].key
    const store = new OfferStore(OfferType.NEW, '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    fetch.mockResponseOnce(JSON.stringify({}))
    await store.signDocument(key, 'signature', '')

    const document = store.getDocument(key)
    expect(document?.signed).toBe(true)
  })

  it('fails to sign the document', async () => {
    const key = offerResponse.documents.acceptance.sign.files[0].key
    const store = new OfferStore(OfferType.NEW, '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    fetch.mockRejectOnce(() => Promise.reject('Failed to sign'))
    await store.signDocument(key, 'signature', '')

    const document = store.getDocument(key)
    expect(document?.signed).toBeFalsy()
  })

  it('allows to accept the offer', async () => {
    const key = offerResponse.documents.acceptance.sign.files[0].key
    const store = new OfferStore(OfferType.NEW, '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    fetch.mockResponseOnce(JSON.stringify({}))
    await store.signDocument(key, 'signature', '')

    expect(store.allDocumentsAreSigned).toBe(true)
    expect(store.isOfferReadyToAccept).toBe(true)
  })
})

describe('Offer with both documents for acceptance and signing', () => {
  const offerResponse = {
    documents: {
      acceptance: {
        accept: {
          files: [
            {
              label: 'Informace pro zákazníka - spotřebitele',
              prefix: 'Souhlasím s',
              key: 'e1c9a5ce583743e6928ee4df91862a91',
              mandatory: true,
            },
            {
              label: 'Dodatek',
              prefix: 'Jsem poučen o',
              key: '8e0ed4754f99464eb2d0155c140a2541',
              mandatory: true,
            },
          ],
        },
        sign: {
          files: [
            {
              label: 'Plná moc',
              prefix: 'Jsem poučen o',
              key: '5e40683abb1441f5a0ec99425c2c6908',
              mandatory: true,
            },
          ],
        },
      },
    },
  }

  it('allows to accept the offer', async () => {
    const key = offerResponse.documents.acceptance.sign.files[0].key
    const store = new OfferStore(OfferType.NEW, '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.acceptAllDocuments(store.documentsToBeAccepted)
    fetch.mockResponseOnce(JSON.stringify({}))
    await store.signDocument(key, '', '')

    expect(store.isOfferReadyToAccept).toBe(true)
  })
})

describe('Offer with documents for upload', () => {
  const offerResponse = {
    documents: {
      uploads: {
        types: [
          {
            id: 'testCategory',
            title: 'Občanský nebo řidičský průkaz',
            info: 'Prostě to tam nahraj a na nic se neptej',
            mandatory: true,
          },
        ],
      },
    },
  }

  it('allows to accept the offer', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const category = 'testCategory'
    const store = new OfferStore(OfferType.NEW, '')

    fetchMock.mockResponseOnce(JSON.stringify(offerResponse))
    store.fetchOffer()

    store.addUserFiles([{ file }], category)

    const document = store.userDocuments[category][0]

    const mockUploadResponse = {
      id: category,
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
    await store.uploadDocument(document, category)

    expect(store.isOfferReadyToAccept).toBe(true)
  })
})

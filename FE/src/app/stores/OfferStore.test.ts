import { OfferDocument, OfferType } from '@types'
import fetch from 'jest-fetch-mock'
import mockDocumentGroups from '../../mocks/api/offer/accepted/documents.json'
import { createFileFromMockFile } from '../../mocks/createFile'
import { OfferStore } from './'

describe('Accepted offer', () => {
  it('should have zero documents after init', () => {
    const store = new OfferStore(OfferType.ACCEPTED, '', '')

    expect(store.documentGroups.length).toBe(0)
  })

  it('should fetch documents', async () => {
    const store = new OfferStore(OfferType.ACCEPTED, '', '')

    fetch.mockResponseOnce(JSON.stringify(mockDocumentGroups))
    await store.fetchOffer()

    expect(store.documentGroups.length).toBe(mockDocumentGroups.groups.length)
  })

  it('should set error when fetch fails', async () => {
    const store = new OfferStore(OfferType.ACCEPTED, '', '')

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
    const store = new OfferStore(OfferType.NEW, '', '')

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
    const store = new OfferStore(OfferType.NEW, '', '')

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
    const store = new OfferStore(OfferType.NEW, '', '')

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
    const store = new OfferStore(OfferType.NEW, '', '')

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
          mandatoryGroups: ['06D969E88C3C1EDB8BD27637116827D8'],
          files: [
            {
              label: 'Informace pro zákazníka - spotřebitele',
              prefix: 'Souhlasím s',
              key: 'doc1',
              mandatory: true,
              group: '06D969E88C3C1EDB8BD27637116827D8',
            },
            {
              label: 'Dodatek',
              prefix: 'Jsem poučen o',
              key: 'doc2',
              mandatory: true,
              group: '06D969E88C3C1EDB8BD27637116827D8',
            },
          ],
        },
        sign: null,
      },
    },
    acceptance: {
      params: [
        {
          title: 'Investor',
          group: '06D969E88C3C1EDB8BD27637116827D8',
        },
      ],
    },
  }

  it('fetches documents', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    expect(store.documentsToBeAccepted.length).toBe(
      offerResponse.documents.acceptance.accept.files.length,
    )
  })

  it('accepts single document', async () => {
    const key = offerResponse.documents.acceptance.accept.files[0].key
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.acceptDocument(key)
    const document = store.getDocument(key)

    expect(document?.accepted).toBe(true)
  })

  it('accepts all documents', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.acceptAllDocuments(store.documentsToBeAccepted)

    expect(store.allDocumentsAreAccepted).toBe(true)
  })

  it('allows to accept the offer', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.acceptAllDocuments(store.documentsToBeAccepted)

    expect(store.isOfferReadyToAccept).toBe(true)
  })

  it('marks the acceptance group as accepted', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.acceptAllDocuments(store.documentsToBeAccepted)

    expect(store.acceptanceGroups[0].accepted).toBe(true)
  })

  it('marks the offer as dirty after accepting document', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    const doc = store.getDocument('doc1') as OfferDocument
    doc.accepted = true

    expect(store.isOfferDirty).toBe(true)
  })
})

describe('Offer with documents for signing', () => {
  const offerResponse = {
    documents: {
      acceptance: {
        accept: null,
        sign: {
          mandatoryGroups: ['06D969E88C3C1EDB8BD27637116827D8'],
          files: [
            {
              label: 'Plná moc',
              prefix: 'Jsem poučen o',
              key: '5e40683abb1441f5a0ec99425c2c6908',
              mandatory: true,
              group: '06D969E88C3C1EDB8BD27637116827D8',
            },
          ],
        },
      },
    },
  }

  it('fetches documents', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    expect(store.documentsToBeSigned.length).toBe(
      offerResponse.documents.acceptance.sign.files.length,
    )
  })

  it('signs the document', async () => {
    const key = offerResponse.documents.acceptance.sign.files[0].key
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    fetch.mockResponseOnce(JSON.stringify({}))
    await store.signDocument(key, 'signature', '')

    const document = store.getDocument(key)
    expect(document?.accepted).toBe(true)
  })

  it('fails to sign the document', async () => {
    const key = offerResponse.documents.acceptance.sign.files[0].key
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    fetch.mockRejectOnce(() => Promise.reject('Failed to sign'))
    await store.signDocument(key, 'signature', '')

    const document = store.getDocument(key)
    expect(document?.accepted).toBeFalsy()
  })

  it('allows to accept the offer', async () => {
    const key = offerResponse.documents.acceptance.sign.files[0].key
    const store = new OfferStore(OfferType.NEW, '', '')

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
          mandatoryGroups: ['06D969E88C3C1EDB8BD27598F0B3C7D8'],
          files: [
            {
              label: 'Informace pro zákazníka - spotřebitele',
              prefix: 'Souhlasím s',
              key: 'e1c9a5ce583743e6928ee4df91862a91',
              mandatory: true,
              group: '06D969E88C3C1EDB8BD27598F0B3C7D8',
            },
            {
              label: 'Dodatek',
              prefix: 'Jsem poučen o',
              key: '8e0ed4754f99464eb2d0155c140a2541',
              mandatory: true,
              group: '06D969E88C3C1EDB8BD27598F0B3C7D8',
            },
          ],
        },
        sign: {
          mandatoryGroups: ['06D969E88C3C1EDB8BD27637116827D8'],
          files: [
            {
              label: 'Plná moc',
              prefix: 'Jsem poučen o',
              key: '5e40683abb1441f5a0ec99425c2c6908',
              mandatory: true,
              group: '06D969E88C3C1EDB8BD27637116827D8',
            },
          ],
        },
      },
    },
  }

  it('allows to accept the offer', async () => {
    const key = offerResponse.documents.acceptance.sign.files[0].key
    const store = new OfferStore(OfferType.NEW, '', '')

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
    const store = new OfferStore(OfferType.NEW, '', '')

    fetchMock.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

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

  it('exceeds maximum allowed size for all upload groups', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const fileSize = 2049
    const category = 'testCategory'
    const store = new OfferStore(OfferType.NEW, '', '')

    store.maxUploadGroupSize = 2048

    fetchMock.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.addUserFiles([{ file }], category)
    const document = store.userDocuments[category][0]

    const mockUploadResponse = {
      id: category,
      size: fileSize,
      files: [
        {
          key: 'testId123',
          name: document.file.name,
          size: fileSize,
          mime: document.file.type,
        },
      ],
    }
    fetch.mockResponseOnce(JSON.stringify(mockUploadResponse))
    await store.uploadDocument(document, category)

    expect(store.uploadGroupSizeExceeded).toBe(true)
  })
})

describe('Offer with product documents', () => {
  const offerResponse = {
    documents: {
      other: {
        products: {
          mandatoryGroups: [],
          files: [
            {
              label: 'Pojištění - Informační dokument 1',
              prefix: 'Jsem poučen o',
              key: 'doc1',
              mandatory: true,
              group: 'group1',
            },
            {
              label: 'Pojištění - Informační dokument 2',
              key: 'doc2',
              prefix: 'Jsem poučen o',
              mandatory: true,
              group: 'group1',
            },
            {
              label: 'Pojištění - Informační dokument 3',
              key: 'doc3',
              prefix: 'Jsem poučen o',
              mandatory: true,
              group: 'group2',
            },
          ],
        },
      },
    },
  }

  it('allows to accept the offer (no mandatory group)', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    expect(store.isOfferReadyToAccept).toBe(true)
  })

  it("doesn't allow to accept the offer (single document from shared group is accepted)", async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    const document = store.getDocument('doc1') as OfferDocument
    document.accepted = true

    expect(store.isOfferReadyToAccept).toBe(false)
  })

  it('allows to accept the offer (both documents from shared group are accepted)', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    const document1 = store.getDocument('doc1') as OfferDocument
    const document2 = store.getDocument('doc2') as OfferDocument
    document1.accepted = true
    document2.accepted = true

    expect(store.isOfferReadyToAccept).toBe(true)
  })

  it('allows to accept the offer (documents from mandatory groups must be accepted)', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    offerResponse.documents.other.products.mandatoryGroups = ['group2' as never]
    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    const document3 = store.getDocument('doc3') as OfferDocument
    document3.accepted = true

    expect(store.isOfferReadyToAccept).toBe(true)
  })
})

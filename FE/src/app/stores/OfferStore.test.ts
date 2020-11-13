import fetch from 'jest-fetch-mock'
import documents from '../../mocks/api/documents/documents.json'
import { createFileFromMockFile } from '../../mocks/createFile'
import { OfferStore, UserDocument } from './'

const offerMockConfig = {
  documentsUrl: '',
  isRetention: false,
  isAcquisition: false,
}

describe('Standard offer', () => {
  it('should have zero documents after init', () => {
    const store = new OfferStore(
      offerMockConfig.documentsUrl,
      offerMockConfig.isRetention,
      offerMockConfig.isAcquisition,
    )

    expect(store.documents.length).toBe(0)
  })

  it('should fetch documents', async () => {
    const store = new OfferStore(
      offerMockConfig.documentsUrl,
      offerMockConfig.isRetention,
      offerMockConfig.isAcquisition,
    )

    fetch.mockResponseOnce(JSON.stringify(documents))
    await store.fetchDocuments()

    expect(store.documents.length).toBe(documents.length)
  })

  it('should set error when fetch fails', async () => {
    const store = new OfferStore(
      offerMockConfig.documentsUrl,
      offerMockConfig.isRetention,
      offerMockConfig.isAcquisition,
    )

    fetch.mockReject(() => Promise.reject('API is unavailable'))
    await store.fetchDocuments()

    expect(store.error).toBe(true)
  })

  it('it should be ready to accept the offer if all documents are accepted', async () => {
    const store = new OfferStore(
      offerMockConfig.documentsUrl,
      offerMockConfig.isRetention,
      offerMockConfig.isAcquisition,
    )

    fetch.mockResponseOnce(JSON.stringify(documents))
    await store.fetchDocuments()

    store.acceptAllDocuments()

    expect(store.isOfferReadyToAccept).toBe(true)
  })

  it('should accept the document', async () => {
    const store = new OfferStore(
      offerMockConfig.documentsUrl,
      offerMockConfig.isRetention,
      offerMockConfig.isAcquisition,
    )

    const mockDocument = {
      id: '009',
      title: 'Informace pro zákazníka – spotřebitele.pdf',
      url: 'null',
      label: 'Souhlasím s',
      sign: false,
    }

    store.documents = [mockDocument]

    const document = store.getDocument(mockDocument.id)

    store.acceptDocument(mockDocument.id)

    expect(document?.accepted).toBe(true)
  })
})

const retentionMockConfig = {
  documentsUrl: '',
  isRetention: true,
  isAcquisition: false,
}

describe('Retention offer', () => {
  it('should sign the document', async () => {
    const store = new OfferStore(
      retentionMockConfig.documentsUrl,
      retentionMockConfig.isRetention,
      retentionMockConfig.isAcquisition,
    )

    const mockDocument = {
      id: '009',
      title: 'Informace pro zákazníka – spotřebitele.pdf',
      url: 'null',
      label: 'Souhlasím s',
      sign: true,
    }

    store.documents = [mockDocument]

    const document = store.getDocument(mockDocument.id)

    fetch.mockResponseOnce('')
    await store.signDocument(mockDocument.id, 'signature', '')

    expect(document?.signed).toBe(true)
  })

  it('should has an error when trying to sign the document', async () => {
    const store = new OfferStore(retentionMockConfig.documentsUrl)

    const mockDocument = {
      id: '009',
      title: 'Informace pro zákazníka – spotřebitele.pdf',
      url: 'null',
      label: 'Souhlasím s',
      sign: true,
    }

    store.documents = [mockDocument]

    fetch.mockRejectOnce(() => Promise.reject('API is unavailable'))
    await store.signDocument(mockDocument.id, 'signature', '')

    expect(store.signError).toBe(true)
  })

  it('it should be ready to accept the offer if all documents are accepted and signed', async () => {
    const store = new OfferStore(
      retentionMockConfig.documentsUrl,
      retentionMockConfig.isRetention,
      retentionMockConfig.isAcquisition,
    )

    const mockDocuments = [
      {
        id: '004',
        title: 'Dodatek.pdf',
        url: 'null',
        label: 'Jsem poučen o',
        sign: false,
      },
      {
        id: '003',
        title: 'Plná moc (Lastcall).pdf',
        url: 'null',
        label: 'Jsem poučen o',
        sign: true,
      },
    ]

    fetch.mockResponseOnce(JSON.stringify(mockDocuments))
    await store.fetchDocuments()

    store.acceptAllDocuments()

    fetch.mockResponseOnce('')
    await store.signDocument(mockDocuments[1].id, '', '')

    expect(store.isOfferReadyToAccept).toBe(true)
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
    const store = new OfferStore('')

    store.addUserFiles([file], category)

    // first check if new a category was created
    expect(store.userDocuments[category]).not.toBeFalsy()

    // then check if the document is there
    expect(store.getUserDocument(file.name, category)).not.toBeFalsy()
  })

  it('does not duplicate file with the same name in one category', () => {
    const file1 = createFileFromMockFile({
      name: 'document.txt',
      body: 'content 1',
      mimeType: 'text/plain',
    })
    const file2 = createFileFromMockFile({
      name: 'document.txt',
      body: 'content 2',
      mimeType: 'text/plain',
    })
    const category = 'testCategory'
    const store = new OfferStore('')

    store.addUserFiles([file1], category)
    store.addUserFiles([file2], category)

    // then check if the is there
    expect(store.userDocuments[category].length).toBe(1)
  })

  it('uploads the document successfully', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const category = 'testCategory'
    const store = new OfferStore('')

    store.addUserFiles([file], category)

    const document = store.getUserDocument(file.name, category) as UserDocument

    fetch.mockResponseOnce(JSON.stringify({ uploaded: true }))
    await store.uploadDocument(document)

    expect(document.touched).toBe(true)
    expect(document.error).toBeFalsy()
  })

  it('rejects the document during upload', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const category = 'testCategory'
    const store = new OfferStore('')

    store.addUserFiles([file], category)

    const document = store.getUserDocument(file.name, category) as UserDocument

    const apiMessage = 'API is unavailable'
    fetch.mockRejectOnce(() => Promise.reject(apiMessage))
    await store.uploadDocument(document)

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
    const store = new OfferStore('')

    store.addUserFiles([file1, file2], category)

    store.removeUserDocument(file1.name, category)

    expect(store.getUserDocument(file1.name, category)).toBeFalsy()
    expect(store.userDocuments[category].length).toBe(1)
  })
})

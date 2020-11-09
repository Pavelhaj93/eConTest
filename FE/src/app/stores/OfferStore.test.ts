import fetch from 'jest-fetch-mock'
import documents from '../../mocks/api/documents/GET.json'
import { OfferStore } from './'

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

  it('should has an error when trying to sing the document', async () => {
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

import { OfferType } from '@types'
import fetch from 'jest-fetch-mock'
import mockDocumentGroups from '../../mocks/api/offer/accepted/documents.json'

import { OfferStore } from './'

describe('Accepted offer', () => {
  it('should have zero documents after init', () => {
    const store = new OfferStore(OfferType.ACCEPTED, '', '')

    expect(store.acceptedDocumentGroups.length).toBe(0)
  })

  it('should fetch documents', async () => {
    const store = new OfferStore(OfferType.ACCEPTED, '', '')

    fetch.mockResponseOnce(JSON.stringify(mockDocumentGroups))
    await store.fetchOffer()

    expect(store.acceptedDocumentGroups.length).toBe(mockDocumentGroups.groups.length)
  })

  it('should set error when fetch fails', async () => {
    const store = new OfferStore(OfferType.ACCEPTED, '', '')

    fetch.mockReject(() => Promise.reject('API is unavailable'))
    await store.fetchOffer()

    expect(store.error).toBe(true)
  })
})

describe('Offer with documents for acceptance', () => {
  const offerResponse = {
    data: [
      {
        type: 'docsCheck',
        position: 5,
        header: {
          title: 'Doplňkové služby',
        },
        body: {
          head: null,
          text: null,
          docs: {
            title: 'Dokument(y) k elektronické akceptaci služby',
            params: null,
            text:
              '<p>Nunc eu ante nec ipsum porttitor ultricies sed non velit. Suspendisse potenti. Ut placerat, enim ac luctus gravida, quam urna pulvinar urna, in placerat libero est viverra tortor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Mauris mollis sapien nisi, eu blandit dui commodo quis. Donec eget lobortis urna. Pellentesque mollis sapien sed eros suscipit, nec elementum nulla maximus.</p>',
            mandatoryGroups: [],
            files: [
              {
                key: 'doc1',
                group: '0635F899B3111EED8E9B080756ABDEC9',
                label: 'Žádost o nesnižování zálohových plateb',
                note: '',
                prefix: 'Souhlasím s',
                mime: 'application/pdf',
                mandatory: false,
                idx: 0,
              },
            ],
          },
          note: null,
        },
      },
      {
        type: 'confirm',
        position: 10,
        header: {
          title: 'Potvrzení nabídky',
        },
        body: {
          head: null,
          text: 'Stisknutím tlačítka Akceptuji potvrdíte souhlas se zahájením dodávky u innogy.',
          params: [
            {
              title: 'Nesnižování záloh',
              group: '0635F899B3111EED8E9B080756ABDEC9',
            },
          ],
        },
      },
    ],
  }

  it('fetches documents', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    expect(store.getAllToBeCheckedDocuments().length).toBe(
      offerResponse?.data?.[0]?.body?.docs?.files.length,
    )
  })

  it('accepts single document', async () => {
    const key = offerResponse?.data?.[0]?.body?.docs?.files[0].key
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.checkDocument(key as string)
    const document = store.getDocument(key as string, store.docGroupsToBeChecked)

    expect(document?.accepted).toBe(true)
  })

  it('accepts all documents', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.checkDocumentsGroup(store.docGroupsToBeChecked[0])

    expect(store.allDocumentsAreChecked).toBe(true)
  })

  it('allows to accept the offer', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.checkDocumentsGroup(store.docGroupsToBeChecked[0])

    expect(store.isOfferReadyToAccept).toBe(true)
  })

  it('marks the acceptance group as accepted', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.checkDocumentsGroup(store.docGroupsToBeChecked[0])

    expect(store.acceptanceGroups?.[0].accepted).toBe(true)
  })

  it('marks the offer as dirty after accepting document', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    const doc = store.getDocument('doc1', store.docGroupsToBeChecked)
    doc !== undefined ? (doc.accepted = true) : null

    expect(store.isOfferDirty).toBe(true)
  })
})

describe('Offer with documents for signing', () => {
  const offerResponse = {
    data: [
      {
        type: 'docsSign',
        position: 4,
        header: {
          title: 'Dokumenty k podpisu',
        },
        body: {
          head: null,
          text: null,
          docs: {
            title: 'Plná moc',
            params: null,
            text:
              '<p>Plnou moc potřebujeme pro zah&aacute;jen&iacute; dod&aacute;vky u innogy.&nbsp;</p>',
            mandatoryGroups: ['0635F899B3111EED8E9B07EF632D3EC9'],
            files: [
              {
                key: 'EEFCB55B021DA472D361BD2E894BBDD9',
                group: '0635F899B3111EED8E9B07EF632D3EC9',
                label: 'Plná moc',
                note: '',
                prefix: 'Jsem poučen o',
                mime: 'application/pdf',
                mandatory: true,
                idx: 3,
              },
            ],
          },
          note: 'Podepište snadno myší přímo ve Vašem prohlížeči nebo prstem v tabletu',
        },
      },
    ],
  }

  it('fetches documents', async () => {
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    expect(store.docGroupsToBeSigned.length).toBe(offerResponse.data[0].body.docs.files.length)
  })

  it('signs the document', async () => {
    const key = offerResponse.data[0].body.docs.files[0].key
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    fetch.mockResponseOnce(JSON.stringify({}))
    await store.signDocument(key, 'signature', '')

    const document = store.getDocument(key, store.docGroupsToBeSigned)
    expect(document?.accepted).toBe(true)
  })

  it('fails to sign the document', async () => {
    const key = offerResponse.data[0].body.docs.files[0].key
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    fetch.mockRejectOnce(() => Promise.reject('Failed to sign'))
    await store.signDocument(key, 'signature', '')

    const document = store.getDocument(key, store.docGroupsToBeSigned)
    expect(document?.accepted).toBeFalsy()
  })

  it('allows to accept the offer', async () => {
    const key = offerResponse.data[0].body.docs.files[0].key
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
    data: [
      {
        type: 'docsSign',
        position: 4,
        header: {
          title: 'Dokumenty k podpisu',
        },
        body: {
          head: null,
          text: null,
          docs: {
            title: 'Plná moc',
            params: null,
            text:
              '<p>Plnou moc potřebujeme pro zah&aacute;jen&iacute; dod&aacute;vky u innogy.&nbsp;</p>',
            mandatoryGroups: ['0635F899B3111EED8E9B07EF632D3EC9'],
            files: [
              {
                key: 'EEFCB55B021DA472D361BD2E894BBDD9',
                group: '0635F899B3111EED8E9B07EF632D3EC9',
                label: 'Plná moc',
                note: '',
                prefix: 'Jsem poučen o',
                mime: 'application/pdf',
                mandatory: true,
                idx: 3,
              },
            ],
          },
          note: 'Podepište snadno myší přímo ve Vašem prohlížeči nebo prstem v tabletu',
        },
      },
      {
        type: 'docsCheck',
        position: 5,
        header: {
          title: 'Doplňkové služby',
        },
        body: {
          head: null,
          text: null,
          docs: {
            title: 'Dokument(y) k elektronické akceptaci služby',
            params: null,
            text:
              '<p>Nunc eu ante nec ipsum porttitor ultricies sed non velit. Suspendisse potenti. Ut placerat, enim ac luctus gravida, quam urna pulvinar urna, in placerat libero est viverra tortor. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Mauris mollis sapien nisi, eu blandit dui commodo quis. Donec eget lobortis urna. Pellentesque mollis sapien sed eros suscipit, nec elementum nulla maximus.</p>',
            mandatoryGroups: [],
            files: [
              {
                key: '176635D0264E5B165AD3AC031A10760F',
                group: '0635F899B3111EED8E9B080756ABDEC9',
                label: 'Žádost o nesnižování zálohových plateb',
                note: '',
                prefix: 'Souhlasím s',
                mime: 'application/pdf',
                mandatory: false,
                idx: 0,
              },
            ],
          },
          note: null,
        },
      },
    ],
  }

  it('allows to accept the offer', async () => {
    const key = offerResponse.data[0].body.docs.files[0].key
    const store = new OfferStore(OfferType.NEW, '', '')

    fetch.mockResponseOnce(JSON.stringify(offerResponse))
    await store.fetchOffer()

    store.checkDocumentsGroup(offerResponse.data[1].body.docs.files)
    fetch.mockResponseOnce(JSON.stringify({}))
    await store.signDocument(key, '', '')

    expect(store.isOfferReadyToAccept).toBe(true)
  })
})

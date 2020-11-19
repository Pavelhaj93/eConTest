import { OfferType } from '@types'
import fetch from 'jest-fetch-mock'
import mockDocumentGroups from '../../mocks/api/offer/accepted/documents.json'
import { createFileFromMockFile } from '../../mocks/createFile'
import { OfferStore, UserDocument } from './'

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
    const store = new OfferStore(OfferType.NEW, '')

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
    const store = new OfferStore(OfferType.NEW, '')

    store.addUserFiles([file], category)

    const document = store.getUserDocument(file.name, category) as UserDocument

    fetch.mockResponseOnce(JSON.stringify({ uploaded: true, id: 'aco123' }))
    await store.uploadDocument(document, category)

    expect(document.touched).toBe(true)
    expect(document.error).toBeFalsy()
    expect(document.id).toBeTruthy()
  })

  it('rejects the document during upload', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })
    const category = 'testCategory'
    const store = new OfferStore(OfferType.NEW, '')

    store.addUserFiles([file], category)

    const document = store.getUserDocument(file.name, category) as UserDocument

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

    store.addUserFiles([file1, file2], category)

    store.removeUserDocument(file1.name, category)

    expect(store.getUserDocument(file1.name, category)).toBeFalsy()
    expect(store.userDocuments[category].length).toBe(1)
  })
})

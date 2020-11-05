import { Document } from '@types'
import { action, computed, observable } from 'mobx'

export class OfferStore {
  private documentsUrl = ''

  @observable
  public documents: Document[] = []

  @observable
  public isRetention = false

  @observable
  public isAcquisition = false

  @observable
  public error = false

  @observable
  public isLoading = false

  constructor(documentsUrl: string, isRetention: boolean, isAcquisition: boolean) {
    this.documentsUrl = documentsUrl
    this.isRetention = isRetention
    this.isAcquisition = isAcquisition
  }

  /** All documents that are marked to be accepted. */
  @computed public get documentsToBeAccepted(): Document[] {
    return this.documents.filter(document => !document.sign)
  }

  /** All documents that are marked to be signed. */
  @computed public get documentsToBeSigned(): Document[] {
    return this.documents.filter(document => document.sign)
  }

  /** True if all documents that are marked to be signed are actually signed, otherwise false. */
  @computed public get allDocumentsAreSigned(): boolean {
    if (this.documentsToBeSigned.find(document => !document.signed)) {
      return false
    }

    return true
  }

  /** True if all documents that are marked to be accepted are actually accepted, otherwise false. */
  @computed public get allDocumentsAreAccepted(): boolean {
    if (this.documentsToBeAccepted.find(document => !document.accepted)) {
      return false
    }

    return true
  }

  /** True if all conditions for particular offer were fullfilled, otherwise false. */
  @computed public get isOfferReadyToAccept(): boolean {
    if (!this.isRetention && !this.isAcquisition) {
      return this.allDocumentsAreAccepted
    }

    if (this.isRetention || this.isAcquisition) {
      return this.allDocumentsAreAccepted && this.allDocumentsAreSigned
    }

    return false
  }

  /**
   * Performs an ajax request to `url` (default set to `this.documentsUrl`), fetch and populate documents.
   */
  @action public async fetchDocuments(url = this.documentsUrl) {
    this.isLoading = true

    try {
      const response = await fetch(url, {
        headers: { Accept: 'application/json' },
      })

      if (!response.ok) {
        throw new Error(`FAILED TO FETCH DOCUMENTS - ${response.status}`)
      }

      const jsonResponse = await (response.json() as Promise<Array<Document>>)

      this.documents = jsonResponse
    } catch (error) {
      console.error(error)
      this.error = true
    } finally {
      this.isLoading = false
    }
  }

  /**
   * Get a document by ID.
   * @param id - ID of document
   */
  public getDocument(id: string): Document | undefined {
    return this.documents.find(document => document.id === id)
  }

  /**
   * Change `signed` state of given document.
   * @param id - ID of document
   */
  @action public signDocument(id: string): void {
    const document = this.getDocument(id)

    if (!document) {
      return
    }

    document.signed = !document.signed

    this.documents = [...this.documents]
  }

  // TODO:
  // private signDocumentRequest(id: string): void {

  // }

  /**
   * Change `accepted` state of given document.
   * @param id - ID of document
   */
  @action public acceptDocument(id: string): void {
    const document = this.getDocument(id)

    if (!document) {
      return
    }

    document.accepted = !document.accepted

    this.documents = [...this.documents]
  }

  /**
   * Change `accepted` state of all documents to true.
   * If all documents are already accepted => set to false.
   */
  @action public acceptAllDocuments(): void {
    let acceptedDocuments: Document[] = []

    if (this.allDocumentsAreAccepted) {
      acceptedDocuments = this.documents.map(document => ({
        ...document,
        accepted: false,
      }))
    } else {
      acceptedDocuments = this.documents.map(document => ({
        ...document,
        accepted: true,
      }))
    }

    this.documents = acceptedDocuments
  }
}

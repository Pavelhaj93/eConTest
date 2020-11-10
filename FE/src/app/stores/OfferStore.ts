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

  @observable
  public isSigning = false

  @observable
  public signError = false

  constructor(documentsUrl: string, isRetention = false, isAcquisition = false) {
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
    if (!this.documentsToBeSigned.length) {
      return false
    }

    if (this.documentsToBeSigned.find(document => !document.signed)) {
      return false
    }

    return true
  }

  /** True if all documents that are marked to be accepted are actually accepted, otherwise false. */
  @computed public get allDocumentsAreAccepted(): boolean {
    if (!this.documentsToBeAccepted.length) {
      return false
    }

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
   * Performs an ajax request to `documentsURL` provided in constructor, fetch and populate documents.
   * @param timeoutMs - number of milliseconds after which the fetch request is canceled.
   */
  @action public async fetchDocuments(timeoutMs?: number): Promise<void> {
    this.isLoading = true

    try {
      let fetchTimeout: NodeJS.Timeout | number | null = null
      let controller: AbortController | null = null

      // if timeoutMs is present => cancel the fetch request after this value
      if (timeoutMs) {
        controller = new AbortController()
        fetchTimeout = setTimeout(() => {
          controller && controller.abort()
        }, timeoutMs)
      }

      const response = await fetch(this.documentsUrl, {
        headers: { Accept: 'application/json' },
        signal: controller?.signal || null,
      })

      fetchTimeout && clearTimeout(fetchTimeout)

      if (!response.ok) {
        throw new Error(`FAILED TO FETCH DOCUMENTS - ${response.status}`)
      }

      const jsonResponse = await (response.json() as Promise<Array<Document>>)

      this.documents = jsonResponse
    } catch (error) {
      // eslint-disable-next-line no-console
      console.error(error.toString())
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
   * Change `signed` state of given document after the signature is successfully sent to API.
   * @param id - ID of document
   * @param signature - PNG image as base64
   * @param signFileUrl - URL where to send `signature` data
   * @returns Promise<boolean> - if true, the document was successfully signed, otherwise false.
   */
  @action public async signDocument(
    id: string,
    signature: string,
    signFileUrl: string,
  ): Promise<boolean> {
    const document = this.getDocument(id)

    if (!document) {
      return false
    }

    this.isSigning = true
    this.signError = false

    return this.signDocumentRequest(id, signature, signFileUrl)
      .then(() => {
        document.signed = true
        this.documents = [...this.documents]
        return true
      })
      .catch(() => {
        this.signError = true
        return false
      })
      .finally(() => {
        this.isSigning = false
      })
  }

  /**
   * Send a request with signature data to sign API.
   * @param id - ID of document
   * @param signature - PNG image as base64
   * @param signFileUrl - URL where to send `signature` data
   * @returns Promise<void>
   */
  private async signDocumentRequest(
    id: string,
    signature: string,
    signFileUrl: string,
  ): Promise<void> {
    const data = {
      signature,
    }

    const response = await fetch(`${signFileUrl}${id}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    })

    return new Promise((resolve, reject) => {
      if (response.ok) {
        resolve()
      } else {
        reject()
      }
    })
  }

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

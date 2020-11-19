import { AcceptedOfferResponse, Document, Group, OfferType, UploadDocumentResponse } from '@types'
import { UserDocument } from './'
import { action, computed, observable } from 'mobx'
import { generateId } from '@utils'

export class OfferStore {
  public offerUrl = ''
  public uploadDocumentUrl = ''
  public removeDocumentUrl = ''
  public errorPageUrl = ''
  private type: OfferType

  @observable
  public documents: Document[] = []

  @observable
  public documentGroups: Group[] = []

  @observable
  public error = false

  @observable
  public isLoading = false

  @observable
  public isSigning = false

  @observable
  public signError = false

  @observable
  public userDocuments: Record<string, UserDocument[]> = {}

  constructor(type: OfferType, offerUrl: string) {
    this.offerUrl = offerUrl
    this.type = type
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

  // TODO: implement
  /** True if all conditions for particular offer were fullfilled, otherwise false. */
  @computed public get isOfferReadyToAccept(): boolean {
    // if (!this.isRetention && !this.isAcquisition) {
    //   return this.allDocumentsAreAccepted
    // }

    // if (this.isRetention || this.isAcquisition) {
    //   return this.allDocumentsAreAccepted && this.allDocumentsAreSigned
    // }

    return false
  }

  /**
   * Performs an ajax request to `documentsURL` provided in constructor, fetch and populate documents.
   * @param timeoutMs - number of milliseconds after which the fetch request is canceled.
   */
  @action public async fetchOffer(timeoutMs?: number): Promise<void> {
    this.isLoading = true

    try {
      if (!this.type) {
        throw new Error('FAILED TO FETCH OFFER - MISSING OFFER TYPE')
      }

      let fetchTimeout: NodeJS.Timeout | number | null = null
      let controller: AbortController | null = null

      // if timeoutMs is present => cancel the fetch request after this value
      if (timeoutMs) {
        controller = new AbortController()
        fetchTimeout = setTimeout(() => {
          controller && controller.abort()
        }, timeoutMs)
      }

      const response = await fetch(this.offerUrl, {
        headers: { Accept: 'application/json' },
        signal: controller?.signal || null,
      })

      fetchTimeout && clearTimeout(fetchTimeout)

      // redirect to error page on 404 response
      if (response.status === 404) {
        window.location.href = this.errorPageUrl
        return
      }

      if (!response.ok) {
        throw new Error(`FAILED TO FETCH OFFER - ${response.status}`)
      }

      let jsonResponse

      switch (this.type) {
        case OfferType.NEW:
          // TODO: implement parsing of new offer response
          break

        case OfferType.ACCEPTED:
          jsonResponse = await     (response.json() as Promise<AcceptedOfferResponse>)
          this.documentGroups = jsonResponse.groups
          break
        default:
          break
      }
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
    return this.documents.find(document => document.key === id)
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

  @action public addUserFiles(files: File[], category: string): void {
    // remap the files to the `UserDocument` shape
    const newDocuments = files.map<UserDocument>(file => new UserDocument(file, generateId()))

    // if category does not exist yet => create one
    if (!this.userDocuments[category]) {
      this.userDocuments[category] = []
    }

    // spread existing documents within the category and add new ones
    this.userDocuments = {
      ...this.userDocuments, // this spread is needed for trigger rerender in React component
      [category]: [...this.userDocuments[category], ...newDocuments],
    }
  }

  /**
   * Get user document by its id and category.
   * @param id - document id
   * @param category - category name
   */
  public getUserDocument(id: string, category: string): UserDocument | undefined {
    return this.userDocuments[category]?.find(document => document.id === id)
  }

  /**
   * Remove user document by its id and category.
   * @param document - `UserDocument`
   * @param category - category name
   */
  @action public async removeUserDocument(id: string, category: string): Promise<void> {
    if (!this.userDocuments[category]) {
      return
    }

    const document = this.getUserDocument(id, category)

    this.userDocuments[category] = this.userDocuments[category].filter(doc => doc.id !== id)

    // if document is still uploading => do not send the request
    if (!document || document.uploading) {
      return
    }

    // if document has been successfully uploaded => sends a request to remove it
    await fetch(`${this.removeDocumentUrl}${document.id}`)
  }

  /**
   * Sends a request with the document (file) to upload API.
   * @param document - file
   * @returns `Promise` in shape of `UploadDocumentResponse`
   */
  public async uploadDocument(
    document: UserDocument,
    category: string,
  ): Promise<UploadDocumentResponse> {
    const formData = new FormData()
    const { file } = document
    formData.append('file', file, file.name)
    formData.append('category', category)

    const controller = new AbortController()

    document.touched = true // change the `touched` state of the document
    document.controller = controller // add a control of the following request to the document
    document.uploading = true

    try {
      const response = await fetch(this.uploadDocumentUrl, {
        method: 'POST',
        headers: { Accept: 'application/json' },
        body: formData,
        signal: controller.signal,
      })

      if (!response.ok) {
        throw new Error(`FAILED TO UPLOAD DOCUMENT - ${response.statusText} (${response.status})`)
      }

      const { uploaded, message, id } = (await response.json()) as UploadDocumentResponse

      if (!uploaded) {
        throw new Error(message)
      }

      // overwrite the existing id generated on frontend using the one from API
      if (id) {
        document.id = id
      }

      return Promise.resolve({ uploaded: true, id })
    } catch (error) {
      let message = undefined

      if (typeof error === 'string') {
        message = error
      } else if (error instanceof Error) {
        message = error.message
      }

      if (document) {
        document.error = message // mark the document as invalid
      }

      return Promise.resolve({ uploaded: false, message })
    } finally {
      document.uploading = false
    }
  }

  public cancelUploadDocument(document: UserDocument): void {
    if (document.controller) {
      document.controller.abort()
    }
  }
}

import {
  AcceptedOfferResponse,
  CustomFile,
  Group,
  NewOfferResponse,
  OfferDocuments,
  OfferDocument,
  OfferType,
  UploadDocumentResponse,
  OfferBox,
  GiftsBox,
  OtherDocuments,
} from '@types'
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
  public documents: OfferDocuments = {}

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

  @observable
  public perex: OfferBox | undefined

  @observable
  public benefits: OfferBox | undefined

  @observable
  public gifts: GiftsBox | undefined

  constructor(type: OfferType, offerUrl: string) {
    this.offerUrl = offerUrl
    this.type = type
  }

  @computed public get offerFetched(): boolean {
    return Boolean(Object.keys(this.documents).length)
  }

  @computed public get singleGiftGroup(): boolean {
    return this.gifts?.groups.length === 1 ?? false
  }

  /** All documents that are marked to be accepted. */
  @computed public get documentsToBeAccepted(): OfferDocument[] {
    return this.documents.acceptance?.accept?.files ?? []
  }

  /** All documents that are marked to be signed. */
  @computed public get documentsToBeSigned(): OfferDocument[] {
    return this.documents.acceptance?.sign?.files ?? []
  }

  @computed public get documentsServices(): OfferDocument[] {
    return this.documents.other?.services?.files ?? []
  }

  @computed public get documentsCommodities(): OfferDocument[] {
    return this.documents.other?.commodities?.files ?? []
  }

  /** True if all documents that are mandatory are actually signed, otherwise false. */
  @computed public get allDocumentsAreSigned(): boolean {
    if (!this.documentsToBeSigned.length) {
      return true
    }

    if (this.documentsToBeSigned.find(document => document.mandatory && !document.signed)) {
      return false
    }

    return true
  }

  /** True if all documents that are mandatory are actually accepted, otherwise false. */
  @computed public get allDocumentsAreAccepted(): boolean {
    if (!this.documentsToBeAccepted.length) {
      return true
    }

    if (this.documentsToBeAccepted.find(document => document.mandatory && !document.accepted)) {
      return false
    }

    return true
  }

  @computed public get allServicesDocumentsAreAccepted(): boolean {
    if (!this.documentsServices.length) {
      return true
    }

    if (this.documentsServices.find(document => document.mandatory && !document.accepted)) {
      return false
    }

    return true
  }

  @computed public get allCommoditiesDocumentsAreAccepted(): boolean {
    if (!this.documentsCommodities.length) {
      return true
    }

    if (this.documentsCommodities.find(document => document.mandatory && !document.accepted)) {
      return false
    }

    return true
  }

  // TODO: implement
  /** True if all conditions for particular offer were fullfilled, otherwise false. */
  @computed public get isOfferReadyToAccept(): boolean {
    // if we don't have a single section under `documents` => there is nothing to accept
    if (!this.offerFetched) {
      return false
    }

    return (
      this.allDocumentsAreAccepted &&
      this.allDocumentsAreSigned &&
      this.allCommoditiesDocumentsAreAccepted &&
      this.allServicesDocumentsAreAccepted
    )
  }

  /**
   * Since the `accepted` and `signed` keys are not present in the JSON response on `OfferDocument` object,
   * here I add them manually.
   * The reason is that MobX can't observe dynamic keys on object that are not present on that object
   * during initialization of the store, so whenever a new dynamic key is added, it won't trigger
   * rerender in React component.
   * Another approach would be to use Map instead of pure Object.
   *
   * https://alexhisen.gitbook.io/mobx-recipes/use-extendobservable-sparingly
   */
  private enrichDocuments(documents: OfferDocument[]): OfferDocument[] {
    return documents.map(document => ({
      ...document,
      accepted: false,
      signed: false,
    }))
  }

  /**
   * This just enriches all the received documents in all sections with two extra keys - `accepted` and `signed`
   * and then merged the enriched documents with the rest of the object.
   * @param documents
   */
  private enrichDocumentsResponse(documents: OfferDocuments): OfferDocuments {
    const acceptance = Object.assign({}, documents.acceptance, {
      accept: !documents.acceptance?.accept
        ? null
        : Object.assign({}, documents.acceptance?.accept, {
            files: this.enrichDocuments(documents.acceptance?.accept?.files),
          }),
      sign: !documents.acceptance?.sign
        ? null
        : Object.assign({}, documents.acceptance?.sign, {
            files: this.enrichDocuments(documents.acceptance?.sign?.files),
          }),
    })

    const other = Object.assign({}, documents.other, {
      commodities: !documents.other?.commodities
        ? null
        : Object.assign({}, documents.other?.commodities, {
            files: this.enrichDocuments(documents.other?.commodities?.files),
          }),
      services: !documents.other?.services
        ? null
        : Object.assign({}, documents.other?.services, {
            files: this.enrichDocuments(documents.other?.services?.files),
          }),
    })

    return {
      uploads: documents.uploads,
      acceptance,
      other,
    }
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
          jsonResponse = await (response.json() as Promise<NewOfferResponse>)
          this.documents = this.enrichDocumentsResponse(jsonResponse.documents)
          this.perex = jsonResponse.perex
          this.gifts = jsonResponse.gifts
          this.benefits = jsonResponse.benefits
          break

        case OfferType.ACCEPTED:
          jsonResponse = await (response.json() as Promise<AcceptedOfferResponse>)
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
   * Get a document by key.
   * @param key - key of document
   */
  public getDocument(key: string): OfferDocument | undefined {
    // search in all documents
    return [
      ...this.documentsToBeAccepted,
      ...this.documentsToBeSigned,
      ...this.documentsServices,
      ...this.documentsCommodities,
    ].find(document => document.key === key)
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
        this.documents = { ...this.documents }
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
   * @param key - ID of document
   */
  @action public acceptDocument(key: string): void {
    const document = this.getDocument(key)

    if (!document) return

    document.accepted = !document.accepted
    this.documents = { ...this.documents }
  }

  /**
   * Change `accepted` state of all documents to true.
   * If all documents are already accepted => set to false.
   */
  @action public acceptAllDocuments(): void {
    if (!this.documentsToBeAccepted.length) {
      return
    }

    let acceptedDocuments: OfferDocument[] = []

    if (this.allDocumentsAreAccepted) {
      acceptedDocuments = this.documentsToBeAccepted.map(document => ({
        ...document,
        accepted: false,
      }))
    } else {
      acceptedDocuments = this.documentsToBeAccepted.map(document => ({
        ...document,
        accepted: true,
      }))
    }

    // update array with all accepted documents
    const accept = Object.assign({}, this.documents.acceptance?.accept, {
      files: acceptedDocuments,
    })
    // merge with acceptance object
    const acceptance = Object.assign({}, this.documents.acceptance, { accept })

    // merge with documents object
    this.documents = {
      ...this.documents,
      acceptance,
    }
  }

  /**
   * Map `CustomFile` items to `UserDocument` shape and append the files to the given category.
   * @param files - array of `CustomFile` objects
   * @param category - category name
   */
  @action public addUserFiles(files: CustomFile[], category: string): void {
    // remap files to the `UserDocument` shape
    const newDocuments = files.map(({ file, error }) => {
      if (error) {
        return new UserDocument(file, generateId(), true, error)
      }

      return new UserDocument(file, generateId())
    })

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

    // if document is still uploading or was rejected => do not send the request
    if (!document || document.uploading || document.error) {
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

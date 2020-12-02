import {
  AcceptedOfferResponse,
  CustomFile,
  Group,
  NewOfferResponse,
  OfferDocuments,
  OfferDocument,
  OfferType,
  OfferBox,
  GiftsBox,
  UploadDocumentPromise,
  Acceptance,
  UploadDocumentResponse,
} from '@types'
import { UserDocument } from './'
import { action, computed, observable, reaction } from 'mobx'
import { generateId } from '@utils'

export class OfferStore {
  public offerUrl = ''
  public uploadDocumentUrl = ''
  public removeDocumentUrl = ''
  public errorPageUrl = ''
  public acceptOfferUrl = ''
  public maxUploadGroupSize = 0
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

  @observable
  public acceptance: Acceptance = { params: [] }

  @observable
  public isAccepting = false

  constructor(type: OfferType, offerUrl: string) {
    this.offerUrl = offerUrl
    this.type = type

    reaction(
      () => this.allDocumentsAreAccepted,
      (allAccepted: boolean) => {
        if (!this.documentsToBeAccepted.length) return

        // all documents have the same group id
        const group = this.documentsToBeAccepted[0].group
        this.updateAcceptanceGroup(group, allAccepted)
      },
    )

    reaction(
      () => this.allDocumentsAreSigned,
      (allSigned: boolean) => {
        if (!this.documentsToBeSigned.length) return

        // all documents have the same group id
        const group = this.documentsToBeSigned[0].group
        this.updateAcceptanceGroup(group, allSigned)
      },
    )

    reaction(
      () => this.allProductsDocumentsAreAccepted,
      (allAccepted: boolean) => {
        if (!this.documentsProducts.length) return

        // all documents have the same group id
        const group = this.documentsProducts[0].group
        this.updateAcceptanceGroup(group, allAccepted)
      },
    )
  }

  /**
   * Set a acceptance group as accepted based on `groupAccepted` value.
   * @param group - a group GUID.
   * @param groupAccepted - boolean indicating whether the whole group should be marked as accepted.
   */
  private updateAcceptanceGroup(group: string, groupAccepted: boolean): void {
    this.acceptance = {
      ...this.acceptance,
      params: this.acceptance.params.map(p => ({
        ...p,
        accepted: p.group === group ? groupAccepted : p.accepted,
      })),
    }
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

  @computed public get documentsProducts(): OfferDocument[] {
    return this.documents.other?.products?.files ?? []
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

  @computed public get allProductsDocumentsAreAccepted(): boolean {
    if (!this.documentsProducts.length) {
      return true
    }

    if (this.documentsProducts.find(document => document.mandatory && !document.accepted)) {
      return false
    }

    return true
  }

  /** True if documents were uploaded to all mandatory sections. */
  @computed public get allDocumentsUploaded(): boolean {
    // if there are no upload groups => nothing to validate
    if (!this.documents.uploads || !this.documents.uploads?.types.length) {
      return true
    }

    let allUploaded = true

    // iterate over all mandatory upload groups
    this.documents.uploads.types
      .filter(type => type.mandatory)
      .forEach(group => {
        // no documents found
        if (!this.userDocuments[group.id] || !this.userDocuments[group.id].length) {
          allUploaded = false
          return
        }

        // if all documents are still untouched or uploading or have error => group is not valid
        if (this.userDocuments[group.id].every(doc => !doc.touched || doc.uploading || doc.error)) {
          allUploaded = false
        }
      })

    return allUploaded
  }

  /** True if all conditions for particular offer were fullfilled, otherwise false. */
  @computed public get isOfferReadyToAccept(): boolean {
    // if we don't have a single section under `documents` => there is nothing to accept
    if (!this.offerFetched) {
      return false
    }

    // TODO: implement checking of mandatory upload sections

    return (
      this.allDocumentsAreAccepted &&
      this.allDocumentsAreSigned &&
      this.allProductsDocumentsAreAccepted &&
      this.allServicesDocumentsAreAccepted &&
      this.allDocumentsUploaded
    )
  }

  @computed public get uploadGroupSizeExceeded(): boolean {
    if (this.maxUploadGroupSize === 0) {
      return false
    }

    // sum sizes from all upload groups
    const totalSize = this.documents.uploads?.types.reduce((sum, group) => sum + group.size, 0) ?? 0

    return totalSize >= this.maxUploadGroupSize
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
      products: !documents.other?.products
        ? null
        : Object.assign({}, documents.other?.products, {
            files: this.enrichDocuments(documents.other?.products?.files),
          }),
      services: !documents.other?.services
        ? null
        : Object.assign({}, documents.other?.services, {
            files: this.enrichDocuments(documents.other?.services?.files),
          }),
    })

    // enrich upload group/type with extra attribute `size`
    const uploads = !documents.uploads
      ? null
      : Object.assign({}, documents.uploads, {
          ...documents.uploads,
          types: documents.uploads.types.map(type => ({
            ...type,
            size: 0,
          })),
        })

    return {
      uploads,
      acceptance,
      other,
    }
  }

  /**
   * Performs an ajax request to `offerUrl` provided in constructor, fetch and populate documents.
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
          jsonResponse = await(response.json() as Promise<NewOfferResponse>)
          this.documents = this.enrichDocumentsResponse(jsonResponse.documents)
          this.perex = jsonResponse.perex
          this.gifts = jsonResponse.gifts
          this.benefits = jsonResponse.benefits
          // enrich acceptance groups with `accepted` key
          this.acceptance = {
            ...jsonResponse.acceptance,
            params: jsonResponse.acceptance.params.map(p => ({ ...p, accepted: false })),
          }
          break

        case OfferType.ACCEPTED:
          jsonResponse = await(response.json() as Promise<AcceptedOfferResponse>)
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
      ...this.documentsProducts,
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

    const response = await fetch(`${signFileUrl}/${id}`, {
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
  }

  /**
   * Change `accepted` state of all given documents to true.
   * If all the documents are already accepted => set to false.
   */
  @action public acceptAllDocuments(documents: OfferDocument[]): void {
    if (!documents.length) {
      return
    }

    // let's assume all `documents` are accepted
    let allAccepted = true

    // if at least one document which isn't accepted is found,
    // mark the whole group of documents as not accepted
    allAccepted = !documents.find(document => !document.accepted)

    // then just update the `accepted` flag of each document
    documents.forEach(document => {
      document.accepted = !allAccepted
    })
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
    return this.userDocuments[category]?.find(document => document.key === id)
  }

  /**
   * Remove user document by its key and category.
   * @param document - `UserDocument`
   * @param category - category name
   */
  @action public async removeUserDocument(key: string, category: string): Promise<void> {
    if (!this.userDocuments[category]) {
      return
    }

    const document = this.getUserDocument(key, category)

    this.userDocuments[category] = this.userDocuments[category].filter(doc => doc.key !== key)

    // if document is still uploading or was rejected => do not send the request
    if (!document || document.uploading || document.error) {
      return
    }

    // if document has been successfully uploaded => sends a request to remove it
    const response = await fetch(`${this.removeDocumentUrl}/${category}?f=${key}`, {
      method: 'DELETE',
      headers: { Accept: 'application/json' },
    })

    if (!response.ok) {
      return
    }

    const { size } = (await response.json()) as UploadDocumentResponse
    this.setUploadGroupSize(category, size)
  }

  /**
   * Sends a request with the document (file) to upload API.
   * @param document - file
   * @returns `Promise` in shape of `UploadDocumentPromise`
   */
  public async uploadDocument(
    document: UserDocument,
    category: string,
  ): Promise<UploadDocumentPromise> {
    const formData = new FormData()
    const { file } = document
    formData.append('file', file, file.name)
    formData.append('key', document.key)

    const controller = new AbortController()

    document.touched = true // change the `touched` state of the document
    document.controller = controller // add a control of the following request to the document
    document.uploading = true

    try {
      const response = await fetch(`${this.uploadDocumentUrl}/${category}`, {
        method: 'POST',
        headers: { Accept: 'application/json' },
        body: formData,
        signal: controller.signal,
      })

      // handle unexpected statuses
      if (response.status !== 200 && response.status !== 400) {
        throw new Error(`FAILED TO UPLOAD DOCUMENT - ${response.statusText} (${response.status})`)
      }

      // all other statuses than 200 are considered as upload failure
      const uploaded = response.status === 200

      // TODO: get some nice application error message
      if (!uploaded) {
        throw new Error(`${response.statusText} (${response.status})`)
      }

      const { size } = (await response.json()) as UploadDocumentResponse
      this.setUploadGroupSize(category, size)

      return Promise.resolve({ uploaded: true })
    } catch (error) {
      let message = undefined

      if (typeof error === 'string') {
        message = error
      } else if (error instanceof Error) {
        message = error.message
      }

      document.error = message // mark the document as invalid (upload failed)

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

  @action private setUploadGroupSize(id: string, size: number): void {
    const group = this.documents.uploads?.types.find(group => group.id === id)

    if (!group) return

    group.size = size
  }

  /**
   * Returns array of keys of all accepted or signed documents.
   * @param documents - an array of `OfferDocument` items.
   */
  private getAcceptedKeys(documents: Array<OfferDocument>): string[] {
    return documents.filter(d => d.accepted || d.signed).map(d => d.key)
  }

  /**
   * Performs an ajax request to `acceptOfferUrl`.
   * @returns Promise<boolean> - true if offer was successfully accepted, false otherwise.
   */
  public async acceptOffer(url = this.acceptOfferUrl): Promise<boolean> {
    if (!this.isOfferReadyToAccept) {
      return false
    }

    // get keys for all uploaded files in all groups
    const uploadedKeys = Object.values(this.userDocuments).reduce((keys: string[], docs) => {
      // filter out all failed uploads
      const uploadedDocs = docs.filter(d => !d.error)
      return [...keys, ...uploadedDocs.map(d => d.key)]
    }, [])

    const data = {
      accepted: this.getAcceptedKeys(this.documentsToBeAccepted),
      signed: this.getAcceptedKeys(this.documentsToBeSigned),
      uploaded: uploadedKeys,
      other: [
        ...this.getAcceptedKeys(this.documentsProducts),
        ...this.getAcceptedKeys(this.documentsServices),
      ],
    }

    this.isAccepting = true

    try {
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      })

      if (!response.ok) {
        throw new Error(`FAILED TO ACCEPT OFFER - ${response.statusText}`)
      }

      return true
    } catch (error) {
      // eslint-disable-next-line no-console
      console.error(error.toString())
      return false
    } finally {
      this.isAccepting = false
    }
  }
}

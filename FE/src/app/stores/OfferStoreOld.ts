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
  AcceptanceGroup,
  OfferErrorResponse,
  UploadDocumentErrorResponse,
  BenefitsBox,
  QueryParams,
} from '@types'
import { UserDocument } from '.'
import { action, computed, observable } from 'mobx'
import { generateId, parseUrl } from '@utils'

export class OfferStoreOld {
  public offerUrl = ''
  public uploadDocumentUrl = ''
  public removeDocumentUrl = ''
  public errorPageUrl = ''
  public sessionExpiredPageUrl = ''
  public acceptOfferUrl = ''
  public cancelOfferUrl = ''
  public maxUploadGroupSize = 0
  public isSupplierMandatory = false
  private globalQueryParams: QueryParams
  private type: OfferType

  @observable
  public documents: OfferDocuments = {}

  @observable
  public documentGroups: Group[] = []

  @observable
  public error = false

  @observable
  public errorMessage = ''

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
  public benefits: BenefitsBox | undefined

  @observable
  public gifts: GiftsBox | undefined

  @observable
  public acceptance: Acceptance = { params: [] }

  @observable
  public isAccepting = false

  @observable
  public forceReload = false

  @observable
  public supplier = ''

  @observable
  public isCancelling = false

  @observable
  public isUnfinishedOfferModalOpen = false

  constructor(type: OfferType, offerUrl: string, guid: string) {
    this.offerUrl = offerUrl
    this.type = type
    this.globalQueryParams = { guid }
  }

  //TODO: will be in accepted offer
  @computed public get offerFetched(): boolean {
    return Boolean(Object.keys(this.documents).length)
  }

  //TODO: removed
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

    // first check if we have some mandatory groups
    if (this.documents.acceptance?.sign?.mandatoryGroups.length) {
      return this.allGroupsAccepted(
        this.documents.acceptance.sign.mandatoryGroups,
        this.documentsToBeSigned,
      )
    }

    const groups = this.createGroups(this.documentsToBeSigned)

    return this.allGroupsAccepted(groups, this.documentsToBeSigned)
  }

  /** True if all documents that are mandatory are actually accepted, otherwise false. */
  @computed public get allDocumentsAreAccepted(): boolean {
    if (!this.documentsToBeAccepted.length) {
      return true
    }

    // first check if we have some mandatory groups
    if (this.documents.acceptance?.accept?.mandatoryGroups.length) {
      return this.allGroupsAccepted(
        this.documents.acceptance.accept.mandatoryGroups,
        this.documentsToBeAccepted,
      )
    }

    const groups = this.createGroups(this.documentsToBeAccepted)

    return this.allGroupsAccepted(groups, this.documentsToBeAccepted)
  }

  @computed public get allServicesDocumentsAreAccepted(): boolean {
    if (!this.documentsServices.length) {
      return true
    }

    // first check if we have some mandatory groups
    if (this.documents.other?.services?.mandatoryGroups.length) {
      return this.allGroupsAccepted(
        this.documents.other.services.mandatoryGroups,
        this.documentsServices,
      )
    }

    // if there is no mandatory group, but one document from one particular group
    // is checked => the whole group of documents must be checked

    // "create" an array of mandatory groups based on which documents are currently accepted
    const groups = this.createGroups(this.documentsServices)

    return this.allGroupsAccepted(groups, this.documentsServices)
  }

  @computed public get allProductsDocumentsAreAccepted(): boolean {
    // if we don't have any product => consider this section as valid
    if (!this.documentsProducts.length) {
      return true
    }

    // first check if we have some mandatory groups
    if (this.documents.other?.products?.mandatoryGroups.length) {
      return this.allGroupsAccepted(
        this.documents.other.products.mandatoryGroups,
        this.documentsProducts,
      )
    }

    // if there is no mandatory group, but one document from one particular group
    // is checked => the whole group of documents must be checked

    // "create" an array of mandatory groups based on which documents are currently accepted
    const groups = this.createGroups(this.documentsProducts)

    return this.allGroupsAccepted(groups, this.documentsProducts)
  }

  /**
   * Returns an array of unique group IDs based on documents that are currently accepted.
   * e.g. two documents from the same group are accepted => array contains a single group ID.
   * e.g. none of the documents are accepted => array is empty
   * e.g. two documents from two different groups are accepted => array contains two unique group IDs
   * @param documents - an array of `OfferDocument`
   */
  private createGroups(documents: OfferDocument[]): string[] {
    return documents
      .filter(document => document.accepted)
      .reduce((acc: string[], document) => {
        if (!acc.includes(document.group)) {
          return [...acc, document.group]
        }
        return acc
      }, [])
  }

  /**
   * Check if all documents with the same group IDs are accepted or not.
   * If at least one document is not accepted => all groups are not valid.
   * @param groups - an array of mandatory group IDs
   * @param documents - an array of `OfferDocument`
   */
  private allGroupsAccepted(groups: string[], documents: OfferDocument[]): boolean {
    if (!groups.length) {
      return true
    }

    let allGroupsAccepted = true

    // iterate over all the groups
    groups.forEach(group => {
      // if at least one document within the current group is unaccepted => the whole section is invalid
      if (documents.filter(d => d.group === group).some(document => !document.accepted)) {
        allGroupsAccepted = false
        return
      }
    })

    return allGroupsAccepted
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

  /**
   * Returns all acceptance groups with updated `accepted` flag based on whether all the documents
   * from the particular group are accepted or not.
   */
  @computed public get acceptanceGroups(): AcceptanceGroup[] {
    const groups = this.acceptance.params.map(({ title, group }) => {
      const docs = this.getDocuments(group)
      const groupAccepted = docs.every(d => d.accepted)

      return {
        title,
        group,
        accepted: groupAccepted,
      }
    })

    return groups
  }

  /** True if all conditions for particular offer were fullfilled, otherwise false. */
  @computed public get isOfferReadyToAccept(): boolean {
    // if we don't have a single section under `documents` => there is nothing to accept
    if (!this.offerFetched) {
      return false
    }

    return (
      this.allDocumentsAreAccepted &&
      this.allDocumentsAreSigned &&
      this.allProductsDocumentsAreAccepted &&
      this.allServicesDocumentsAreAccepted &&
      this.allDocumentsUploaded &&
      this.isSupplierSelected
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
   * Returns true if user has already did some action e.g. accept or upload any document.
   */
  @computed public get isOfferDirty(): boolean {
    // if at least one document was accepted => the offer is dirty
    if (this.getAllDocuments().some(document => document.accepted)) {
      return true
    }

    // if at least one document was uploaded (or is uploading) => the offer is dirty
    if (Object.values(this.userDocuments).some(docs => docs.length)) {
      return true
    }

    return false
  }

  /**
   * Returns true if the current supplier has been already selected.
   */
  @computed public get isSupplierSelected(): boolean {
    // skip validation if a supplier is not mandatory
    if (!this.isSupplierMandatory) {
      return true
    }

    return this.supplier !== ''
  }

  /**
   * Since the `accepted` key is not present in the JSON response on `OfferDocument` object,
   * here I add it manually.
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

    const description = documents.description ? documents.description : ''

    return {
      uploads,
      acceptance,
      other,
      description,
    }
  }

  /**
   * Enrich all the acceptance groups/params with an extra key - `accepted`.
   * And change the order of the groups to match the order of rendered boxes on the page.
   */
  private enrichAndSortAcceptanceParams(acceptance: Acceptance): Acceptance {
    const accParams: Acceptance = {
      params: [],
    }

    if (this.documentsToBeAccepted.length) {
      // all this documents have the same group, so we can take the first one
      const accGroup = acceptance.params.find(
        param => param.group === this.documentsToBeAccepted[0].group,
      )
      accGroup && accParams.params.push(accGroup)
    }

    if (this.documentsToBeSigned.length) {
      // all this documents have the same group, so we can take the first one
      const signGroup = acceptance.params.find(
        param => param.group === this.documentsToBeSigned[0].group,
      )
      // signGroup might have the same ID as accGroup
      // so first check if we already have such ID
      if (signGroup && !accParams.params.find(p => p.group === signGroup.group)) {
        accParams.params.push(signGroup)
      }
    }

    // this documents have different groups
    this.documentsServices.forEach(doc => {
      const serviceGroup = acceptance.params.find(param => param.group === doc.group)
      serviceGroup && accParams.params.push(serviceGroup)
    })

    if (this.documentsProducts.length) {
      // all this documents have the same group, so we can take the first one
      const productGroup = acceptance.params.find(
        param => param.group === this.documentsProducts[0].group,
      )
      productGroup && accParams.params.push(productGroup)
    }

    // enrich acceptance groups with `accepted` key
    return {
      ...accParams,
      params: accParams.params.map(p => ({ ...p, accepted: false })),
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

      const response = await fetch(parseUrl(this.offerUrl, this.globalQueryParams), {
        headers: { Accept: 'application/json' },
        signal: controller?.signal || null,
      })

      fetchTimeout && clearTimeout(fetchTimeout)

      // redirect to error page when 404 response
      if (response.status === 404) {
        this.forceReload = true
        window.location.href = this.errorPageUrl
        return
      }

      // handle 5xx statuses and custom error message
      if (response.status.toString().startsWith('5')) {
        const { Message } = await (response.json() as Promise<OfferErrorResponse>)
        this.errorMessage = Message
        throw new Error(Message)
      }

      // the rest of the statuses are treated as unknown errors
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
          this.acceptance = this.enrichAndSortAcceptanceParams(jsonResponse.acceptance)
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
      console.error(String(error))
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
    return this.getAllDocuments().find(document => document.key === key)
  }

  /**
   * Get documents by group.
   * @param group - group ID
   */
  public getDocuments(group: string): OfferDocument[] {
    return this.getAllDocuments().filter(document => document.group === group)
  }

  public getAllDocuments(): OfferDocument[] {
    return [
      ...this.documentsToBeAccepted,
      ...this.documentsToBeSigned,
      ...this.documentsServices,
      ...this.documentsProducts,
    ]
  }

  /**
   * Change `signed` state of given document after the signature is successfully sent to API.
   * @param key - ID of document
   * @param signature - PNG image as base64
   * @param signFileUrl - URL where to send `signature` data
   * @returns Promise<boolean> - if true, the document was successfully signed, otherwise false.
   */
  @action public async signDocument(
    key: string,
    signature: string,
    signFileUrl: string,
  ): Promise<boolean> {
    const document = this.getDocument(key)

    if (!document) return false

    this.isSigning = true
    this.signError = false

    let signed = false

    await this.signDocumentRequest(key, signature, signFileUrl)
      .then(() => {
        document.accepted = true
        signed = true
      })
      .catch(() => {
        this.signError = true
        signed = false
      })

    this.isSigning = false

    return signed
  }

  /**
   * Send a request with signature data to sign API.
   * @param key - ID of document
   * @param signature - PNG image as base64
   * @param signFileUrl - URL where to send `signature` data
   * @returns Promise<void>
   */
  private async signDocumentRequest(
    key: string,
    signature: string,
    signFileUrl: string,
  ): Promise<void> {
    const data = {
      signature,
    }

    const response = await fetch(parseUrl(`${signFileUrl}/${key}`, this.globalQueryParams), {
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

  @action public selectSupplier(supplier: string): void {
    this.supplier = supplier
  }

  /** Set state of "unfinishedOfferModal" */
  @action public setIsUnfinishedOfferModalOpen(isUnfinishedOfferModalOpen: boolean): void {
    this.isUnfinishedOfferModalOpen = isUnfinishedOfferModalOpen
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
    const response = await fetch(
      parseUrl(`${this.removeDocumentUrl}/${category}`, { ...this.globalQueryParams, f: key }),
      {
        method: 'DELETE',
        headers: { Accept: 'application/json' },
      },
    )

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
      const response = await fetch(
        parseUrl(`${this.uploadDocumentUrl}/${category}`, this.globalQueryParams),
        {
          method: 'POST',
          headers: { Accept: 'application/json' },
          body: formData,
          signal: controller.signal,
        },
      )

      // handle unexpected statuses
      if (response.status !== 200 && response.status !== 400 && response.status !== 401) {
        throw new Error(`FAILED TO UPLOAD DOCUMENT - ${response.statusText} (${response.status})`)
      }

      // all other statuses than 200 are considered as upload failure
      const uploaded = response.status === 200

      if (response.status === 401) {
        this.forceReload = true
        window.location.href = this.sessionExpiredPageUrl
        // @ts-ignore
        return
      }

      // parse error message from API
      if (!uploaded) {
        const { Message } = (await response.json()) as UploadDocumentErrorResponse
        throw new Error(Message)
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
   * Returns array of keys of all accepted documents.
   * @param documents - an array of `OfferDocument` items.
   */
  private getAcceptedKeys(documents: Array<OfferDocument>): string[] {
    return documents.filter(d => d.accepted).map(d => d.key)
  }

  /**
   * Performs an ajax request to `acceptOfferUrl`.
   * @returns Promise<boolean> - true if offer was successfully accepted, false otherwise.
   */
  public async acceptOffer(url = this.acceptOfferUrl): Promise<boolean> {
    if (!this.isOfferReadyToAccept) {
      return false
    }

    // transform uploaded files into a different data structure that BE understands
    const uploadedGroups = Object.entries(this.userDocuments).map(([group, docs]) => {
      // filter out all failed uploads
      const uploadedDocs = docs.filter(d => !d.error)
      return {
        group,
        keys: uploadedDocs.map(d => d.key),
      }
    })

    const data = {
      accepted: this.getAcceptedKeys(this.documentsToBeAccepted),
      signed: this.getAcceptedKeys(this.documentsToBeSigned),
      uploaded: uploadedGroups,
      other: [
        ...this.getAcceptedKeys(this.documentsProducts),
        ...this.getAcceptedKeys(this.documentsServices),
      ],
      supplier: this.supplier ? this.supplier : null,
    }

    this.isAccepting = true

    try {
      const response = await fetch(parseUrl(url, this.globalQueryParams), {
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
      this.isAccepting = false

      // eslint-disable-next-line no-console
      console.error(String(error))
      return false
    }
  }

  /**
   * Performs an ajax request to `cancelOfferUrl`.
   * @returns Promise<boolean> - true if offer was successfully canceled, false otherwise.
   */
  public async cancelOffer(url = this.cancelOfferUrl): Promise<boolean> {
    this.isCancelling = true

    try {
      const response = await fetch(parseUrl(url, this.globalQueryParams))

      if (!response.ok) {
        throw new Error(`FAILED TO CANCEL OFFER - ${response.statusText}`)
      }

      return true
    } catch (error) {
      this.isCancelling = false

      // eslint-disable-next-line no-console
      console.error(String(error))
      return false
    }
  }
}

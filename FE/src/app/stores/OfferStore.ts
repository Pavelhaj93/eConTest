import {
  AcceptedOfferResponse,
  CommodityProductType,
  NewOfferResponse,
  OfferErrorResponse,
  OfferType,
  QueryParams,
  StoredUploadFile,
  StoredUploadFileGroup,
} from '@types'
import { action, computed, observable } from 'mobx'
import { parseUrl } from '../utils/strings'

export class OfferStore {
  public offerUrl = ''
  public errorPageUrl = ''
  public sessionExpiredPageUrl = ''
  public acceptOfferUrl = ''
  public cancelOfferUrl = ''
  public isSupplierMandatory = false
  private globalQueryParams: QueryParams
  private type: OfferType

  @observable
  public acceptedDocumentGroups: AcceptedOfferResponse.AcceptedGroup[] = []

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
  public isAccepting = false

  @observable
  public forceReload = false

  @observable
  public supplier = ''

  @observable
  public isCancelling = false

  @observable
  public isUnfinishedOfferModalOpen = false

  @observable
  public consumption: NewOfferResponse.ResponseItem | undefined = undefined

  @observable
  public perex: NewOfferResponse.ResponseItem[] | undefined = undefined

  @observable
  public docsCheck: NewOfferResponse.ResponseItem[] | undefined = undefined

  @observable
  public docsSign: NewOfferResponse.ResponseItem[] | undefined = undefined

  @observable
  public confirm: NewOfferResponse.ResponseItem | undefined = undefined

  @observable
  public storedUploads = JSON.parse(localStorage.getItem('uploadedFiles') ?? '[]')

  constructor(type: OfferType, offerUrl: string, guid: string) {
    this.type = type
    this.globalQueryParams = { guid }
    this.offerUrl = offerUrl
  }

  /**
   * Returns all data from the offer response and sorts them by position.
   */
  @computed
  public get concatedSortedData(): NewOfferResponse.ResponseItem[] {
    return [
      ...(this.consumption ? [this.consumption] : []),
      ...(this.docsCheck ?? []),
      ...(this.docsSign ?? []),
      ...(this.confirm ? [this.confirm] : []),
    ].sort((a, b) => a.position - b.position)
  }

  /**
   * Returns true if the offer has been fetched.
   * The offer is fetched if at least one response item is present.
   * @returns {boolean}
   */
  @computed public get offerFetched(): boolean {
    return Boolean(Object.keys(this.docsCheck?.concat(this.docsSign ?? []) ?? {}).length)
  }

  /**
   * Returns array of file arrays from all docCheck groups.
   */
  @computed
  public get docGroupsToBeChecked(): NewOfferResponse.File[][] {
    return this.docsCheck?.map(item => item.body.docs?.files) ?? []
  }

  /**
   * Returns array of file arrays from all docCheck groups which are related to commodities. So type of docsCheck is either GAS or ELECTRICITY
   * Just for acceptOffer API purposes. there is a need to separate product documents and documents which are related to other services
   */
  @computed
  public get productDocGroupsToBeChecked(): NewOfferResponse.File[][] {
    return (
      this.docsCheck
        ?.filter(
          item =>
            (item.type === NewOfferResponse.ResponseItemType.DocsCheck &&
              item.header.type?.includes(CommodityProductType.GAS)) ??
            item.header.type?.includes(CommodityProductType.ELECTRICITY),
        )
        .map(item => item.body.docs.files) ?? []
    )
  }

  /**
   * Returns array of file arrays from all docCheck groups which are related to other services. So just pure type of docsCheck
   * Just for acceptOffer API purposes. there is a need to separate product documents and documents which are related to other services
   */
  @computed
  public get otherDocGroupsToBeChecked(): NewOfferResponse.File[][] {
    return (
      this.docsCheck
        ?.filter(
          item => item.type === NewOfferResponse.ResponseItemType.DocsCheck && !item.header.type,
        )
        .map(item => item.body.docs.files) ?? []
    )
  }

  /**
   * Returns array of file arrays from all docSign groups.
   */
  @computed
  public get docGroupsToBeSigned(): NewOfferResponse.File[][] {
    return this.docsSign?.map(item => item.body.docs?.files) ?? []
  }

  /**
   * Filter docGroupsToBeChecked based on their mandatory status.
   */
  @computed
  public get mandatoryDocumentsToBeChecked(): (NewOfferResponse.File | undefined)[] {
    return this.filterMandatoryDocuments(this.docGroupsToBeChecked)
  }

  /**
   * Filter docGroupsToBeSigned based on their mandatory status.
   */
  @computed
  public get mandatoryDocumentsToBeSigned(): (NewOfferResponse.File | undefined)[] {
    return this.filterMandatoryDocuments(this.docGroupsToBeSigned)
  }

  /**
   * Returns true if the current supplier has been already selected.
   */
  @computed
  public get isSupplierSelected(): boolean {
    // skip validation if a supplier is not mandatory
    if (!this.isSupplierMandatory) {
      return true
    }

    return this.supplier !== ''
  }

  /**
   * Returns true if all mandatory documents have been accepted.
   */
  @computed
  public get allDocumentsAreChecked(): boolean {
    if (!this.docGroupsToBeChecked.length) return true

    return this.mandatoryDocumentsToBeChecked.every(file => file?.accepted)
  }

  /**
   * Returns true if all mandatory documents have been signed.
   */
  @computed
  public get allDocumentsAreSigned(): boolean {
    if (!this.docGroupsToBeSigned.length) return true

    return this.mandatoryDocumentsToBeSigned.every(file => file?.accepted)
  }

  /**
   * Returns true if the offer is ready to be accepted.
   * The offer is ready to be accepted if:
   * - the offer has been fetched
   * - all mandatory documents are checked
   * - all mandatory documents are signed
   * - the supplier is selected
   */
  @computed
  public get isOfferReadyToAccept(): boolean {
    if (!this.offerFetched) return false

    return this.allDocumentsAreChecked && this.allDocumentsAreSigned && this.isSupplierSelected
  }

  /**
   * Return groups of documents that are accepted and these groups are used in confirmation modal.
   * If all documents in a group are accepted, the group is accepted.
   */
  @computed public get acceptanceGroups(): NewOfferResponse.AcceptanceGroup[] | undefined {
    const groups = this.confirm?.body.params?.map(({ title, group }) => {
      const docsGroup = this.getDocumentGroup(group)

      // first find if there are some mandatory documents in the group
      const groupHasMandatory = docsGroup?.some(doc => doc.mandatory)

      let groupAccepted

      // if there are some mandatory documents in the group, then all of them must be accepted
      if (groupHasMandatory) {
        groupAccepted = docsGroup.filter(doc => doc.mandatory).every(doc => doc.accepted)
      } else {
        // if there are no mandatory documents in the group, then all of them must be accepted
        groupAccepted = docsGroup.every(doc => doc.accepted)
      }

      return {
        title,
        group,
        accepted: groupAccepted,
      }
    })

    return groups
  }

  /**
   * Returns true if the offer is dirty.
   * The offer is dirty if at least one document was accepted or signed.
   */
  @computed
  public get isOfferDirty(): boolean {
    // if at least one document was accepted => the offer is dirty
    if (this.getAllDocuments().some(document => document.accepted)) {
      return true
    }

    return false
  }

  /**
   * This method is used to filter groups of documents based on the current group string.
   * @param group - group of documents
   * @returns filtered documents
   */
  public getDocumentGroup(group: NewOfferResponse.Param['group']): NewOfferResponse.File[] {
    return this.getAllDocuments().filter(document => document.group === group)
  }

  /**
   * This method is used to getDocument from documents as an input which can be either documentsToBeChecked or documentsToBeSigned.
   * @param key - key of the document
   * @param documents - documents to be checked or signed
   * @returns document or undefined
   */
  public getDocument(
    key: string,
    documents: NewOfferResponse.File[][],
  ): NewOfferResponse.File | undefined {
    return documents
      .map(docGroup => docGroup?.find(file => file.key === key))
      .find(doc => doc !== undefined)
  }

  /**
   * This method is used to concat files from docGroupsToBeChecked and docGroupsToBeSigned.
   * @returns all files
   */
  public getAllDocuments(): NewOfferResponse.File[] {
    return [
      ...this.docGroupsToBeChecked.map(docGroup => docGroup?.map(files => files).flat()).flat(),
      ...this.docGroupsToBeSigned.map(docGroup => docGroup?.map(files => files).flat()).flat(),
    ]
  }

  /**
   * This method is used to filter mandatory documents from docGroupsToBeChecked or docGroupsToBeSigned.
   * @param documents - documents to be checked or signed
   * @returns mandatory documents
   */
  public filterMandatoryDocuments(documents: NewOfferResponse.File[][]): NewOfferResponse.File[] {
    return documents
      .map(docGroup => docGroup?.map(files => files).flat())
      .flat()
      .filter(file => file.mandatory)
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
  private enrichToBeCheckedDocuments(files: NewOfferResponse.File[]) {
    const storedKeys = localStorage.getItem('checkedDocuments')

    return files.map(file => ({
      ...file,
      accepted: storedKeys?.includes(file.key), // check if the document was accepted
    }))
  }

  private enrichToBeSignedDocuments(files: NewOfferResponse.File[]) {
    const storedSignatures = localStorage.getItem('signedDocuments')

    return files.map(file => ({
      ...file,
      accepted: storedSignatures?.includes(file.key), // check if the document was signed
    }))
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
   * This action first uses getDocument from documentsToBeChecked and then check it or uncheck it based on the current state.
   * @param key - key of the document
   */
  @action public checkDocument(key: string): void {
    const document = this.getDocument(key, this.docGroupsToBeChecked)
    if (!document) return
    document.accepted = !document.accepted

    // get all checked documents from localStorage
    const storedKeys = JSON.parse(localStorage.getItem('checkedDocuments') ?? '[]')
    // remove the key from the list of checked documents
    const updatedKeys = storedKeys.filter((k: string) => k !== key) // if key is already present, remove it

    // if document is checked, add it to the list of checked documents
    if (document?.accepted) {
      updatedKeys.push(key)
    }

    // update the list of checked documents in localStorage
    localStorage.setItem('checkedDocuments', JSON.stringify(updatedKeys))
  }

  /**
   * This action accepts files group and then checks all documents in that group or uncheck them based on the current state.
   * @param files - files to be checked
   */
  @action public checkDocumentsGroup(files: NewOfferResponse.File[]): void {
    if (!files.length) return

    // if at least one document which isn't checked is found,
    // mark the whole group of documents as checked
    const allChecked = files.every(file => file.accepted)

    // then just update the `checked` flag of each document
    files.forEach(file => (file.accepted = !allChecked))

    // get all checked documents from localStorage
    const storedKeys = JSON.parse(localStorage.getItem('checkedDocuments') ?? '[]')
    // remove all keys from the list of checked documents
    const updatedKeys = storedKeys.filter((k: string) => !files.map(file => file.key).includes(k)) // remove all keys from the group

    // if at least one document is checked, add all keys from the group
    if (!allChecked) {
      updatedKeys.push(...files.map(file => file.key))
    }

    // update the list of checked documents in localStorage
    localStorage.setItem('checkedDocuments', JSON.stringify(updatedKeys))
  }

  /**
   * This action accept argument of a supplier in confirm modal and then set the state of supplier.
   */
  @action public selectSupplier(supplier: string): void {
    this.supplier = supplier
  }

  /** Set state of "unfinishedOfferModal" */
  @action public setIsUnfinishedOfferModalOpen(isUnfinishedOfferModalOpen: boolean): void {
    this.isUnfinishedOfferModalOpen = isUnfinishedOfferModalOpen
  }

  @action public async signDocument(
    key: string,
    signature: string,
    signFileUrl: string,
  ): Promise<boolean> {
    const document = this.getDocument(key, this.docGroupsToBeSigned)

    if (!document) return false

    this.isSigning = true
    this.signError = false

    let signed = false

    await this.signDocumentRequest(key, signature, signFileUrl)

      .then(() => {
        document.accepted = true

        // retrieve all signed documents from localStorage
        const storedSignatureKeys = JSON.parse(localStorage.getItem('signedDocuments') ?? '[]')
        const updatedSignatureKeys = [...storedSignatureKeys, key] // add key to array of signed documents

        // set new array of signed documents to localStorage
        localStorage.setItem('signedDocuments', JSON.stringify(updatedSignatureKeys))

        signed = true
      })
      .catch(() => {
        this.signError = true
        signed = false
      })
    this.isSigning = false

    return signed
  }

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
          controller?.abort()
        }, timeoutMs)
      }

      const response = await fetch(parseUrl(this.offerUrl, this.globalQueryParams), {
        headers: { Accept: 'application/json' },
        signal: controller?.signal ?? null,
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
          {
            jsonResponse = await (response.json() as Promise<NewOfferResponse.RootObject>)

            // 1. filter out items by type and assign them to the corresponding properties of the store object
            // enrich documents with `accepted` key and set it to false by default (see `enrichDocuments` method)
            // - this is needed for MobX to observe the changes of this key and trigger rerender in React component when the key is changed

            this.consumption = jsonResponse.data.find(
              item => item.type === NewOfferResponse.ResponseItemType.Consumption,
            )

            this.docsCheck = jsonResponse.data
              .filter(item => item.type === NewOfferResponse.ResponseItemType.DocsCheck)
              .map(item => ({
                ...item,
                body: {
                  ...item.body,
                  docs: {
                    ...item.body.docs,
                    files: this.enrichToBeCheckedDocuments(item.body.docs?.files ?? []),
                  },
                },
              }))

            this.docsSign = jsonResponse.data
              .filter(item => item.type === NewOfferResponse.ResponseItemType.DocsSign)
              .map(item => ({
                ...item,
                body: {
                  ...item.body,
                  docs: {
                    ...item.body.docs,
                    files: this.enrichToBeSignedDocuments(item.body.docs?.files ?? []),
                  },
                },
              }))

            this.confirm = jsonResponse.data.find(
              item => item.type === NewOfferResponse.ResponseItemType.Confirm,
            )
          }
          break

        case OfferType.ACCEPTED:
          jsonResponse = await (response.json() as Promise<AcceptedOfferResponse.RootObject>)

          this.acceptedDocumentGroups = jsonResponse.groups
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
   * Return array of all documents that are to be checked.
   */
  public getAllToBeCheckedDocuments(): NewOfferResponse.File[] {
    return this.docGroupsToBeChecked.map(docGroup => docGroup?.map(files => files).flat()).flat()
  }

  /**
   * Return array of all documents that are to be checked related to Products
   * Just for acceptOffer API purposes. there is a need to separate product documents and documents which are related to other services
   */
  public getAllToBeCheckedProductDocuments(): NewOfferResponse.File[] {
    return this.productDocGroupsToBeChecked
      .map(docGroup => docGroup.map(files => files).flat())
      .flat()
  }

  /**
   * Return array of all documents that are to be checked related to other services
   * Just for acceptOffer API purposes. there is a need to separate product documents and documents which are related to other services
   */
  public getAllToBeCheckedOtherDocuments(): NewOfferResponse.File[] {
    return this.otherDocGroupsToBeChecked
      .map(docGroup => docGroup.map(files => files).flat())
      .flat()
  }

  /**
   * Return array of all documents that are to be signed.
   */
  public getAllToBeSignedDocuments(): NewOfferResponse.File[] {
    return this.docGroupsToBeSigned.map(docGroup => docGroup?.map(files => files).flat()).flat()
  }

  /**
   * Return array of keys from documents that are passed in the argument.
   * It is necessary to pass the keys to the backend when accepting the offer.
   * @param documents - array of documents
   * @returns array of keys
   */
  private getAcceptedKeys(documents: NewOfferResponse.File[]): string[] {
    return documents.filter(d => d.accepted).map(d => d.key)
  }

  private getUploadedKeys(
    storedUploads: StoredUploadFileGroup[],
  ): { categoryId: string; keys: string[] }[] {
    const modifiedUploads = storedUploads.map((upload: any) => ({
      categoryId: upload.categoryId,
      keys: upload.userDocuments.map((file: StoredUploadFile) => file.key),
    }))

    return modifiedUploads
  }

  /**
   * clear whole localStorage from saved progress of the current offer
   */
  private async clearLocalStorage(): Promise<void> {
    localStorage.removeItem('checkedDocuments')
    localStorage.removeItem('signedDocuments')
    localStorage.removeItem('uploadedFiles')
  }

  /**
   * Performs an ajax request to `acceptOfferUrl`.
   * @returns Promise<boolean> - true if offer was successfully accepted, false otherwise.
   */
  public async acceptOffer(url = this.acceptOfferUrl): Promise<boolean> {
    if (!this.isOfferReadyToAccept) {
      return false
    }

    const data = {
      accepted: this.getAcceptedKeys(this.getAllToBeCheckedProductDocuments()), // docsCheck with commodity
      signed: this.getAcceptedKeys(this.getAllToBeSignedDocuments()), // all docsSigns
      uploaded: this.getUploadedKeys(this.storedUploads), // all uploads
      other: [this.getAcceptedKeys(this.getAllToBeCheckedOtherDocuments())], // docsCheck without commodity
      supplier: this.supplier ? this.supplier : null, // supplier
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

      await this.clearLocalStorage()

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

      await this.clearLocalStorage()

      return true
    } catch (error) {
      this.isCancelling = false

      // eslint-disable-next-line no-console
      console.error(String(error))
      return false
    }
  }
}

import {
  AcceptedOfferResponse,
  NewOfferResponse,
  OfferErrorResponse,
  OfferType,
  QueryParams,
} from '@types'
import { action, computed, observable } from 'mobx'
import { parseUrl } from '../utils/strings'

export class OfferStore {
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
  public acceptedDocumentGroups: AcceptedOfferResponse.Group[] = []

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
  public perex: NewOfferResponse.ResponseItem[] | undefined = undefined

  @observable
  public benefits: NewOfferResponse.ResponseItem[] | undefined = undefined

  @observable
  public docsCheck: NewOfferResponse.ResponseItem[] | undefined = undefined

  @observable
  public docsSign: NewOfferResponse.ResponseItem[] | undefined = undefined

  @observable
  public confirm: NewOfferResponse.ResponseItem | undefined = undefined

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
      ...(this.perex ?? []),
      ...(this.benefits ?? []),
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
      const docs = this.getDocumentGroup(group)
      const groupAccepted = docs?.every(d => d.accepted)

      return {
        title,
        group: group,
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
  private enrichDocuments(files: NewOfferResponse.File[]) {
    return files.map(file => ({
      ...file,
      accepted: false,
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
    document && (document.accepted = !document.accepted)
  }

  /**
   * This action accepts files group and then checks all documents in that group or uncheck them based on the current state.
   * @param files - files to be checked
   */
  @action public checkDocumentsGroup(files: NewOfferResponse.File[]): void {
    if (!files.length) return

    // if at least one document which isn't checked is found,
    // mark the whole group of documents as checked
    const allChecked = !files.find(document => !document.accepted)

    // then just update the `checked` flag of each document
    files.forEach(file => (file.accepted = !allChecked))
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
            this.perex = jsonResponse.data.filter(
              item => item.type === NewOfferResponse.ResponseItemType.Perex,
            )
            this.benefits = jsonResponse.data.filter(
              item => item.type === NewOfferResponse.ResponseItemType.Benefit,
            )

            this.docsCheck = jsonResponse.data
              .filter(item => item.type === NewOfferResponse.ResponseItemType.DocsCheck)
              .map(item => ({
                ...item,
                body: {
                  ...item.body,
                  docs: {
                    ...item.body.docs,
                    files: this.enrichDocuments(item.body.docs?.files ?? []),
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
                    files: this.enrichDocuments(item.body.docs?.files ?? []),
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

  /**
   * Performs an ajax request to `acceptOfferUrl`.
   * @returns Promise<boolean> - true if offer was successfully accepted, false otherwise.
   */
  public async acceptOffer(url = this.acceptOfferUrl): Promise<boolean> {
    if (!this.isOfferReadyToAccept) {
      return false
    }

    const data = {
      accepted: this.getAcceptedKeys(this.getAllToBeCheckedDocuments()),
      signed: this.getAcceptedKeys(this.getAllToBeSignedDocuments()),
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

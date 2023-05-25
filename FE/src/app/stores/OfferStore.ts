import { NewOfferResponseCopy, OfferErrorResponse, OfferType, QueryParams } from '@types'
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

  //TODO: will be in accepted offer
  // @observable documentGroups: Group[] = []

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
  public perex: NewOfferResponseCopy.ResponseItem[] | undefined = undefined

  @observable
  public benefits: NewOfferResponseCopy.ResponseItem[] | undefined = undefined

  @observable
  public docsCheck: NewOfferResponseCopy.ResponseItem[] | undefined = undefined

  @observable
  public docsSign: NewOfferResponseCopy.ResponseItem[] | undefined = undefined

  @observable
  public confirm: NewOfferResponseCopy.ResponseItem | undefined = undefined

  constructor(type: OfferType, offerUrl: string, guid: string) {
    this.type = type
    this.globalQueryParams = { guid }
    this.offerUrl = offerUrl
  }

  @computed
  public get concatedSortedData(): NewOfferResponseCopy.ResponseItem[] {
    return [
      ...(this.perex || []),
      ...(this.benefits || []),
      ...(this.docsCheck || []),
      ...(this.docsSign || []),
      ...(this.confirm ? [this.confirm] : []),
    ].sort((a, b) => a.position - b.position)
  }

  @computed public get offerFetched(): boolean {
    return Boolean(Object.keys(this.docsCheck?.concat(this.docsSign ?? []) ?? {}).length)
  }

  //TODO: fix TS
  @computed
  public get docGroupsToBeSigned(): NewOfferResponseCopy.File[][] {
    return this.docsSign?.map(item => item.body.docs?.files).flat() ?? []
  }

  //TODO: fix TS
  @computed
  public get docGroupsToBeChecked(): NewOfferResponseCopy.File[][] {
    return this.docsCheck?.map(item => item.body.docs?.files).flat() ?? []
  }

  @computed
  public get mandatoryDocumentsToBeChecked(): (NewOfferResponseCopy.File | undefined)[] {
    return this.filterMandatoryDocuments(this.docGroupsToBeChecked)
  }

  @computed
  public get mandatoryDocumentsToBeSigned(): (NewOfferResponseCopy.File | undefined)[] {
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

  @computed
  public get allDocumentsAreChecked(): boolean {
    if (!this.docGroupsToBeChecked.length) return true

    return this.mandatoryDocumentsToBeChecked.every(file => file?.accepted)
  }

  @computed
  public get allDocumentsAreSigned(): boolean {
    if (!this.docGroupsToBeSigned.length) return true

    return this.mandatoryDocumentsToBeSigned.every(file => file?.accepted)
  }

  @computed
  public get isOfferReadyToAccept(): boolean {
    if (!this.offerFetched) return false

    return this.allDocumentsAreChecked && this.allDocumentsAreSigned && this.isSupplierSelected
  }

  @computed public get acceptanceGroups(): NewOfferResponseCopy.AcceptanceGroup[] | undefined {
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

  @computed
  public get isOfferDirty(): boolean {
    // if at least one document was accepted => the offer is dirty
    if (this.getAllDocuments().some(document => document.accepted)) {
      return true
    }

    return false
  }

  public getDocumentGroup(group: NewOfferResponseCopy.Param['group']): NewOfferResponseCopy.File[] {
    return this.getAllDocuments().filter(document => document.group === group)
  }

  public getDocument(
    key: string,
    documents: NewOfferResponseCopy.File[][],
  ): NewOfferResponseCopy.File | undefined {
    return documents
      .map(docGroup => docGroup?.find(file => file.key === key))
      .find(doc => doc !== undefined)
  }

  public getAllDocuments(): NewOfferResponseCopy.File[] {
    return [
      ...this.docGroupsToBeChecked.map(docGroup => docGroup?.map(files => files).flat()).flat(),
      ...this.docGroupsToBeSigned.map(docGroup => docGroup?.map(files => files).flat()).flat(),
    ]
  }

  public filterMandatoryDocuments(
    documents: NewOfferResponseCopy.File[][],
  ): NewOfferResponseCopy.File[] {
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
  private enrichDocuments(files: NewOfferResponseCopy.File[]) {
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

  @action public checkDocument(key: string): void {
    const document = this.getDocument(key, this.docGroupsToBeChecked)
    document && (document.accepted = !document.accepted)
  }

  @action public checkDocumentsGroup(files: NewOfferResponseCopy.File[]): void {
    if (!files.length) return

    // if at least one document which isn't checked is found,
    // mark the whole group of documents as checked
    const allChecked = !files.find(document => !document.accepted)

    // then just update the `checked` flag of each document
    files.forEach(file => (file.accepted = !allChecked))
  }

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

      if (OfferType.NEW) {
        jsonResponse = await (response.json() as Promise<NewOfferResponseCopy.RootObject>)

        // this.documents = this.enrichDocumentsResponse(jsonResponse.documents)
        this.perex = jsonResponse.data.filter(
          item => item.type === NewOfferResponseCopy.ResponseItemType.Perex,
        )
        this.benefits = jsonResponse.data.filter(
          item => item.type === NewOfferResponseCopy.ResponseItemType.Benefit,
        )

        this.docsCheck = jsonResponse.data
          .filter(item => item.type === NewOfferResponseCopy.ResponseItemType.DocsCheck)
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
          .filter(item => item.type === NewOfferResponseCopy.ResponseItemType.DocsSign)
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
          item => item.type === NewOfferResponseCopy.ResponseItemType.Confirm,
        )
      }

      // case OfferType.ACCEPTED:
      //   jsonResponse = await (response.json() as Promise<AcceptedOfferResponse>)

      //   this.documentGroups = jsonResponse.groups
      //   break
    } catch (error) {
      // eslint-disable-next-line no-console
      console.error(String(error))
      this.error = true
    } finally {
      this.isLoading = false
    }
  }

  /**
   * Returns array of keys of all accepted documents.
   * @param documents - an array of `OfferDocument` items.
   */

  public getAllToBeCheckedDocuments(): NewOfferResponseCopy.File[] {
    return this.docGroupsToBeChecked.map(docGroup => docGroup?.map(files => files).flat()).flat()
  }

  public getAllToBeSignedDocuments(): NewOfferResponseCopy.File[] {
    return this.docGroupsToBeSigned.map(docGroup => docGroup?.map(files => files).flat()).flat()
  }

  private getAcceptedKeys(documents: NewOfferResponseCopy.File[]): string[] {
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

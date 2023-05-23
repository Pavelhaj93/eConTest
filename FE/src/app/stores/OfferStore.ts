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

  constructor(type: OfferType, offerUrl: string, guid: string) {
    this.type = type
    this.globalQueryParams = { guid }
    this.offerUrl = offerUrl
  }

  @observable
  public documents: NewOfferResponseCopy.File | undefined = undefined

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
  public confirm: NewOfferResponseCopy.ResponseItem[] | undefined = undefined

  @computed
  public get concatedSortedData(): NewOfferResponseCopy.ResponseItem[] {
    return [
      ...(this.perex || []),
      ...(this.benefits || []),
      ...(this.docsCheck || []),
      ...(this.docsSign || []),
      ...(this.confirm || []),
    ].sort((a, b) => a.position - b.position)
  }

  @computed
  public get documentsToBeSigned(): (NewOfferResponseCopy.File | undefined)[] {
    return this.docsSign?.map(item => item.body.docs?.files).flat() ?? []
  }

  @computed
  public get documentsToBeChecked(): (NewOfferResponseCopy.File | undefined)[] {
    return this.docsCheck?.map(item => item.body.docs?.files).flat() ?? []
  }

  @computed
  public get mandatoryDocuments(): (NewOfferResponseCopy.File | undefined)[] {
    return this.documentsToBeChecked
      .map(docGroup => docGroup?.map(files => files).flat())
      .flat()
      .filter(file => file.mandatory)
  }

  @computed
  public get allDocumentsAreAccepted(): boolean {
    if (!this.documentsToBeChecked.length) return true

    return this.mandatoryDocuments.every(file => file?.checked)
  }

  @action public checkDocument(key: string): void {
    const document = this.documentsToBeChecked
      .map(docGroup => docGroup?.find(file => file.key === key))
      .find(doc => doc !== undefined)
    document && (document.checked = !document.checked)
  }

  @action public checkDocumentsGroup(files: NewOfferResponseCopy.File[]): void {
    if (!files.length) return

    // if at least one document which isn't checked is found,
    // mark the whole group of documents as checked
    const allChecked = !files.find(document => !document.checked)

    // then just update the `checked` flag of each document
    files.forEach(file => (file.checked = !allChecked))
  }

  private enrichDocsCheckItems(files: NewOfferResponseCopy.File[]) {
    return files.map(file => ({
      ...file,
      checked: false,
    }))
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
                files: this.enrichDocsCheckItems(item.body.docs?.files ?? []),
              },
            },
          }))

        this.docsSign = jsonResponse.data.filter(
          item => item.type === NewOfferResponseCopy.ResponseItemType.DocsSign,
        )
        this.confirm = jsonResponse.data.filter(
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
}

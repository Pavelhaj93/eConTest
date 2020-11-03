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

  @computed
  public get documentsToBeSigned(): Document[] {
    return this.documents.filter(document => document.sign)
  }

  @computed
  public get allDocumentsAreSigned(): boolean {
    if (this.documentsToBeSigned.find(document => !document.signed)) {
      return false
    }

    return true
  }

  constructor(documentsUrl: string, isRetention: boolean, isAcquisition: boolean) {
    this.documentsUrl = documentsUrl
    this.isRetention = isRetention
    this.isAcquisition = isAcquisition
  }

  @action
  public async fetchDocuments() {
    this.isLoading = true

    try {
      const response = await fetch(this.documentsUrl, {
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

  public getDocument(id: string): Document | undefined {
    return this.documents.find(document => document.id === id)
  }

  @action
  public signDocument(id: string): void {
    const document = this.getDocument(id)

    if (!document) {
      return
    }

    document.signed = !document.signed
  }

  @action acceptDocument(id: string): void {
    const document = this.getDocument(id)

    if (!document) {
      return
    }

    document.accepted = !document.accepted
  }
}

import {
  CustomFile,
  QueryParams,
  UploadDocumentErrorResponse,
  UploadDocumentPromise,
  UploadDocumentsResponse,
} from '@types'
import { UserDocument } from './UserDocument'
import { generateId, parseUrl } from '@utils'
import { action, computed, observable } from 'mobx'

export class UploadStore {
  private globalQueryParams: QueryParams
  public uploadDocumentUrl = ''
  public removeDocumentUrl = ''
  public maxUploadGroupSize = 0
  public uploadUrl = ''
  public errorPageUrl = ''
  public sessionExpiredPageUrl = ''

  @observable
  public userDocuments: Record<string, UserDocument[]> = {}

  @observable
  public uploadResponseItems: UploadDocumentsResponse.ResponseItem[] | undefined = undefined

  @observable
  public error = false

  @observable
  public errorMessage = ''

  @observable
  public isLoading = false

  @observable
  public forceReload = false

  constructor(uploadUrl: string, guid: string) {
    this.uploadUrl = uploadUrl
    this.globalQueryParams = { guid }
  }

  /**
   * Returns true if at least one document was uploaded (or is uploading)
   * @returns boolean
   */
  @computed public get isDirty(): boolean {
    // if at least one document was uploaded (or is uploading) => the offer is dirty
    if (Object.values(this.userDocuments).some(docs => docs.length)) {
      return true
    }

    return false
  }

  /**
   * Returns true if all mandatory documents are uploaded so the upload is finished and client can continue to the acceptance page
   * @returns boolean
   */
  @computed public get isUploadFinished(): boolean {
    // if there are no upload groups => nothing to validate
    if (!this.uploadResponseItems?.length) {
      return true
    }

    let allUploaded = true

    // iterate over all mandatory uploadGroups
    this.uploadResponseItems
      .map(uploadGroup => uploadGroup.body.docs.files)
      .map(file => file.filter(file => file.mandatory))
      .flat()
      .forEach(doc => {
        // if all mandatory documents are uploaded => the upload is finished
        if (!this.userDocuments[doc.id]?.length) {
          allUploaded = false
        }

        // if all documents are stil untouched or uploading or have error => group is not finished
        if (this.userDocuments[doc.id]?.some(doc => !doc.touched || doc.uploading || doc.error)) {
          allUploaded = false
        }
      })

    return allUploaded
  }

  /**
   * Enriches the uploadResponse items with size
   */
  private enrichUploadDocuments(documents: UploadDocumentsResponse.File[]) {
    return documents.map(document => ({
      ...document,
      size: 0,
    }))
  }

  /**
   * Cancel uploading of the document.
   * @param document - document to cancel
   */
  public cancelUploadDocument(document: UserDocument): void {
    if (document.controller) {
      document.controller.abort()
    }
  }

  /**
   * Get user document by its id and categoryid.
   * @param id - document id
   * @param categoryId - categoryId
   */
  public getUserDocument(id: string, categoryId: string): UserDocument | undefined {
    return this.userDocuments[categoryId]?.find(document => document.key === id)
  }

  @action public async fetchUploads(timeoutMs?: number): Promise<void> {
    this.isLoading = true

    try {
      let fetchTimeout: NodeJS.Timeout | number | null = null
      let controller: AbortController | null = null
      // if timeoutMs is present => cancel the fetch request after this value
      if (timeoutMs) {
        controller = new AbortController()
        fetchTimeout = setTimeout(() => {
          controller?.abort()
        }, timeoutMs)
      }

      const response = await fetch(parseUrl(this.uploadUrl, this.globalQueryParams), {
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
        const { Message } = await (response.json() as Promise<UploadDocumentErrorResponse>)
        this.errorMessage = Message
        throw new Error(Message)
      }

      // the rest of the statuses are treated as unknown errors
      if (!response.ok) {
        throw new Error(`FAILED TO FETCH OFFER - ${response.status}`)
      }

      const jsonResponse = await (response.json() as Promise<UploadDocumentsResponse.RootObject>)

      this.uploadResponseItems = jsonResponse.data.map(uploadGroup => ({
        ...uploadGroup,
        body: {
          ...uploadGroup.body,
          docs: {
            ...uploadGroup.body.docs,
            files: this.enrichUploadDocuments(uploadGroup.body.docs.files),
          },
        },
      }))
    } catch (error) {
      // eslint-disable-next-line no-console
      console.error(String(error))
      this.error = true
    } finally {
      this.isLoading = false
    }
  }

  /**
   * Sends a request with the document (file) to upload API.
   * @param document - file
   * @returns `Promise` in shape of `UploadDocumentPromise`
   */
  public async uploadDocument(
    document: UserDocument,
    categoryId: string,
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
        parseUrl(`${this.uploadDocumentUrl}/${categoryId}`, this.globalQueryParams),
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

      const { size } = await response.json()
      this.setUploadGroupSize(categoryId, size)

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

  /**
   * Remove user document by its key and categoryId.
   * @param document - `UserDocument`
   * @param categoryId - categoryId
   */
  @action public async removeUserDocument(key: string, categoryId: string): Promise<void> {
    if (!this.userDocuments[categoryId]) {
      return
    }

    const document = this.getUserDocument(key, categoryId)

    this.userDocuments[categoryId] = this.userDocuments[categoryId].filter(doc => doc.key !== key)

    // if document is still uploading or was rejected => do not send the request
    if (!document || document.uploading || document.error) {
      return
    }

    // if document has been successfully uploaded => sends a request to remove it
    const response = await fetch(
      parseUrl(`${this.removeDocumentUrl}/${categoryId}`, { ...this.globalQueryParams, f: key }),
      {
        method: 'DELETE',
        headers: { Accept: 'application/json' },
      },
    )

    if (!response.ok) {
      return
    }

    const { size } = await response.json()
    this.setUploadGroupSize(categoryId, size)
  }

  @action public addUserFiles(files: CustomFile[], categoryId: string): void {
    // remap files to the `UserDocument` shape
    const newDocuments = files.map(({ file, error }) => {
      if (error) {
        return new UserDocument(file, generateId(), true, error)
      }

      return new UserDocument(file, generateId())
    })

    // if categoryId does not exist yet => create one
    if (!this.userDocuments[categoryId]) {
      this.userDocuments[categoryId] = []
    }

    // spread existing documents within the categoryId and add new ones
    this.userDocuments = {
      ...this.userDocuments, // this spread is needed for trigger rerender in React component
      [categoryId]: [...this.userDocuments[categoryId], ...newDocuments],
    }
  }

  @action private setUploadGroupSize(id: string, size: number): void {
    const group =
      this.uploadResponseItems?.map?.(item => item.body.docs.files).flat()?.length &&
      this.uploadResponseItems
        ?.map(item => item.body.docs.files)
        .flat()
        .find(file => file.id === id)

    if (!group) return

    group.size = size
  }

  /**
   * gets item from upload documents response by its position
   * @param position - position of the item
   * @returns `UploadDocumentsResponse.ResponseItem`
   */
  public getItemByPosition(
    position: UploadDocumentsResponse.ResponseItem['position'],
  ): UploadDocumentsResponse.ResponseItem | undefined {
    return this.uploadResponseItems?.find(item => item.position === position)
  }

  /**
   * Returns true if upload group size exceeded
   * @param position - position of the item
   * @returns `boolean`
   * */
  uploadGroupSizeExceeded(position: UploadDocumentsResponse.ResponseItem['position']): boolean {
    if (this.maxUploadGroupSize === 0) {
      return false
    }
    // find group by its position and sum sizes from all upload documents of the group
    const totalSize = this.getItemByPosition(position)?.body.docs.files.reduce(
      (acc, file) => acc + file.size,
      0,
    )

    return totalSize !== undefined && totalSize >= this.maxUploadGroupSize
  }
}

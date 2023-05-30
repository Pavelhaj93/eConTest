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

  // @computed public get uploadGroupSizeExceeded(): boolean {
  //   if (this.maxUploadGroupSize === 0) {
  //     return false
  //   }
  //   // sum sizes from all upload groups
  //   const totalSize =
  //     this.uploadResponseItems
  //       ?.map(uploadGroup => uploadGroup.body.docs.files)
  //       .flat()
  //       .reduce((acc, file) => acc + file.size, 0) ?? 0

  //   return totalSize >= this.maxUploadGroupSize
  // }

  @computed public get isOfferDirty(): boolean {
    // if at least one document was uploaded (or is uploading) => the offer is dirty
    if (Object.values(this.userDocuments).some(docs => docs.length)) {
      return true
    }

    return false
  }

  private enrichUploadDocuments(documents: UploadDocumentsResponse.File[]) {
    return documents.map(document => ({
      ...document,
      size: 0,
    }))
  }

  public cancelUploadDocument(document: UserDocument): void {
    if (document.controller) {
      document.controller.abort()
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

      const { size } = await response.json()
      // this.setUploadGroupSize(category, size)

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

    const { size } = await response.json()
    // this.setUploadGroupSize(category, size)
  }

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

  // @action private setUploadGroupSize(id: string, size: number): void {
  //   const group = this.uploadResponseItems
  //     ?.map?.(item => item.body.docs.files)
  //     .flat()
  //     .find(file => file.id === id)

  //   if (!group) return

  //   group.size = size
  // }
}

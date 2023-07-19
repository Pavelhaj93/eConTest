import { ErrorResponse, ResponseItemType } from '@types'

export namespace UploadDocumentsResponse {
  export interface RootObject {
    data: ResponseItem[]
  }

  export interface ResponseItem {
    type: ResponseItemType
    position: number
    header: Header | null
    body: Body
  }

  export interface Header {
    title: string
  }

  export interface Body {
    docs: Docs
  }

  export interface Docs {
    title: string
    text: string | null
    note: string
    files: File[]
  }

  export interface File {
    id: string
    title: string
    info: string | null
    mandatory: boolean
    idx: number
    size: number
    name: string | null
  }
}

export type UploadDocumentPromise = {
  uploaded: boolean
  Message?: string
}

export type UploadDocumentErrorResponse = ErrorResponse

export type StoredUploadFile = {
  key: string
  file: File
}

export type StoredUploadFileGroup = {
  categoryId: string
  userDocuments: StoredUploadFile[]
}

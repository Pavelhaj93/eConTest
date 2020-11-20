export type Choice = {
  /** Use for HTML label. */
  label: string
  /** Value of the choice. */
  key: string
  /** Placeholder used for the additional text input. */
  placeholder?: string | null
  /** If provided, it will be rendered as as tooltip. */
  helpText?: string | null
  /** Regex used for user input validation. */
  regex?: string | null
}

export type AppConfig = {
  view: string
  offerUrl: string
  labels: {
    [key: string]: any
  }
  formAction?: string
  choices: Choice[]
  /** URL that returns the document itself. Need to append file ID. */
  getFileUrl?: string
  /** URL that returns an image preview of the document. Need to append file ID. */
  getFileForSignUrl?: string
  /** URL that accepts a signature as base64 PNG within POST body. Need to append file ID. */
  signFileUrl?: string
  /** After how many milliseconds the documents request will be canceled. */
  doxTimeout?: number
  /** URL where to upload user files. */
  uploadFileUrl?: string
  /** URL that accepts document id to remove the uploaded document. */
  removeFileUrl?: string
  errorPageUrl: string
  allowedContentTypes?: string[]
  maxFileSize?: number
}

declare global {
  interface Window {
    appConfig: AppConfig
  }
}

export type View = AppConfig

// used for documents coming from API
export type Document = {
  key: string
  title: string
  label: string
  mime?: string
  size?: string
  sign: boolean
  signed?: boolean
  accepted?: boolean
}

export type Color =
  | 'orange'
  | 'orange-dark'
  | 'red'
  | 'red-dark'
  | 'red-purple-dark'
  | 'purple'
  | 'purple-light'
  | 'blue'
  | 'blue-dark'
  | 'blue-green-dark'
  | 'blue-green-light'
  | 'green'
  | 'green-dark'
  | 'yellow'
  | 'gray-5'
  | 'gray-10'
  | 'gray-20'
  | 'gray-40'
  | 'gray-60'
  | 'gray-80'
  | 'gray-100'

export type UploadDocumentResponse = {
  id?: string | null
  uploaded: boolean
  message?: string
}

export type Group = {
  title: string
  files: Document[]
}

export type AcceptedOfferResponse = {
  groups: Group[]
}

export enum OfferType {
  NEW = 'NEW',
  ACCEPTED = 'ACCEPTED',
}

export enum FileError {
  INVALID_TYPE = 'INVALID_TYPE',
  SIZE_EXCEEDED = 'SIZE_EXCEEDED',
}

export type CustomFile = {
  file: File
  error?: FileError
}

import { Choice, GaEventClickData } from './Authentication'

export * from './Offer'
export * from './Authentication'

export type AppConfig = {
  guid: string
  view: string

  offerUrl: string
  labels: {
    [key: string]: any
  }
  gaEventClickData?: GaEventClickData
  formAction?: string
  choices?: Choice[]
  /** URL that returns the document itself. Need to append file ID. */
  getFileUrl?: string
  /** URL that returns an image preview of the document. Need to append file ID. */
  thumbnailUrl?: string
  /** URL that accepts a signature as base64 PNG within POST body. Need to append file ID. */
  signFileUrl?: string
  /** After how many milliseconds the documents request will be canceled. */
  timeout?: number
  /** URL where to upload user files. */
  uploadFileUrl?: string
  /** URL that accepts document id to remove the uploaded document. */
  removeFileUrl?: string
  errorPageUrl: string
  acceptOfferUrl?: string
  keepAliveUrl?: string
  thankYouPageUrl: string
  sessionExpiredPageUrl: string
  allowedContentTypes?: string[]
  maxFileSize?: number
  maxGroupFileSize?: number
  suppliers?: {
    label: string
    items: Supplier[]
  }
  /** URl for innogy account button on login screen. */
  innogyAccountUrl?: string
  /** Controls whether or not the Innogy account login option is enabled. */
  hideInnogyAccount?: boolean
}

declare global {
  interface Window {
    appConfig: AppConfig
  }
}

export type View = AppConfig

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

export enum FileError {
  INVALID_TYPE = 'INVALID_TYPE',
  SIZE_EXCEEDED = 'SIZE_EXCEEDED',
}

export type CustomFile = {
  file: File
  error?: FileError
}

export type Supplier = {
  label: string
  value: string
}
export interface QueryParams {
  [key: string]: string
}

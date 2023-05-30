import { NewOfferResponse } from './Offer'

export type UploadDocumentResponse = {
  /** Category / group ID. */
  id: string
  /** Total size of all uploaded documents in current group. */
  size: number
  /** Array of all files that were successfully uploaded. */
  files: NewOfferResponse.File[]
}

export type UploadDocumentPromise = {
  uploaded: boolean
  Message?: string
}

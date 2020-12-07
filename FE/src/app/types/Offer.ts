export enum OfferType {
  NEW = 'NEW',
  ACCEPTED = 'ACCEPTED',
}

export enum GiftType {
  LED = 'LED',
  DET = 'DET',
  PKZ = 'PKZ',
}

export type OfferDocument = {
  label: string
  prefix: string
  key: string
  accepted: boolean
  mime: string
  group: string
}

export type Group = {
  title: string
  files: OfferDocument[]
}

export type AcceptedOfferResponse = {
  groups: Group[]
}

export type UploadType = {
  id: string
  title: string
  info: string
  mandatory: boolean
  size: number
}

export type OfferParams<T> = Array<T>

export type OfferBox = {
  title: string
  params: OfferParams<{
    title: string
    value: string
  }>
}

export type GiftsBox = {
  title: string
  note: string
  groups: Array<{
    title: string
    params: OfferParams<{
      title: string
      icon: GiftType
      count: number
    }>
  }>
}

export type AcceptanceDocuments = {
  title: string
  text: string
  accept?: {
    title: string
    subTitle: string
    mandatoryGroups: string[]
    files: OfferDocument[]
  } | null
  sign?: {
    title: string
    subTitle: string
    mandatoryGroups: string[]
    files: OfferDocument[]
    note: string
  } | null
}

export type UploadDocuments = {
  title: string
  note: string
  types: UploadType[]
}

export type OtherDocuments = {
  products?: {
    title: string
    subTitle: string
    params: OfferParams<{
      title: string
      value: string
    }>
    arguments: Array<{ value: string }>
    subTitle2: string
    mandatoryGroups: string[]
    files: OfferDocument[]
    note: string
    text: string
  }
  services?: {
    title: string
    text: string
    mandatoryGroups: string[]
    files: OfferDocument[]
  }
}

export type OfferDocuments = {
  acceptance?: AcceptanceDocuments | null
  uploads?: UploadDocuments | null
  other?: OtherDocuments | null
}

export type AcceptanceGroup = {
  title: string
    group: string
    accepted: boolean
}

export type Acceptance = {
  params: OfferParams<AcceptanceGroup>
}

export type NewOfferResponse = {
  perex?: OfferBox
  benefits?: OfferBox
  gifts?: GiftsBox
  documents: OfferDocuments
  acceptance: Acceptance
}

export type UploadDocumentResponse = {
  /** Category / group ID. */
  id: string
  /** Total size of all uploaded documents in current group. */
  size: number
  /** Array of all files that were successfully uploaded. */
  files: OfferDocument[]
}

export type UploadDocumentPromise = {
  uploaded: boolean
  message?: string
}

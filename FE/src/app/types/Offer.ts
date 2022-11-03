export enum OfferType {
  NEW = 'NEW',
  ACCEPTED = 'ACCEPTED',
}

export enum GiftType {
  LED = 'LED',
  DET = 'DET',
  PKZ = 'PKZ',
}

export enum CommodityProductType {
  ELECTRICITY = 'E',
  GAS = 'G',
}

export type SectionInfo = {
  text: string
  title: string
  note: string
}

export type OfferDocument = {
  label: string
  prefix: string
  key: string
  /** Common key "accepted" is used for both accepted and signed documents. */
  accepted: boolean
  mime: string
  group: string
  note: string
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

export type BenefitsBox = OfferBox & {
  commodityProductType?: CommodityProductType | null
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
  } | null
  /** Object sectionInfo currently will be using like main source for data (title, text, note). But still we kept original source like fallback. When BE confirmed that we can go with only sectionInfo, should be removed fallback and unused original properties. */
  sectionInfo: SectionInfo
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
    /** Object sectionInfo currently will be using like main source for data (title, text, note). But still we kept original source like fallback. When BE confirmed that we can go with only sectionInfo, should be removed fallback and unused original properties. */
    sectionInfo: SectionInfo
  }
  services?: {
    title: string
    subTitle?: string | null
    params: OfferParams<{
      title: string
      value: string
    }>
    text: string
    arguments: Array<{ value: string }>
    subTitle2?: string | null
    mandatoryGroups: string[]
    files: OfferDocument[]
    note?: string
    /** Object sectionInfo currently will be using like main source for data (title, text, note). But still we kept original source like fallback. When BE confirmed that we can go with only sectionInfo, should be removed fallback and unused original properties. */
    sectionInfo: SectionInfo
  }
}

export type OfferDocuments = {
  acceptance?: AcceptanceDocuments | null
  uploads?: UploadDocuments | null
  other?: OtherDocuments | null
  description?: string | ''
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
  benefits?: BenefitsBox
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
  Message?: string
}

type ErrorResponse = {
  Message: string
}

export type OfferErrorResponse = ErrorResponse

export type UploadDocumentErrorResponse = ErrorResponse

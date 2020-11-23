export enum OfferType {
  NEW = 'NEW',
  ACCEPTED = 'ACCEPTED',
}

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

export type Group = {
  title: string
  files: Document[]
}

export type AcceptedOfferResponse = {
  groups: Group[]
}

export type OfferDocument = {
  label: string
  prefix: string
  key: string
  mandatory: boolean
}

export type UploadType = {
  id: string
  title: string
  info: string
  mandatory: boolean
}

export type OfferParams<T> = Array<T>

export type OfferBox = {
  title: string
  params: OfferParams<{
    title: string
    value: string
  }>
}

export type BenefitBox = {
  title: string
  note: string
  groups: Array<{
    title: string
    params: OfferParams<{
      title: string
      icon: string
      count: number
    }>
  }>
}

export type AcceptanceDocuments = {
  title: string
  accept: {
    title: string
    subTitle: string
    params: OfferParams<{
      title: string
      value: string
    }>
    files: OfferDocument[]
  }
  sign: {
    title: string
    subTitle: string
    files: OfferDocument[]
  }
}

export type UploadDocuments = {
  title: string
  note: string
  types: UploadType[]
}

export type OtherDocuments = {
  commodities: {
    title: string
    subTitle: string
    params: OfferParams<{
      title: string
      value: string
    }>
    arguments: Array<{ title: string }>
    subTitle2: string
    files: OfferDocument[]
  }
  services: {
    title: string
    files: OfferDocument[]
  }
}

// export type NewOfferResponse = {
//   perex?: OfferBox
//   gifts?: OfferBox
//   benefits?: BenefitBox
//   documents: {
//     acceptance?: AcceptanceDocuments
//     upload?: UploadDocuments
//     other?: OtherDocuments
//   }
// }

// TODO: remove after demo
export type NewOfferResponse = Document[]

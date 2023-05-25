export namespace NewOfferResponse {
  export interface RootObject {
    data: ResponseItem[]
  }

  export interface ResponseItem {
    type: ResponseItemType
    position: number
    header: Header
    body: Body
  }
  export interface Body {
    params?: Param[]
    head?: Header | null
    text: string | null
    docs: Docs
    note: string | null
    points: Point[]
  }
  export interface Param {
    title: string
    value?: string
    group: string
  }

  export interface Header {
    title: string
    params: Param[]
    text: string
  }

  export interface Docs {
    title: string
    params: any[] | null
    text: string | null
    mandatoryGroups: string[]
    files: File[]
  }

  export interface Point {
    value: string
  }

  export interface File {
    key: string
    group: string
    label: string
    note: string | null
    prefix: string
    mime: string
    mandatory: boolean
    idx: number
    accepted?: boolean
  }

  export enum ResponseItemType {
    Perex = 'perex',
    Benefit = 'benefit',
    DocsCheck = 'docsCheck',
    DocsSign = 'docsSign',
    Confirm = 'confirm',
  }

  export type AcceptanceGroup = {
    title: string
    group: string
    accepted: boolean
  }
}

export namespace AcceptedOfferResponse {
  export interface RootObject {
    groups: Group[]
  }

  export type Group = {
    title: string
    files: File[]
  }
}

type ErrorResponseCopy = {
  Message: string
}

export type OfferErrorResponse = ErrorResponseCopy

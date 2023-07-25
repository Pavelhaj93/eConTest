import { CommodityProductType, ErrorResponse } from '@types'

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
    text: string
    type?: CommodityProductType
  }

  export interface Docs {
    perex: { header: Header; body: Body } | null
    title: string
    header: Header | null
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
    DocsCheck = 'docsCheck',
    DocsCheckG = 'docsCheck-G',
    DocsCheckE = 'docsCheck-E',
    DocsSign = 'docsSign',
    Confirm = 'confirm',
    Consumption = 'consumption',
    DocsSignG = 'docsSign-G',
    DocsSignE = 'docsSign-E',
    DocsSignEG = 'docsSign-E/G',
  }

  export type AcceptanceGroup = {
    title: string
    group: string
    accepted: boolean
  }
}

export namespace AcceptedOfferResponse {
  export interface RootObject {
    groups: AcceptedGroup[]
  }

  export type AcceptedGroup = {
    title: string
    files: NewOfferResponse.File[]
  }
}

export type OfferErrorResponse = ErrorResponse

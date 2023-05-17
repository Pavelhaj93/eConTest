import { CommodityProductType, ResponseItemType } from '@types'

export namespace SummaryResponse {
  export interface RootObject {
    data: ResponseItem[]
  }

  export interface ResponseItem {
    type: ResponseItemType
    position: number
    header: ResponseItemHeader
    body: ResponseItemBody
  }

  export interface ResponseItemHeader {
    title: string
    type?: CommodityProductType
    data?: HeaderData[]
    note?: string
  }

  export interface HeaderData {
    type: string
    title: string
    value: string
    note?: string
  }

  export interface ResponseItemBody {
    personalData?: PersonalData[]
    addresses?: Address[]
    contacts?: Contact[]
    infos?: Info[]
    points?: Point[]
    prices?: Price[]
    groups?: Group[]
  }

  export interface PersonalData {
    title: string
    values: string[]
  }

  export interface Address {
    title: string
    values: string[]
  }

  export interface Contact {
    title: string
    values: string[]
  }

  export interface Info {
    title?: string
    value: string
  }

  export interface Point {
    value: string
  }

  export interface Price {
    title: string
    price: string
    price2: string | null
    unit: string
  }

  export interface Group {
    title: string
    params: Param[]
  }

  export interface Param {
    title: string
    icon: string
    count: number
  }
}

type ErrorResponse = {
  Message: string
}

export type SumarryErrorResponse = ErrorResponse

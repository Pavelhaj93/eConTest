import { GiftType } from '@types'

export namespace SummaryResponse {
  export interface Datum {
    title: string
    lines: string[]
  }

  export interface Communication {
    Title: string
    Value: string
  }

  export interface Personal {
    title: string
    data: Datum[]
    communication: Communication
  }

  export interface DistributorChange {
    title: string
    name: string
    description: string
  }

  export interface Header {
    name: string
    price1_description: string
    price1: string
    price2_description: string
    price2: string
    info: string
  }

  export interface InfoPrice {
    title: string
    previous_price: string
    value: string
    value_unit: string
  }

  export interface Product {
    header: Header
    info_prices: InfoPrice[]
    middle_texts: string[]
    middle_texts_help: string
    benefits: string[]
    type: 'G' | 'E'
  }

  export interface Param {
    value: string
  }

  export interface Benefits {
    title: string
    params: Param[]
    commodityProductType: string
    summary: BenefitSummary[]
  }

  export interface BenefitSummary {
    title: string
    value: string
  }
  export interface Param2 {
    title: string
    icon: GiftType
    count: number
  }

  export interface Group {
    title: string
    params: Param2[]
  }

  export interface Gifts {
    title: string
    note: string
    groups: Group[]
  }

  export interface RootObject {
    personal: Personal
    distributor_change: DistributorChange
    product: Product
    benefits: Benefits[]
    gifts: Gifts
  }
}

type ErrorResponse = {
  Message: string
}

export type SumarryErrorResponse = ErrorResponse

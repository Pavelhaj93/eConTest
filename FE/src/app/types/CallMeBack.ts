interface ITime {
  label: string
  value: string
}

export interface IGetCallMeBackResponse {
  title: string | undefined
  image: {
    url: string
  }
  phone: string
  maxFileSize?: number
  maxFiles?: number
  times: ITime[]
  allowedFiles: string[]
  labels: {
    [key: string]: string
  }
}

export interface IPostCallMeBackResponse {
  labels: {
    [key: string]: string
  }
}

export interface ICallMeBackForm {
  phone: string
  time?: string
  note?: string
  file?: string[]
}

export interface IErrorResponse {
  message: string
}

export type CallMeBackErrorResponse = IErrorResponse

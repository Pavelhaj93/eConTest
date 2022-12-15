interface ITime {
  label: string
  value: string
}

interface IGetCallMeBackCommonResponse {
  succeed: boolean
}

export interface IGetCallMeBackResponseSucceed extends IGetCallMeBackCommonResponse {
  title: string | undefined
  image: {
    url: string
  }
  phone: string
  maxFileSize: number
  maxFiles: number
  times: ITime[]
  allowedFiles: string[]
  labels: {
    [key: string]: string
  }
}

export interface IGetCallMeBackResponseFailed extends IGetCallMeBackCommonResponse {
  error: string
}

export type IGetCallMeBackResponse = IGetCallMeBackResponseSucceed | IGetCallMeBackResponseFailed

export interface IPostCallMeBackResponse {
  labels: {
    [key: string]: string
  }
}

export interface ICallMeBackForm {
  phone: string
  time: string
  note: string
  file: Array<File | undefined>
  fileA?: File
  fileB?: File
  currentBrowserUrl: string
}

export type FileVariant = 'A' | 'B'

export interface IErrorResponse {
  error: string
}

export type CallMeBackErrorResponse = IErrorResponse

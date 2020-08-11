export type AppConfig = {
  view: string
  doxReadyUrl: string
  labels: {
    [key: string]: any
  }
  isAgreed: boolean
  isAcquisition: boolean
  formAction?: string
}

declare global {
  interface Window {
    appConfig: AppConfig
  }
}

export type View = AppConfig

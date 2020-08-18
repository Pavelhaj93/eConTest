export type Choice = {
  /** Use for HTML label. */
  label: string
  /** Value of the choice. */
  key: string
  /** Placeholder used for the additional text input. */
  placeholder?: string | null
  /** If provided, it will be rendered as as tooltip. */
  helpText?: string | null
  /** Regex used for user input validation. */
  regex?: string | null
}

export type AppConfig = {
  view: string
  doxReadyUrl: string
  labels: {
    [key: string]: any
  }
  isAgreed: boolean
  isAcquisition: boolean
  formAction?: string
  choices: Choice[]
}

declare global {
  interface Window {
    appConfig: AppConfig
  }
}

export type View = AppConfig

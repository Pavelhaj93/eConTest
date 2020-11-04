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
  isRetention: boolean
  formAction?: string
  choices: Choice[]
}

declare global {
  interface Window {
    appConfig: AppConfig
  }
}

export type View = AppConfig

export type Document = {
  id: string
  title: string
  url: string | null
  label: string
  sign: boolean
  signed?: boolean
  accepted?: boolean
}

export type Color =
  | 'orange'
  | 'orange-dark'
  | 'red'
  | 'red-dark'
  | 'red-purple-dark'
  | 'purple'
  | 'purple-light'
  | 'blue'
  | 'blue-dark'
  | 'blue-green-dark'
  | 'blue-green-light'
  | 'green'
  | 'green-dark'
  | 'yellow'
  | 'gray-5'
  | 'gray-10'
  | 'gray-20'
  | 'gray-40'
  | 'gray-60'
  | 'gray-80'
  | 'gray-100'

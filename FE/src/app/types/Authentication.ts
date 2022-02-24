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

export interface GaEventClickData {
  eCat: string
  eAct: string
  eLab: string
}

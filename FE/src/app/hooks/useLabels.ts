import { useCallback, useState } from 'react'

type TranslateFunction = (key: string, fallbackStr?: string) => string

const notFoundPrefix = '???-'

/**
 * Returns function for translating labels.
 * @param initialLabels - labels object
 * @returns TranslateFunction
 */
export const useLabels = (initialLabels: Record<string, any>): TranslateFunction => {
  const [labels] = useState(initialLabels)

  /**
   * If the specified key is not found in `labels`, it returns a fallback string with predefined prefix.
   */
  const translate = useCallback<TranslateFunction>(
    (key, fallbackStr): string => {
      if (!labels[key]) {
        return `${notFoundPrefix}${fallbackStr ?? ''}`
      }

      return labels[key]
    },
    [labels],
  )

  return translate
}

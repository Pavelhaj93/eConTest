import { useEffect } from 'react'

export const useKeepAlive = (intervalMs: number, url: string | undefined): void => {
  useEffect(() => {
    if (!url) {
      return
    }

    const id = setInterval(() => {
      fetch(url)
    }, intervalMs)

    return () => {
      id && clearInterval(id)
    }
  }, [intervalMs, url])
}

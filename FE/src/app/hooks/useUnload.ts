import { useEffect, useRef } from 'react'

export const useUnload = (callbackFn: (ev: BeforeUnloadEvent) => void): void => {
  const callbackRef = useRef(callbackFn)

  useEffect(() => {
    callbackRef.current = callbackFn
  }, [callbackFn])

  useEffect(() => {
    const onUnload = callbackRef.current

    window.addEventListener('beforeunload', onUnload)

    return () => window.removeEventListener('beforeunload', onUnload)
  }, [])
}

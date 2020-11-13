import React, { useEffect, useRef } from 'react'

export const useIsMountedRef = (): React.MutableRefObject<boolean> => {
  const isMountedRef = useRef(false)

  useEffect(() => {
    if (isMountedRef) {
      isMountedRef.current = true
    }

    // perform cleanup to prevent `Can’t perform a React state update on an unmounted component.`
    return () => {
      isMountedRef.current = false
    }
  })

  return isMountedRef
}

import React, { useEffect, useState } from 'react'
import { View } from '@types'
import { observer } from 'mobx-react-lite'
import { OfferStore } from '@stores'

export const Offer: React.FC<View> = observer(({ doxReadyUrl, isRetention, isAcquisition }) => {
  const [store] = useState(() => new OfferStore(doxReadyUrl, isRetention, isAcquisition))

  // useEffect(() => {
  //   store.fetchDocuments()
  // }, [])

  return <div>Offer</div>
})

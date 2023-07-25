import 'what-input'
import React from 'react'
import ReactDOM from 'react-dom'
import { View as ViewType } from '@types'
import { TooltipUI } from '@ui'

// for non React pages, init some UI components when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
  TooltipUI()
})

import {
  Authentication,
  Offer,
  AcceptedOffer,
  Summary,
  Upload,
  OfferGas,
  OfferEle,
  OdhlaskaGas,
  OdhlaskaEle,
  OdhlaskaBoth,
} from '@views'

type Views = {
  [index: string]: React.FC<ViewType>
}

const error = (
  <h1 className="text-center">
    The application could not be rendered due to missing or invalid configuration.
  </h1>
)

const App: React.FC = () => {
  const config = window.appConfig

  // config not found? do not continue
  if (!config) {
    return error
  }

  const views: Views = {
    Authentication,
    Offer,
    AcceptedOffer,
    Summary,
    Upload,
    OfferGas,
    OfferEle,
    OdhlaskaEle,
    OdhlaskaGas,
    OdhlaskaBoth,
  }

  const View = views[config.view]

  if (!View) {
    return error
  }

  return <View {...config} />
}

const appEl = document.getElementById('app')

if (appEl) {
  appEl.classList.add('app')
  ReactDOM.render(<App />, appEl)
}

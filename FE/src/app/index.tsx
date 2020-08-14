import 'what-input'
import React from 'react'
import ReactDOM from 'react-dom'
import { View as ViewType } from '@types'

import { Authentication } from '@views'

type Views = {
  [index: string]: React.FC<ViewType>
}

const error = (
  <h1>The application could not be rendered due to missing or invalid configuration.</h1>
)

const App: React.FC = () => {
  const config = window.appConfig

  // config not found? do not continue
  if (!config) {
    return error
  }

  const views: Views = {
    Authentication,
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

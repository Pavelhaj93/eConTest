// following svg imports will be extracted into svg sprite during build time
import '@icons/logo.svg'
import '@icons/bubble-info.svg'

import React from 'react'
import ReactDOM from 'react-dom'

const App: React.FC = () => {
  return <h1>It works!</h1>
}

const appEl = document.getElementById('app')

if (appEl) {
  appEl.classList.add('app')
  ReactDOM.render(<App />, appEl)
}

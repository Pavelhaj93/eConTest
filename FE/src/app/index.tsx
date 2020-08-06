import React from 'react'
import ReactDOM from 'react-dom'

const App: React.FC = () => {
  return <h1>It works!</h1>
}

const appEl = document.getElementById('app')

appEl?.classList.add('app')

ReactDOM.render(<App />, appEl)

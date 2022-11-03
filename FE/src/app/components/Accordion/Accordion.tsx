import React, { useCallback, useState } from 'react'
import { Collapse } from 'react-bootstrap'

type Props = {
  header: React.ReactNode
  isOpen?: boolean
}

export const Accordion: React.FC<Props> = ({ header, children, isOpen }) => {
  const [open, setOpen] = useState(isOpen)

  const handleKeyDown = useCallback(
    (ev: React.KeyboardEvent) => {
      switch (ev.key.toLowerCase()) {
        case 'enter':
        case 'space':
        case 'Spacebar':
        case ' ':
          ev.preventDefault()
          setOpen(!open)
          break

        default:
          break
      }
    },
    [open],
  )

  return (
    <div className="accordion mb-4">
      <div
        className="accordion__header"
        onClick={() => setOpen(!open)}
        aria-expanded={open}
        tabIndex={0}
        role="button"
        onKeyDown={handleKeyDown}
      >
        {header}
      </div>
      <Collapse in={open}>
        <div>{children}</div>
      </Collapse>
    </div>
  )
}

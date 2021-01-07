import React from 'react'
import { Icon } from '@components'
import { colors } from '@theme'

type Props = {
  url: string
  label: string
  onClick: () => void
}

export const DocumentLink: React.FC<Props> = ({ url, label, onClick }) => (
  <>
    {/* By adding target="_blank" it bypasses `beforeunload` event and allows to download the file. */}
    <a
      href={url}
      download
      target="_blank"
      rel="noreferrer"
      onClick={onClick}
      className="d-flex align-items-center flex-grow-1"
    >
      <span className="mr-1">{label}</span>
      <Icon name="pdf" size={36} color={colors.orange} className="ml-md-auto d-none d-sm-block" />
    </a>
  </>
)

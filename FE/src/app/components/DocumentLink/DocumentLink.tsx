import React, { MouseEvent, useCallback } from 'react'
import classNames from 'classnames'
import { Icon } from '@components'
import { colors } from '@theme'
import { isMobileDevice } from '@utils'

type Props = {
  url: string
  label: string
  onClick: () => void
  noIcon?: boolean
}

export const DocumentLink: React.FC<Props> = ({ url, label, onClick, noIcon = false }) => {
  const handleClick = useCallback(
    (ev: MouseEvent) => {
      if (isMobileDevice()) {
        ev.preventDefault()
        window.open(url, '_blank', 'noopener,noreferrer')
      } else {
        onClick()
      }
    },
    [onClick, url],
  )

  return (
    <>
      {/* By adding target="_blank" it bypasses `beforeunload` event and allows to download the file. */}
      <a
        href={url}
        download
        target="_blank"
        rel="noreferrer"
        onClick={handleClick}
        className={classNames({
          'd-flex align-items-center flex-grow-1': !noIcon,
        })}
      >
        {noIcon ? (
          label
        ) : (
          <>
            <span className="mr-1">{label}</span>
            <Icon
              name="pdf"
              size={36}
              color={colors.orange}
              className="ml-md-auto d-none d-sm-block"
            />
          </>
        )}
      </a>
    </>
  )
}

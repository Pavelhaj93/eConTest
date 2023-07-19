import { Icon } from '@components'
import { colors } from '@theme'
import classNames from 'classnames'
import React, { FC, Fragment } from 'react'

interface InfoElementProps {
  value: string
  className?: string
}

const InfoElement: FC<InfoElementProps> = ({ value, className }) => {
  return (
    <Fragment>
      {value && (
        <div className={classNames('text-center mt-4 d-flex flex-row', className)}>
          <div className="mr-2">
            <Icon name="info-circle" size={16.5} color={colors.gray100} className="mx-1 mb-1" />
          </div>
          <div
            className="editorial-content text-muted small align-self-center"
            dangerouslySetInnerHTML={{ __html: value }}
          />
        </div>
      )}
    </Fragment>
  )
}

export default InfoElement

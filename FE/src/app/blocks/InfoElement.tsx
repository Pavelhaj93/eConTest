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
        <div className={classNames('text-center mt-4', className)}>
          <Icon
            name="info-circle"
            size={40}
            color={colors.gray100}
            className="d-block mx-auto mb-3"
          />
          <div className="editorial-content" dangerouslySetInnerHTML={{ __html: value }} />
        </div>
      )}
    </Fragment>
  )
}

export default InfoElement

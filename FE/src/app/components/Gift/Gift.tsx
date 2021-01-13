import React, { useMemo } from 'react'
import classNames from 'classnames'
import { Icon, IconName } from '@components'
import { colors } from '@theme'
import { GiftType } from '@types'

type Props = {
  type: GiftType
  title: string
  className?: string
}

export const Gift: React.FC<Props> = ({ type, title, className, ...rest }) => {
  const getIcon: IconName = useMemo(() => {
    switch (type) {
      case GiftType.LED:
        return 'bulb' as IconName

      case GiftType.PKZ:
        return 'voucher-100' as IconName

      case GiftType.DET:
        return 'gift' as IconName

      default:
        return 'gift' as IconName
    }
  }, [type])

  return (
    <div className={classNames('gift', className)} {...rest}>
      <span className="gift__icon-wrapper">
        <Icon name={getIcon} size={30} color={colors.orange} className="gift__icon" />
      </span>
      <span className="gift__title">{title}</span>
    </div>
  )
}

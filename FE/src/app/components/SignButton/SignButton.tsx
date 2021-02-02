import React from 'react'
import { Button } from 'react-bootstrap'
import classNames from 'classnames'
import { Icon } from '@components'
import { colors } from '@theme'

type SignButtonProps = {
  signed: boolean
  onClick: () => void
  labelEdit: string
  labelSign: string
  descriptionId: string
  className?: string
  showLabelEdit: boolean
}

export const SignButton: React.FC<SignButtonProps> = ({
  signed,
  labelEdit,
  labelSign,
  onClick,
  descriptionId,
  className,
  showLabelEdit,
}) => (
  <>
    <Button
      variant={signed ? 'primary' : 'secondary'}
      className={classNames(
        {
          'btn-icon': signed && !showLabelEdit,
          'ml-auto': true,
          'form-item-wrapper__btn': true,
        },
        className,
      )}
      aria-label={signed ? labelEdit : labelSign}
      onClick={onClick}
      aria-describedby={descriptionId}
    >
      {signed ? (
        showLabelEdit ? (
          labelEdit
        ) : (
          <Icon name="edit" size={18} color={colors.white} />
        )
      ) : (
        labelSign
      )}
    </Button>
  </>
)

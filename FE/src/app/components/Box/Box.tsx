import React from 'react'
import { Color } from '@types'
import classNames from 'classnames'

type BoxProps = {
  backgroundColor?: Color
  className?: string
}

type BoxHeadingProps = {
  id?: string
  className?: string
}

export const Box: React.FC<BoxProps> = ({
  children,
  className,
  backgroundColor = 'gray-10',
  ...rest
}) => (
  <div className={classNames('box', 'mb-4', `bg--${backgroundColor}`, className)} {...rest}>
    {children}
  </div>
)

export const BoxHeading: React.FC<BoxHeadingProps> = ({ children, className, ...rest }) => (
  <h3 className={classNames('box__heading', className)} {...rest}>
    {children}
  </h3>
)

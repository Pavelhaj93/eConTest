import React from 'react'
import { Color } from '@types'
import classNames from 'classnames'

type BoxProps = {
  backgroundColor?: Color
  className?: string
}

export const Box: React.FC<BoxProps> = ({ children, className, backgroundColor = 'gray-10' }) => (
  <div className={classNames('box', 'mb-4', `bg--${backgroundColor}`, className)}>{children}</div>
)

export const BoxHeading: React.FC = ({ children }) => <h3 className="box__heading">{children}</h3>

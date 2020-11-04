import React from 'react'
import { Color } from '@types'
import classNames from 'classnames'

type BoxProps = {
  backgroundColor?: Color
}

export const Box: React.FC<BoxProps> = ({ children, backgroundColor = 'gray-10' }) => (
  <div className={classNames('box', 'mb-4', `bg--${backgroundColor}`)}>{children}</div>
)

export const BoxHeading: React.FC = ({ children }) => <h3 className="box__heading">{children}</h3>

import React from 'react'

import ArrowNext from '@icons/arrow-next.svg'

const icons: { [key: string]: any } = {
  ArrowNext: ArrowNext,
}

type IconProps = {
  name: keyof typeof icons
  size?: number
  width?: number
  height?: number
  className?: string
  color?: string
}

export const Icon: React.FC<IconProps> = ({
  name,
  size,
  width,
  height,
  className,
  color = '#000000',
  ...rest
}) => {
  const Component = icons[name]

  return (
    <Component
      width={size || width}
      height={size || height}
      aria-hidden="true"
      focusable={false}
      className={`icon ${className ? className : ''}`}
      fill={color}
      {...rest}
    />
  )
}

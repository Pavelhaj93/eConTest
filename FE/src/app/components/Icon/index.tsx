/* eslint-disable */
// This file was generated. Do not modify the file manually.
// If you want to add a new icon check README file for instructions.
import '@icons/arrow-next.svg'
import '@icons/arrow-right.svg'
import '@icons/arrow-up.svg'
import '@icons/bubble-info.svg'
import '@icons/calendar.svg'
import '@icons/check.svg'
import '@icons/check-circle.svg'
import '@icons/chevron-down.svg'
import '@icons/close.svg'
import '@icons/email.svg'
import '@icons/loading.svg'
import '@icons/logo.svg'
import '@icons/number-one-circle.svg'
import '@icons/number-two-circle.svg'
import '@icons/pdf.svg'
import '@icons/phone.svg'
import '@icons/question-mark.svg'

import React from 'react'

type Name =  'arrow-next' | 'arrow-right' | 'arrow-up' | 'bubble-info' | 'calendar' | 'check' | 'check-circle' | 'chevron-down' | 'close' | 'email' | 'loading' | 'logo' | 'number-one-circle' | 'number-two-circle' | 'pdf' | 'phone' | 'question-mark'

type IconProps = {
  name: Name,
  /** Set a color for the icon. Hex format is required. */
  color?: string
  /** Size is used for both width and height. */
  size?: number
  /** Set width for the icon. */
  width?: number
  /** Set height for the icon. */
  height?: number
  /** If the icon should have its own specific class or classes. */
  className?: string
}

export const Icon: React.FC<IconProps> = ({ name, size, width, height, className, color = '#000000', ...rest }) => {
  return (
    <svg
      width={size || width}
      height={size || height}
      className={`icon ${className ? className : ''}`}
      preserveAspectRatio="none"
      focusable={false}
      aria-hidden="true"
      fill={color}
      {...rest}
    >
      <use xlinkHref={`#${name}`} />
    </svg>
  )
}

/* eslint-disable jsx-a11y/no-noninteractive-tabindex */
import React from 'react'
import { OverlayTrigger, Tooltip as BSTooltip } from 'react-bootstrap'
import { Icon } from '@components'
import { colors } from '@theme'
import { Placement } from 'react-bootstrap/esm/Overlay'
import { generateId } from '@utils'

type TooltipProps = {
  placement?: Placement
  visible?: boolean
}

// custom wrapper around Bootstrap tooltip component
export const Tooltip: React.FC<TooltipProps> = ({
  children,
  placement = 'top',
  visible = true,
}) => (
  <OverlayTrigger
    placement={placement}
    overlay={<BSTooltip id={`tooltip${generateId()}`}>{children}</BSTooltip>}
  >
    <span className="tooltip-icon-wrapper" tabIndex={visible ? 0 : -1}>
      <span className="sr-only">{children}</span>
      <Icon name="info-inversed-circle" size={30} color={colors.orange} className="icon-tooltip" />
    </span>
  </OverlayTrigger>
)

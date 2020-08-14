import React from 'react'
import { OverlayTrigger, Tooltip as BSTooltip } from 'react-bootstrap'
import { Icon } from '@components'
import { colors } from '@theme'
import { Placement } from 'react-bootstrap/esm/Overlay'

type TooltipProps = {
  id: string
  placement?: Placement
  visible?: boolean
}

// custom wrapper around Bootstrap tooltip component
export const Tooltip: React.FC<TooltipProps> = ({
  id,
  children,
  placement = 'top',
  visible = true,
}) => (
  <OverlayTrigger placement={placement} overlay={<BSTooltip id={id}>{children}</BSTooltip>}>
    <span className="tooltip-icon-wrapper" tabIndex={visible ? 0 : -1}>
      <span className="sr-only">{children}</span>
      <Icon name="question-mark" size={40} color={colors.orange} className="icon-tooltip" />
    </span>
  </OverlayTrigger>
)

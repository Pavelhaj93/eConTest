import React from 'react'
import { OverlayTrigger, Tooltip as BSTooltip } from 'react-bootstrap'
import { Icon } from '@components'
import { colors } from '@theme'
import { Placement } from 'react-bootstrap/esm/Overlay'

type TooltipProps = {
  id: string
  placement?: Placement
}

// custom wrapper around Bootstrap tooltip component
export const Tooltip: React.FC<TooltipProps> = ({ id, children, placement = 'top' }) => (
  <OverlayTrigger placement={placement} overlay={<BSTooltip id={id}>{children}</BSTooltip>}>
    <span className="tooltip-icon-wrapper" tabIndex={0}>
      <Icon name="question-mark" size={40} color={colors.orange} />
    </span>
  </OverlayTrigger>
)

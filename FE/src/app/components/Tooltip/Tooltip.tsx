/* eslint-disable jsx-a11y/no-noninteractive-tabindex */
import React from 'react'
import { OverlayTrigger, Tooltip as BSTooltip } from 'react-bootstrap'
import { Icon, IconName } from '@components'
import { colors } from '@theme'
import { Placement } from 'react-bootstrap/esm/Overlay'
import { generateId } from '@utils'
import classNames from 'classnames'

type TooltipProps = {
  placement?: Placement
  visible?: boolean
  size?: number
  name?: IconName
  iconColor?: string
  tooltipClassName?: string
}

// custom wrapper around Bootstrap tooltip component
export const Tooltip: React.FC<TooltipProps> = ({
  children,
  placement = 'top',
  visible = true,
  size = 30,
  name = 'info-inversed-circle',
  iconColor = colors.orange,
  tooltipClassName,
}) => (
  <OverlayTrigger
    placement={placement}
    overlay={
      <BSTooltip id={`tooltip${generateId()}`}>
        <div
          dangerouslySetInnerHTML={{
            __html: children as string,
          }}
        />
      </BSTooltip>
    }
  >
    <div
      className={classNames('tooltip-icon-wrapper', tooltipClassName)}
      tabIndex={visible ? 0 : -1}
    >
      <div
        className="sr-only"
        dangerouslySetInnerHTML={{
          __html: children as string,
        }}
      />
      <Icon name={name} size={size} color={iconColor} className="icon-tooltip" />
    </div>
  </OverlayTrigger>
)

import { computePosition, arrow, shift } from '@floating-ui/dom'
import { generateId } from '@utils'

export const TooltipUI = (): void => {
  Array.from(document.querySelectorAll('.js-tooltip')).forEach(async item => {
    const content = item.getAttribute('title')
    let isActive = false

    let tooltip: HTMLDivElement
    ;['mouseenter', 'focus'].forEach(eventName => {
      item.addEventListener(eventName, async () => {
        if (isActive) return

        const id = `tooltip_${generateId()}`
        tooltip = document.createElement('div')

        tooltip.id = id
        tooltip.classList.add(...['tooltip', 'bs-tooltip-top', 'fade'])
        tooltip.setAttribute('role', 'tooltip')
        tooltip.innerHTML = /* html */ `
        <div class="arrow js-tooltip-arrow"></div>
        <div class="tooltip-inner">${content}</div>
      `

        document.body.appendChild(tooltip)
        item.setAttribute('aria-describedby', id)

        const arrowEl: HTMLElement | null = tooltip.querySelector('.js-tooltip-arrow')

        const { x, y, middlewareData } = await computePosition(item, tooltip, {
          placement: 'top',
          middleware: [shift(), arrow({ element: arrowEl })],
        })

        Object.assign(tooltip.style, {
          left: `${x}px`,
          top: `${y}px`,
        })

        // Accessing the data
        const arrowX = middlewareData.arrow?.x
        const arrowY = middlewareData.arrow?.y

        Object.assign(arrowEl?.style, {
          left: arrowX !== null ? `${arrowX}px` : '',
          top: arrowY !== null ? `${arrowY}px` : '',
          right: '',
          bottom: '',
        })

        tooltip.classList.add('show')

        isActive = true
      })
    })
    ;['mouseleave', 'blur'].forEach(eventName => {
      item.addEventListener(eventName, () => {
        document.body.removeChild(tooltip)

        isActive = false
      })
    })
  })
}

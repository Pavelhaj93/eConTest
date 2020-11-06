import React, { forwardRef } from 'react'
import DatePicker, { ReactDatePickerProps } from 'react-datepicker'
import { FormControl, Button } from 'react-bootstrap'
import { cs } from 'date-fns/locale'
import classNames from 'classnames'
import { isMobileDevice } from '@utils'
import { Icon } from '@components'
import { colors } from '@theme'

type DatepickerProps = {
  id: string
  name?: string
  /* Label for calendar icon/button. */
  ariaLabelOpen?: string
  isInvalid?: boolean
}

export const Datepicker: React.FC<DatepickerProps & ReactDatePickerProps> = ({
  id,
  name,
  placeholderText,
  ariaLabelOpen,
  isInvalid,
  ...rest
}) => {
  const CustomDatepickerInput = forwardRef(
    // @ts-ignore
    ({ onClick, ...rest }, ref: React.Ref<HTMLInputElement>) => {
      return (
        <>
          <FormControl
            ref={ref}
            {...rest}
            className={classNames({
              'react-datepicker__input': true,
              invalid: isInvalid,
            })}
            placeholder={placeholderText}
            onFocus={!isMobileDevice() ? onClick : undefined}
          />
          <Button
            variant="link"
            size="sm"
            className="react-datepicker__button-calendar"
            onClick={onClick}
            {...(ariaLabelOpen ? { 'aria-label': ariaLabelOpen } : {})}
          >
            <Icon name="calendar" size={30} color={colors.orange} />
          </Button>
        </>
      )
    },
  )

  CustomDatepickerInput.displayName = 'CustomDatepickerInput'

  return (
    <DatePicker
      id={id}
      name={name || id}
      locale={cs}
      dateFormat={['dd. MM. yyyy', 'dd.MM.yyyy']}
      autoComplete="off"
      popperPlacement="top-start"
      customInput={<CustomDatepickerInput />}
      {...rest}
    />
  )
}

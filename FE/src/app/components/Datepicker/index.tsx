import React, { forwardRef } from 'react'
import DatePicker, { ReactDatePickerProps } from 'react-datepicker'
import { FormControl, Button } from 'react-bootstrap'
import { cs } from 'date-fns/locale'
import { isMobileDevice } from '@utils'
import { Icon } from '@components'
import { colors } from '@theme'

type DatepickerProps = {
  id: string
  name?: string
  /* Label for calendar icon/button. */
  ariaLabelOpen?: string
}

export const Datepicker: React.FC<DatepickerProps & ReactDatePickerProps> = ({
  id,
  name,
  placeholderText,
  ariaLabelOpen,
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
            className="react-datepicker__input"
            placeholder={placeholderText}
            onFocus={!isMobileDevice() ? onClick : () => {}}
          />
          <Button
            variant="link"
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

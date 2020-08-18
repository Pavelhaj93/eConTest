import React, {
  useState,
  useCallback,
  ChangeEvent,
  createRef,
  RefObject,
  FormEvent,
  useEffect,
  useMemo,
} from 'react'
import { Row, Col, Form, Button, FormControl, Collapse, Fade, Alert } from 'react-bootstrap'
import { subDays } from 'date-fns'
import classNames from 'classnames'
import Media from 'react-media'
import { View } from '@types'
import { Datepicker, FormControlTooltipWrapper, Tooltip } from '@components'
import { breakpoints } from '@theme'
import { useForm } from '@hooks'

type FormValues = {
  [key: string]: string
}

export const Authentication: React.FC<View> = ({ labels, formAction, choices }) => {
  const [isFormValid, setFormValid] = useState(false)
  const [date, setDate] = useState<Date | null>(null)
  const [selectedChoice, setSelectedChoice] = useState<string | null>(null)
  const [wasValidated, setValidated] = useState(false)

  const initialValues = useMemo(
    () =>
      choices.reduce((acc, choice) => {
        return {
          ...acc,
          [choice.key]: '',
        }
      }, {}),
    [choices],
  )
  const { values, handleInputChange } = useForm<FormValues>(initialValues)

  // create refs for each choice's input
  const choiceInputRefs: {
    [key: string]: RefObject<HTMLInputElement>
  } = choices.reduce((acc, choice) => {
    return {
      ...acc,
      [choice.key]: createRef<HTMLInputElement>(),
    }
  }, {})

  const handleChangeChoice = useCallback((event: ChangeEvent<HTMLInputElement>) => {
    event.persist()
    setSelectedChoice(event.target.value)
  }, [])

  const handleSubmit = useCallback(
    (event: FormEvent) => {
      if (!isFormValid) {
        event.preventDefault()
        event.stopPropagation()
        setValidated(true)
      }
    },
    [isFormValid],
  )

  /**
   * Validate field value by provided regular expression.
   */
  const isFieldValid = useCallback((value: string, expression: RegExp | string): boolean => {
    // if expression is an empty string => do not validate
    if (!expression) {
      return true
    }

    console.log(expression)
    const re = new RegExp(expression)
    return re.test(value)
  }, [])

  // 1. if verification method is changed, mark the form as invalid
  useEffect(() => {
    setFormValid(false)
  }, [selectedChoice])

  // 2. when some choice value is changed, check whether form can be marked as valid
  useEffect(() => {
    const choice = choices.find(choice => choice.key === selectedChoice)

    if (!choice) {
      return
    }

    // value for the selected choice
    const value = values[choice.key]

    // if
    // 1. choice value is valid
    // 2. choice is selected
    // 3. date is filled
    // then form is good to go
    if (isFieldValid(value, choice.regex ?? '') && selectedChoice && date) {
      setFormValid(true)
    } else {
      setFormValid(false)
    }
  }, [values, date, selectedChoice])

  return (
    <Form noValidate className="mt-4" action={formAction} method="post" onSubmit={handleSubmit}>
      {(wasValidated || labels.validationError) && (
        <Alert variant="danger">
          {wasValidated ? labels.requiredFields : labels.validationError}
        </Alert>
      )}

      <Row>
        <Col xs={12} md={8} lg={6}>
          <Form.Group>
            <Form.Label
              htmlFor="birthDate"
              className={classNames({ 'text-danger': !date && wasValidated })}
            >
              {labels.birthDate}
            </Form.Label>
            <Datepicker
              id="birthDate"
              placeholderText={labels.birthDatePlaceholder}
              maxDate={subDays(new Date(), 1)}
              onChange={(date: Date) => setDate(date)}
              selected={date}
              showYearDropdown
              dropdownMode="select"
              ariaLabelOpen={labels.ariaOpenCalendar}
              isInvalid={!date && wasValidated}
              nextMonthButtonLabel={labels.ariaNextMonth}
              previousMonthButtonLabel={labels.ariaPreviousMonth}
              chooseDayAriaLabelPrefix={labels.ariaChooseDay}
            />
          </Form.Group>
        </Col>
      </Row>

      <Form.Group>
        <div
          className={classNames({
            'like-label': true,
            'text-danger': !selectedChoice && wasValidated,
          })}
        >
          {labels.verificationMethod}
        </div>
        {/* render choices */}
        {choices.map(({ key, label, placeholder, helpText, regex }, idx) => {
          return (
            <div className="d-md-flex align-items-md-center" key={`${key}-${idx}`}>
              <Form.Check
                type="radio"
                label={label}
                name="SelectedKey"
                id={key}
                onChange={handleChangeChoice}
                value={key}
                custom
              />
              <Media query={{ maxWidth: breakpoints.mdMax }}>
                {matches => {
                  // use different type of component/animation on mobile and desktop
                  const ShowHideComponent = matches ? Collapse : Fade
                  const isVisible = selectedChoice === key

                  return (
                    <ShowHideComponent
                      in={isVisible}
                      className="mb-2 mb-md-0"
                      onEntered={() => choiceInputRefs[key].current?.focus()}
                    >
                      <Form.Group controlId={`Additional${idx}`} className="mb-0 ml-4 ml-md-3">
                        <FormControlTooltipWrapper>
                          <Form.Label srOnly>{label}</Form.Label>
                          <FormControl
                            inputMode="numeric"
                            pattern="[0-9]*"
                            name="Additional"
                            {...(placeholder ? { placeholder } : {})}
                            ref={choiceInputRefs[key]}
                            // use custom "id" instead of the one on input element
                            onChange={event =>
                              handleInputChange(
                                event as ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
                                key,
                              )
                            }
                            className={classNames({
                              invalid:
                                isVisible &&
                                !isFieldValid(values[key], regex ?? '') &&
                                wasValidated,
                            })}
                            tabIndex={isVisible ? 0 : -1}
                          />
                          {helpText && (
                            <Tooltip id={`${key}HelpText`} visible={isVisible}>
                              {helpText}
                            </Tooltip>
                          )}
                        </FormControlTooltipWrapper>
                      </Form.Group>
                    </ShowHideComponent>
                  )
                }}
              </Media>
            </div>
          )
        })}
      </Form.Group>

      <Button
        variant="primary"
        type="submit"
        className={classNames({ 'btn-block-mobile': true, 'btn-inactive': !isFormValid })}
      >
        {labels.submitBtn}
      </Button>
    </Form>
  )
}
